using Newtonsoft.Json;

namespace QSR.NVivo.Plugins.PlatformsIdentity.Responses.CloudAccounts
{
	/// <summary>
	/// Response representing the Uluru Accounts API RemainingCreditsByProduct DTO
	/// </summary>
	public class RemainingCreditsByProduct
	{
		[JsonProperty("productId")]
		public long ProductId { get; set; }

		[JsonProperty("remainingCredits")]
		public long RemainingCredits { get; set; }
	}
}
