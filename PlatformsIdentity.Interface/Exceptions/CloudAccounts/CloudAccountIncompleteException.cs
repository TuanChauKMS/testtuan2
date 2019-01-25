using System;

namespace QSR.NVivo.Plugins.PlatformsIdentity.Interface.Exceptions.CloudAccounts
{
	public class CloudAccountIncompleteException : Exception
	{
		public CloudAccountIncompleteException() : base(string.Empty)
		{
		}

		public CloudAccountIncompleteException(string aMessage) : base(aMessage)
		{
		}
	}
}
