using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using NAnt.Core;
using NAnt.Core.Tasks;
using NAnt.Core.Attributes;
using NAnt.Core.Util;
using CIFactory.NAnt.Types;
using System.Xml;
using System.Text.RegularExpressions;
using CIFactory.TargetProcess.NAnt.Helpers;
using CIFactory.TargetProcess.NAnt.DataTypes;
using CIFactory.TargetProcess.Common.IterationWebService;
using CIFactory.TargetProcess.Common.ProjectWebService;

namespace CIFactory.TargetProcess.NAnt.Tasks
{
    [TaskName("targetprocessgetcurrentuserstories")]
    public class TargetProcessGetCurrentUserStoriesTask : Task
    {

        #region Fields

        private ConnectionInformation _ConnectionInformation;

        private bool _IsProjectIdSet;

        private string _Project;

        private int _ProjectId;

        private string _UserStoryListRef;

        #endregion

        #region Properties

        [BuildElement("connectioninformation", Required = false)]
        public ConnectionInformation ConnectionInformation
        {
            get
            {
                if (this._ConnectionInformation == null)
                    this._ConnectionInformation = new ConnectionInformation();
                return this._ConnectionInformation;
            }
            set
            {
                this._ConnectionInformation = value;
            }
        }

        private bool IsProjectIdSet
        {
            get { return _IsProjectIdSet; }
            set
            {
                _IsProjectIdSet = value;
            }
        }

        [TaskAttribute("projectid")]
        public int ProjectId
        {
            get
            {
                if (!IsProjectIdSet)
                    ProjectId = this.FindProjectId();
                return _ProjectId;
            }
            set
            {
                _ProjectId = value;
                this.IsProjectIdSet = true;
            }
        }

        [TaskAttribute("project")]
        public string TargetProcessProject
        {
            get { return _Project; }
            set { _Project = value; }
        }

        [TaskAttribute("userstorylistref", Required = true)]
        public string UserStoryListRef
        {
            get { return _UserStoryListRef; }
            set
            {
                _UserStoryListRef = value;
            }
        }

        #endregion



        private int FindProjectId()
        {
            ProjectService projectService = ServicesCF.GetService<ProjectService>();

            string hqlQuery = "select from Project as project where project.Name = ?";
            ProjectDTO[] projects = projectService.Retrieve(hqlQuery, new object[] { this.TargetProcessProject });

            if (projects.Length == 0)
                throw new BuildException(string.Format("Could not find a project named: '{0}'.", this.TargetProcessProject));

            return projects[0].ProjectID.Value;
        }

        #region Protected Methods

        public void GetCurrentUserStories()
        {
            if (!this.Project.DataTypeReferences.Contains(this.UserStoryListRef))
                throw new BuildException(String.Format("The refid userstorylistref {0} is not defined.", this.UserStoryListRef));

            StringList RefStringList = (StringList)this.Project.DataTypeReferences[this.UserStoryListRef];

            ServicesCF.ConnectionInformation = this.ConnectionInformation;
            IterationService iterationService = ServicesCF.GetService<IterationService>();

            IterationDTO[] iterations = iterationService.RetrieveAll();

            List<IterationDTO> currentIterationList = iterations.Where(iteration => iteration.ParentProjectID == this.ProjectId && iteration.StartDate <= DateTime.Today && iteration.EndDate >= DateTime.Today).ToList<IterationDTO>();

            if (currentIterationList.Count == 0)
                throw new BuildException("Could not find the current iteration.", this.Location);

            IterationDTO currentIteration = currentIterationList[0];

            UserStoryDTO[] userStories = iterationService.RetrieveUserStoriesForIteration(currentIteration.IterationID.Value);

            foreach (UserStoryDTO userStory in userStories)
            {
                string userStoryId = userStory.UserStoryID.ToString();
                RefStringList.StringItems.Add(userStoryId, new StringItem(userStoryId));
            }
        }

        protected override void ExecuteTask()
        {
            this.GetCurrentUserStories();
        }

        protected override void InitializeElement(XmlNode elementNode)
        {
            if (String.IsNullOrEmpty(this.TargetProcessProject) && !this.IsProjectIdSet)
                throw new BuildException("You must either the project or the projectid.", this.Location);

            base.InitializeElement(elementNode);
        }

        #endregion

    }
}
