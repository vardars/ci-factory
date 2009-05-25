using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting;
using NAnt.Core;
using NAnt.Core.Types;
using NAnt.Core.Attributes;
using ThoughtWorks.CruiseControl.Remote;
using CIFactory.NAnt.Types;

namespace CIFactory.NAnt.CCNet.Functions
{
    [FunctionSet("ccnet", "ccnet")]
    public class CCNetFunctions : FunctionSetBase
    {

        #region Constructors

        public CCNetFunctions(Project project, Location location, PropertyDictionary properties)
            : base(project, location, properties)
        {

        }

        #endregion

        [Function("get-project-activity")]
        public string GetProjectActivity(string serverUrl, string projectName)
        {
            ICruiseManager Manager = (ICruiseManager)RemotingServices.Connect(typeof(ICruiseManager), serverUrl);
            return Manager.GetProjectStatus(projectName).Activity.ToString();
        }

        [Function("try-get-project-activity")]
        public string TryGetProjectActivity(string serverUrl, string projectName)
        {
            try
            {
                ICruiseManager Manager = (ICruiseManager)RemotingServices.Connect(typeof(ICruiseManager), serverUrl);
                return Manager.GetProjectStatus(projectName).Activity.ToString();
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        [Function("get-project-build-status")]
        public string GetProjectBuildStatus(string serverUrl, string projectName)
        {
            ICruiseManager Manager = (ICruiseManager)RemotingServices.Connect(typeof(ICruiseManager), serverUrl);
            return Manager.GetProjectStatus(projectName).BuildStatus.ToString();
        }

        [Function("try-get-project-state")]
        public string TryGetProjectState(string serverUrl, string projectName)
        {
            try
            {
                ICruiseManager Manager = (ICruiseManager)RemotingServices.Connect(typeof(ICruiseManager), serverUrl);
                return Manager.GetProjectStatus(projectName).Status.ToString();
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
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

		[Function("get-project-last-build-date")]
		public string GetProjectLastBuildDate(string serverUrl, string projectName)
		{
			ICruiseManager Manager = (ICruiseManager)RemotingServices.Connect(typeof(ICruiseManager), serverUrl);
			return Manager.GetProjectStatus(projectName).LastBuildDate.ToString(ThoughtWorks.CruiseControl.Core.LogFile.FilenameDateFormat);
		}

		[Function("get-project-some-build-labels")]
		public void GetProjectSomeBuildLabels(string serverUrl, string projectName, int numberToRetrieve, string refID)
		{
			if (!this.Project.DataTypeReferences.Contains(refID))
				throw new BuildException(String.Format("The refid {0} is not defined.", refID));

			StringList RefStringList = (StringList)this.Project.DataTypeReferences[refID];

			ICruiseManager Manager = (ICruiseManager)RemotingServices.Connect(typeof(ICruiseManager), serverUrl);
            string[] Builds = Manager.GetMostRecentBuildNames(projectName, numberToRetrieve);

			foreach (string BuildName in Builds)
			{
				RefStringList.StringItems.Add(BuildName, new StringItem(BuildName));
			}
        }

        [Function("get-project-last-build-log")]
        public string GetProjectLastBuildLog(string serverUrl, string projectName)
        {
            ICruiseManager Manager = (ICruiseManager)RemotingServices.Connect(typeof(ICruiseManager), serverUrl);
            return Manager.GetLatestBuildName(projectName);
        }
    }
}
