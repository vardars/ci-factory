using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Remote;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace ThoughtWorks.CruiseControl.CCTrayLib.ServerConnection
{
    public class WClient
    {
        public bool ForceBuild(string serverUrl, string projectName, string serializedClientInfo)
        {
            WebChannelFactory<ICIFactoryServer> webChannelFactory = getWebChannelFactory(serverUrl);
            ICIFactoryServer commChannel = webChannelFactory.CreateChannel();
            bool result = commChannel.ForceBuild(projectName, serializedClientInfo);
            
            return result;
        }

        public string GetProject(string serverUrl, string projectName)
        {
            WebChannelFactory<ICIFactoryServer> webChannelFactory = getWebChannelFactory(serverUrl);
            ICIFactoryServer commChannel = webChannelFactory.CreateChannel();
            string project = commChannel.GetProject(projectName);

            return project;
        }
            
        public ProjectStatus[] GetProjectsStatus(string serverUrl)
        {
            WebChannelFactory<ICIFactoryServer> webChannelFactory = getWebChannelFactory(serverUrl);
            ICIFactoryServer commChannel = webChannelFactory.CreateChannel();
            ProjectStatus[] projectsStatus = commChannel.GetProjectsStatus();

            return projectsStatus;
        }

        public ProjectStatus GetProjectStatus(string serverUrl, string projectName)
        {
            WebChannelFactory<ICIFactoryServer> webChannelFactory = getWebChannelFactory(serverUrl);
            ICIFactoryServer commChannel = webChannelFactory.CreateChannel();
            ProjectStatus projectStatus = commChannel.GetProjectStatus(projectName);

            return projectStatus;
        }

        public ProjectStatus[] GetProjectsStatusLite(string serverUrl)
        {
            WebChannelFactory<ICIFactoryServer> webChannelFactory = getWebChannelFactory(serverUrl);
            ICIFactoryServer commChannel = webChannelFactory.CreateChannel();
            ProjectStatus[] projectsStatus = commChannel.GetProjectsStatusLite();

            return projectsStatus;
        }

        public ProjectStatus GetProjectStatusLite(string serverUrl, string projectName)
        {
            WebChannelFactory<ICIFactoryServer> webChannelFactory = getWebChannelFactory(serverUrl);
            ICIFactoryServer commChannel = webChannelFactory.CreateChannel();
            ProjectStatus projectStatus = commChannel.GetProjectStatusLite(projectName);

            return projectStatus;
        }

        public string GetServerVersion(string serverUrl)
        {
            WebChannelFactory<ICIFactoryServer> webChannelFactory = getWebChannelFactory(serverUrl);
            ICIFactoryServer commChannel = webChannelFactory.CreateChannel();
            string serverVersion = commChannel.GetVersion();

            return serverVersion;
        } 
        
        private WebChannelFactory<ICIFactoryServer> getWebChannelFactory(string serverUrl)
        {
            Uri uri = new Uri(serverUrl);
            WebHttpBinding webHttpBinding;
            WebChannelFactory<ICIFactoryServer> webChannelFactory;

            if (serverUrl.Contains("https"))
            {
                webHttpBinding = new WebHttpBinding(WebHttpSecurityMode.Transport);
                webHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
                webChannelFactory = new WebChannelFactory<ICIFactoryServer>(webHttpBinding, uri);
                webChannelFactory.Credentials.UserName.UserName = Credentials.UserName;
                webChannelFactory.Credentials.UserName.Password = Credentials.Password;
            }
            else
            {
                webChannelFactory = new WebChannelFactory<ICIFactoryServer>(new WebHttpBinding(), uri);
            }

            return webChannelFactory;
        }
    }
}
