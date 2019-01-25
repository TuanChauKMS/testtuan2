using System;

namespace QSR.NVivo.Plugins.PlatformsIdentity.Interface.Exceptions.Authentication
{
	/// <summary>
	/// Trying to refresh with an invalid or revoked refresh token
	/// </summary>
	public class InvalidRefreshTokenException : Exception
	{
		public InvalidRefreshTokenException() : base(string.Empty)
		{
		}

		public InvalidRefreshTokenException(string aMessage) : base(aMessage)
		{
		}
	}
}
