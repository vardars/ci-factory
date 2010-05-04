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
using System.Text.RegularExpressions;

namespace CIFactory.TargetProcess.NAnt.DataTypes
{
    public abstract class TargetProcessEntity : DataTypeBase
    {

        #region Fields

        private string _Description;

        private string _HyperLink;

        private bool _IsProjectIdSet;

        private string _Name;

        private int? _ProcessId;

        private string _Project;

        private int _ProjectId;

        private string _State;

        private int? _StateId;

        private TargetProcessUser[] _UsersToAssign;

        #endregion

        #region Properties

        protected abstract string Category { get; }

        private bool _IsDescriptionSet;
        public bool IsDescriptionSet
        {
            get { return _IsDescriptionSet; }
            set { _IsDescriptionSet = value; }
        }
        

        [TaskAttribute("description")]
        public string Description
        {
            get
            {
                if (_Description == null)
                    Description = this.FindEntityDescription();
                return _Description;
            }
            set
            {
                _Description = value;
                IsDescriptionSet = true;
            }
        }

        protected abstract int EntityId { get; }

        [TaskAttribute("name", Required = true)]
        public string EntityName
        {
            get
            {
                if (_Name == null)
                    _Name = this.FindEntityName();
                return _Name;
            }
            set { _Name = value; }
        }

        protected abstract string EntityType { get; }

        protected abstract string EntityTypeName { get; }

        protected string HyperLink
        {
            get
            {
                if (_HyperLink == null)
                    _HyperLink = this.CreateHyperLink(this.Category, this.EntityType, this.EntityId);
                return _HyperLink;
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

        protected int? ProcessId
        {
            get
            {
                if (_ProcessId == null)
                    _ProcessId = this.FindProcessId();
                return _ProcessId;
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

        [TaskAttribute("state")]
        public string State
        {
            get
            {
                if (_State == null)
                    _State = this.FindEntityState();
                return _State;
            }
            set
            {
                _State = value;
            }
        }

        public int? StateId
        {
            get
            {
                if (_StateId == null)
                    _StateId = this.FindStateId();
                return _StateId;
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

        #endregion

        #region Public Methods

        public abstract bool Exists();

        public abstract void Create();

        public virtual XmlDocument GenerateReport()
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(document.CreateElement("Entity").OuterXml);
            XmlElement rootElement = document.DocumentElement;

            XmlAttribute nameAttribute = document.CreateAttribute("Name");
            nameAttribute.InnerText = this.EntityName;
            rootElement.Attributes.Append(nameAttribute);

            XmlAttribute stateAttribute = document.CreateAttribute("State");
            stateAttribute.InnerText = this.State;
            rootElement.Attributes.Append(stateAttribute);

            XmlAttribute linkAttribute = document.CreateAttribute("HyperLink");
            linkAttribute.InnerText = ServicesCF.ConnectionInformation.RootServiceUrl + this.HyperLink;
            rootElement.Attributes.Append(linkAttribute);

            XmlAttribute typeAttribute = document.CreateAttribute("Type");
            typeAttribute.InnerText = this.EntityType;
            rootElement.Attributes.Append(typeAttribute);

            XmlAttribute idAttribute = document.CreateAttribute("Id");
            idAttribute.InnerText = this.EntityId.ToString();
            rootElement.Attributes.Append(idAttribute);

            XmlNode descriptionNode = document.CreateNode(XmlNodeType.Element, null, "Description", null);

            string entityDescription = String.Empty;
            if (this.Description != null)
            {
                entityDescription = this.Description;

                entityDescription = Regex.Replace(entityDescription, @"(?'pre'\</{0,})\w+\:(?'name'\w+)", "${pre}${name}");
                entityDescription = Regex.Replace(entityDescription, @"\&nbsp\;", @"&#0160;");
                entityDescription = Regex.Replace(entityDescription, @"namespaceuri\="".*""", "");
            }

            try
            {
                descriptionNode.InnerXml = entityDescription;
            }
            catch (Exception ex)
            {
                Log(Level.Error, String.Format("The description for {0} # {1} is not well formed xml: {2}.", this.EntityType, this.EntityId, ex.Message));
                descriptionNode.InnerText = entityDescription;
            }

            rootElement.AppendChild(descriptionNode);

            return document;
        }

        public abstract void Update();

        #endregion

        #region Protected Methods

        protected string CreateHyperLink(string category, string entityType, int Id)
        {
            return string.Format("/Project/{0}/{1}/View.aspx?{1}ID={2}", category, entityType, Id);
        }

        protected abstract string FindEntityDescription();
        protected abstract string FindEntityName();
        protected abstract string FindEntityState();

        protected override void InitializeElement(XmlNode elementNode)
        {
            if (String.IsNullOrEmpty(this.TargetProcessProject) && !this.IsProjectIdSet)
                throw new BuildException("You must either the project or the projectid.", this.Location);

            base.InitializeElement(elementNode);
        }

        #endregion

        #region Private Methods

        private int FindProcessId()
        {
            ProjectService projectService = ServicesCF.GetService<ProjectService>();

            string hqlQuery = "from Project as project where project.Name = ?";
            ProjectDTO[] projects = projectService.Retrieve(hqlQuery, new object[] { this.TargetProcessProject });

            if (projects.Length == 0)
                throw new BuildException(string.Format("Could not find a project named: '{0}'.", this.TargetProcessProject));

            return projects[0].ProcessID.Value;
        }

        private int FindProjectId()
        {
            ProjectService projectService = ServicesCF.GetService<ProjectService>();

            string hqlQuery = "from Project as project where project.Name = ?";
            ProjectDTO[] projects = projectService.Retrieve(hqlQuery, new object[] { this.TargetProcessProject });

            if (projects.Length == 0)
                throw new BuildException(string.Format("Could not find a project named: '{0}'.", this.TargetProcessProject));

            return projects[0].ProjectID.Value;
        }

        private int FindStateId()
        {
            ProcessService processService = ServicesCF.GetService<ProcessService>();

            EntityStateDTO[] states = processService.RetrieveEntityStatesForProcess(this.ProcessId.Value);

            IEnumerable<int?> stateIdsEnum = from state in states where state.EntityTypeName == this.EntityTypeName && state.Name == this.State select state.ID;
            
            List<int?> stateIds = stateIdsEnum.ToList<int?>();

            if (stateIds.Count == 0)
            {
                string flatOptions = String.Empty;
                states
                    .Where(state => state.EntityTypeName == this.EntityTypeName)
                    .Select(state => state.Name)
                    .ToList<string>()
                    .ForEach(option => flatOptions += " '" + option + "'");
                string message = string.Format("Could not find a state named '{0}' in the options: {1}.", this.State, flatOptions);
                throw new BuildException(message);
            }

            return stateIds[0].Value;
        }

        #endregion

    }
}
