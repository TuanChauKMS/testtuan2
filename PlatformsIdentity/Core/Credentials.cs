namespace QSR.NVivo.Plugins.PlatformsIdentity.Core
{
	/// <summary>
	/// Information about a user's login. This is persisted to a file.
	/// </summary>
	public class Credentials
	{
		/// <summary>
		/// The logged in user nickname
		/// </summary>
		public string UserNickName { get; set; }
		
		/// <summary>
		/// The logged in user email address
		/// </summary>
		public string UserEmail { get; set; }

		/// <summary>
		/// OAuth access token which has expiry period set by our Transcription service
		/// </summary>
		public string AccessToken { get; set; }

		/// <summary>
		/// OAuth refresh token which can be used to get a new Access Token - lasts forever unless revoked by authorization server (Auth0)
		/// </summary>
		public string RefreshToken { get; set; }
	}
}
