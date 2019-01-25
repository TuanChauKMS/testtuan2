using QSR.NVivo.Plugins.PlatformsIdentity.Interface.Interfaces;

namespace QSR.NVivo.Plugins.PlatformsIdentity.Interface.Factories
{
	/// <summary>
	/// Provides access to the singleton instance of an ILoggedInUserService implementation.
	/// Used by NVivo.
	/// </summary>
	public class LoggedInUserServiceFactory
	{
		/// <summary>
		/// The singleton ILoggedInUserService instance.
		/// </summary>
		public static ILoggedInUserService ServiceInstance => CredentialStorageService.Instance;
	}
}