using System;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.WebDashboard.Configuration;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using System.Web;

namespace ThoughtWorks.CruiseControl.WebDashboard.ServerConnection
{
	public class ServerAggregatingCruiseManagerWrapper : ICruiseManagerWrapper, IFarmService
	{
		private readonly ICruiseManagerFactory managerFactory;
		private readonly IRemoteServicesConfiguration configuration;
		private readonly IProjectSerializer projectSerializer = new NetReflectorProjectSerializer();
		
		private IProject GetProject(ICruiseManager manager, string projectName)
		{
			string serializedProject = string.Empty;
			serializedProject = manager.GetProject(projectName);
			try
			{
				return this.projectSerializer.Deserialize(serializedProject);
			}
			catch
			{
				foreach (string filePath in Directory.GetFiles(Path.Combine(System.Web.HttpContext.Current.Request.PhysicalApplicationPath, "bin"), "*plugin*.dll"))
				{
					System.Reflection.Assembly.LoadFile(filePath);
				}
				serializedProject = manager.GetProject(projectName);
				return this.projectSerializer.Deserialize(serializedProject);
			}
		}

		public ServerAggregatingCruiseManagerWrapper(IRemoteServicesConfiguration configuration, ICruiseManagerFactory managerFactory)
		{
			this.configuration = configuration;
			this.managerFactory = managerFactory;
		}

		public IBuildSpecifier GetLatestBuildSpecifier(IProjectSpecifier projectSpecifier)
		{
			return new DefaultBuildSpecifier(projectSpecifier, GetCruiseManager(projectSpecifier.ServerSpecifier).GetLatestBuildName(projectSpecifier.ProjectName));
		}

        public string[] GetLogMessages(IProjectSpecifier projectSpecifier)
        {
            string HostName = this.GetCruiseManager(projectSpecifier)
                .GetHostServerName(projectSpecifier.ProjectName);
            String[] Messages = new String[] { };
            if (Environment.MachineName == HostName)
            {
                string BuildLogDirectoryPath = GetCruiseManager(projectSpecifier)
                    .GetBuildLogDirectory(projectSpecifier.ProjectName);
                string LogMessagesFilePath = Path.Combine(BuildLogDirectoryPath, "LogMessages.txt");
                if (File.Exists(LogMessagesFilePath))
                {
                    try
                    {
                        Messages = File.ReadAllLines(LogMessagesFilePath);
                    }
                    catch (IOException ex)
                    {
                        //swallow
                    }
                }
            }
            return Messages;
        }

        public string GetLog(IBuildSpecifier buildSpecifier)
        {
            string HostName = this.GetCruiseManager(buildSpecifier).GetHostServerName(buildSpecifier.ProjectSpecifier.ProjectName);
            if (Environment.MachineName == HostName)
            {
                string BuildLogDirectoryPath = GetCruiseManager(buildSpecifier).GetBuildLogDirectory(buildSpecifier.ProjectSpecifier.ProjectName);
                return File.ReadAllText(Path.Combine(BuildLogDirectoryPath, buildSpecifier.BuildName));
            }
            else
                return GetCruiseManager(buildSpecifier).GetLog(buildSpecifier.ProjectSpecifier.ProjectName, buildSpecifier.BuildName);
        }

		public IBuildSpecifier[] GetBuildSpecifiers(IProjectSpecifier projectSpecifier)
		{
			return CreateBuildSpecifiers(projectSpecifier, GetCruiseManager(projectSpecifier).GetBuildNames(projectSpecifier.ProjectName));
		}

		public IBuildSpecifier[] GetMostRecentBuildSpecifiers(IProjectSpecifier projectSpecifier, int buildCount)
		{
			return CreateBuildSpecifiers(projectSpecifier, GetCruiseManager(projectSpecifier).GetMostRecentBuildNames(projectSpecifier.ProjectName, buildCount));
		}

		private IBuildSpecifier[] CreateBuildSpecifiers(IProjectSpecifier projectSpecifier, string[] buildNames)
		{
			ArrayList buildSpecifiers = new ArrayList();
			foreach (string buildName in buildNames)
			{
				buildSpecifiers.Add(new DefaultBuildSpecifier(projectSpecifier, buildName));
			}
			return (IBuildSpecifier[]) buildSpecifiers.ToArray(typeof (IBuildSpecifier));
		}

		public void DeleteProject(IProjectSpecifier projectSpecifier, bool purgeWorkingDirectory, bool purgeArtifactDirectory, bool purgeSourceControlEnvironment)
		{
			GetCruiseManager(projectSpecifier).DeleteProject(projectSpecifier.ProjectName, purgeWorkingDirectory, purgeArtifactDirectory, purgeSourceControlEnvironment);
		}

