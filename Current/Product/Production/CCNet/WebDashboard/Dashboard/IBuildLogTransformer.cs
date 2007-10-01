
using System.Collections.Generic;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IBuildLogTransformer
	{
        string Transform(IBuildSpecifier build, Dictionary<string, string> xslParams, params string[] transformerFileNames);
	}
}
