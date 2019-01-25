using QSR.NVivo.Plugins.PlatformsIdentity.Interface;

namespace QSR.NVivo.Plugins.PlatformsIdentity.Services
{
	/// <summary>
	/// Service Proxy which handles calls to Uluru Accounts API
	/// </summary>
	public interface ICloudAccountsServiceProxy
	{
		/// <summary>
		/// Get logged in user account id
		/// </summary>
		/// <param name="accessToken">user access token</param>
		/// <returns>an account id</returns>
		long GetAccountId(string accessToken);

		/// <summary>
		/// Get logged in user account state
		/// </summary>
		/// <param name="accessToken">user access token</param>
		/// <returns>The <see cref="CloudAccountState"/> of the logged in user</returns>
		CloudAccountState GetAccountState(string accessToken);

		/// <summary>
		/// Get the remaining transcription credits
		/// </summary>
		/// <param name="accessToken">user access token</param>
		/// <param name="accountId">user account id</param>
		/// <param name="productId">product id to retrieve the product. For transcription product it is 1</param>
		/// <returns>the remaining credits in minutes</returns>
		long GetRemainingCredit(string accessToken, long accountId, int productId);

		/// <summary>
		/// The insufficient credit page base Url property
		/// </summary>
		string BuyCreditPageUrl { get; }

		/// <summary>
		/// The complete account signup page Url property
		/// </summary>
		string CompleteAccountSignUpUrl { get; }
	}
}
