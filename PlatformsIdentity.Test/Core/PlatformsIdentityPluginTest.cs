using System;
using System.Fakes;
using System.IdentityModel.Tokens.Fakes;
using System.Security.Authentication;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Fakes;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using QSR.NVivo.Plugins.PlatformsIdentity.Core;
using QSR.NVivo.Plugins.PlatformsIdentity.Forms;
using QSR.NVivo.Plugins.PlatformsIdentity.Forms.Fakes;
using QSR.NVivo.Plugins.PlatformsIdentity.Interface;
using QSR.NVivo.Plugins.PlatformsIdentity.Interface.Exceptions.Authentication;
using QSR.NVivo.Plugins.PlatformsIdentity.Interface.Interfaces;
using QSR.NVivo.Plugins.PlatformsIdentity.Services.Fakes;

namespace QSR.NVivo.Plugins.PlatformsIdentity.Test.Core
{
	[TestClass]
	public class PlatformsIdentityPluginTest
	{
		[TestMethod]
		public async Task GetValidAccessToken_ValidAccessToken_Success()
		{
			using (ShimsContext.Create())
			{
				/*---ARRANGE---*/

				string testToken = "LsaQWEWAFAFS21882=";

				Credentials testCredentials = new Credentials
				{
					UserEmail = "test@email.com",
					UserNickName = "test",
					AccessToken = testToken,
					RefreshToken = "KSDndfnWJLAJSMNK"
				};

				// set up the mock for ICredentialStorageService object
				var cssMock = new Mock<ICredentialStorageService>();
				cssMock.SetupGet(service => service.Credentials).Returns(testCredentials);
				cssMock.Setup(service => service.CheckIfCredentialsFileExists()).Returns(true);

				// set up the current date and valid date of the token for comparison to pass
				ShimJwtSecurityToken.ConstructorString = (token, s) => { };
				ShimJwtSecurityToken.AllInstances.ValidToGet = token => new DateTime(1995, 10, 2);
				ShimDateTime.UtcNowGet = () => new DateTime(1995, 9, 4);

				/*---ACT---*/

				PlatformsIdentityPlugin piPlugin = new PlatformsIdentityPlugin(cssMock.Object);
				string returnedToken= await piPlugin.GetValidAccessToken();

				/*---ASSERT---*/

				Assert.AreEqual(returnedToken, testToken);
			}
		}

		[TestMethod]
		public async Task GetValidAccessToken_ExpiredAccessToken_Success()
		{
			using (ShimsContext.Create())
			{
				string testToken = "LsaQWEWAFAFS21882=";

				Credentials testCredentials = new Credentials
				{
					UserEmail = "test@email.com",
					UserNickName = "test",
					AccessToken = testToken,
					RefreshToken = "KSDndfnWJLAJSMNK"
				};

				Credentials updatedCredentials = new Credentials
				{
					UserEmail = "test_updated@email.com",
					UserNickName = "test_updated",
					AccessToken = "LsaQWEWAFAFS21882=UPDATED",
					RefreshToken = "KSDndfnWJLAJSMNK___UPDATED"
				};

				// set up the mock for ICredentialStorageService object
				var cssMock = new Mock<ICredentialStorageService>();
				cssMock.SetupGet(service => service.Credentials).Returns(testCredentials);
				cssMock.Setup(service => service.CheckIfCredentialsFileExists()).Returns(true);
				cssMock.SetupSet(service => service.Credentials = updatedCredentials).Verifiable();

				// set up the CredentialsChanged so we can fake its invocation
				EventHandler<Credentials> credentialsChangedEventHandler = null;

				// capture the event handler
				ShimAuth0ServiceProxy.AllInstances.CredentialsChangedAddEventHandlerOfCredentials = (proxy, handler) =>
				{
					credentialsChangedEventHandler = handler;
				};

				// invoke the event handler and return updatedCredentials
				ShimAuth0ServiceProxy.AllInstances.RefreshAccessToken = proxy =>
				{
					credentialsChangedEventHandler.Invoke(proxy, updatedCredentials);
					return Task.FromResult(updatedCredentials);
				};

				// set up the current date and valid date of the token for comparison to fail
				ShimJwtSecurityToken.ConstructorString = (token, s) => { };
				ShimJwtSecurityToken.AllInstances.ValidToGet = token => new DateTime(1995, 9, 4);
				ShimDateTime.UtcNowGet = () => new DateTime(1995, 10, 2);

				/*---ACT---*/

				PlatformsIdentityPlugin plugin = new PlatformsIdentityPlugin(cssMock.Object);
				string refreshedToken = await plugin.GetValidAccessToken();

				/*---ASSERT---*/

				Assert.AreEqual(refreshedToken, updatedCredentials.AccessToken);
				cssMock.Verify(service => service.Save(), Times.AtLeastOnce);
				cssMock.VerifyAll();
			}
		}

