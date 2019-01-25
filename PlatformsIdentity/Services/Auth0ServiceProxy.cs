using System;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using QSR.NVivo.Plugins.PlatformsIdentity.Core;
using QSR.NVivo.Plugins.PlatformsIdentity.Factory;
using QSR.NVivo.Plugins.PlatformsIdentity.Interface;
using QSR.NVivo.Plugins.PlatformsIdentity.Interface.Exceptions.Authentication;
using QSR.NVivo.Plugins.PlatformsIdentity.Responses;
using RestSharp;

namespace QSR.NVivo.Plugins.PlatformsIdentity.Services
{
	internal class Auth0ServiceProxy : IAuth0ServiceProxy
	{
		#region Constructors

		/// <summary>
		/// Constructor to generate a new Auth0ServiceProxy instance to communicate with Uluru API.
		/// </summary>
		/// <param name="restClientFactory">Factory used to create RestClient instance for HTTP RESTful API comms.</param>
		public Auth0ServiceProxy(IRestClientFactory restClientFactory)
		{
			myRestClientFactory = restClientFactory;
		}

		/// <summary>
		/// Constructor to generate a new Auth0ServiceProxy instance to communicate with Uluru API
		/// using an existing set of credentials.
		/// </summary>
		/// <param name="restClientFactory">Factory used to create RestClient instance for HTTP RESTful API comms.</param>
		/// <param name="credentials">Existing user credentials.</param>
		public Auth0ServiceProxy(IRestClientFactory restClientFactory, Credentials credentials) : this (restClientFactory)
		{
			myCredentials = credentials;
		}

		#endregion

		#region Internal Static Methods

		/// <summary>
		/// Returns the URL to be loaded into the browser to begin the authentication process.
		/// The URL is formed per Auth0 Documentation on the Authorization Code Grant Flow with PKCE:
		/// https://auth0.com/docs/api-auth/tutorials/authorization-code-grant-pkce
		/// </summary>
		internal static string GetAuth0AuthorizeEndpoint()
		{
			// create random bytes
			Random random = new Random();
			byte[] randomBytes = new byte[32];
			random.NextBytes(randomBytes);

			// encode the random bytes to generate a verifier
			myVerifier = Base64UrlEncoder.Encode(randomBytes);
			byte[] verifierBytes = Encoding.ASCII.GetBytes(myVerifier);

			// generate the challenge by encoding the hash of the verifier
			string challenge = Base64UrlEncoder.Encode(ComputeSha256Hash(verifierBytes));

			// send the challenge and the method which generates it as part of the authorization call
			string url = $"{MyUrlContent.auth0.loginDomainUrl}/authorize?" +
									$"audience={MyUrlContent.auth0.audienceApi}" +
									"&scope=openid profile offline_access" +
									"&response_type=code" +
									$"&client_id={ClientId}" +
									$"&code_challenge=" + challenge +
									$"&code_challenge_method=S256" +
									$"&redirect_uri={MyUrlContent.auth0.loginCallbackUri}";

			return url;
		}

		/// <summary>
		/// Returns the Sign Up Url which will be opened in an external browser
		/// </summary>
		internal static string GetExternalSignUpUrl()
		{
			return MyUrlContent.external.signupUrl;
		}

		/// <summary>
		/// Returns the Logout Url which the web browser will navigate to in order to clear the SSO cookie
		/// </summary>
		/// <returns></returns>
		internal static string GetLogoutUrl()
		{
			return MyUrlContent.auth0.logoutUrl;
		}

		#endregion

		#region Public Events

		/// <summary>
		/// Raised when the current credentials have been refreshed, through the use of a refresh token
		/// to get a new access token.
		/// </summary>
		public event EventHandler<Credentials> CredentialsChanged;

		#endregion

		#region Public Methods

		/// <summary>
		/// According to Auth0 Documentation: https://auth0.com/docs/tokens/access-token
		/// we must exchange the Authorization Code for the Access Token
		/// </summary>
		/// <param name="authorizationCode"></param>
		/// <returns>The <see cref="Credentials"/> object which contains the Access and Refresh Tokens</returns>
		public async Task<Credentials> ExchangeAuthorizationCodeForAccessToken(string authorizationCode)
		{
			IRestClient client = myRestClientFactory.Create(MyUrlContent.auth0.loginDomainUrl.ToString());

			// here we also pass the code verifier as part of the Authorization Code Grant Flow with PKCE
			var request = new RestRequest("/oauth/token", Method.POST);
			request.AddParameter("grant_type", "authorization_code");
			request.AddParameter("client_id", ClientId);
			request.AddParameter("code_verifier", myVerifier);
			request.AddParameter("code", authorizationCode);
			request.AddParameter("redirect_uri", MyUrlContent.auth0.loginCallbackUri);

			IRestResponse restResponse = await client.ExecutePostTaskAsync(request);

			if (String.IsNullOrWhiteSpace(restResponse.Content))
			{
				throw new Auth0Exception();
			}

			return ExtractTokensFromResponse(restResponse, OAuthGrantType.AuthorizationCode);
		}

