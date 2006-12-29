using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Net;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard.Actions
{
    [ReflectorType("artifactXslReportBuildAction")]
	public class ArtifactXslReportBuildAction : ICruiseAction
	{
		private readonly IBuildLogTransformer buildLogTransformer;
        private string xslFileName;
        private string _XmlFileName;
        private string _ArtifactRootUrl;

        public ArtifactXslReportBuildAction(IBuildLogTransformer buildLogTransformer)
		{
			this.buildLogTransformer = buildLogTransformer;
		}

		public IResponse Execute(ICruiseRequest cruiseRequest)
		{
			if (xslFileName == null)
			{
				throw new ApplicationException("XSL File Name has not been set for XSL Report Action");
			}
            string ArctifactDirectoryName = cruiseRequest.BuildName.Substring("log".Length, "yyyyMMddHHmmss".Length);

            Uri url = new Uri(string.Format(@"{0}/{1}/{2}", System.Web.HttpContext.Current.Server.MapPath(this.ArtifactRootUrl), ArctifactDirectoryName, this.XmlFileName));

            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            StringWriter output = new StringWriter();
            using (Stream inputReader = response.GetResponseStream())
            {
                XslTransform transform = new XslTransform();
                transform.Load(System.Web.HttpContext.Current.Server.MapPath(xslFileName));
                transform.Transform(new XPathDocument(inputReader), null, output);
            }

			return new HtmlFragmentResponse(output.ToString());
		}

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

        [ReflectorProperty("artifactRootUrl")]
        public string ArtifactRootUrl
        {
            get
            {
                return _ArtifactRootUrl;
            }
            set
            {
                _ArtifactRootUrl = value;
            }
        }

        [ReflectorProperty("xmlFileName")]
        public string XmlFileName
        {
            get
            {
                return _XmlFileName;
            }
            set
            {
                _XmlFileName = value;
            }
        }
	}
}