		[TestMethod]
		public async Task GetValidAccessToken_CredentialsFileDoNotExist_FailureThrowException()
		{
			using (ShimsContext.Create())
			{
				/*---ARRANGE---*/

				string testToken = "LsaQWEWAFAFS21882=";

				Credentials testCredentials = new Credentials
				{
					UserEmail = "test@email.com",
					UserNickName = "test",
					AccessToken = testToken,
					RefreshToken = "KSDndfnWJLAJSMNK"
				};

				var cssMock = new Mock<ICredentialStorageService>();
				cssMock.SetupGet(service => service.Credentials).Returns(testCredentials);
				cssMock.Setup(service => service.CheckIfCredentialsFileExists()).Returns(false);

				/*---ACT---*/ /*---ASSERT---*/

				PlatformsIdentityPlugin plugin = new PlatformsIdentityPlugin(cssMock.Object);

				await Assert.ThrowsExceptionAsync<DeletedUserDataException>(async () => await plugin.GetValidAccessToken());
			}
		}

		[TestMethod]
		public void Login_NormalFlow_Success()
		{
			using (ShimsContext.Create())
			{
				/*---ARRANGE---*/

				// capture the Shown handler on the LoginProgressForm so we can fake its invocation
				EventHandler loginProgressFormEventHandler = null;
				ShimForm.AllInstances.ShownAddEventHandler = (form, handler) =>
				{
					if (form is LoginProgressForm)
					{
						loginProgressFormEventHandler = handler;
					}
				};

				// skip the login form and setup a "logged in" scenario
				// here we are handling both the LoginForm and the LoginProgressForm shown events
				ShimForm.AllInstances.ShowDialogIWin32Window = (form, parent) =>
				{
					if (form is LoginProgressForm)
					{
						loginProgressFormEventHandler.Invoke(form, EventArgs.Empty);
					}

					return DialogResult.OK;
				};

				// setup the login handling in LoginProgressForm Shown handler to obtain and save user's 
				// shim the AuthorizationCode property
				ShimLoginForm.AllInstances.AuthorizationCodeGet = f => "shim_access_token_123";

				Credentials testCredentials = new Credentials
				{
					UserEmail = "derp@derpvillage.com",
					UserNickName = "derp",
					AccessToken = "kdjiwaANFJnmmdlsK12SF31EKl=",
					RefreshToken = "=lKE13FS21KsldmmnJFNAawijdk"
				};

				ShimAuth0ServiceProxy.AllInstances.ExchangeAuthorizationCodeForAccessTokenString = (proxy, code) =>
				{
					Assert.AreEqual("shim_access_token_123", code);
					return Task.FromResult(testCredentials);
				};

				// mock the cloud account state verification to return active
				ShimCloudAccountsServiceProxy.AllInstances.GetAccountStateString = (proxy, token) => CloudAccountState.Active;

				// mock the ICredentialStorageService object
				var cssMock = new Mock<ICredentialStorageService>();
				cssMock.SetupSet(service => service.Credentials = testCredentials).Verifiable();
				cssMock.Setup(service => service.Save());

				/*---ACT---*/

				PlatformsIdentityPlugin plugin = new PlatformsIdentityPlugin(cssMock.Object);
				bool result = plugin.Login(null);

				/*---ASSERT---*/

				Assert.IsTrue(result);
				cssMock.VerifyAll();
			}
		}

