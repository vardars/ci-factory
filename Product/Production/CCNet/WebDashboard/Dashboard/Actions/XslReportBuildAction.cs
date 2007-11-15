using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using System.Collections.Generic;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard.Actions
{
    [ReflectorType("xslReportBuildAction")]
    public class XslReportBuildAction : ICruiseAction
    {
        #region Fields

        private readonly IBuildLogTransformer buildLogTransformer;

        private string xslFileName;

        #endregion

        #region Constructors

        public XslReportBuildAction(IBuildLogTransformer buildLogTransformer)
        {
            this.buildLogTransformer = buildLogTransformer;
        }

        #endregion

        #region Properties

        [ReflectorProperty("xslFileName")]
        public string XslFileName
        {
            get
            {
                return xslFileName;
            }
            set
            {
                xslFileName = value;
            }
        }

        #endregion

        #region Public Methods

        public IResponse Execute(ICruiseRequest cruiseRequest)
        {
            if (xslFileName == null)
            {
                throw new ApplicationException("XSL File Name has not been set for XSL Report Action");
            }
            Dictionary<string, string> XslParms = new Dictionary<string, string>();
            XslParms.Add("CCNetServer", cruiseRequest.ServerName);
            XslParms.Add("CCNetBuild", cruiseRequest.BuildName);
            XslParms.Add("CCNetProject", cruiseRequest.ProjectName);
            XslParms.Add("applicationPath", cruiseRequest.Request.ApplicationPath);
            return new HtmlFragmentResponse(buildLogTransformer.Transform(cruiseRequest.BuildSpecifier, XslParms, xslFileName));
        }

        #endregion

    }
}
