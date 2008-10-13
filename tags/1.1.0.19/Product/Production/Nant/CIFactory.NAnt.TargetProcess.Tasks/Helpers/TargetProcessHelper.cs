using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using CIFactory.TargetProcess.Common.BugWebService;
using CIFactory.TargetProcess.Common.TaskWebService;
using CIFactory.TargetProcess.Common.UserStoryWebService;
using CIFactory.TargetProcess.Common;

namespace CIFactory.TargetProcess.NAnt.Helpers
{
    public class TargetProcessHelper : ITargetProcessHelper
    {
        #region Fields

        private string _Password = String.Empty;
        private string _RootWebServiceUrl;
        private string _UserName = String.Empty;
        private UserStoryService _UserStoryService;
        private TaskService _TaskService;
        private BugService _BugService;

        #endregion

        #region Constructors

        public TargetProcessHelper()
        {

        }

        #endregion

        #region Properties

        public string Password
        {
            get
            {
                return this._Password;
            }
            set
            {
                this._Password = value;
            }
        }

        public string RootWebServiceUrl
        {
            get
            {
                return _RootWebServiceUrl;
            }
            set
            {
                _RootWebServiceUrl = value;
            }
        }
        public string UserName
        {
            get
            {
                return this._UserName;
            }
            set
            {
                this._UserName = value;
            }
        }

        public TaskService TaskService
        {
            get
            {
                if (_TaskService == null)
                {
                    _TaskService = new TaskService();
                    _TaskService.Url = RootWebServiceUrl + "/Services/TaskService.asmx";
                    TpPolicy.ApplyAutheticationTicket(_TaskService, this.UserName, this.Password);
                }
                return _TaskService;
            }
            set
            {
                _TaskService = value;
            }
        }

        public UserStoryService UserStoryService
        {
            get
            {
                if (_UserStoryService == null)
                {
                    _UserStoryService = new UserStoryService();
                    _UserStoryService.Url = RootWebServiceUrl + "/Services/UserStoryService.asmx";
                    TpPolicy.ApplyAutheticationTicket(_UserStoryService, this.UserName, this.Password);
                }
                return _UserStoryService;
            }
            set
            {
                _UserStoryService = value;
            }
        }

        public BugService BugService
        {
            get
            {
                if (_BugService == null)
                {
                    _BugService = new BugService();
                    _BugService.Url = RootWebServiceUrl + "/Services/BugService.asmx";
                    TpPolicy.ApplyAutheticationTicket(_BugService, this.UserName, this.Password);
                }
                return _BugService;
            }
            set
            {
                _BugService = value;
            }
        }
        #endregion

        #region Public Methods

        public Entity RetrieveEntity(int id, string entityType)
        {
            ITpDto tpEntity = null;
            string LinkTypePart = String.Empty;

            if (entityType == "Task")
            {
                tpEntity = TaskService.GetByID(id);
                LinkTypePart = "Planning/" + entityType;
            }
            else if (entityType == "UserStory")
            {
                tpEntity = UserStoryService.GetByID(id);
                LinkTypePart = "Planning/" + entityType;
            }
            else if (entityType == "Bug")
            {
                tpEntity = BugService.GetByID(id);
                LinkTypePart = "QA/" + entityType;
            }

            String hyperLink = "/Project/" + LinkTypePart + "/View.aspx?" + entityType + "ID=" + tpEntity.ID;
            Entity entity = new Entity(tpEntity.Description, hyperLink, tpEntity.Name, entityType, id);

            return entity;
        }

        #endregion

    }
}
