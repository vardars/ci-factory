using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using System.Xml.Serialization;
using System.IO;
using ThoughtWorks.CruiseControl.CCTrayLib.ServerConnection;
using ThoughtWorks.CruiseControl.Core.Util;

namespace CCNet.Server.Aggregator
{
    public class AggregateCruiseServer : ICruiseServer
    {

        #region Fields

        private ICruiseManager manager;
        private ICruiseManagerFactory _CruiseManagerFactory;
        private PersistentConfiguration _Configuration;
        private bool _disposed;
        private Dictionary<string ,ICruiseManager> _Projects;
        
        #endregion

        #region Properties

        public Dictionary<string, ICruiseManager> Projects
        {
            get
            {
                if (_Projects == null)
                    _Projects = new Dictionary<string, ICruiseManager>();
                return _Projects;
            }
            set
            {
                _Projects = value;
            }
        }

        public PersistentConfiguration Configuration
        {
            get
            {
                return _Configuration;
            }
            set
            {
                _Configuration = value;
            }
        }

        public ICruiseManagerFactory CruiseManagerFactory
        {
        	get 
        	{
                return _CruiseManagerFactory; 
        	}
        	set
        	{
                _CruiseManagerFactory = value;
        	}
        }

        public ICruiseManager  CruiseManager
        {
	        get 
            { 
                if (manager == null)
                    manager = new CruiseManager(this);
                return manager;
            }
        }

        #endregion