		[TestMethod]
		public void Login_AccountStateNotActive_Failure()
		{
			using (ShimsContext.Create())
			{
				/*---ARRANGE---*/

				// capture the Shown handler on the LoginProgressForm so we can fake its invocation
				EventHandler loginProgressFormEventHandler = null;
				ShimForm.AllInstances.ShownAddEventHandler = (form, handler) =>
				{
					if (form is LoginProgressForm)
					{
						loginProgressFormEventHandler = handler;
					}
				};

				// skip the login form and setup a "logged in" scenario
				// here we are handling both the LoginForm and the LoginProgressForm shown events
				ShimForm.AllInstances.ShowDialogIWin32Window = (form, parent) =>
				{
					if (form is LoginProgressForm)
					{
						loginProgressFormEventHandler.Invoke(form, EventArgs.Empty);
					}

					return DialogResult.OK;
				};

				// setup the login handling in LoginProgressForm Shown handler to obtain and save user's 
				// shim the AuthorizationCode property
				ShimLoginForm.AllInstances.AuthorizationCodeGet = f => "shim_access_token_123";

				Credentials testCredentials = new Credentials
				{
					UserEmail = "derp@derpvillage.com",
					UserNickName = "derp",
					AccessToken = "kdjiwaANFJnmmdlsK12SF31EKl=",
					RefreshToken = "=lKE13FS21KsldmmnJFNAawijdk"
				};

				ShimAuth0ServiceProxy.AllInstances.ExchangeAuthorizationCodeForAccessTokenString = (proxy, code) =>
				{
					Assert.AreEqual("shim_access_token_123", code);
					return Task.FromResult(testCredentials);
				};

				// mock the cloud account state verification to return any other state rather than active
				ShimCloudAccountsServiceProxy.AllInstances.GetAccountStateString = (proxy, token) => CloudAccountState.Incomplete;

				// mock the ICredentialStorageService object
				var cssMock = new Mock<ICredentialStorageService>();
				cssMock.SetupSet(service => service.Credentials = testCredentials).Verifiable();
				cssMock.Setup(service => service.Save());

				/*---ACT---*/ /*---ASSERT---*/

				PlatformsIdentityPlugin plugin = new PlatformsIdentityPlugin(cssMock.Object);
				Assert.ThrowsException<AuthenticationException>(() => plugin.Login(null));
			}
		}

		[TestMethod]
		public void Logout_NormalFlow_Success()
		{
			using (ShimsContext.Create())
			{
				/*---ARRANGE---*/

				Credentials testCredentials = new Credentials
				{
					UserEmail = "derp@derpvillage.com",
					UserNickName = "derp",
					AccessToken = "kdjiwaANFJnmmdlsK12SF31EKl=",
					RefreshToken = "=lKE13FS21KsldmmnJFNAawijdk"
				};

				// mock the ICredentialStorageService instance
				var cssMock = new Mock<ICredentialStorageService>();
				cssMock.SetupGet(service => service.Credentials).Returns(testCredentials);
				cssMock.Setup(serviceMock => serviceMock.Delete());

				// shim the LogoutForm and LogoutProgressForm
				ShimForm.AllInstances.ShowDialogIWin32Window = (form, window) => DialogResult.OK;
				ShimForm.AllInstances.ShowDialog = form => DialogResult.OK;

				/*---ACT---*/

				PlatformsIdentityPlugin plugin = new PlatformsIdentityPlugin(cssMock.Object);
				plugin.Logout(null);

				/*---ASSERT---*/

				cssMock.Verify(service => service.Delete(), Times.Once);
				Assert.IsNull(plugin.myAuth0ServiceProxy);
			}
		}

