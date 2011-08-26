using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Core.Util;
using System.Configuration;
using System.Runtime.Remoting.Channels.Tcp;
using System.Xml;
using System.Xml.Serialization;
using Exortech.NetReflector;

namespace ThoughtWorks.CruiseControl.Core
{
    public class CIFactoryServer : ICIFactoryServer
    {
        #region ICIFactoryServer Members

        public bool ForceBuild(string projectName, string serializedClientInfo)
        {
            bool result = false;
            if (serializedClientInfo == string.Empty)
            {
                result = RemoteCruiseServer.Instance.ForceBuild(projectName, new Dictionary<string, string>(), new ForceFilterClientInfo[1]);
            }
            else
            {
                Type[] extraTypes = new Type[] { typeof(PasswordInformation), typeof(HostInformation), typeof(UserInformation) };

                XmlSerializer clientInfoSerializer = new XmlSerializer(typeof(ForceFilterClientInfo[]), extraTypes);
                XmlReader clientInfoReader = XmlReader.Create(new StringReader(serializedClientInfo));
                ForceFilterClientInfo[] clientInfo = (ForceFilterClientInfo[])clientInfoSerializer.Deserialize(clientInfoReader);
                clientInfoReader.Close();

                result = RemoteCruiseServer.Instance.ForceBuild(projectName, new Dictionary<string, string>(), clientInfo);
            }
            return result;
        }

        public string GetProject(string projectName)
        {
            string serializedProject = string.Empty;
            serializedProject = RemoteCruiseServer.Instance.GetProject(projectName);

            return serializedProject;			
        }

        public string GetHostServerName(string projectName)
        {
            return RemoteCruiseServer.Instance.GetHostServerName(projectName);
        }
        
        public ProjectStatus[] GetProjectsStatus()
        {
            return RemoteCruiseServer.Instance.GetProjectStatus();
        }

        public ProjectStatus GetProjectStatus(string projectName)
        {
            return RemoteCruiseServer.Instance.GetProjectStatus(projectName);
        }

        public ProjectStatus[] GetProjectsStatusLite()
        {
            return RemoteCruiseServer.Instance.GetProjectStatusLite();
        }

        public ProjectStatus GetProjectStatusLite(string projectName)
        {
            return RemoteCruiseServer.Instance.GetProjectStatusLite(projectName);
        }

        public string GetLatestBuildName(string projectName)
        {
            return RemoteCruiseServer.Instance.GetLatestBuildName(projectName);
        }

        public string[] GetBuildNames(string projectName)
        {
            return RemoteCruiseServer.Instance.GetBuildNames(projectName);
        }

        public string[] GetMostRecentBuildNames(string projectName, string buildCount)
        {
            return RemoteCruiseServer.Instance.GetMostRecentBuildNames(projectName, int.Parse(buildCount));
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

        public ProcessInformationList GetProjectProcessInformation(string projectName)
        {
            return ProcessExecutor.RetrieveProcessInformation(projectName);
        }

        #endregion
    }

    //[Serializable]
    //public class PasswordInformation : ForceFilterClientInfo
    //{
    //    private string _Password;
    //    public string Password
    //    {
    //        get { return _Password; }
    //        set
    //        {
    //            _Password = value;
    //        }
    //    }

    //}

    //[Serializable]
    //public class HostInformation : ForceFilterClientInfo
    //{

    //    private string _Name;
    //    public string Name
    //    {
    //        get { return _Name; }
    //        set
    //        {
    //            _Name = value;
    //        }
    //    }
    //}

    //[Serializable]
    //public class UserInformation : ForceFilterClientInfo
    //{
    //    private string _Name;
    //    private StringCollection _Groups;

    //    public StringCollection Groups
    //    {
    //        get
    //        {
    //            return _Groups;
    //        }
    //        set
    //        {
    //            _Groups = value;
    //        }
    //    }

    //    public string Name
    //    {
    //        get
    //        {
    //            return _Name;
    //        }
    //        set
    //        {
    //            _Name = value;
    //        }
    //    }
    //}
}
