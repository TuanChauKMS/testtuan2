namespace QSR.NVivo.Plugins.PlatformsIdentity.Interface
{
	/// <summary>
	/// The information about the Cloud Services user, exposed to NVivo.
	/// </summary>
	public class CloudServicesUserInfo
	{
		/// <summary>
		/// The logged in user nickname
		/// </summary>
		public string UserNickName { get; set; }

		/// <summary>
		/// The logged in user email address
		/// </summary>
		public string UserEmail { get; set; }
	}
}