using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Xml;
using NAnt.Core;
using NAnt.Core.Attributes;
using TargetProcess.Common.TaskWebService;
using TargetProcess.Common;
using CIFactory.TargetProcess.NAnt.Helpers;
using UserStoryWebService = TargetProcess.Common.UserStoryWebService;

namespace CIFactory.TargetProcess.NAnt.DataTypes
{
    [Serializable, ElementName("task")]
    public class TargetProcessTask : TargetProcessEntity
    {

        #region Fields

        private bool _IsUserStoryIdSet;

        private string _UserStory;

        private int _UserStoryId;

        #endregion

        #region Properties

        private bool IsUserStoryIdSet
        {
            get { return _IsUserStoryIdSet; }
            set
            {
                _IsUserStoryIdSet = value;
            }
        }

        [TaskAttribute("userstory")]
        public string UserStory
        {
            get { return _UserStory; }
            set
            {
                _UserStory = value;
            }
        }

        [TaskAttribute("userstoryid")]
        public int UserStoryId
        {
            get
            {
                if (!IsUserStoryIdSet)
                    _UserStoryId = this.FindUserStoryId();
                return _UserStoryId;
            }
            set
            {
                _UserStoryId = value;
                this.IsUserStoryIdSet = true;
            }
        }

        #endregion

        #region Public Methods

        public override void Create(string rootWebServiceUrl, string userName, string password)
        {
            TaskService taskService = ServicesCF.GetService<TaskService>();

            TaskDTO taskDTO = new TaskDTO
            {
                Name = this.EntityName,
                Description = this.Description,
                UserStoryID = this.UserStoryId,
                ProjectID = this.ProjectId
            };
            int taskId = taskService.Create(taskDTO);

            foreach (TargetProcessUser user in this.UsersToAssign)
            {
                taskService.AssignUser(taskId, user.GetId());
            }
        }

        #endregion

        #region Protected Methods

        protected override void InitializeElement(XmlNode elementNode)
        {
            if (String.IsNullOrEmpty(this.UserStory) && !this.IsUserStoryIdSet)
                throw new BuildException("You must either the userstory or the userstoryid.", this.Location);

            base.InitializeElement(elementNode);
        }

        #endregion

        #region Private Methods

        private int FindUserStoryId()
        {
            UserStoryWebService.UserStoryService storyService = ServicesCF.GetService<UserStoryWebService.UserStoryService>();

            string hqlQuery = "select from UserStory as story where story.Name = ?";
            UserStoryWebService.UserStoryDTO[] storyies = storyService.Retrieve(hqlQuery, new object[] { this.UserStory });

            if (storyies.Length == 0)
                throw new BuildException(string.Format("Could not find a story named: '{0}'.", this.UserStory));

            return storyies[0].UserStoryID.Value;
        }

        #endregion

    }
}
