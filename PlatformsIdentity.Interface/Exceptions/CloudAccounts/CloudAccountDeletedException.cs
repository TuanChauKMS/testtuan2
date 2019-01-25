using System;

namespace QSR.NVivo.Plugins.PlatformsIdentity.Interface.Exceptions.CloudAccounts
{
	public class CloudAccountDeletedException : Exception
	{
		public CloudAccountDeletedException() : base(string.Empty)
		{
		}

		public CloudAccountDeletedException(string aMessage) : base(aMessage)
		{
		}
	}
}
