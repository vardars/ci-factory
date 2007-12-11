using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core
{
	public interface IIntegrationResultManager
	{
		IIntegrationResult LastIntegrationResult { get; }

		IIntegrationResult StartNewIntegration();
		void FinishIntegration();
	}
}