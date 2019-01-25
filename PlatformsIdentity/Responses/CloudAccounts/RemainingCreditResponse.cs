using Newtonsoft.Json;

namespace QSR.NVivo.Plugins.PlatformsIdentity.Responses.CloudAccounts
{
	/// <summary>
	/// Response representing the Uluru Accounts API RemainingCreditResponse DTO
	/// </summary>
	public class RemainingCreditResponse
	{
		[JsonProperty("accountId")]
		public long AccountId { get; set; }

		[JsonProperty("remainingCreditsByProduct")]
		public RemainingCreditsByProduct[] RemainingCreditsByProduct { get; set; }
	}
}