		public bool ForceBuild(IProjectSpecifier projectSpecifier)
		{
			bool clientInfoRequired = false;
			ArrayList clientInfoList = new ArrayList();
			ICruiseManager manager = GetCruiseManager(projectSpecifier.ServerSpecifier);
			IForceFilter[] forceFilters = this.GetProject(manager, projectSpecifier.ProjectName).ForceFilters;
			
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

            Dictionary<string, string> webParams = new Dictionary<string, string>();
            foreach (String key in HttpContext.Current.Request.Form.Keys)
            {
                String value = HttpContext.Current.Request.Params[key];
                webParams.Add(key, value);
            }

			if (clientInfoRequired)
			{
				ForceFilterClientInfo[] clientInfo;
				clientInfo = (ForceFilterClientInfo[]) clientInfoList.ToArray(typeof(ForceFilterClientInfo));
				return manager.ForceBuild(projectSpecifier.ProjectName, webParams, clientInfo);
			}
			else
			{
				return manager.ForceBuild(projectSpecifier.ProjectName, webParams);
			}
		}

		private string GetServerUrl(IServerSpecifier serverSpecifier)
		{
			foreach (ServerLocation serverLocation in ServerLocations)
			{
				if (serverLocation.Name == serverSpecifier.ServerName)
				{
					return serverLocation.Url;
				}
			}

			throw new UnknownServerException(serverSpecifier.ServerName);
		}

		public ProjectStatusListAndExceptions GetProjectStatusListAndCaptureExceptions()
		{
			return GetProjectStatusListAndCaptureExceptions(GetServerSpecifiers());
		}

		public ProjectStatusListAndExceptions GetProjectStatusListAndCaptureExceptions(IServerSpecifier serverSpecifier)
		{
			return GetProjectStatusListAndCaptureExceptions(new IServerSpecifier[] { serverSpecifier });
		}

		public ExternalLink[] GetExternalLinks(IProjectSpecifier projectSpecifier)
		{
			return GetCruiseManager(projectSpecifier).GetExternalLinks(projectSpecifier.ProjectName);
		}

		private ProjectStatusListAndExceptions GetProjectStatusListAndCaptureExceptions(IServerSpecifier[] serverSpecifiers)
		{
			ArrayList projectStatusOnServers = new ArrayList();
			ArrayList exceptions = new ArrayList();

			foreach (IServerSpecifier serverSpecifier in serverSpecifiers)
			{
				try
				{
					foreach (ProjectStatus projectStatus in GetCruiseManager(serverSpecifier).GetProjectStatus())
					{
						projectStatusOnServers.Add(new ProjectStatusOnServer(projectStatus, serverSpecifier));
					}
				}
				catch (Exception e)
				{
					exceptions.Add(new CruiseServerException(serverSpecifier.ServerName, GetServerUrl(serverSpecifier), e));
				}
			}

			return new ProjectStatusListAndExceptions((ProjectStatusOnServer[]) projectStatusOnServers.ToArray(typeof (ProjectStatusOnServer)),
			                                          (CruiseServerException[]) exceptions.ToArray(typeof (CruiseServerException)));
		}

		public string GetServerLog(IServerSpecifier serverSpecifier)
		{
			return GetCruiseManager(serverSpecifier).GetServerLog();
		}

		public string GetServerVersion(IServerSpecifier serverSpecifier)
		{
			return GetCruiseManager(serverSpecifier).GetServerVersion();
		}

		public IServerSpecifier[] GetServerSpecifiers()
		{
			ArrayList serverSpecifiers = new ArrayList();
			foreach (ServerLocation serverLocation in ServerLocations)
			{
				serverSpecifiers.Add(new DefaultServerSpecifier(serverLocation.Name));
			}
			return (IServerSpecifier[]) serverSpecifiers.ToArray(typeof (IServerSpecifier));
		}

		public void AddProject(IServerSpecifier serverSpecifier, string serializedProject)
		{
			GetCruiseManager(serverSpecifier).AddProject(serializedProject);
		}

		public string GetProject(IProjectSpecifier projectSpecifier)
		{
			return GetCruiseManager(projectSpecifier.ServerSpecifier).GetProject(projectSpecifier.ProjectName);
		}

		public void UpdateProject(IProjectSpecifier projectSpecifier, string serializedProject)
		{
			GetCruiseManager(projectSpecifier.ServerSpecifier).UpdateProject(projectSpecifier.ProjectName, serializedProject);
		}

		public ICruiseManager GetCruiseManager(IBuildSpecifier buildSpecifier)
		{
			return GetCruiseManager(buildSpecifier.ProjectSpecifier);
		}

		public ICruiseManager GetCruiseManager(IProjectSpecifier projectSpecifier)
		{
			return GetCruiseManager(projectSpecifier.ServerSpecifier);
		}

		public ICruiseManager GetCruiseManager(IServerSpecifier serverSpecifier)
		{
			return managerFactory.GetCruiseManager(GetServerUrl(serverSpecifier));
		}

		private ServerLocation[] ServerLocations
		{
			get { return configuration.Servers; }
		}

    }
}
