using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Xml;
using NAnt.Core;
using NAnt.Core.Attributes;
using CIFactory.TargetProcess.Common.BugWebService;
using CIFactory.TargetProcess.Common;
using CIFactory.TargetProcess.NAnt.Helpers;
using UserStoryWebService = CIFactory.TargetProcess.Common.UserStoryWebService;

namespace CIFactory.TargetProcess.NAnt.DataTypes
{
    [Serializable, ElementName("targetprocessbug")]
    public class TargetProcessBug : TargetProcessEntity
    {
        private bool _IsBugIdSet;

        private bool _IsUserStoryIdSet;

        private int _BugId;

        private BugService _BugService;

        private string _UserStory;

        private int _UserStoryId;

        public bool IsBugIdSet
        {
            get { return _IsBugIdSet; }
            set
            {
                _IsBugIdSet = value;
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

        [TaskAttribute("bugid")]
        public int BugId
        {
            get
            {
                if (!IsBugIdSet)
                    _BugId = this.FindBugIdByName(this.EntityName);
                return _BugId;
            }
            set
            {
                _BugId = value;
                this.IsBugIdSet = true;
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

        public BugService BugService
        {
            get
            {
                if (_BugService == null)
                    _BugService = ServicesCF.GetService<BugService>();
                return _BugService;
            }
        }
        protected override string Category
        {
            get { return "QA"; }
        }

        protected override int EntityId
        {
            get { return this.BugId; }
        }

        protected override string EntityType
        {
            get { return "Bug"; }
        }

        protected override string EntityTypeName
        {
            get { return "Tp.BusinessObjects.Bug"; }
        }

        public override void Create()
        {
            BugDTO bug = new BugDTO
            {
                Name = this.EntityName,
                ProjectID = this.ProjectId
            };

            if (!String.IsNullOrEmpty(this.UserStory) || IsUserStoryIdSet)
                bug.UserStoryID = this.UserStoryId;

            if (IsDescriptionSet)
                bug.Description = this.Description;

            if (!String.IsNullOrEmpty(this.State))
                bug.EntityStateID = this.StateId;

            int taskId = BugService.Create(bug);

            foreach (TargetProcessUser user in this.UsersToAssign)
            {
                BugService.AssignUser(taskId, user.GetId());
            }
        }

        public override void Update()
        {
            BugDTO bug = BugService.GetByID(this.BugId);

            bug.Name = this.EntityName;
            bug.ProjectID = this.ProjectId;

            if (IsDescriptionSet)
                bug.Description = this.Description;

            if (!String.IsNullOrEmpty(this.UserStory) || IsUserStoryIdSet)
                bug.UserStoryID = this.UserStoryId;

            if (!String.IsNullOrEmpty(this.State))
                bug.EntityStateID = this.StateId;

            BugService.Update(bug);

            foreach (TargetProcessUser user in this.UsersToAssign)
            {
                BugService.AssignUser(bug.BugID.Value, user.GetId());
            }
        }

        private string FindBugNameById(int bugId)
        {
            return this.BugService.GetByID(this.BugId).Name;
        }

        protected override string FindEntityName()
        {
            return this.FindBugNameById(this.BugId);
        }

        private int FindBugIdByName(string entityName)
        {
            string hqlQuery = "select from Bug as bug where bug.Name = ?";
            BugDTO[] bugs = BugService.Retrieve(hqlQuery, new object[] { entityName });

            if (bugs.Length == 0)
                throw new BuildException(string.Format("Could not find a bug named: '{0}'.", entityName));

            return bugs[0].BugID.Value;
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

        protected override string FindEntityDescription()
        {
            return this.BugService.GetByID(this.BugId).Description;
        }

        protected override string FindEntityState()
        {
            return this.BugService.GetByID(this.BugId).EntityStateName;
        }

        public override bool Exists()
        {
            try
            {
                this.BugService.GetByID(this.BugId);
                return true;
            }
            catch (Exception ex)
            {
                this.Log(Level.Error, string.Format("Could not find a {0} with id {1}: {2}", this.EntityType, this.BugId, ex.Message));
                return false;
            }
        }
    }
}
