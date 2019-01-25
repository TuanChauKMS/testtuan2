using System;
using Newtonsoft.Json;

namespace QSR.NVivo.Plugins.PlatformsIdentity.Responses
{
	/// <summary>
	/// Encapsulates the StatusCode and Message from a RestResponse
	/// </summary>
	[Serializable]
	internal class RestErrorResponse
	{
		#region Properties

		[JsonProperty("statusCode")]
		public long StatusCode { get; set; }

		[JsonProperty("message")]
		public string Message { get; set; }

		#endregion

	}
}
