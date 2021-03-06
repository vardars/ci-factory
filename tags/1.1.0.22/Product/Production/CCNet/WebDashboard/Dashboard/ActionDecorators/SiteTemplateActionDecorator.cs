using System.Collections;
using ObjectWizard;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using System.Reflection;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard.ActionDecorators
{
	// ToDo - test - I think doing so will change the design a bit - will probably get more in on the constructor - should do this after 1.0
	public class SiteTemplateActionDecorator : IAction
	{
		private readonly IAction decoratedAction;
		private readonly IVelocityViewGenerator velocityViewGenerator;
		private readonly ObjectGiver objectGiver;

		public SiteTemplateActionDecorator(IAction decoratedAction, IVelocityViewGenerator velocityViewGenerator, ObjectGiver objectGiver)
		{
			this.decoratedAction = decoratedAction;
			this.velocityViewGenerator = velocityViewGenerator;
			this.objectGiver = objectGiver;
		}

		public IResponse Execute(IRequest cruiseRequest)
		{
			Hashtable velocityContext = new Hashtable();
			IResponse decoratedActionResponse = decoratedAction.Execute(cruiseRequest);
			if (decoratedActionResponse is HtmlFragmentResponse)
			{
                ICruiseRequestFactory CruiseRequestFactory = (ICruiseRequestFactory)objectGiver.GiveObjectByType(typeof(ICruiseRequestFactory));
                IBuildNameFormatter BuildNameFormatter = (IBuildNameFormatter)objectGiver.GiveObjectByType(typeof(IBuildNameFormatter));
                ICruiseRequest request = CruiseRequestFactory.CreateCruiseRequest(cruiseRequest);

                if (!string.IsNullOrEmpty(request.ServerName))
                {
                    velocityContext["serverName"] = request.ServerName;
                    if (!string.IsNullOrEmpty(request.ProjectName))
                    {
                        velocityContext["projectName"] = request.ProjectName;
                        if (!string.IsNullOrEmpty(request.BuildName))
                            velocityContext["prettybuildName"] = BuildNameFormatter.GetPrettyBuildName(request.BuildSpecifier);
                    }
                }

				velocityContext["breadcrumbs"] = (((TopControlsViewBuilder) objectGiver.GiveObjectByType(typeof(TopControlsViewBuilder))).Execute()).ResponseFragment;
				velocityContext["sidebar"] = (((SideBarViewBuilder) objectGiver.GiveObjectByType(typeof(SideBarViewBuilder))).Execute()).ResponseFragment;
				velocityContext["mainContent"] = ((HtmlFragmentResponse) decoratedActionResponse).ResponseFragment;
				velocityContext["dashboardversion"] = GetVersion();

				return velocityViewGenerator.GenerateView("SiteTemplate.vm", velocityContext);
			}
			else
			{
				return decoratedActionResponse;
			}
		}

		private string GetVersion()
		{
			System.Reflection.Assembly assembly;
			
			assembly = System.Reflection.Assembly.GetExecutingAssembly();
            return ((AssemblyInformationalVersionAttribute)assembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), true)[0]).InformationalVersion;
		}
	}
}
