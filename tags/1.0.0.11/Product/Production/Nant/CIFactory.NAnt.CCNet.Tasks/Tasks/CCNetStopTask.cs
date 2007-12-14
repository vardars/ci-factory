using System;
using System.Runtime.Remoting;
using System.Threading;
using NAnt.Core;
using NAnt.Core.Attributes;
using ThoughtWorks.CruiseControl.Remote;

namespace CIFactory.NAnt.CCNet.Tasks
{
    [TaskName("ccnetstop")]
    public class CCNetStopTask : Task
    {

        private ICruiseManager _cruiseManager;
        private string _projectName;
        private string _serverUrl;

        [TaskAttribute("projectname")]
        public string ProjectName
        {
            get { return _projectName; }
            set { _projectName = value; }
        }

        [TaskAttribute("serverurl", Required = true)]
        public string ServerUrl
        {
            get { return _serverUrl; }
            set { _serverUrl = value; }
        }


        protected override void ExecuteTask()
        {
            Log(Level.Info, "Connecting to CCNet server " + ServerUrl);
            _cruiseManager = (ICruiseManager)RemotingServices.Connect(typeof(ICruiseManager), ServerUrl);
            if (!String.IsNullOrEmpty(this.ProjectName))
                _cruiseManager.Stop(this.ProjectName);
            else
                _cruiseManager.Stop();
        }
    }
}
