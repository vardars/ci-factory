using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Core.Util;
using System.Configuration;
using System.Runtime.Remoting.Channels.Tcp;
using System.Xml;
using System.Xml.XPath;

namespace ThoughtWorks.CruiseControl.Core
{
    public class CIFactoryServer : ICIFactoryServer
    {
        #region ICIFactoryServer Members

        public string GetHostServerName(string projectName)
        {
            return RemoteCruiseServer.Instance.GetHostServerName(projectName);
        }

        public ProjectStatus[] GetProjectStatus()
        {
            return RemoteCruiseServer.Instance.GetProjectStatus();
        }

        public ProjectStatus GetProjectStatus(string projectName)
        {
            return RemoteCruiseServer.Instance.GetProjectStatus(projectName);
        }

        public string GetLatestBuildName(string projectName)
        {
            return RemoteCruiseServer.Instance.GetLatestBuildName(projectName);
        }

        public string[] GetBuildNames(string projectName)
        {
            return RemoteCruiseServer.Instance.GetBuildNames(projectName);
        }

        public string[] GetMostRecentBuildNames(string projectName, int buildCount)
        {
            return RemoteCruiseServer.Instance.GetMostRecentBuildNames(projectName, buildCount);
        }

        public string GetLog(string projectName, string buildName)
        {
            return RemoteCruiseServer.Instance.GetLog(projectName, buildName);
        }

        public string GetVersion()
        {
            return RemoteCruiseServer.Instance.GetVersion();
        }

        public ExternalLink[] GetExternalLinks(string projectName)
        {
            return RemoteCruiseServer.Instance.GetExternalLinks(projectName);
        }

        #endregion
    }
}
