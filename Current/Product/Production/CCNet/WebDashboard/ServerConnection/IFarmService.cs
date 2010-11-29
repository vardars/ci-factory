using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard.ServerConnection
{
	public interface IFarmService
	{
		IBuildSpecifier[] GetMostRecentBuildSpecifiers(IProjectSpecifier projectSpecifier, int buildCount);
		IBuildSpecifier[] GetBuildSpecifiers(IProjectSpecifier serverSpecifier);
		void DeleteProject(IProjectSpecifier projectSpecifier, bool purgeWorkingDirectory, bool purgeArtifactDirectory, bool purgeSourceControlEnvironment);
		string GetServerLog(IServerSpecifier serverSpecifier);
        string GetLog(IBuildSpecifier buildSpecifier);
        string[] GetLogMessages(IProjectSpecifier projectSpecifier);
		bool ForceBuild(IProjectSpecifier projectSpecifier);
        void Kill(IProjectSpecifier projectSpecifier);
		ProjectStatusListAndExceptions GetProjectStatusListAndCaptureExceptions();
		ProjectStatusListAndExceptions GetProjectStatusListAndCaptureExceptions(IServerSpecifier serverSpecifier);
		ExternalLink[] GetExternalLinks(IProjectSpecifier projectSpecifier);
		string GetServerVersion(IServerSpecifier serverSpecifier);

        ICruiseManager GetCruiseManager(IServerSpecifier serverSpecifier);
        ICruiseManager GetCruiseManager(IProjectSpecifier projectSpecifier);
        ICruiseManager GetCruiseManager(IBuildSpecifier buildSpecifier);
	}
}
