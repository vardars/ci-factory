using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Core;

namespace ThoughtWorks.CruiseControl.CCTrayLib.ServerConnection
{
    public class WebCruiseManager : IWebCruiseManager
    {
        private string _serverUrl;
                
        public WebCruiseManager (string serverUrl)
        {
            this._serverUrl = serverUrl;
        }

        //public void ForceBuild(string projectName, ForceFilterClientInfo[] clientInfo)
        //{
        //    WClient wClient = new WClient();
        //    string project = wClient.ForceBuild(this._serverUrl, projectName, clientInfo);
        //    return project;
        //}

        public bool ForceBuild(string projectName, string serializedClientInfo)
        {
            WClient wClient = new WClient();
            bool result = wClient.ForceBuild(this._serverUrl, projectName, serializedClientInfo);
            return result;
        }

        public string GetProject(string projectName)
        {
            WClient wClient = new WClient();
            string project = wClient.GetProject(this._serverUrl, projectName);
            return project;
        }

        public ProjectStatus[] GetProjectStatusLite()
        {
            WClient wClient = new WClient();
            ProjectStatus[] projectsStatus = wClient.GetProjectsStatusLite(this._serverUrl);
            return projectsStatus;
        }

        public ProjectStatus GetProjectStatusLite(string project)
        {
            WClient wClient = new WClient();
            ProjectStatus projectStatus = wClient.GetProjectStatusLite(this._serverUrl, project);
            return projectStatus;
        }

        public ProjectStatus[] GetProjectStatus()
        {
            WClient wClient = new WClient();
            ProjectStatus[] projectsStatus = wClient.GetProjectsStatus(this._serverUrl);
            return projectsStatus;
        }

        public ProjectStatus GetProjectStatus(string projectName)
        {
            WClient wClient = new WClient();
            ProjectStatus projectStatus = wClient.GetProjectStatus(this._serverUrl, projectName);
            return projectStatus;
        }

        public string GetServerVersion()
        {
            WClient wClient = new WClient();
            string serverVersion = wClient.GetServerVersion(this._serverUrl);
            return serverVersion;
        }
    }
}
