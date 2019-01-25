using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;

using QSR.NVivo.Plugins.PlatformsIdentity.Interface.Interfaces;

namespace QSR.NVivo.Plugins.PlatformsIdentity.Interface
{
	/// <summary>
	/// Encrypts/decrypts credentials for storage.
	/// </summary>
	internal class EncryptionService : IEncryptionService
	{
		/// <summary>
		/// Protect the given object using the user's data protection scope. Serializes the object as JSON, then encrypts.
		/// </summary>
		/// <param name="anObject">The object to protect.</param>
		/// <returns>The encrypted bytes, otherwise null on failure.</returns>
		public byte[] Protect(object anObject)
		{
			byte[] result = null;

			try
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					DataContractJsonSerializer serializer = new DataContractJsonSerializer(anObject.GetType());
					serializer.WriteObject(memoryStream, anObject);

					result = ProtectedData.Protect(memoryStream.ToArray(), ADDITIONAL_PROTECTION_ENTROPY, DataProtectionScope.CurrentUser);
				}
			}
			catch
			{
			}

			return result;
		}

		/// <summary>
		/// Unprotects the given byte array using the current user's data protection scope. Decrypts the bytes, then
		/// deserializes from JSON to the given object type.
		/// </summary>
		/// <param name="anEncryptedByteArray">The encrypted byte array as returned by Protect</param>
		/// <param name="anObjectType">The type of the object represented by the byte array.</param>
		/// <returns>The unprotected object, otherwise null or empty on failure</returns>
		public object Unprotect(byte[] anEncryptedByteArray, Type anObjectType)
		{
			object result = null;

			try
			{
				byte[] decrypted = ProtectedData.Unprotect(anEncryptedByteArray, ADDITIONAL_PROTECTION_ENTROPY, DataProtectionScope.CurrentUser);

				using (MemoryStream memoryStream = new MemoryStream(decrypted))
				{
					DataContractJsonSerializer serializer = new DataContractJsonSerializer(anObjectType);
					result = serializer.ReadObject(memoryStream);
				}
			}
			catch
			{
			}

			return result;
		}

		/// <summary>
		/// Additional entropy for protecting/unprotecting strings.
		/// https://security.stackexchange.com/questions/20358/what-is-the-purpose-of-the-entropy-parameter-for-dpapi-protect
		/// </summary>
		private static readonly byte[] ADDITIONAL_PROTECTION_ENTROPY =
		{
			0x49, 0xac, 0xfd, 0xec, 0x17, 0xfa, 0x08, 0xbd, 0x53, 0xf8
		};
	}
}