		/// <summary>
		/// Per the Auth0 documentation at https://auth0.com/docs/tokens/refresh-token/current#get-a-refresh-token
		/// Access Tokens have an expiry, by which time we must exchange the Refresh Code for an updated Access Token to
		/// continue to access the Uluru Cloud Services API.
		/// </summary>
		/// <param name="refreshToken">The refresh token associated with an authorized session - this is used to request a new access token</param>
		/// <returns>An updated set of credentials with refreshed tokens</returns>
		/// <remarks>Make sure the proxy instance that calls this method always has the latest credentials.</remarks>
		public async Task<Credentials> RefreshAccessToken()
		{
			IRestClient client = myRestClientFactory.Create(MyUrlContent.auth0.loginDomainUrl.ToString());

			var request = new RestRequest("/oauth/token", Method.POST);
			request.AddParameter("grant_type", "refresh_token");
			request.AddParameter("client_id", ClientId);
			request.AddParameter("refresh_token", myCredentials.RefreshToken);

			IRestResponse restResponse = await client.ExecutePostTaskAsync(request);

			if (string.IsNullOrWhiteSpace(restResponse.Content) || restResponse.StatusCode != HttpStatusCode.OK)
			{
				if (restResponse.StatusCode == HttpStatusCode.Forbidden)
				{
					throw new InvalidRefreshTokenException();
				}

				throw new Auth0Exception();
			}

			myCredentials = ExtractTokensFromResponse(restResponse, OAuthGrantType.RefreshToken);

			// Notify subscribers that the credentials have changed.
			CredentialsChanged?.Invoke(this, myCredentials);

			return myCredentials;
		}

		/// <summary>
		/// Invalidate the current user's refresh token. Used when logging the user out
		/// As Uluru do not set Auth0 cookies for authentication, so we won't be able to use the default logout endpoint
		/// </summary>
		public async Task RevokeRefreshToken()
		{
			IRestClient client = myRestClientFactory.Create(MyUrlContent.auth0.loginDomainUrl.ToString());

			var request = new RestRequest("/oauth/revoke", Method.POST);
			request.AddParameter("client_id", ClientId);
			request.AddParameter("token", myCredentials.RefreshToken);

			IRestResponse restResponse = await client.ExecutePostTaskAsync(request);

			if (restResponse.StatusCode != HttpStatusCode.OK)
			{
				throw new Auth0Exception();
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// This method deserialises the response from an access token request and returns
		/// an <see cref="Credentials"/>object which can be persisted to credential storage
		/// </summary>
		/// <param name="restResponse">The Rest Response containing the access tokens</param>
		/// <param name="grantType">The OAuth grant type used to get specific tokens</param>
		/// <remarks>The response object is a JSON blob similar to
		/// {
		///     "access_token":"VygNPcxVEG3...PSX",
		///     "refresh_token":"NzE...nwud",
		///     "id_token": "eyJ0...wAcQ",
		///     "expires_in":"3600",
		///     "token_type": "Bearer"
		/// }
		/// </remarks>
		private Credentials ExtractTokensFromResponse(IRestResponse restResponse, OAuthGrantType grantType)
		{
			Credentials credentials = null;
			string responseData = restResponse.Content;

			if (!String.IsNullOrWhiteSpace(responseData))
			{
				ExchangeAuthorizationCodeResponse response = JsonConvert.DeserializeObject<ExchangeAuthorizationCodeResponse>(responseData);

				string userEmail = null;
				string userNickName = null;
				if (!string.IsNullOrEmpty(response.Id_Token))
				{
					JwtSecurityToken token = new JwtSecurityToken(response.Id_Token);

					Claim nickname = token.Claims.FirstOrDefault(x => x.Type.Equals("nickname", StringComparison.InvariantCultureIgnoreCase));
					Claim name = token.Claims.FirstOrDefault(x => x.Type.Equals("name", StringComparison.InvariantCultureIgnoreCase));

					if (nickname != null)
					{
						userNickName = nickname.Value;
					}

					if (name != null)
					{
						userEmail = name.Value;
					}
				}

				credentials = new Credentials
				{
					AccessToken = response.Access_Token,
					UserNickName = userNickName,
					UserEmail = userEmail
				};

				switch (grantType)
				{
					case OAuthGrantType.AuthorizationCode:
						credentials.RefreshToken = response.Refresh_Token;
						break;

					// If grant type is refresh_token, persist the current refresh_token (until it's revoked)
					case OAuthGrantType.RefreshToken:
						credentials.RefreshToken = myCredentials.RefreshToken;
						break;
				}
			}

			return credentials;
		}

		/// <summary>
		/// Return the computed SHA256 hash of the buffer
		/// </summary>
		/// <param name="buffer"></param>
		private static byte[] ComputeSha256Hash(byte[] buffer)
		{
			SHA256 sha256 = SHA256.Create();
			byte[] hashValue = sha256.ComputeHash(buffer);

			return hashValue;
		}

		#endregion

		#region Private Members

		/// <summary>
		/// Factory for creating <see cref="IRestClient"/> instances
		/// </summary>
		private readonly IRestClientFactory myRestClientFactory;

		/// <summary>
		/// The current logged-in credentials.
		/// </summary>
		private Credentials myCredentials;

		/// <summary>
		/// Provides all needed urls to call Auth0 API
		/// NOTE: DO NOT use this private field directly. Use it's property for null checking and exception handling instead.
		/// </summary>
		private static dynamic myUrlContent;

		/// <summary>
		/// Private property for myUrlContent. USE THIS for null checking and exception handling.
		/// </summary>
		private static dynamic MyUrlContent
		{
			get
			{
				if (myUrlContent == null)
				{
					myUrlContent = RedirectUrlServiceFactory.ServiceInstance.UrlContent;
				}

				return myUrlContent;
			}
		}

		#endregion

		#region Auth0 Client Information

		/// <summary>
		/// The Auth0 client ID
		/// </summary>
		private static Base64String ClientId => myClientId ?? (myClientId = new Base64String(MyUrlContent.auth0.obfuscatedClientId.ToString()));
		private static Base64String myClientId;

		/// <summary>
		/// The verifier used in the Authorization Code Grant Flow with PKCE
		/// </summary>
		private static string myVerifier;

		#endregion
	}
}