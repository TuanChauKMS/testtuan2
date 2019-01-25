using System;
using System.Threading.Tasks;

using QSR.NVivo.Plugins.PlatformsIdentity.Core;

namespace QSR.NVivo.Plugins.PlatformsIdentity.Services
{
	public interface IAuth0ServiceProxy
	{
		/// <summary>
		/// Raised when the current credentials have been refreshed, through the use of a refresh token
		/// to get a new access token.
		/// </summary>
		event EventHandler<Credentials> CredentialsChanged;

		/// <summary>
		/// Per the Auth0 documentation we must exchange the Authorization Code for an 
		/// Access Token which actually grants access to the Uluru API
		/// </summary>
		/// <param name="authorizationCode">the authorization code received during the OAuth login</param>>
		Task<Credentials> ExchangeAuthorizationCodeForAccessToken(string authorizationCode);

		/// <summary>
		/// Per the Auth0 documentation Access Tokens have an expiry, by which time we must exchange 
		/// the Refresh Token for an updated Access Token to continue to access the Uluru API.
		/// </summary>
		/// <returns>An updated set of credentials with refreshed tokens</returns>
		Task<Credentials> RefreshAccessToken();

		/// <summary>
		/// Invalidate the current user's refresh token. Used when logging the user out
		/// As Uluru do not set Auth0 cookies for authentication, so we won't be able to use the default logout endpoint
		/// </summary>
		Task RevokeRefreshToken();
	}
}
