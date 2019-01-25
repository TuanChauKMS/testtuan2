using System;

namespace QSR.NVivo.Plugins.PlatformsIdentity.Interface.Exceptions.Authentication
{
	/// <summary>
	/// The general exception to throw when Auth0 returns an unknown exception
	/// </summary>
	public class Auth0Exception : Exception
	{
		public Auth0Exception() : base(string.Empty)
		{
		}

		public Auth0Exception(string aMessage) : base(aMessage)
		{
		}
	}
}
