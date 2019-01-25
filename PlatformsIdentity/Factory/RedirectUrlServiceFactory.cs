using QSR.NVivo.Plugins.PlatformsIdentity.Services;
using IRedirectUrlService = QSR.NVivo.Plugins.PlatformsIdentity.Services.IRedirectUrlService;

namespace QSR.NVivo.Plugins.PlatformsIdentity.Factory
{
	/// <summary>
	/// default implementation of <see cref="IRedirectUrlService"/> which creates <see cref="RedirectUrlService"/> instances
	/// </summary>
	public class RedirectUrlServiceFactory
	{
		public static IRedirectUrlService ServiceInstance => RedirectUrlService.Instance;
	}
}
