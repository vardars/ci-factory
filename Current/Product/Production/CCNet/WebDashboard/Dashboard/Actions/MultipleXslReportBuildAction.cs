using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using System.Collections.Generic;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard.Actions
{
	[ReflectorType("multipleXslReportAction")]
	public class MultipleXslReportBuildAction : ICruiseAction
	{
		private readonly IBuildLogTransformer buildLogTransformer;
		private string[] xslFileNames;

		public MultipleXslReportBuildAction(IBuildLogTransformer buildLogTransformer)
		{
			this.buildLogTransformer = buildLogTransformer;
		}

		public IResponse Execute(ICruiseRequest cruiseRequest)
		{
			if (xslFileNames == null)
			{
				throw new ApplicationException("XSL File Name has not been set for XSL Report Action");
			}
            Dictionary<string, string> XslParms = new Dictionary<string, string>();
            XslParms.Add("CCNetServer", cruiseRequest.ServerName);
            XslParms.Add("CCNetBuild", cruiseRequest.BuildName);
            XslParms.Add("CCNetProject", cruiseRequest.ProjectName);
            XslParms.Add("applicationPath", cruiseRequest.Request.ApplicationPath);

            return new HtmlFragmentResponse(buildLogTransformer.Transform(cruiseRequest.BuildSpecifier, XslParms, xslFileNames));
		}

		[ReflectorArrayAttribute("xslFileNames")]
		public string[] XslFileNames
		{
			get
			{
				return xslFileNames;
			}
			set
			{
				xslFileNames = value;
			}
		}
	}
}
