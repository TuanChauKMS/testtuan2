using RestSharp;

namespace QSR.NVivo.Plugins.PlatformsIdentity.Factory
{
	/// <summary>
	/// default implementation of <see cref="IRestClientFactory"/> which creates <see cref="RestClient"/> instances
	/// </summary>
	class RestClientFactory : IRestClientFactory
	{
		/// <summary>
		/// Create a new Rest Client which uses the base url to make requests
		/// </summary>
		/// <param name="baseUrl">The base Url which the requests will be formed on</param>
		public IRestClient Create(string baseUrl)
		{
			return new RestClient(baseUrl) { Timeout = REST_DEFAULT_TIMEOUT_MS };
		}

		/// <summary>
		/// Create a new Rest Client which uses the base url of dynamic type to make requests
		/// </summary>
		/// <param name="baseUrl">The base Url which the requests will be formed on</param>
		public IRestClient Create(dynamic baseUrl)
		{
			return Create(baseUrl.ToString());
		}

		/// <summary>
		/// Default timeout (in milliseconds) when making a REST call.
		/// </summary>
		private const int REST_DEFAULT_TIMEOUT_MS = 30 * 1000;
	}
}