        public AggregateCruiseServer(ICruiseManagerFactory cruiseManagerFactory, string configPath)
        {
            this.ReadConfigurationFile(configPath);
            this.CruiseManagerFactory = cruiseManagerFactory;
            RemotingConfiguration.Configure(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            this.GrabProjects();
			RegisterForRemoting();
        }

        public void GrabProjects()
        {
	            foreach (ThoughtWorks.CruiseControl.CCTrayLib.Configuration.Project ProjectConfig in this.Configuration.Projects)
                {
                    Projects.Add(ProjectConfig.ProjectName, this.CruiseManagerFactory.GetCruiseManager(ProjectConfig.ServerUrl));
                    Log.Info(string.Format("Connecting to project {0} on {1}.", ProjectConfig.ProjectName, ProjectConfig.ServerUrl));
                }
        }

        private void ReadConfigurationFile(string configFileName)
        {
            if (!File.Exists(configFileName))
            {
                this.Configuration = new PersistentConfiguration();
                return;
            }

            XmlSerializer serializer = new XmlSerializer(typeof(PersistentConfiguration));

            using (StreamReader configFile = File.OpenText(configFileName))
                this.Configuration = (PersistentConfiguration)serializer.Deserialize(configFile);
        }

        #region Public Methods

        private void RegisterForRemoting()
		{
			MarshalByRefObject marshalByRef = (MarshalByRefObject)this.CruiseManager;
            RemotingServices.Marshal(marshalByRef, RemoteCruiseServer.URI);
 
			foreach (IChannel channel in ChannelServices.RegisteredChannels)
			{
				Log.Info("Registered channel: " + channel.ChannelName);
				if (channel is IChannelReceiver)
				{
                    foreach (string url in ((IChannelReceiver)channel).GetUrlsForUri(RemoteCruiseServer.URI))
					{
						Log.Info("CruiseManager: Listening on url: " + url);
					}
				}
			}
		}

        public void  Dispose()
        {
 	        if (_disposed) return;		
				_disposed = true;
			Log.Info("Disconnecting remote server: ");
			RemotingServices.Disconnect((MarshalByRefObject)this.CruiseManager);
			foreach (IChannel channel in ChannelServices.RegisteredChannels)
			{
				Log.Info("Unregistering channel: " + channel.ChannelName);
				ChannelServices.UnregisterChannel(channel);
			}
        }

        public bool  ForceBuild(string projectName, ForceFilterClientInfo[] clientInfo)
        {
            Log.Debug(string.Format("Forcing {0}.", projectName));
            return this.Projects[projectName].ForceBuild(projectName, clientInfo);
        }

        public string[]  GetBuildNames(string projectName)
        {
            Log.Debug(string.Format("Get build names for {0}", projectName));
            return this.Projects[projectName].GetBuildNames(projectName);
        }

        public ExternalLink[]  GetExternalLinks(string projectName)
        {
            Log.Debug(string.Format("Get external links for {0}", projectName));
            return this.Projects[projectName].GetExternalLinks(projectName);
        }

        public string  GetProject(string name)
        {
            Log.Debug(string.Format("Get project configuration for {0}", name));
            return this.Projects[name].GetProject(name);
        }

        public ProjectStatus GetProjectStatus(string projectName)
        {
            Log.Debug(string.Format("Get project status for {0}", projectName));

            try
            {
                return this.Projects[projectName].GetProjectStatus(projectName);
            }
            catch{}

            return new List<ProjectStatus>(this.Projects[projectName].GetProjectStatus()).Find(FindStatus(projectName));
        }

        public ProjectStatus[] GetProjectStatus()
        {
            List<ProjectStatus> Stati = new List<ProjectStatus>();

            foreach (KeyValuePair<string, ICruiseManager> Pair in this.Projects)
            {
                Log.Debug(string.Format("Get status for {0}", Pair.Key));
                Stati.Add(new List<ProjectStatus>(Pair.Value.GetProjectStatus()).Find(FindStatus(Pair.Key)));
            }
            return Stati.ToArray();
        }

        Predicate<ProjectStatus> FindStatus(string projectName)
        {
            return delegate(ProjectStatus item) { return item.Name == projectName; };
        }

		public void Stop(string projectName)
		{
			this.Projects[projectName].Stop(projectName);
		}

		public void Start(string projectName)
		{
			this.Projects[projectName].Start(projectName);
		}

		public string GetBuildLogDirectory(string projectName)
		{
			return this.Projects[projectName].GetBuildLogDirectory(projectName);
		}

		public string GetHostServerName(string projectName)
		{
			return this.Projects[projectName].GetHostServerName(projectName);
		}

		public string GetLog(string projectName, string buildName)
		{
			return this.Projects[projectName].GetLog(projectName, buildName);
		}

		public string[] GetMostRecentBuildNames(string projectName, int buildCount)
		{
			return this.Projects[projectName].GetMostRecentBuildNames(projectName, buildCount);
		}

		public void WaitForExit(string projectName)
		{
			this.Projects[projectName].WaitForExit(projectName);
		}

		public string GetLatestBuildName(string projectName)
		{
			return this.Projects[projectName].GetLatestBuildName(projectName);
		}

        #endregion
        
        #region Not Going To Implement

        public void  Abort()
        {
 	        throw new Exception("The method or operation is not implemented.");
        }

        public void  AddProject(string serializedProject)
        {
 	        throw new Exception("The method or operation is not implemented.");
        }

        public void  DeleteProject(string projectName, bool purgeWorkingDirectory, bool purgeArtifactDirectory, bool purgeSourceControlEnvironment)
        {
 	        throw new Exception("The method or operation is not implemented.");
        }

        public void  Start()
        {
 	        throw new Exception("The method or operation is not implemented.");
        }

        public void  Stop()
        {
 	        throw new Exception("The method or operation is not implemented.");
        }

        public void  UpdateProject(string projectName, string serializedProject)
        {
 	        throw new Exception("The method or operation is not implemented.");
        }

        public void  WaitForExit()
        {
 	        throw new Exception("The method or operation is not implemented.");
        }

        public string  GetServerLog()
        {
 	        throw new Exception("The method or operation is not implemented.");
        }

        public string  GetVersion()
        {
 	        throw new Exception("The method or operation is not implemented.");
		}
        
        #endregion



    }
}
