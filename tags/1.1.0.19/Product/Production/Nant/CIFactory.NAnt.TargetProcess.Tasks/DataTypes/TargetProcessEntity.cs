using System;
using System.Linq;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Xml;
using NAnt.Core;
using NAnt.Core.Attributes;
using CIFactory.TargetProcess.Common;
using CIFactory.TargetProcess.NAnt.Helpers;
using CIFactory.TargetProcess.Common.ProjectWebService;
using CIFactory.TargetProcess.Common.ProcessWebService;
using System.Collections.Generic;

namespace CIFactory.TargetProcess.NAnt.DataTypes
{
    public abstract class TargetProcessEntity : DataTypeBase
    {
        #region Fields

        private string _Description;

        private bool _IsProjectIdSet;

        private string _Name;

        private string _Project;

        private int _ProjectId;

        private TargetProcessUser[] _UsersToAssign;

        #endregion

        #region Properties

        [TaskAttribute("description")]
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        [TaskAttribute("name", Required = true)]
        public string EntityName
        {
            get { return _Name; }
            set { _Name = value; }
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
                    _ProjectId = this.FindProjectId();
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

        [BuildElementArray("users")]
        public TargetProcessUser[] UsersToAssign
        {
            get
            {
                if (_UsersToAssign == null)
                    _UsersToAssign = new TargetProcessUser[] { };
                return this._UsersToAssign;
            }
            set { this._UsersToAssign = value; }
        }

        private string _State;

        [TaskAttribute("state")]
        public string State
        {
            get { return _State; }
            set
            {
                _State = value;
            }
        }

        private int? _ProcessId;
        protected int? ProcessId
        {
            get
            {
                if (_ProcessId == null)
                    _ProcessId = this.FindProcessId();
                return _ProcessId;
            }
        }

        private int? _StateId;
        public int? StateId
        {
            get
            {
                if (_StateId == null)
                    _StateId = this.FindStateId();
                return _StateId;
            }
        }

        protected abstract string EntityTypeName { get; }

        #endregion

        #region Public Methods

        public abstract void Create();
        public abstract void Update();

        #endregion

        #region Protected Methods

        protected override void InitializeElement(XmlNode elementNode)
        {
            if (String.IsNullOrEmpty(this.TargetProcessProject) && !this.IsProjectIdSet)
                throw new BuildException("You must either the project or the projectid.", this.Location);

            base.InitializeElement(elementNode);
        }

        #endregion

        #region Private Methods

        private int FindStateId()
        {
            ProcessService processService = ServicesCF.GetService<ProcessService>();
            
            EntityStateDTO[] states = processService.RetrieveEntityStatesForProcess(this.ProcessId.Value);

            IEnumerable<int?> stateIdsEnum = from state in states where state.EntityTypeName == this.EntityTypeName && state.Name == this.State select state.ID;

            List<int?> stateIds = stateIdsEnum.ToList<int?>();

            if (stateIds.Count == 0)
                throw new BuildException(string.Format("Could not find a state named: '{0}'.", this.State));

            return stateIds[0].Value;
        }

        private int FindProcessId()
        {
            ProjectService projectService = ServicesCF.GetService<ProjectService>();

            string hqlQuery = "select from Project as project where project.Name = ?";
            ProjectDTO[] projects = projectService.Retrieve(hqlQuery, new object[] { this.TargetProcessProject });

            if (projects.Length == 0)
                throw new BuildException(string.Format("Could not find a project named: '{0}'.", this.TargetProcessProject));

            return projects[0].ProcessID.Value;
        }

        private int FindProjectId()
        {
            ProjectService projectService = ServicesCF.GetService<ProjectService>();

            string hqlQuery = "select from Project as project where project.Name = ?";
            ProjectDTO[] projects = projectService.Retrieve(hqlQuery, new object[] { this.TargetProcessProject });

            if (projects.Length == 0)
                throw new BuildException(string.Format("Could not find a project named: '{0}'.", this.TargetProcessProject));

            return projects[0].ProjectID.Value;
        }

        #endregion

    }
}
