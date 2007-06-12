using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting;
using NAnt.Core;
using NAnt.Core.Types;
using NAnt.Core.Attributes;
using ThoughtWorks.CruiseControl.Remote;

namespace CIFactory.NAnt.CCNet.Functions
{
    [FunctionSet("ccnet", "ccnet")]
    public class CCNetFunctions : FunctionSetBase
    {

        #region Constructors

        public CCNetFunctions(Project project, PropertyDictionary properties)
            : base(project, properties)
        {

        }

        #endregion

        [Function("get-project-activity")]
        public string GetProjectActivity(string serverUrl, string projectName)
        {
            ICruiseManager Manager = (ICruiseManager)RemotingServices.Connect(typeof(ICruiseManager), serverUrl);
            return Manager.GetProjectStatus(projectName).Activity.ToString();
        }

        [Function("get-project-build-status")]
        public string GetProjectBuildStatus(string serverUrl, string projectName)
        {
            ICruiseManager Manager = (ICruiseManager)RemotingServices.Connect(typeof(ICruiseManager), serverUrl);
            return Manager.GetProjectStatus(projectName).BuildStatus.ToString();
        }

        [Function("get-project-state")]
        public string GetProjectState(string serverUrl, string projectName)
        {
            ICruiseManager Manager = (ICruiseManager)RemotingServices.Connect(typeof(ICruiseManager), serverUrl);
            return Manager.GetProjectStatus(projectName).Status.ToString();
        }

        [Function("get-project-last-build-label")]
        public string GetProjectLastBuildLabel(string serverUrl, string projectName)
        {
            ICruiseManager Manager = (ICruiseManager)RemotingServices.Connect(typeof(ICruiseManager), serverUrl);
            return Manager.GetProjectStatus(projectName).LastBuildLabel;
        }
    }
}
