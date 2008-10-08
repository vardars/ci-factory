using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Xml;
using NAnt.Core;
using NAnt.Core.Attributes;
using Microsoft.Web.Services3;
using TargetProcess.Common;
using CIFactory.TargetProcess.NAnt.Helpers;
using TargetProcess.Common.ProjectWebService;

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

        [TaskAttribute("description", Required = true)]
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
            get { return this._UsersToAssign; }
            set { this._UsersToAssign = value; }
        }

        #endregion

        #region Public Methods

        public abstract void Create(string rootWebServiceUrl, string userName, string password);

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
