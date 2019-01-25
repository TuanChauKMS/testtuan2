extern alias ComponentModel;
using System;
using System.IdentityModel.Tokens;
using System.Net;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Security.Authentication;
using System.Threading.Tasks;
using System.Windows.Forms;
using ComponentModel::System.ComponentModel.Composition;

using QSR.NVivo.Plugins.PlatformsIdentity.Factory;
using QSR.NVivo.Plugins.PlatformsIdentity.Forms;
using QSR.NVivo.Plugins.PlatformsIdentity.Interface;
using QSR.NVivo.Plugins.PlatformsIdentity.Interface.Exceptions.Authentication;
using QSR.NVivo.Plugins.PlatformsIdentity.Interface.Exceptions.CloudAccounts;
using QSR.NVivo.Plugins.PlatformsIdentity.Interface.Factories;
using QSR.NVivo.Plugins.PlatformsIdentity.Interface.Interfaces;
using QSR.NVivo.Plugins.PlatformsIdentity.Services;

namespace QSR.NVivo.Plugins.PlatformsIdentity.Core
{
	[Export(typeof(IPlatformsIdentityPlugin))]
	public class PlatformsIdentityPlugin : IPlatformsIdentityPlugin
	{
		static PlatformsIdentityPlugin()
		{
			// Resolve packed assembly
			// Approach taken from CLR via C# -- http://blogs.msdn.com/b/microsoft_press/archive/2010/02/03/jeffrey-richter-excerpt-2-from-clr-via-c-third-edition.aspx
			AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
			{
				const string CURRENT_ASSEMBLY_NAME = "QSR.NVivo.Plugins.PlatformsIdentity";
				const string LOC_RESOURCES_ASSEMBLY_NAME = "QSR.NVivo.Plugins.PlatformsIdentity.Localization.resources";
				var resourceAssemblyName = new AssemblyName(args.Name);
				
				string resourceName;
				if (resourceAssemblyName.Name == LOC_RESOURCES_ASSEMBLY_NAME)
				{
					// Special handling for localized resource assemblies.
					// See the PlatformsIdentity.csproj (XML) file for details on how the EmbeddedResources
					// for these resource assemblies are defined.
					resourceName = string.Format("{0}\\{1}.dll", resourceAssemblyName.CultureName, resourceAssemblyName.Name);
				}
				else
				{
					resourceName = string.Format("{0}.Resources.{1}.dll", CURRENT_ASSEMBLY_NAME, resourceAssemblyName.Name);
				}

				var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
				foreach (var loadedAssembly in loadedAssemblies)
				{
					AssemblyName loadedAssemblyName = new AssemblyName(loadedAssembly.FullName);
					if (loadedAssemblyName.Name == resourceAssemblyName.Name &&
					    loadedAssemblyName.CultureName == resourceAssemblyName.CultureName)
					{
						return loadedAssembly;
					}
				}

				using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
				{
					if (stream != null)
					{
						Byte[] assemblyData = new Byte[stream.Length];
						stream.Read(assemblyData, 0, assemblyData.Length);
						return Assembly.Load(assemblyData);
					}

					return null;
				}
			};
		}

		/// <summary>
		/// Default constructor, used by MEF when NVivo loads this plugin.
		/// </summary>
		public PlatformsIdentityPlugin()
			: this(null)
		{
		}

		/// <summary>
		/// Constructor which takes in an optional ICredentialStorageService implementation. Defaults to the 
		/// singleton instance provided by CredentialStorageServiceFactory if not explicitly provided.
		/// </summary>
		/// <param name="aCredentialStorageService">Optional ICredentialStorageService implementation.</param>
		public PlatformsIdentityPlugin(ICredentialStorageService aCredentialStorageService = null)
		{
			myCredentialStorageService = aCredentialStorageService ?? CredentialStorageServiceFactory.ServiceInstance;

			myCredentialStorageService.Load(typeof(Credentials));
		}

		#region IPlatformsIdentityPlugin Methods

		#region Authentication

		/// <summary>
		/// Logs the user into NVivo Platforms
		/// </summary>
		/// <param name="parentControl">The control or form to be set as the owner of the dialog</param>
		/// <returns>True if authenticated, false if the user cancelled</returns>
		public bool Login(Control parentControl)
		{
			LoginForm loginForm = new LoginForm();

			if (loginForm.ShowDialog((parentControl)) == DialogResult.OK)
			{
				// Show progress bar while executing the job in it's Shown event handler
				using (LoginProgressForm progressForm = new LoginProgressForm())
				{
					progressForm.Shown += async (sender, e) =>
					{
						LoginProgressForm thisForm = (LoginProgressForm) sender;

						try
						{
							myAuth0ServiceProxy = new Auth0ServiceProxy(new RestClientFactory());
							myAuth0ServiceProxy.CredentialsChanged += Auth0ServiceProxy_CredentialsChanged;

							Credentials credentials =
								await myAuth0ServiceProxy.ExchangeAuthorizationCodeForAccessToken(loginForm.AuthorizationCode);

							VerifyCloudAccountStateIsActive(credentials.AccessToken);

							myCredentialStorageService.Credentials = credentials;
							myCredentialStorageService.Save();
						}
						catch (Exception ex)
						{
							// Wrap all other exceptions into AuthenticationException as they are thrown during login process
							thisForm.Exception = !(ex is AuthenticationException) ? new AuthenticationException(string.Empty, ex) : ex;
						}
						finally
						{
							thisForm.Close();
						}
					};

					progressForm.ShowDialog(parentControl);

					if (progressForm.Exception != null)
					{
						ExceptionDispatchInfo.Capture(progressForm.Exception).Throw();
					}
				}

				return true;
			}

			if (!String.IsNullOrWhiteSpace(loginForm.ErrorMessage))
			{
				throw new AuthenticationException(loginForm.ErrorMessage);
			}

			return false;
		}

