using System;
using System.Linq;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Xml;
using NAnt.Core;
using NAnt.Core.Attributes;
using CIFactory.TargetProcess.Common.UserStoryWebService;
using CIFactory.TargetProcess.Common;
using CIFactory.TargetProcess.NAnt.Helpers;
using UserStoryWebService = CIFactory.TargetProcess.Common.UserStoryWebService;
using CustomFieldWebService = CIFactory.TargetProcess.Common.CustomFieldWebService;
using System.Reflection;
using System.Collections.Generic;

namespace CIFactory.TargetProcess.NAnt.DataTypes
{
    [Serializable, ElementName("tasktargetprocessuserstory")]
    public class TargetProcessUserStory : TargetProcessEntity
    {

        #region Fields

        private bool _IsUserStoryIdSet;

        private UserStoryService _StoryService;

        private int _UserStoryId;

        #endregion

        #region Properties

        protected override string Category
        {
            get { return "Planning"; }
        }

        protected override int EntityId
        {
            get { return this.UserStoryId; }
        }

        protected override string EntityType
        {
            get { return "UserStory"; }
        }

        protected override string EntityTypeName
        {
            get { return "Tp.BusinessObjects.UserStory"; }
        }

        private bool IsUserStoryIdSet
        {
            get { return _IsUserStoryIdSet; }
            set
            {
                _IsUserStoryIdSet = value;
            }
        }

        public UserStoryService StoryService
        {
            get
            {
                if (_StoryService == null)
                    _StoryService = ServicesCF.GetService<UserStoryService>();
                return _StoryService;
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

        public override void Create()
        {
            UserStoryDTO story = new UserStoryDTO
            {
                Name = this.EntityName,
                ProjectID = this.ProjectId
            };

            if (IsDescriptionSet)
                story.Description = this.Description;

            if (!String.IsNullOrEmpty(this.State))
                story.EntityStateID = this.StateId;

            int storyId = StoryService.Create(story);

            foreach (TargetProcessUser user in this.UsersToAssign)
            {
                StoryService.AssignUser(storyId, user.GetId());
            }
        }

        public override XmlDocument GenerateReport()
        {
            XmlDocument report = base.GenerateReport();
            XmlElement rootElement = report.DocumentElement;

            UserStoryDTO story = StoryService.GetByID(this.UserStoryId);
            this.ProjectId = story.ProjectID.Value;
            this.TargetProcessProject = story.ProjectName;

            CustomFieldWebService.CustomFieldService fieldService = ServicesCF.GetService<CustomFieldWebService.CustomFieldService>();

            CustomFieldWebService.CustomFieldDTO[] allCustomFields = fieldService.RetrieveAll();
            List<CustomFieldWebService.CustomFieldDTO> customFields = allCustomFields.Where(field => field.EntityTypeName == this.EntityTypeName && field.ProcessID == this.ProcessId).ToList<CustomFieldWebService.CustomFieldDTO>();
            foreach (CustomFieldWebService.CustomFieldDTO customField in customFields)
            {
                XmlNode customFieldNode = report.CreateNode(XmlNodeType.Element, null, "CustomField", null);

                XmlNode customFieldNameNode = report.CreateNode(XmlNodeType.Element, null, "Name", null);
                customFieldNameNode.InnerText = customField.Name;
                customFieldNode.AppendChild(customFieldNameNode);

                XmlNode customFieldValueNode = report.CreateNode(XmlNodeType.Element, null, "Value", null);
                PropertyInfo propertyInfo = story.GetType().GetProperty(customField.EntityFieldName);
                customFieldValueNode.InnerText = (String)propertyInfo.GetValue(story, null);
                customFieldNode.AppendChild(customFieldValueNode);

                rootElement.AppendChild(customFieldNode);
            }

            return report;
        }

        public override void Update()
        {
            UserStoryDTO story = StoryService.GetByID(this.UserStoryId);

            story.Name = this.EntityName;
            story.ProjectID = this.ProjectId;

            if (IsDescriptionSet)
                story.Description = this.Description;

            if (!String.IsNullOrEmpty(this.State))
                story.EntityStateID = this.StateId;

            StoryService.Update(story);

            foreach (TargetProcessUser user in this.UsersToAssign)
            {
                StoryService.AssignUser(story.UserStoryID.Value, user.GetId());
            }
        }

        #endregion

        #region Protected Methods

        protected override string FindEntityName()
        {
            return this.FindUserStoryNameById(this.UserStoryId);
        }

        #endregion

        #region Private Methods

        private int FindUserStoryId()
        {
            string hqlQuery = "from UserStory as story where story.Name = ?";
            UserStoryDTO[] storyies = StoryService.Retrieve(hqlQuery, new object[] { this.EntityName });

            if (storyies.Length == 0)
                throw new BuildException(string.Format("Could not find a story named: '{0}'.", this.EntityName));

            return storyies[0].UserStoryID.Value;
        }

        private string FindUserStoryNameById(int userStoryId)
        {
            return this.StoryService.GetByID(userStoryId).Name;
        }

        #endregion


        protected override string FindEntityDescription()
        {
            return this.StoryService.GetByID(this.UserStoryId).Description;
        }

        protected override string FindEntityState()
        {
            return this.StoryService.GetByID(this.UserStoryId).EntityStateName;
        }

        public override bool Exists()
        {
            try
            {
                this.StoryService.GetByID(this.UserStoryId);
                return true;
            }
            catch (Exception ex)
            {
                this.Log(Level.Error, string.Format("Could not find a {0} with id {1}: {2}", this.EntityType, this.UserStoryId, ex.Message));
                return false;
            }
        }
    }
}
