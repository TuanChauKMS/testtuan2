using System;

namespace QSR.NVivo.Plugins.PlatformsIdentity.Interface.Exceptions.CloudAccounts
{
	public class FailedRetrievingRemainingCreditException : Exception
	{
		public FailedRetrievingRemainingCreditException() : base(string.Empty)
		{
		}

		public FailedRetrievingRemainingCreditException(string aMessage) : base(aMessage)
		{
		}
	}
}
