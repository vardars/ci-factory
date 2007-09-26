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
using System.Reflection;
using System.IO;

namespace CIFactory.NAnt.CCNet.Tasks
{
    [TaskName("ccnetforce")]
    public class CCNetForceTask : Task
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

        private FileSet _CCNetPlugins;

        [BuildElement("ccnetplugins", Required = false)]
        public FileSet CCNetPlugins
        {
            get
            {
                return _CCNetPlugins;
            }
            set
            {
                _CCNetPlugins = value;
            }
        }

        private readonly IProjectSerializer projectSerializer = new NetReflectorProjectSerializer();

        private IProject project
        {
            get
            {
                string serializedProject = string.Empty;
                serializedProject = _cruiseManager.GetProject(this.ProjectName);
                return this.projectSerializer.Deserialize(serializedProject);
            }
        }

        private bool PluginAlreadyLoaded(string assemblyPath)
        {
            String AssemblyFileName = Path.GetFileName(assemblyPath);

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (Path.GetFileName(assembly.Location) == AssemblyFileName)
                    return true;
            }
            return false;
        }

        protected override void ExecuteTask()
        {
            Log(Level.Info, "Connecting to CCNet server " + ServerUrl);
            _cruiseManager = (ICruiseManager)RemotingServices.Connect(typeof(ICruiseManager), ServerUrl);

            if (this.CCNetPlugins != null && this.CCNetPlugins.FileNames.Count > 0)
            {
                foreach (string AssemblyPath in this.CCNetPlugins.FileNames)
                {
                    if (!PluginAlreadyLoaded(AssemblyPath))
                    {
                        Assembly LoadedAssembly = Assembly.LoadFile(AssemblyPath);
                        Log(Level.Debug, "Loaded {0}", LoadedAssembly.Location);
                    }
                }
            }

            bool clientInfoRequired = false;
            ArrayList clientInfoList = new ArrayList();

            IForceFilter[] forceFilters = this.project.ForceFilters;

            if (forceFilters != null && forceFilters.Length != 0)
            {
                foreach (IForceFilter forceFilter in forceFilters)
                {
                    if (forceFilter.RequiresClientInformation)
                    {
                        clientInfoRequired = true;
                        ForceFilterClientInfo info = forceFilter.GetClientInfo();
                        clientInfoList.Add(info);
                    }
                }
            }
            bool SuccessfullyForced = false;
            if (clientInfoRequired)
            {
                ForceFilterClientInfo[] clientInfo;
                clientInfo = (ForceFilterClientInfo[])clientInfoList.ToArray(typeof(ForceFilterClientInfo));
                SuccessfullyForced = _cruiseManager.ForceBuild(ProjectName, clientInfo);
            }
            else
            {
                SuccessfullyForced = _cruiseManager.ForceBuild(ProjectName);
            }
            if (!SuccessfullyForced)
                throw new BuildException(string.Format("Unable to force build for {0} on {1}.", this.ProjectName, this.ServerUrl), this.Location);
            Log(Level.Info, "Successfully forced CCNet project " + this.ProjectName);
        }
    }
}
