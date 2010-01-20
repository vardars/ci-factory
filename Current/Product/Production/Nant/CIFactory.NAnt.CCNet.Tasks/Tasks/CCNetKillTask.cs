using System;
using System.Collections;
using System.Runtime.Remoting;
using System.Threading;
using NAnt.Core;
using NAnt.Core.Types;
using NAnt.Core.Attributes;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Core;
using Exortech.NetReflector.Util;

namespace CIFactory.NAnt.CCNet.Tasks
{
    [TaskName("ccnetkill")]
    public class CCNetKillTask : Task
    {

        private ICruiseManager _cruiseManager;
        private string _projectName;
        private string _serverUrl;

        [TaskAttribute("projectname", Required = true)]
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

            _cruiseManager.Kill(this.ProjectName);
        }
    }
}
