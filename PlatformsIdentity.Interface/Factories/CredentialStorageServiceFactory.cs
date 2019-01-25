using QSR.NVivo.Plugins.PlatformsIdentity.Interface.Interfaces;

namespace QSR.NVivo.Plugins.PlatformsIdentity.Interface.Factories
{
	/// <summary>
	/// The factory that provides access to the CredentialStorageService singleton
	/// </summary>
	public class CredentialStorageServiceFactory
	{
		/// <summary>
		/// The ICredentialStorageService singleton instance
		/// </summary>
		public static ICredentialStorageService ServiceInstance => CredentialStorageService.Instance;
	}
}
