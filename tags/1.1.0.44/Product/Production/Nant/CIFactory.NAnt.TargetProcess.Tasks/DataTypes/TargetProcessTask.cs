using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Xml;
using NAnt.Core;
using NAnt.Core.Attributes;
using CIFactory.TargetProcess.Common.TaskWebService;
using CIFactory.TargetProcess.Common;
using CIFactory.TargetProcess.NAnt.Helpers;
using UserStoryWebService = CIFactory.TargetProcess.Common.UserStoryWebService;

namespace CIFactory.TargetProcess.NAnt.DataTypes
{
    [Serializable, ElementName("targetprocesstask")]
    public class TargetProcessTask : TargetProcessEntity
    {

        #region Fields

        private bool _IsTaskIdSet;

        private bool _IsUserStoryIdSet;

        private int _TaskId;

        private TaskService _TaskService;

        private string _UserStory;

        private int _UserStoryId;

        #endregion

        #region Properties

        protected override string Category
        {
            get { return "Planning"; }
        }

        protected override int EntityId
        {
            get { return this.TaskId; }
        }

        protected override string EntityType
        {
            get { return "Task"; }
        }

        protected override string EntityTypeName
        {
            get { return "Tp.BusinessObjects.Task"; }
        }

        public bool IsTaskIdSet
        {
            get { return _IsTaskIdSet; }
            set
            {
                _IsTaskIdSet = value;
            }
        }

        private bool IsUserStoryIdSet
        {
            get { return _IsUserStoryIdSet; }
            set
            {
                _IsUserStoryIdSet = value;
            }
        }

        [TaskAttribute("taskid")]
        public int TaskId
        {
            get
            {
                if (!IsTaskIdSet)
                    _TaskId = this.FindTaskIdByName(this.EntityName);
                return _TaskId;
            }
            set
            {
                _TaskId = value;
                this.IsTaskIdSet = true;
            }
        }

        public TaskService TaskService
        {
            get
            {
                if (_TaskService == null)
                    _TaskService = ServicesCF.GetService<TaskService>();
                return _TaskService;
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
                    _UserStoryId = this.FindUserStoryIdByName(this.UserStory);
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

        public override void Create()
        {
            TaskDTO task = new TaskDTO
            {
                Name = this.EntityName,
                UserStoryID = this.UserStoryId,
                ProjectID = this.ProjectId
            };

            if (IsDescriptionSet)
                task.Description = this.Description;

            if (!String.IsNullOrEmpty(this.State))
                task.EntityStateID = this.StateId;

            int taskId = TaskService.Create(task);

            foreach (TargetProcessUser user in this.UsersToAssign)
            {
                TaskService.AssignUser(taskId, user.GetId());
            }
        }

        public override void Update()
        {
            TaskDTO task = TaskService.GetByID(this.TaskId);

            task.Name = this.EntityName;
            task.UserStoryID = this.UserStoryId;
            task.ProjectID = this.ProjectId;

            if (IsDescriptionSet)
                task.Description = this.Description;

            if (!String.IsNullOrEmpty(this.State))
                task.EntityStateID = this.StateId;

            TaskService.Update(task);

            foreach (TargetProcessUser user in this.UsersToAssign)
            {
                TaskService.AssignUser(task.TaskID.Value, user.GetId());
            }
        }

        #endregion

        #region Protected Methods

        protected override string FindEntityName()
        {
            return this.FindTaskNameById(this.TaskId);
        }

        protected override void InitializeElement(XmlNode elementNode)
        {
            if (String.IsNullOrEmpty(this.UserStory) && !this.IsUserStoryIdSet)
                throw new BuildException("You must either the userstory or the userstoryid.", this.Location);

            base.InitializeElement(elementNode);
        }

        #endregion

        #region Private Methods

        private int FindTaskIdByName(string entityName)
        {
            string hqlQuery = "select from Task as task where task.Name = ?";
            TaskDTO[] tasks = TaskService.Retrieve(hqlQuery, new object[] { entityName });

            if (tasks.Length == 0)
                throw new BuildException(string.Format("Could not find a task named: '{0}'.", entityName));

            return tasks[0].TaskID.Value;
        }

        private string FindTaskNameById(int taskId)
        {
            return TaskService.GetByID(taskId).Name;
        }

        private int FindUserStoryIdByName(string userStory)
        {
            UserStoryWebService.UserStoryService storyService = ServicesCF.GetService<UserStoryWebService.UserStoryService>();

            string hqlQuery = "select from UserStory as story where story.Name = ?";
            UserStoryWebService.UserStoryDTO[] storyies = storyService.Retrieve(hqlQuery, new object[] { userStory });

            if (storyies.Length == 0)
                throw new BuildException(string.Format("Could not find a story named: '{0}'.", userStory));

            return storyies[0].UserStoryID.Value;
        }

        #endregion


        protected override string FindEntityDescription()
        {
            return this.TaskService.GetByID(this.TaskId).Description;
        }

        protected override string FindEntityState()
        {
            return this.TaskService.GetByID(this.TaskId).EntityStateName;
        }

        public override bool Exists()
        {
            try
            {
                this.TaskService.GetByID(this.TaskId);
                return true;
            }
            catch (Exception ex)
            {
                this.Log(Level.Error, string.Format("Could not find a {0} with id {1}: {2}", this.EntityType, this.TaskId, ex.Message));
                return false;
            }
        }
    }
}
