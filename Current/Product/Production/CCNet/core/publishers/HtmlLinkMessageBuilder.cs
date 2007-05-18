using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;


namespace ThoughtWorks.CruiseControl.Core.Publishers
{
	public class HtmlLinkMessageBuilder : IMessageBuilder
	{
		private bool includeAnchorTag;

		public HtmlLinkMessageBuilder(bool includeAnchorTag)
		{
			this.includeAnchorTag = includeAnchorTag;
		}

		public string BuildMessage(IIntegrationResult result)
		{
            StringBuilder linkBuilder = new StringBuilder();
            linkBuilder.Append(Regex.Replace(result.ProjectUrl, @"\?.*", ""));
            linkBuilder.AppendFormat("?_action_ViewBuildReport=true&amp;server={2}&amp;project={0}&amp;build={1}",
                result.ProjectName, 
				Path.GetFileName((string)result.IntegrationProperties["CCNetLogFilePath"]),
				result.IntegrationProperties["CCNetDashboardServerName"]);
            string link = linkBuilder.ToString();
			if (includeAnchorTag) link = string.Format(@"<a href=""{0}"">web page</a>", link);
			return string.Format("CruiseControl.NET Build Results for project {0} ({1})", result.ProjectName, link);
		}
	}
}
