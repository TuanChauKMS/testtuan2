using System;

namespace QSR.NVivo.Plugins.PlatformsIdentity.Interface.Interfaces
{
	/// <summary>
	/// Operations for storing/retrieving credentials. Used by the plugin only, not NVivo.
	/// This interface handles a generic credentials object, with the concrete type defined and
	/// used internally within the plugin.
	/// </summary>
	public interface ICredentialStorageService
	{
		/// <summary>
		/// Get/set the credentials object. The object type is defined and used internally within the plugin.
		/// </summary>
		object Credentials { get; set; }

		/// <summary>
		/// Loads credentials from file. Fails silently if unable to do so.
		/// </summary>
		/// <param name="aCredentialsObjectType">The type of the credentials object.</param>
		void Load(Type aCredentialsObjectType);

		/// <summary>
		/// Save the credentials. Fails silently if unable to do so.
		/// </summary>
		void Save();

		/// <summary>
		/// Delete credentials from file system.
		/// </summary>
		void Delete();

		/// <summary>
		/// Attempt to check if the user's credentials data file is still on disk
		/// </summary>
		bool CheckIfCredentialsFileExists();
	}
}