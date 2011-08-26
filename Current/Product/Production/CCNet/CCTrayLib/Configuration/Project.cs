using System;
using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Configuration
{
	public class Project
	{
		private const int DefaultPort = 21237;

		public Project()
		{
		}

		public Project(string serverUrl, string projectName)
		{
			ServerUrl = serverUrl;
			ProjectName = projectName;
		}

		[XmlAttribute(AttributeName="serverUrl")] public string ServerUrl;

		[XmlAttribute(AttributeName="projectName")] public string ProjectName;

		[XmlIgnore]
		public string ServerDisplayName
		{
			get
			{
                Uri serverUri = new Uri(ServerUrl);

                if (ServerUrl.ToLower().Contains("proxy"))
                    return serverUri.Segments[serverUri.Segments.Length - 1] + ":proxy";
                
				if (serverUri.Port == DefaultPort)
					return serverUri.Host;

				return serverUri.Host + ":" + serverUri.Port;
			}
		}

		public void SetServerUrlFromDisplayName(string displayName, string proxyServerUrl)
		{
			string[] displayNameParts = displayName.Split(':');

			if (displayNameParts.Length == 1)
			{
				ServerUrl = string.Format("http://{0}:{1}", displayNameParts[0], DefaultPort);
			}
			else if (displayNameParts.Length == 2)
			{
                if (displayNameParts[1].ToLower() == "proxy")
                {
                    ServerUrl = string.Format("{0}/{1}", proxyServerUrl, displayNameParts[0]);
                }
                else
                {
                    try
                    {
                        ServerUrl =
                            string.Format("http://{0}:{1}", displayNameParts[0], Convert.ToInt32(displayNameParts[1]));
                    }
                    catch (FormatException)
                    {
                        throw new ApplicationException("Port number must be an integer");
                    }
                }
			}
			else
			{
				throw new ApplicationException("Expected string in format server[:port]");
			}
		}
	}
}