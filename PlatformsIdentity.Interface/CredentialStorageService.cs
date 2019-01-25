using System;
using System.IO;

using QSR.NVivo.Plugins.PlatformsIdentity.Interface.Interfaces;

namespace QSR.NVivo.Plugins.PlatformsIdentity.Interface
{
	/// <summary>
	/// Storing and retrieving user's credentials
	/// </summary>
	internal class CredentialStorageService : ICredentialStorageService, ILoggedInUserService
	{
		#region Constructor

		/// <summary>
		/// Constructor which takes in a IEncryptionService implementation.
		/// If no implementation is provided, the default EncryptionService will be used.
		/// </summary>
		/// <param name="encryptionService"></param>
		public CredentialStorageService(IEncryptionService encryptionService)
		{
			myEncryptionService = encryptionService ?? new EncryptionService();
		}

		#endregion

		#region ICredentialStorageService Members

		/// <summary>
		/// Get/set the credentials object - used internally within plugin
		/// </summary>
		public object Credentials
		{
			get => myCredentials;

			set
			{
				myCredentials = value;

				// Update the encrypted form of credentials
				myCredentialFileBytes = myCredentials == null ? null : myEncryptionService.Protect(value);
			}
		}

		/// <summary>
		/// Loads credentials from a file. Fail silently if unable to do so.
		/// </summary>
		/// <param name="aCredentialsObjectType">The type of the credentials object.</param>
		public void Load(Type aCredentialsObjectType)
		{
			myCredentials = null;

			if (myCredentialFileBytes == null)
			{
				myCredentialFileBytes = LoadCredentialBytesFromFile();
				if (myCredentialFileBytes == null)
				{
					// silently fail
					return;
				}
			}

			myCredentials = myEncryptionService.Unprotect(myCredentialFileBytes, aCredentialsObjectType);
		}

		/// <summary>
		/// Save the credentials to file. Fail silently if unable to do so.
		/// </summary>
		public void Save()
		{
			SaveCredentialBytesToFile(myCredentialFileBytes);
		}

		/// <summary>
		/// Delete credentials from file system.
		/// </summary>
		public void Delete()
		{
			myCredentials = null;
			myCredentialFileBytes = null;
			myCloudServicesUserInfo = null;

			if (File.Exists(CredentialFilePath))
			{
				try
				{
					File.Delete(CredentialFilePath);
				}
				catch
				{
					// silently fail
				}
			}
		}

		/// <summary>
		/// Attempt to check if the user's credentials data file is still on disk
		/// </summary>
		/// <returns>True if the file is still on disk, otherwise false</returns>
		public bool CheckIfCredentialsFileExists() => File.Exists(CredentialFilePath);

		#endregion

		#region ILoggedInUserService Members

		/// <summary>
		/// Information about logged in user. Null if there is no logged in user.
		/// </summary>
		public CloudServicesUserInfo LoggedInUserInfo
		{
			get
			{
				if (myCloudServicesUserInfo != null)
				{
					return myCloudServicesUserInfo;
				}

				if (myCredentialFileBytes == null)
				{
					myCredentialFileBytes = LoadCredentialBytesFromFile();

					if (myCredentialFileBytes == null)
					{
						return null;
					}
				}

				myCloudServicesUserInfo = (CloudServicesUserInfo) 
					myEncryptionService.Unprotect(myCredentialFileBytes, typeof(CloudServicesUserInfo));

				return myCloudServicesUserInfo;
			}
		}

		#endregion

		#region Internal Properties

		/// <summary>
		/// Get the singleton instance of the class
		/// </summary>
		internal static CredentialStorageService Instance => myInstance ?? (myInstance = new CredentialStorageService(null));

		#endregion

		#region Private Static Methods & Properties

		/// <summary>
		/// Save/Update the credentials file with the bytes array
		/// </summary>
		/// <param name="credentialBytes"></param>
		private static void SaveCredentialBytesToFile(byte[] credentialBytes)
		{
			if (credentialBytes == null)
			{
				return;
			}

			try
			{
				File.WriteAllBytes(CredentialFilePath, credentialBytes);
			}
			catch
			{
				// silently fail
			}
		}

		/// <summary>
		/// Open and read the contents of the credentials file a byte array
		/// </summary>
		private static byte[] LoadCredentialBytesFromFile()
		{
			byte[] fileBytes = null;

			try
			{
				if (File.Exists(CredentialFilePath))
				{
					using (Stream stream = new FileStream(CredentialFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
					{
						int numBytes = (int) stream.Length;
						fileBytes = new byte[numBytes];
						stream.Read(fileBytes, 0, numBytes);
					}
				}
			}
			catch
			{
				// silently fail
			}

			return fileBytes;
		}

		/// <summary>
		/// Full file path to where the credentials are stored (per windows user)
		/// </summary>
		private static string CredentialFilePath
		{
			get
			{
				string fullPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
					QSR_PLATFORMSIDENTITY_FOLDER_NAME);

				if (!Directory.Exists(fullPath))
				{
					try
					{
						Directory.CreateDirectory(fullPath);
					}
					catch
					{
						// silently fail
					}
				}

				return Path.Combine(fullPath, CREDENTIAL_FILE);
			}
		}

		#endregion

		#region Private Variables

		/// <summary>
		/// Information about the Uluru Cloud services logged in user
		/// </summary>
		private CloudServicesUserInfo myCloudServicesUserInfo;

		/// <summary>
		/// The credentials object pertaining the user
		/// </summary>
		private object myCredentials;

		/// <summary>
		/// The contents of the credentials to be encrypted and saved
		/// </summary>
		private byte[] myCredentialFileBytes;

		/// <summary>
		/// The service which will encrypt the credentials
		/// </summary>
		private readonly IEncryptionService myEncryptionService;

		/// <summary>
		/// The singleton instance of the class
		/// </summary>
		private static CredentialStorageService myInstance;

		#endregion

		#region Private Constants

		/// <summary>
		/// The file used for storing credentials. PlatformsIdentityCredentialsData.dat
		/// </summary>
		private const string CREDENTIAL_FILE = @"picd.dat";

		/// <summary>
		/// The QSR PlatformsIdentity folder used for settings/credentials for NVivo Services
		/// </summary>
		private const string QSR_PLATFORMSIDENTITY_FOLDER_NAME = @"QSR_International\PlatformsIdentity";

		#endregion
	}
}
