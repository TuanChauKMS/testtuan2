using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

using QSR.NVivo.Plugins.PlatformsIdentity.Core;
using QSR.NVivo.Plugins.PlatformsIdentity.Interface;
using QSR.NVivo.Plugins.PlatformsIdentity.Interface.Factories;
using QSR.NVivo.Plugins.PlatformsIdentity.Interface.Interfaces;

namespace TestApplication
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
			_myPIPlugin = new PlatformsIdentityPlugin(null);
		}

		private void LoginButton_Click(object sender, EventArgs e)
		{
			_myPIPlugin.Login(this);
			CheckUserInfo();
		}

		private void ReAuthenticateButton_Click(object sender, EventArgs e)
		{
			_myPIPlugin.AuthenticateFromStoredCredentials(this);
			CheckUserInfo();
		}

		private void LogoutButton_Click(object sender, EventArgs e)
		{
			_myPIPlugin.Logout(this);
			CheckUserInfo();
		}

		private async void GetAccessToken_Click(object sender, EventArgs e)
		{
			string accessToken = await _myPIPlugin.GetValidAccessToken();
			CheckUserInfo();
		}

		private static void CheckUserInfo()
		{
			ILoggedInUserService userService = LoggedInUserServiceFactory.ServiceInstance;
			CloudServicesUserInfo userInfo = userService.LoggedInUserInfo;

			if (userInfo != null)
			{
				Debug.WriteLine(userInfo.UserNickName);
			}
			else
			{
				MessageBox.Show(@"Null User Info");
			}
		}

		private readonly PlatformsIdentityPlugin _myPIPlugin;

		private void BtnRedirectEncryption_Click(object sender, EventArgs e)
		{
			try
			{
				/* IMPORTANT!
				 * PLEASE MAKE SURE THE KEY AND IV IN THIS METHOD (FOR ENCRYPTION) ARE IDENTICAL WITH THE ONE IN RedirectUrlProvider.cs class (FOR DECRYPTION).
				 * ANY INCONSISTENCY BETWEEN THESE TWO PLACES RESULTS IN FAILING AUTHENTICATION AS DECRYPTION FAILES.
				 */
				var key = Encoding.ASCII.GetBytes("uvdM64XHe375UB7rU!6ZE7^SE3h8SW*A");
				var iv = Encoding.ASCII.GetBytes("WJ872d^uPmA4H*57");

				using (AesCryptoServiceProvider myAes = new AesCryptoServiceProvider() { Key = key, IV = iv })
				{
					// Encrypt the string to an array of bytes.
					byte[] encrypted = EncryptStringToBytesAES(generalTxtBox.Text, myAes.Key, myAes.IV);

					var base64Cipher =  Convert.ToBase64String(encrypted);

					generalTxtBox.Text = base64Cipher;
					btnRedirectEncryption.Enabled = false;
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		private static byte[] EncryptStringToBytesAES(string plainText, byte[] Key, byte[] iv)
		{
			// Check arguments.
			if (plainText == null || plainText.Length <= 0)
				throw new ArgumentNullException("plainText");
			if (Key == null || Key.Length <= 0)
				throw new ArgumentNullException("Key");
			if (iv == null || iv.Length <= 0)
				throw new ArgumentNullException("iv");
			byte[] encrypted;

			// Create an AesCryptoServiceProvider object
			// with the specified key and IV.
			using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
			{
				aesAlg.Key = Key;
				aesAlg.IV = iv;

				// Create an encryptor to perform the stream transform.
				ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

				// Create the streams used for encryption.
				using (MemoryStream msEncrypt = new MemoryStream())
				{
					using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
					{
						using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
						{
							//Write all data to the stream.
							swEncrypt.Write(plainText);
						}
						encrypted = msEncrypt.ToArray();
					}
				}
			}

			// Return the encrypted bytes from the memory stream.
			return encrypted;
		}

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

		private void DecryptButton_Click(object sender, EventArgs e)
		{
			/* IMPORTANT!
				 * PLEASE MAKE SURE THE KEY AND IV IN THIS METHOD (FOR ENCRYPTION) ARE IDENTICAL WITH THE ONE IN RedirectUrlProvider.cs class (FOR DECRYPTION).
				 * ANY INCONSISTENCY BETWEEN THESE TWO PLACES RESULTS IN FAILING AUTHENTICATION AS DECRYPTION FAILES.
				 */
			var key = Encoding.ASCII.GetBytes("uvdM64XHe375UB7rU!6ZE7^SE3h8SW*A");
			var iv = Encoding.ASCII.GetBytes("WJ872d^uPmA4H*57");

			using (AesCryptoServiceProvider myAes = new AesCryptoServiceProvider() { Key = key, IV = iv })
			{
				// Encrypt the string to an array of bytes.
				string plain = DecryptStringFromBytesAES(Convert.FromBase64String(generalTxtBox.Text), myAes.Key, myAes.IV);


				generalTxtBox.Text = plain;
				decryptJsonButton.Enabled = false;
			}
		}

		private void GetCreditButtn_Click(object sender, EventArgs e)
		{
			long credit = _myPIPlugin.GetRemainingCredits(generalTxtBox.Text);
		}

		private async void GetCreditAsyncButtn_Click(object sender, EventArgs e)
		{
			long credit = await _myPIPlugin.GetRemainingCreditsAsync(generalTxtBox.Text);
		}

		private void BuyCreditBttn_Click(object sender, EventArgs e)
		{
			_myPIPlugin.OpenBuyCreditPage();
		}
	}
}