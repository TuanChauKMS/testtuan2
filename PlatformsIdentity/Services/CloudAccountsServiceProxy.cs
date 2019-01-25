using System;
using System.Linq;
using System.Net;

using QSR.NVivo.Plugins.PlatformsIdentity.Factory;
using QSR.NVivo.Plugins.PlatformsIdentity.Interface;
using QSR.NVivo.Plugins.PlatformsIdentity.Interface.Exceptions.CloudAccounts;
using QSR.NVivo.Plugins.PlatformsIdentity.Responses;
using QSR.NVivo.Plugins.PlatformsIdentity.Responses.CloudAccounts;

using Newtonsoft.Json;
using RestSharp;

namespace QSR.NVivo.Plugins.PlatformsIdentity.Services
{
	/// <summary>
	/// Service Proxy which handles calls to Uluru Accounts API
	/// </summary>
	public class CloudAccountsServiceProxy : ICloudAccountsServiceProxy
	{
		#region Constructors

		public CloudAccountsServiceProxy(IRestClientFactory restClientFactory)
		{
			myRestClientFactory = restClientFactory;
		}

		#endregion

		#region Cloud Accounts Interface

		/// <summary>
		/// Get login user account id
		/// </summary>
		/// <param name="accessToken">user access token</param>
		/// <returns>an account id</returns>
		public long GetAccountId(string accessToken)
		{
			IRestClient client = myRestClientFactory.Create(MyUrlContent.api.accountApiUrl);
			var request = new RestRequest("/myprofile", Method.GET);

			SetCommonHeader(request, accessToken);

			int retry = 0;
			IRestResponse response;
			do
			{
				response = client.Execute(request);
				retry++;
			} while (retry < NUM_RETRY && response?.StatusCode == HttpStatusCode.RequestTimeout);

			if (response?.StatusCode != HttpStatusCode.OK)
			{
				throw new FailedRetrievingRemainingCreditException(GetErrorMessage(response));
			}

			MyProfileResponse profile = JsonConvert.DeserializeObject<MyProfileResponse>(response.Content);

			return profile.UserProfile.AccountId;
		}

		/// <summary>
		/// Get logged in user account state
		/// </summary>
		/// <param name="accessToken">user access token</param>
		/// <returns>The <see cref="CloudAccountState"/> of the logged in user</returns>
		public CloudAccountState GetAccountState(string accessToken)
		{
			IRestClient restClient = myRestClientFactory.Create(MyUrlContent.api.accountApiUrl);
			var restRequest = new RestRequest("/myprofile", Method.GET);

			SetCommonHeader(restRequest, accessToken);

			int retry = 0;
			IRestResponse restResponse;

			do
			{
				restResponse = restClient.Execute(restRequest);
				retry++;
			} while (retry < NUM_RETRY && restResponse?.StatusCode == HttpStatusCode.RequestTimeout);

			if (restResponse?.StatusCode != HttpStatusCode.OK)
			{
				throw new FailedRetrievingAccountStateException(GetErrorMessage(restResponse));
			}

			MyProfileResponse profile = JsonConvert.DeserializeObject<MyProfileResponse>(restResponse.Content);

			return (CloudAccountState) profile.UserProfile.AccountStateId;
		}

		/// <summary>
		/// Get the remaining transcription credits
		/// </summary>
		/// <param name="accessToken">user access token</param>
		/// <param name="accountId">user account id</param>
		/// <param name="productId">product id to retrieve the product. For transcription product it is 1</param>
		/// <returns>the remaining credits in minutes</returns>
		public long GetRemainingCredit(string accessToken, long accountId, int productId)
		{
			IRestClient client = myRestClientFactory.Create(MyUrlContent.api.accountApiUrl);
			var request = new RestRequest($"/{accountId}/productcreditsremaining", Method.GET);
			request.AddParameter("productId", productId);
			SetCommonHeader(request, accessToken);

			int retry = 0;
			IRestResponse response;
			do
			{
				response = client.Execute(request);
				retry++;
			} while (retry < NUM_RETRY && response?.StatusCode == HttpStatusCode.RequestTimeout);

			if (response?.StatusCode != HttpStatusCode.OK)
			{
				throw new FailedRetrievingRemainingCreditException(GetErrorMessage(response));
			}

			RemainingCreditResponse credit = JsonConvert.DeserializeObject<RemainingCreditResponse>(response.Content);

			return credit.RemainingCreditsByProduct.First(x => x.ProductId == productId).RemainingCredits;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Deserialize the Rest Response into a RestErrorResponse object
		/// </summary>
		/// <returns></returns>
		private string GetErrorMessage(IRestResponse response)
		{
			RestErrorResponse error = JsonConvert.DeserializeObject<RestErrorResponse>(response.Content);
			return error != null ? error.Message : response.StatusCode.ToString();
		}

		private void SetCommonHeader(IRestRequest request, string accessToken)
		{
			if (request != null && !string.IsNullOrEmpty(accessToken))
			{
				request.AddHeader("Authorization", $"Bearer {accessToken}");
				request.AddHeader("Ocp-Apim-Subscription-Key", SubscriptionKey);
				request.AddHeader("Ocp-Apim-Trace", "True");
			}
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// The insufficient credit page Url property
		/// </summary>
		public string BuyCreditPageUrl => MyUrlContent.external.buyCreditPageUrl;

		/// <summary>
		/// The complete account signup page Url property
		/// </summary>
		public string CompleteAccountSignUpUrl => MyUrlContent.external.completeAccountSignUpUrl;

		#endregion

		#region Private Properties

		/// <summary>
		/// Get the SubscriptionKey used to make API calls
		/// </summary>
		private string SubscriptionKey
		{
			get
			{
				if (string.IsNullOrEmpty(mySubscriptionKey))
				{
					string obfuscatedKey = (string) MyUrlContent.api.obfuscatedSubscriptionKey;
					byte[] key =
						Convert.FromBase64String((new string(obfuscatedKey.Reverse().ToArray())));

					mySubscriptionKey = System.Text.Encoding.UTF8.GetString(key);
				}

				return mySubscriptionKey;
			}
		}

		#endregion

		#region Private Members

		/// <summary>
		/// Create a Rest Client to make API calls
		/// </summary>
		private readonly IRestClientFactory myRestClientFactory;

		/// <summary>
		/// Provides all needed urls to call Auth0 API
		/// NOTE: DO NOT use this private field directly. Use it's property for null checking and exception handling instead.
		/// </summary>
		private static dynamic myUrlContent;

		/// <summary>
		/// Private property for myUrlContent. USE THIS for null checking and exception handling.
		/// </summary>
		private static dynamic MyUrlContent
		{
			get
			{
				if (myUrlContent == null)
				{
					myUrlContent = RedirectUrlServiceFactory.ServiceInstance.UrlContent;
				}

				return myUrlContent;
			}
		}

		/// <summary>
		/// Key used to make API calls
		/// </summary>
		private string mySubscriptionKey;

		private const int NUM_RETRY = 2;

		#endregion
	}
}
