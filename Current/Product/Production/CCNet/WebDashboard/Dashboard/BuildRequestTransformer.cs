
using System.Collections.Generic;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class BuildRequestTransformer : IBuildLogTransformer
	{
		private readonly IMultiTransformer transformer;
		private readonly IBuildRetriever buildRetriever;

		public BuildRequestTransformer(IBuildRetriever buildRetriever, IMultiTransformer transformer)
		{
			this.buildRetriever = buildRetriever;
			this.transformer = transformer;
		}

        public string Transform(IBuildSpecifier buildSpecifier, Dictionary<string, string> xslParams, params string[] transformerFileNames)
		{
			return transformer.Transform(buildRetriever.GetBuild(buildSpecifier).Log, transformerFileNames, xslParams);
		}
	}
}
