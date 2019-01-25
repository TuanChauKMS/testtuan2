namespace QSR.NVivo.Plugins.PlatformsIdentity.Services
{
	public interface IRedirectUrlService
	{
		/// <summary>
		/// Returns content of url json file as a dynamic type. In case that json content changes, the interface will be isolated.
		/// </summary>
		/// <returns>decrypted content value of url redirect json file.</returns>
		dynamic UrlContent { get; }
	}
}