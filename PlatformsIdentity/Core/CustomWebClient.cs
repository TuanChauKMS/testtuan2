using System;
using System.Net;

namespace QSR.NVivo.Plugins.PlatformsIdentity.Core
{
	/// <summary>
	/// Custom WebClient inherited from System.Net.WebClient
	/// </summary>
	internal class CustomWebClient : System.Net.WebClient
	{
		public long GetContentLength(Uri uri)
		{
			var req = WebRequest.Create(uri);
			req.Method = "HEAD";

			using (var resp = req.GetResponse())
			{
				if (long.TryParse(resp.Headers.Get("Content-Length"), out var contentSize))
				{
					return contentSize;
				}

				return 0;
			}
		}
	}
}