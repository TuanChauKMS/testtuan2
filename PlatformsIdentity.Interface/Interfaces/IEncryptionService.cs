using System;

namespace QSR.NVivo.Plugins.PlatformsIdentity.Interface.Interfaces
{
	/// <summary>
	/// Encrypt/decrypt service for persiting user's credentials
	/// </summary>
	internal interface IEncryptionService
	{
		/// <summary>
		/// Protect the given object using the user's data protection scope
		/// </summary>
		/// <param name="anObject">The object to protect</param>
		/// <returns>The encrypted bytes, otherwise null on failure</returns>
		byte[] Protect(object anObject);

		/// <summary>
		/// Unprotects the given byte array using the current user's key
		/// </summary>
		/// <param name="anEncryptedByteArray">The encrypted byte array as returned by Protect</param>
		/// <param name="anObjectType">The type of the object represented by the byte array.</param>
		/// <returns>The unprotected object, otherwise null or empty on failure</returns>
		object Unprotect(byte[] anEncryptedByteArray, Type anObjectType);
	}
}
