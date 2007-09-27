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

    [ReflectorType("artifactViewBuildAction")]
    public class ArtifactViewBuildAction : ICruiseAction
    {

        private string _ArtifactRootUrl;

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

        public ArtifactViewBuildAction()
        {

        }

        #region ICruiseAction Members

        public IResponse Execute(ICruiseRequest cruiseRequest)
        {
            string ArctifactDirectoryName = cruiseRequest.BuildName.Substring("log".Length, "yyyyMMddHHmmss".Length);
            Uri url = new Uri(string.Format(@"http://{0}/{1}/{2}/",
                System.Web.HttpContext.Current.Server.MachineName,
                this.ArtifactRootUrl,
                ArctifactDirectoryName));

            string iframe = @"
		<script language=""javascript"" type=""text/javascript"">
		    function iFrameHeight() {
		        var h = 0;
		        if ( !document.all ) {
		            h = document.getElementById('blockrandom').contentDocument.height;
		            document.getElementById('blockrandom').style.height = h + 60 + 'px';
		        } else if( document.all ) {
		            h = document.frames('blockrandom').document.body.scrollHeight;
		            document.all.blockrandom.style.height = h + 20 + 'px';
		        }
		    }
		</script>
		  
		<iframe
		  onload=""iFrameHeight()""		id=""blockrandom""
		  name=""iframe""
		  width=""100%""
		  height=""10000""
		  scrolling=""auto""
		  align=""top""
		  frameborder=""0""
		  class=""wrapper""
		  src=""" + url + @""">
		  This option will not work correctly.  Unfortunately, your browser does not support Inline Frames
		</iframe>
		";

            return new HtmlFragmentResponse(iframe);
        }

        #endregion
    }

}
