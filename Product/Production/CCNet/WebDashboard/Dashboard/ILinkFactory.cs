using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface ILinkFactory
	{
        IAbsoluteLink CreateBuildLink(IBuildSpecifier buildSpecifier, string text, string action, string img);
		IAbsoluteLink CreateBuildLink(IBuildSpecifier buildSpecifier, string action);
		IAbsoluteLink CreateBuildLinkWithFileName(IBuildSpecifier buildSpecifier, string action, string fileName);
		IAbsoluteLink CreateStyledBuildLink(IBuildSpecifier buildSpecifier, string action);
		IAbsoluteLink CreateProjectLink(IProjectSpecifier projectSpecifier, string text, string action);
		IAbsoluteLink CreateProjectLink(IProjectSpecifier projectSpecifier, string action);
		IAbsoluteLink CreateServerLink(IServerSpecifier serverSpecifier, string text, string action);
		IAbsoluteLink CreateServerLink(IServerSpecifier serverSpecifier, string action);
		IAbsoluteLink CreateFarmLink(string text, string action);
	}
}