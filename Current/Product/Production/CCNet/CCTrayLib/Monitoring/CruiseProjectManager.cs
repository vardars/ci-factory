using System;
using System.IO;
using System.Collections;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Remote;
using Exortech.NetReflector.Util;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	/// <summary>
	/// Allows access to the state of a single cruise control project 
	/// </summary>
	public class CruiseProjectManager : ICruiseProjectManager
	{
		private readonly ICruiseManager manager;
		private readonly string projectName;
		private readonly IProjectSerializer projectSerializer = new NetReflectorProjectSerializer();
		
		private IProject project
		{
			get
			{
				string serializedProject = string.Empty;
				serializedProject = manager.GetProject(projectName);
				try
				{
					return this.projectSerializer.Deserialize(serializedProject);
				}
				catch (Exception ex)
				{
					foreach (string filePath in Directory.GetFiles(Path.GetDirectoryName(this.GetType().Assembly.Location), "*plugin*.dll"))
					{
						System.Reflection.Assembly.LoadFile(filePath);
					}
					serializedProject = manager.GetProject(projectName);
					return this.projectSerializer.Deserialize(serializedProject);
				}
			}
		}


		public CruiseProjectManager(ICruiseManager server, string projectName)
		{
			this.manager = server;
			this.projectName = projectName;
		}

		public void ForceBuild()
		{
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
			if (clientInfoRequired)
			{
				ForceFilterClientInfo[] clientInfo;
				clientInfo = (ForceFilterClientInfo[]) clientInfoList.ToArray(typeof(ForceFilterClientInfo));
				manager.ForceBuild(ProjectName, clientInfo);
			}
			else
			{
				manager.ForceBuild(ProjectName);
			}
		}

		public ProjectStatus ProjectStatus
		{
			get
			{
                return manager.GetProjectStatus(ProjectName);
			}
		}

		public string ProjectName
		{
			get { return projectName; }
		}
	}
}