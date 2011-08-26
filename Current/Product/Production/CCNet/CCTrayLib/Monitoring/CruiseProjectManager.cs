using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
//using System.Runtime.Serialization;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Remote;
using Exortech.NetReflector;
using Exortech.NetReflector.Util;
using ThoughtWorks.CruiseControl.CCTrayLib.ServerConnection;
//using FF=CCNET.Extensions.Plugin.ForceFilters;
//using UFF=CCNET.Extensions.Plugin.ForceFilters.UserFilter; 

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	/// <summary>
	/// Allows access to the state of a single cruise control project 
	/// </summary>
	public class CruiseProjectManager : ICruiseProjectManager
	{
        private readonly IWebCruiseManager manager;
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
                catch
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
        
		public CruiseProjectManager(IWebCruiseManager server, string projectName)
		{
			this.manager = server;
			this.projectName = projectName;
		}

		public void ForceBuild()
		{
            bool clientInfoRequired = false;
            List<ForceFilterClientInfo> clientInfoList = new List<ForceFilterClientInfo>();
            
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
                Type[] extraTypes = new Type[] { typeof(PasswordInformation), typeof(HostInformation), typeof(UserInformation) };
                ForceFilterClientInfo[] forceFilterClientInfoList = new ForceFilterClientInfo[clientInfoList.Count];
                forceFilterClientInfoList = clientInfoList.ToArray();

                XmlSerializer clientInfoSerializer = new XmlSerializer(typeof(ForceFilterClientInfo[]), extraTypes);
                StringWriter clientInfoBuffer = new StringWriter();
                clientInfoSerializer.Serialize(clientInfoBuffer, forceFilterClientInfoList);
                string serializedClientInfo = clientInfoBuffer.ToString();
                clientInfoBuffer.Close();

                manager.ForceBuild(ProjectName, serializedClientInfo);
                
            }
            else
            {
                manager.ForceBuild(ProjectName, string.Empty);
            }            
		}

		public ProjectStatus ProjectStatus
		{
			get
			{
                return manager.GetProjectStatusLite(ProjectName);
			}
		}

		public string ProjectName
		{
			get { return projectName; }
		}
	}
}