using System;

namespace QSR.NVivo.Plugins.PlatformsIdentity.Interface.Exceptions.Authentication
{
	/// <summary>
	/// User (credentials) data file is deleted in-session
	/// </summary>
	public class DeletedUserDataException : Exception
	{
		public DeletedUserDataException() : base(string.Empty)
		{
		}

		public DeletedUserDataException(string aMessage) : base(aMessage)
		{
		}
	}
}