		[TestMethod]
		public void Logout_RevokeRefreshTokenUnsuccessful_Success()
		{
			using (ShimsContext.Create())
			{
				/*---ARRANGE---*/

				Credentials testCredentials = new Credentials
				{
					UserEmail = "derp@derpvillage.com",
					UserNickName = "derp",
					AccessToken = "kdjiwaANFJnmmdlsK12SF31EKl=",
					RefreshToken = "=lKE13FS21KsldmmnJFNAawijdk"
				};

				// mock the ICredentialStorageService instance
				var cssMock = new Mock<ICredentialStorageService>();
				cssMock.SetupGet(service => service.Credentials).Returns(testCredentials);

				// Exception will be caught by LogoutForm => silent failure
				ShimAuth0ServiceProxy.AllInstances.RevokeRefreshToken = proxy => throw new Auth0Exception();

				// shim the LogoutForm and LogoutProgressForm
				ShimForm.AllInstances.ShowDialogIWin32Window = (form, window) => DialogResult.OK;
				ShimForm.AllInstances.ShowDialog = form => DialogResult.OK;

				/*---ACT---*/

				PlatformsIdentityPlugin plugin = new PlatformsIdentityPlugin(cssMock.Object);
				plugin.Logout(null);

				/*---ASSERT---*/

				cssMock.Verify(service => service.Delete(), Times.Once);
				Assert.IsNull(plugin.myAuth0ServiceProxy);
			}
		}

		[TestMethod]
		public void AuthenticateFromStoredCredentials_NormalFlow_Success()
		{
			using (ShimsContext.Create())
			{
				/*---ARRANGE---*/

				// set up the credential objects to simulate updated credentials
				Credentials initialCredentials = new Credentials
				{
					UserEmail = "test@email.com",
					UserNickName = "test",
					AccessToken = "LsaQWEWAFAFS21882=",
					RefreshToken = "KSDndfnWJLAJSMNK"
				};

				Credentials updatedCredentials = new Credentials
				{
					UserEmail = "test_updated@email.com",
					UserNickName = "test_updated",
					AccessToken = "LsaQWEWAFAFS21882=UPDATED",
					RefreshToken = "KSDndfnWJLAJSMNK___UPDATED"
				};

				// capture Shown handler on the LoginProgressForm so we can fake its invocation
				EventHandler loginProgressFormShownEventHandler = null;
				ShimForm.AllInstances.ShownAddEventHandler = (form, handler) =>
				{
					loginProgressFormShownEventHandler = handler;
				};

				// fake the invocation of the handler we just captured
				ShimForm.AllInstances.ShowDialogIWin32Window = (f, p) =>
				{
					loginProgressFormShownEventHandler.Invoke(f, EventArgs.Empty);
					return DialogResult.OK;
				};

				// capture the event handler registration to fire
				EventHandler<Credentials> credentialsChangedEventHandler = null;
				ShimAuth0ServiceProxy.AllInstances.CredentialsChangedAddEventHandlerOfCredentials =
					(proxy, handler) => { credentialsChangedEventHandler = handler; };

				// setup call to Auth0 and fire the event handler with updated credentials object
				ShimAuth0ServiceProxy.AllInstances.RefreshAccessToken = proxy =>
				{
					credentialsChangedEventHandler.Invoke(proxy, updatedCredentials);
					return Task.FromResult(updatedCredentials);
				};

				// mock an ICredentialStorageService instance to return a credential set
				var cssMock = new Mock<ICredentialStorageService>();
				cssMock.SetupGet(service => service.Credentials).Returns(initialCredentials);

				// after credentials are refreshed and the CredentialsChanged event is fired, we expect a new set to be saved
				cssMock.SetupSet(service => service.Credentials = updatedCredentials).Verifiable();
				cssMock.Setup(service => service.Save());

				/*---ACT---*/

				PlatformsIdentityPlugin piPlugin = new PlatformsIdentityPlugin(cssMock.Object);
				bool result = piPlugin.AuthenticateFromStoredCredentials(null);
				
				/*---ASSERT---*/

				Assert.IsTrue(result);
				cssMock.VerifyAll();
			}
		}