		/// <summary>
		/// Make use of the stored credentials, if any, to authenticate against the Uluru Cloud services.
		/// </summary>
		/// <param name="parentControl">The control or form to be set as the owner for the progress dialog</param>
		/// <returns>True if authentication was successful, false if no stored credentials or the stored credentials
		/// are invalid.</returns>
		public bool AuthenticateFromStoredCredentials(Control parentControl)
		{
			// if plugin first started, we don't require CheckIfCredentialsFileExists
			// as if the file is not there, we will treat that scenario as a new user has not logged in;
			if (!myIsFirstRun)
			{
				// if the data file is deleted in-session, invalidate re-authentication
				// and letting NVivo handles it
				if (!myCredentialStorageService.CheckIfCredentialsFileExists())
				{
					return false;
				}
			}
			else
			{
				myIsFirstRun = false;
			}

			// if the user has no stored credentials
			if (!(myCredentialStorageService.Credentials is Credentials credentials))
			{
				return false;
			}

			// Show progress bar while executing the job in it's Shown event handler
			using (LoginProgressForm progressForm = new LoginProgressForm())
			{
				progressForm.Shown += async (sender, e) =>
				{
					LoginProgressForm thisForm = (LoginProgressForm) sender;
					try
					{
						// attempt refresh; the call to RefreshAccessToken will update the proxy instance's credential set
						myAuth0ServiceProxy = new Auth0ServiceProxy(new RestClientFactory(), credentials);
						myAuth0ServiceProxy.CredentialsChanged += Auth0ServiceProxy_CredentialsChanged;

						await myAuth0ServiceProxy.RefreshAccessToken();
					}
					catch (Exception ex)
					{
						// Pass the exception to the LoginProgressForm
						thisForm.Exception = ex;
						myAuth0ServiceProxy = null;
					}
					finally
					{
						thisForm.Close();
					}
				};

				progressForm.ShowDialog(parentControl);

				// The stored credentials are bad, treat every exception at this stage as an AuthenticationException
				// and wrap them as the inner exception to rethrow and let NVivo handle error messages
				if (progressForm.Exception != null)
				{
					throw new AuthenticationException("", progressForm.Exception);
				}

				return true;
			}
		}

		/// <summary>
		/// Clear both the stored credentials and user's SSO cookie by calling Auth0 logout endpoint
		/// </summary>
		/// <param name="parentControl"></param>
		public void Logout(Control parentControl)
		{
			Credentials credentials = myCredentialStorageService.Credentials as Credentials;

			// Clear all stored user information
			myCredentialStorageService.Delete();

			// In the case where the user has restarted NVivo, and then click the Sign Out button
			// or if the user's credentials file is manually deleted so we need to kill the Auth0 cookie
			myAuth0ServiceProxy = new Auth0ServiceProxy(new RestClientFactory(), credentials);

			Uri logoutUri = new Uri(Auth0ServiceProxy.GetLogoutUrl(), UriKind.Absolute);

			LogoutForm logoutForm = new LogoutForm(myAuth0ServiceProxy, logoutUri);
			logoutForm.Show();

			var progressForm = new LogoutProgressForm(logoutForm);
			progressForm.ShowDialog();

			myAuth0ServiceProxy = null;
		}

		#endregion

		#region Cloud Accounts

		/// <summary>
		/// Asynchronously get the remaining available credits of the logged in user
		/// </summary>
		/// <param name="accessToken">the logged in user access toker</param>
		/// <returns>the number of available credit in minutes</returns>
		public async Task<long> GetRemainingCreditsAsync(string accessToken)
		{
			return await Task.Run(() => GetRemainingCredits(accessToken));
		}

		/// <summary>
		/// Synchronously get the remaining available credits of the logged in user
		/// </summary>
		/// <param name="accessToken">the logged in user access toker</param>
		/// <returns>the number of available credit in minutes</returns>
		public long GetRemainingCredits(string accessToken)
		{
			if (string.IsNullOrEmpty(accessToken))
			{
				throw new AuthenticationException("Invalid Access Token Exception");
			}

			long accountId = CloudAccountsServiceProxy.GetAccountId(accessToken);

			const int transcriptionProductId = 1;
			return CloudAccountsServiceProxy.GetRemainingCredit(accessToken, accountId, transcriptionProductId);
		}

