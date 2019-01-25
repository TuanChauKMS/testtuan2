using System;

namespace QSR.NVivo.Plugins.PlatformsIdentity.Interface.Exceptions.CloudAccounts
{
	public class CloudAccountSuspendedException : Exception
	{
		public CloudAccountSuspendedException() : base(string.Empty)
		{
		}

		public CloudAccountSuspendedException(string aMessage) : base(aMessage)
		{
		}
	}
}
