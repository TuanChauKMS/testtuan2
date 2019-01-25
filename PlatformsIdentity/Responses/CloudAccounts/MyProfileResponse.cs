using Newtonsoft.Json;

namespace QSR.NVivo.Plugins.PlatformsIdentity.Responses.CloudAccounts
{
	/// <summary>
	/// Response representing the Uluru Accounts API MyProfile DTO
	/// </summary>
	public class MyProfileResponse
	{
		[JsonProperty("userProfile")]
		public UserProfile UserProfile { get; set; }
	}
}