		/// <summary>
		/// Launch the insufficient credit page externally
		/// </summary>
		public void OpenBuyCreditPage()
		{
			string buyCreditUrl = CloudAccountsServiceProxy.BuyCreditPageUrl;
			System.Diagnostics.Process.Start(buyCreditUrl);
		}

		/// <summary>
		/// Launch the complete account signup page externally
		/// </summary>
		public void OpenCompleteAccountSignUpPage()
		{
			var completeAccountSignUpUrl = CloudAccountsServiceProxy.CompleteAccountSignUpUrl;
			System.Diagnostics.Process.Start(completeAccountSignUpUrl);
		}

		#endregion

		#region Utilities

		/// <summary>
		/// Allow other plugins to get the current user's access token to make API calls.
		/// Attempt to always retrieve a valid token (by refresh token if the previous token expired)
		/// </summary>
		/// <returns>The access token in the user's credentials object</returns>
		public async Task<string> GetValidAccessToken()
		{
			if (!(myCredentialStorageService.Credentials is Credentials credentials))
			{
				return null;
			}

			if (!myCredentialStorageService.CheckIfCredentialsFileExists())
			{
				throw new DeletedUserDataException();
			}

			myAuth0ServiceProxy = new Auth0ServiceProxy(new RestClientFactory(), credentials);

			// if we need to refresh an expired token, update subscribers with the new credentials
			myAuth0ServiceProxy.CredentialsChanged += Auth0ServiceProxy_CredentialsChanged;

			string accessToken = credentials.AccessToken;
			JwtSecurityToken token = new JwtSecurityToken(accessToken);
			DateTime currentDateTime = DateTime.UtcNow;
			DateTime expiryDateTime = token.ValidTo;

			// token expired if this comparison returns an int that is <= 0
			if (DateTime.Compare(expiryDateTime, currentDateTime) <= 0)
			{
				return (await myAuth0ServiceProxy.RefreshAccessToken()).AccessToken;
			}

			// still valid, keep using the current token
			return accessToken;
		}

		#endregion

		#endregion

		#region Private Methods

		/// <summary>
		/// Saves the updated credentials to storage when the CredentialsChanged event
		/// by Auth0ServiceProxy is triggered
		/// </summary>
		/// <param name="sender">The event sender</param>
		/// <param name="aCredentials">The Credentials object to be stored</param>
		private void Auth0ServiceProxy_CredentialsChanged(object sender, Credentials aCredentials)
		{
			myCredentialStorageService.Credentials = aCredentials;
			myCredentialStorageService.Save();
		}

		/// <summary>
		/// Verify if the logged in user account state is active in the system
		/// Return if account is active, throw an according exception otherwise
		/// </summary>
		/// <param name="accessToken"></param>
		/// <exception cref="AuthenticationException"></exception>
		/// <exception cref="CloudAccountSuspendedException"></exception>
		/// <exception cref="CloudAccountDeletedException"></exception>
		/// <exception cref="CloudAccountIncompleteException"></exception>
		private void VerifyCloudAccountStateIsActive(string accessToken)
		{
			if (string.IsNullOrEmpty(accessToken))
			{
				throw new AuthenticationException("Invalid Access Token Exception");
			}

			CloudAccountState accountState = CloudAccountsServiceProxy.GetAccountState(accessToken);

			if (accountState == CloudAccountState.Active)
			{
				return;
			}

			if (accountState == CloudAccountState.Suspended)
			{
				throw new CloudAccountSuspendedException();
			}

			if (accountState == CloudAccountState.Deleted)
			{
				throw new CloudAccountDeletedException();
			}

			if (accountState == CloudAccountState.Incomplete)
			{
				throw new CloudAccountIncompleteException();
			}
		}

		#endregion

		#region Private Properties

		/// <summary>
		/// The Cloud Accounts Service Proxy Singleton Property, used to make API calls to the Uluru Accounts API
		/// </summary>
		private ICloudAccountsServiceProxy CloudAccountsServiceProxy =>
			myCloudAccountsServiceProxy ?? (myCloudAccountsServiceProxy = new CloudAccountsServiceProxy(new RestClientFactory()));

		#endregion

		#region Private Variables

		/// <summary>
		/// The Cloud Accounts Service Proxy Singleton
		/// </summary>
		private ICloudAccountsServiceProxy myCloudAccountsServiceProxy;

		/// <summary>
		/// Credential storage service
		/// </summary>
		private readonly ICredentialStorageService myCredentialStorageService;

		/// <summary>
		/// Service Proxy to the Auth0 authorization server endpoints
		/// </summary>
		internal IAuth0ServiceProxy myAuth0ServiceProxy;

		/// <summary>
		/// Track if this is currently the first time the plugin is being used
		/// </summary>
		private bool myIsFirstRun = true;

		#endregion
	}
}