		[TestMethod]
		public void AuthenticateFromStoredCredentials_CredentialsFileDoNotExist_Failure()
		{
			/*---ARRANGE---*/

			// as the first call to this method won't require the check for credentials file, need to setup the test to run on the second call
			// set up a mock of the ICredentialStorageService instance to return null for Credentials property to bypass the first call
			var ccsMock = new Mock<ICredentialStorageService>();
			ccsMock.SetupGet(service => service.Credentials).Returns(null);

			// on second call, always return false on the check for file existence
			ccsMock.Setup(service => service.CheckIfCredentialsFileExists()).Returns(false);

			/*---ACT---*/

			PlatformsIdentityPlugin piPlugin = new PlatformsIdentityPlugin(ccsMock.Object);
			piPlugin.AuthenticateFromStoredCredentials(null); // first pass
			bool result = piPlugin.AuthenticateFromStoredCredentials(null); // second pass

			/*---ASSERT---*/

			Assert.IsFalse(result);
		}

		[TestMethod]
		public void AuthenticateFromStoredCredentials_RefreshAccessTokenUnsuccessful_FailureThrowException()
		{
			using (ShimsContext.Create())
			{
				/*---ARRANGE---*/

				// set up the test credential object
				Credentials testCredentials = new Credentials
				{
					UserEmail = "test@email.com",
					UserNickName = "test",
					AccessToken = "LsaQWEWAFAFS21882=",
					RefreshToken = "KSDndfnWJLAJSMNK"
				};

				// mock an ICredentialStorageService instance to return a credential set
				var cssMock = new Mock<ICredentialStorageService>();
				cssMock.SetupGet(service => service.Credentials).Returns(testCredentials);

				// simulate invalid token scenario
				ShimAuth0ServiceProxy.AllInstances.RefreshAccessToken = proxy => throw new InvalidRefreshTokenException();

				/*---ACT---*/

				try
				{
					PlatformsIdentityPlugin piPlugin = new PlatformsIdentityPlugin(cssMock.Object);
					piPlugin.AuthenticateFromStoredCredentials(null);
				}

				/*---ASSERT---*/

				catch (AuthenticationException e)
				{
					Assert.IsNotNull(e.InnerException);
					Assert.AreEqual(e.InnerException.GetType(), typeof(InvalidRefreshTokenException));
				}
			}
		}

		[TestMethod]
		public void GetRemainingCredits_NormalFlow_Success()
		{
			using (ShimsContext.Create())
			{
				/*---ARRANGE---*/

				string token = "LsaQWEWAFAFS21882=";
				long id = 49;
				long creditVal = 9001;

				// shim the CloudAccountsServiceProxy instance
				ShimCloudAccountsServiceProxy.AllInstances.GetAccountIdString = (proxy, strToken) => id;
				ShimCloudAccountsServiceProxy.AllInstances.GetRemainingCreditStringInt64Int32 =
					(proxy, strToken, longId, longProdId) => creditVal;

				/*---ACT---*/

				PlatformsIdentityPlugin plugin = new PlatformsIdentityPlugin();
				long result = plugin.GetRemainingCredits(token);

				/*---ASSERT---*/

				Assert.IsNotNull(result);
				Assert.AreEqual(result, creditVal);
			}
		}

		[TestMethod]
		public void GetRemainingCredits_InvalidAccessToken_FailureThrowException()
		{
			using (ShimsContext.Create())
			{
				/*---ARRANGE---*/

				string token = "";

				/*---ACT---*/ /*---ASSERT---*/

				PlatformsIdentityPlugin plugin = new PlatformsIdentityPlugin();
				Assert.ThrowsException<AuthenticationException>(() => { plugin.GetRemainingCredits(token); }); 
			}
		}
	}
}
