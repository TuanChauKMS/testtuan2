using System.Threading.Tasks;
using System.Windows.Forms;

namespace QSR.NVivo.Plugins.PlatformsIdentity.Interface.Interfaces
{
	/// <summary>
	/// Allows the user to login to NVivo-provided services
	/// </summary>
	public interface IPlatformsIdentityPlugin
	{
		#region Authentication

		/// <summary>
		/// Logs the user into NVivo Platforms
		/// </summary>
		/// <param name="parentControl">The control or form to be set as the owner of the dialog</param>
		/// <returns>True if authenticated, false if the user cancelled</returns>
		bool Login(Control parentControl);

		/// <summary>
		/// Make use of the stored credentials, if any, to authenticate the user
		/// </summary>
		/// <param name="parentControl">The control or form to be set as the owner for the progress dialog</param>
		/// <returns>True if authentication was successful, false if no stored credentials
		/// or the stored credentials are invalid.</returns>
		bool AuthenticateFromStoredCredentials(Control parentControl);

		/// <summary>
		/// Clears any stored/cached credentials and logs the user out
		/// </summary>
		/// <param name="parentControl">The control or form to be set as the owner for any dialog shown.</param>
		void Logout(Control parentControl);

		#endregion

		#region Cloud Accounts

		/// <summary>
		/// Gets the remaining available credit
		/// </summary>
		/// <param name="accessToken">requested user</param>
		/// <returns>the number of available credit in minutes</returns>
		/// <exception cref="InvalidAccessTokenException"></exception>
		/// <exception cref="FailedRetrievingRemainingBalanceException"></exception>
		/// <exception cref="InvalidResponseException"></exception>
		Task<long> GetRemainingCreditsAsync(string accessToken);

		/// <summary>
		/// Gets the remaining available credit
		/// </summary>
		/// <param name="accessToken">requested user</param>
		/// <returns>the number of available credit in minutes</returns>
		/// <exception cref="InvalidAccessTokenException"></exception>
		/// <exception cref="FailedRetrievingRemainingBalanceException"></exception>
		/// <exception cref="InvalidResponseException"></exception>
		long GetRemainingCredits(string accessToken);

		/// <summary>
		/// Launch the insufficient credit page externally
		/// </summary>
		void OpenBuyCreditPage();

		/// <summary>
		/// Launch the complete account signup page externally
		/// </summary>
		void OpenCompleteAccountSignUpPage();

		#endregion

		#region Utilities

		/// <summary>
		/// Allow other plugins to get the current user's access token to make API calls.
		/// Attempt to always retrieve a valid token (by refresh token if the previous token expired)
		/// </summary>
		/// <returns></returns>
		Task<string> GetValidAccessToken();

		#endregion
	}
}
