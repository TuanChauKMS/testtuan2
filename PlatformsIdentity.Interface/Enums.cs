namespace QSR.NVivo.Plugins.PlatformsIdentity.Interface
{
	/// <summary>
	/// The Auth0 grant types, used to get specific tokens from the /oauth/token endpoint
	/// </summary>
	public enum OAuthGrantType
	{
		AuthorizationCode,
		RefreshToken
	}

	/// <summary>
	/// The Uluru user's account state, defined according to the accountStateId property in the Cloud Accounts userProfile DTO
	/// 1 - Active (Account is active within the system)
	/// 2 - Suspended (Account is currently valid but is suspended)
	/// 3 - Deleted (Account is no longer valid and is deleted)
	/// 4 - Incomplete (The account hasn't yet been fully provisioned)
	/// </summary>
	public enum CloudAccountState
	{
		Active = 1,
		Suspended = 2,
		Deleted = 3,
		Incomplete = 4
	}
}
