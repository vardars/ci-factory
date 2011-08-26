using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib.ServerConnection
{
    public interface IWebCruiseManager
    {
        bool ForceBuild(string projectName, string serializedClientInfo);
        string GetProject(string projectName);
        ProjectStatus[] GetProjectStatus();
        ProjectStatus GetProjectStatus(string projectName);
        ProjectStatus[] GetProjectStatusLite();
        ProjectStatus GetProjectStatusLite(string projectName);
        string GetServerVersion();
    }
}
