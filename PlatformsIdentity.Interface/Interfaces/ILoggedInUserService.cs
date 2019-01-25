namespace QSR.NVivo.Plugins.PlatformsIdentity.Interface.Interfaces
{
	/// <summary>
	/// Get infromation about the logged in user.
	/// Used by NVivo.
	/// </summary>
	public interface ILoggedInUserService
	{
		/// <summary>
		/// Information about logged in user. Null if there is no logged in user.
		/// </summary>
		CloudServicesUserInfo LoggedInUserInfo { get; }
	}
}
