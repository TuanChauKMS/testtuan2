using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using QSR.NVivo.Plugins.PlatformsIdentity.Core;

namespace QSR.NVivo.Plugins.PlatformsIdentity.Services
{
	/// <summary>
	/// Service provider for reading and decrypting the content of redirect url json file 
	/// </summary>
	public class RedirectUrlService : IRedirectUrlService
	{
		#region Public Properties

		/// <summary>
		/// to get the decrypted content of the redirect url json file in dynamic form. 
		/// </summary>
		/// <returns></returns>
		public dynamic UrlContent
		{
			get
			{
				if (myUrlContent != null)
					return myUrlContent;

				var redirectUrl = new Uri(REDIRECT_FILE_CONTAINER_URL);

				var webClient = new CustomWebClient();

				if (webClient.GetContentLength(redirectUrl) == 0)
				{
					return null;
				}

				string responseStr = webClient.DownloadString(redirectUrl);

				myUrlContent = JsonConvert.DeserializeObject<dynamic>(Decrypt(responseStr, ASE_KEY, ASE_SALT));

				return myUrlContent;
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Decrypts the encrypted file content
		/// </summary>
		/// <param name="fileContent">encrypted file content</param>
		/// <param name="key">AES key or password to decrypt the file content</param>
		/// <param name="salt">AES Salt value to decrypt the file content</param>
		/// <returns></returns>
		private static string Decrypt(string fileContent, string key, string salt)
		{
			var keyByte = Encoding.ASCII.GetBytes(key);
			var ivByte = Encoding.ASCII.GetBytes(salt);

			using (AesCryptoServiceProvider myAes = new AesCryptoServiceProvider { Key = keyByte, IV = ivByte })
			{
				// Decrypt the bytes to a string.
				var plainUrls = DecryptStringFromBytesAES(Convert.FromBase64String(fileContent), myAes.Key, myAes.IV);
				return plainUrls;
			}
		}

		#region AES Symmetric Encryption

		/// <summary>
		/// This method can decrypt an AES encrypted string. (e.g. Redirect url file content)
		/// </summary>
		/// <param name="cipherText">The encrypted string</param>
		/// <param name="Key">AES password or key</param>
		/// <param name="iv">AES salt value</param>
		/// <returns></returns>
		private static string DecryptStringFromBytesAES(byte[] cipherText, byte[] Key, byte[] iv)
		{
			// Check arguments.
			if (cipherText == null || cipherText.Length <= 0)
				throw new ArgumentNullException("cipherText");
			if (Key == null || Key.Length <= 0)
				throw new ArgumentNullException("Key");
			if (iv == null || iv.Length <= 0)
				throw new ArgumentNullException("iv");

			// Declare the string used to hold
			// the decrypted text.
			string plaintext = null;

			// Create an AesCryptoServiceProvider object
			// with the specified key and IV.
			using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
			{
				aesAlg.Key = Key;
				aesAlg.IV = iv;

				// Create a decryptor to perform the stream transform.
				ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

				// Create the streams used for decryption.
				using (MemoryStream msDecrypt = new MemoryStream(cipherText))
				{
					using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
					{
						using (StreamReader srDecrypt = new StreamReader(csDecrypt))
						{
							// Read the decrypted bytes from the decrypting stream
							// and place them in a string.
							plaintext = srDecrypt.ReadToEnd();
						}
					}
				}
			}

			return plaintext;
		}

		#endregion

		#endregion

		#region Static Members

		/// <summary>
		/// static variable to keep file content and to avoid downloading the file every time needed
		/// </summary>
		private static dynamic myUrlContent;

		/// <summary>
		/// static instance of the class
		/// </summary>
		private static RedirectUrlService myInstance;

		/// <summary>
		/// Provides a Singleton instance of <see cref="RedirectUrlService"/> class
		/// </summary>
		internal static RedirectUrlService Instance => myInstance ?? (myInstance = new RedirectUrlService());

		#endregion

		#region Contants

#if PRODUCTION
		/// <summary>
		/// Redirect URL json file path
		/// </summary>
		internal const string REDIRECT_FILE_CONTAINER_URL = @"https://nvivoweb.qsrinternational.com/CloudRedirections/NVivo12Win/resources-win-prod.json";
#else
		/// <summary>
		/// Redirect URL json file path
		/// </summary>
		internal const string REDIRECT_FILE_CONTAINER_URL = @"https://nvivoweb.qsrtest.com/CloudRedirections/NVivo12Win/resources-win-test.json";
#endif
		/// <summary>
		/// The Key and the Salt const variables here are required to decrypt the content of the redirect url json file, after downloading from server.
		/// This a symmetric encryption. If you need to change these credentials, you need to re-generate the json file again and upload it on the server.
		/// To re-generate the url json file, go to the TestApplication project and modify it to use new pair of Key and Salt value and run it. Then in the poped up form,
		/// press encryption button to get the new encrypted cipher. Then upload it to the server.
		/// You need to do the same if need to add/remove url from the file.
		///
		/// The reason we keep these credentials in this class and then pass it into the FactoryClass is because we want to keep interface clean as we do not obfuscate interface classes.
		/// Also, as interface will be consumed inside NVivo directly, any change in these keys leads to change in Interface classes that leads to a new version in NVivo.
		/// </summary>
		private const string ASE_KEY = "uvdM64XHe375UB7rU!6ZE7^SE3h8SW*A";
		private const string ASE_SALT = "WJ872d^uPmA4H*57";

		#endregion
	}
}