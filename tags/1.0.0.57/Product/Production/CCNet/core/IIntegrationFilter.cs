using System;

namespace ThoughtWorks.CruiseControl.Core
{
	public interface IIntegrationFilter
	{
		bool ShouldRunBuild(IIntegrationResult result);
	}
}
