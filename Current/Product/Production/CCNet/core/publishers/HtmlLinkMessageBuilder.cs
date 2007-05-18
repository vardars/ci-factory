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
            StringBuilder link = new StringBuilder();
            link.Append(Regex.Replace(result.ProjectUrl, @"\?.*", ""));
            link.AppendFormat("?_action_ViewBuildReport=true&amp;server={0}&amp;project={0}&amp;build={1}",
                result.ProjectName, Path.GetFileName((string)result.IntegrationProperties["CCNetLogFilePath"]));
            return link.ToString();
		}
	}
}
