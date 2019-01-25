using RestSharp;

namespace QSR.NVivo.Plugins.PlatformsIdentity.Factory
{
	/// <summary>
	/// Interface for creating <see cref="IRestClient"/> instances in the <see cref="Auth0ServiceProxy"/>
	/// </summary>
	public interface IRestClientFactory
	{
		/// <summary>
		/// Create a new Rest Client which uses the base url to make requests
		/// </summary>
		/// <param name="baseUrl">The base Url which the requests will be formed on</param>
		/// <returns></returns>
		IRestClient Create(string baseUrl);

		/// <summary>
		/// Create a new Rest Client which uses the base url to make requests
		/// </summary>
		/// <param name="baseUrl">The base Url as dynamic type which the requests will be formed on</param>
		/// <returns></returns>
		IRestClient Create(dynamic baseUrl);
	}
}
