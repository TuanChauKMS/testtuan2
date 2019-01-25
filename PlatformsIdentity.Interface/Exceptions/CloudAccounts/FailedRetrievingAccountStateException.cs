using System;

namespace QSR.NVivo.Plugins.PlatformsIdentity.Interface.Exceptions.CloudAccounts
{
	public class FailedRetrievingAccountStateException : Exception
	{
		public FailedRetrievingAccountStateException() : base(string.Empty)
		{
		}

		public FailedRetrievingAccountStateException(string aMessage) : base(aMessage)
		{
		}
	}
}
