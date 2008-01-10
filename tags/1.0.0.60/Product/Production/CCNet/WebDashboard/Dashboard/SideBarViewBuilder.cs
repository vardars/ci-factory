using System.Collections;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class SideBarViewBuilder
	{
		private readonly ICruiseRequest request;
		private readonly IBuildNameRetriever buildNameRetriever;
		private readonly IRecentBuildsViewBuilder recentBuildsViewBuilder;
		private readonly IPluginLinkCalculator pluginLinkCalculator;
		private readonly IVelocityViewGenerator velocityViewGenerator;
		private readonly ILinkFactory linkFactory;

		public SideBarViewBuilder(ICruiseRequest request, IBuildNameRetriever buildNameRetriever, IRecentBuildsViewBuilder recentBuildsViewBuilder, IPluginLinkCalculator pluginLinkCalculator, IVelocityViewGenerator velocityViewGenerator, ILinkFactory linkFactory)
		{
			this.request = request;
			this.buildNameRetriever = buildNameRetriever;
			this.recentBuildsViewBuilder = recentBuildsViewBuilder;
			this.pluginLinkCalculator = pluginLinkCalculator;
			this.velocityViewGenerator = velocityViewGenerator;
			this.linkFactory = linkFactory;
		}

		public HtmlFragmentResponse Execute()
		{
			Hashtable velocityContext = new Hashtable();
			string velocityTemplateName = "";

            string serverName = request.ServerName;
            string projectName = request.ProjectName;
            string buildName = request.BuildName;

            velocityContext["serverName"] = serverName;
            velocityContext["projectName"] = projectName;
            velocityContext["buildName"] = buildName;

			if (serverName == "")
			{
				velocityContext["links"] = pluginLinkCalculator.GetFarmPluginLinks();
				velocityTemplateName = @"FarmSideBar.vm";
			}
			else
			{
				if (projectName == "")
				{
					velocityContext["links"] = pluginLinkCalculator.GetServerPluginLinks(request.ServerSpecifier);
					velocityTemplateName = @"ServerSideBar.vm";
				}
				else
				{
					if (buildName == "")
					{
						IProjectSpecifier projectSpecifier = request.ProjectSpecifier;
						velocityContext["links"] = pluginLinkCalculator.GetProjectPluginLinks(projectSpecifier);
						velocityContext["recentBuildsTable"] = recentBuildsViewBuilder.BuildRecentBuildsTable(projectSpecifier);
						velocityTemplateName = @"ProjectSideBar.vm";
					}
					else
					{
						IBuildSpecifier buildSpecifier = request.BuildSpecifier;
						velocityContext["links"] = pluginLinkCalculator.GetBuildPluginLinks(buildSpecifier);
						velocityContext["recentBuildsTable"] = recentBuildsViewBuilder.BuildRecentBuildsTable(buildSpecifier.ProjectSpecifier);
						velocityContext["latestLink"] = linkFactory.CreateProjectLink(request.ProjectSpecifier, LatestBuildReportProjectPlugin.ACTION_NAME);
						velocityContext["nextLink"] = linkFactory.CreateBuildLink(buildNameRetriever.GetNextBuildSpecifier(buildSpecifier), "", BuildReportBuildPlugin.ACTION_NAME, null);
						velocityContext["previousLink"] = linkFactory.CreateBuildLink(buildNameRetriever.GetPreviousBuildSpecifier(buildSpecifier), "", BuildReportBuildPlugin.ACTION_NAME, null);
						velocityTemplateName = @"BuildSideBar.vm";
					}
				}
			}

			return velocityViewGenerator.GenerateView(velocityTemplateName, velocityContext) as HtmlFragmentResponse;
		}
	}
}
