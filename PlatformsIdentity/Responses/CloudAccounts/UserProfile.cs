using Newtonsoft.Json;

namespace QSR.NVivo.Plugins.PlatformsIdentity.Responses.CloudAccounts
{
	/// <summary>
	/// Response representing the Uluru Accounts API UserProfile DTO
	/// </summary>
	public class UserProfile
	{
		[JsonProperty("accountId")]
		public long AccountId { get; set; }

		[JsonProperty("accountStateId")]
		public int AccountStateId { get; set; }
	}
}
