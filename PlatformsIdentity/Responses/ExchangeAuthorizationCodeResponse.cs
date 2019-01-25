using System;

namespace QSR.NVivo.Plugins.PlatformsIdentity.Responses
{
	[Serializable]
	internal class ExchangeAuthorizationCodeResponse
	{
		#region Properties

		/// <summary>
		/// Access Token needed to make API calls
		/// </summary>
		public string Access_Token { get; set; }

		/// <summary>
		/// Refresh Token to allow user to be continually logged in without the need to re-authenticate
		/// </summary>
		public string Refresh_Token { get; set; }

		/// <summary>
		/// The Id Token which contains claims about the users such as the username, or user email
		/// See: https://auth0.com/docs/tokens/id-token
		/// </summary>
		public string Id_Token { get; set; }

		#endregion

	}
}
