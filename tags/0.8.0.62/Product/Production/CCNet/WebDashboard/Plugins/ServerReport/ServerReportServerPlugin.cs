using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ServerReport
{
	// ToDo - Test!
	[ReflectorType("serverReportServerPlugin")]
	public class ServerReportServerPlugin : ICruiseAction, IPlugin
	{
		public static readonly string ACTION_NAME = "ViewServerReport";

		private readonly IProjectGridAction projectGridAction;

		public ServerReportServerPlugin(IProjectGridAction projectGridAction)
		{
			this.projectGridAction = projectGridAction;
		}

		public IResponse Execute(ICruiseRequest request)
		{
			return projectGridAction.Execute(ACTION_NAME, request.ServerSpecifier, request.Request);
		}

		public string LinkDescription
		{
			get { return "Server Report"; }
		}

		public INamedAction[] NamedActions
		{
			get {  return new INamedAction[] { new ImmutableNamedAction(ACTION_NAME, this) }; }
		}
	}
}
