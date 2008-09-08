using System;
using System.Collections.Generic;
using System.Text;
using NAnt.Core;
using NAnt.Core.Tasks;
using NAnt.Core.Attributes;
using NAnt.Core.Util;
using CIFactory.NAnt.Types;
using System.Xml;
using System.Text.RegularExpressions;

namespace CIFactory.TargetProcess.NAnt.Tasks
{
    [TaskName("targetprocessreport")]
    public class TargetProcessReportTask : Task
    {
        #region Fields

        private StringList _BugIds;
        private ITargetProcessHelper _Helper;
        private StringList _StoryIds;
        private StringList _TaskIds;
        private string _Password = String.Empty;
        private string _ReportFilePath = String.Empty;
        private string _UserName = String.Empty;
        private string _RootServiceUrl;

        #endregion

        #region Properties

        [TaskAttribute("rootserviceurl", Required = true)]
        public string RootServiceUrl
        {
            get { return _RootServiceUrl; }
            set
            {
                _RootServiceUrl = value;
            }
        }

        public ITargetProcessHelper Helper
        {
            get
            {
                if (_Helper == null)
                    this._Helper = new TargetProcessHelper();
                return _Helper;
            }
            set
            {
                _Helper = value;
            }
        }

        [BuildElement("taskids", Required = false)]
        public StringList TaskIds
        {
            get
            {
                if (_TaskIds == null)
                    _TaskIds = new StringList();
                return this._TaskIds;
            }
            set
            {
                this._TaskIds = value;
            }
        }

        [BuildElement("storyids", Required = false)]
        public StringList StoryIds
        {
            get
            {
                if (_StoryIds == null)
                    _StoryIds = new StringList();
                return this._StoryIds;
            }
            set
            {
                this._StoryIds = value;
            }
        }

        [BuildElement("bugids", Required = false)]
        public StringList BugIds
        {
            get
            {
                if (_BugIds == null)
                    _BugIds = new StringList();
                return this._BugIds;
            }
            set
            {
                this._BugIds = value;
            }
        }

        [TaskAttribute("password", Required = true)]
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

        [TaskAttribute("reportfilepath", Required = true)]
        public string ReportFilePath
        {
            get
            {
                return this._ReportFilePath;
            }
            set
            {
                this._ReportFilePath = value;
            }
        }

        [TaskAttribute("username", Required = true)]
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

        #endregion

        #region Public Methods

        public void GenerateReport()
        {
            Helper.UserName = this.UserName;
            Helper.Password = this.Password;
            Helper.RootWebServiceUrl = this.RootServiceUrl;

            List<Entity> entities = new List<Entity>();

            foreach (string id in this.TaskIds)
            {
                Entity entity = Helper.RetrieveEntity(int.Parse(id), "Task");
                entities.Add(entity);
            }

            foreach (string id in this.StoryIds)
            {
                Entity entity = Helper.RetrieveEntity(int.Parse(id), "UserStory");
                entities.Add(entity);
            }

            foreach (string id in this.BugIds)
            {
                Entity entity = Helper.RetrieveEntity(int.Parse(id), "Bug");
                entities.Add(entity);
            }


            XmlDocument document = new XmlDocument();
            document.LoadXml(document.CreateElement("TargetProcess").OuterXml);
            XmlElement rootElement = document.DocumentElement;

            foreach (Entity entity in entities)
            {
                XmlNode node = document.CreateNode(XmlNodeType.Element, null, "Entity", null);

                XmlAttribute nameAttribute = document.CreateAttribute("Name");
                nameAttribute.InnerText = entity.Name;
                node.Attributes.Append(nameAttribute);

                XmlAttribute linkAttribute = document.CreateAttribute("HyperLink");
                linkAttribute.InnerText = this.RootServiceUrl + entity.HyperLink;
                node.Attributes.Append(linkAttribute);

                XmlAttribute typeAttribute = document.CreateAttribute("Type");
                typeAttribute.InnerText = entity.Type;
                node.Attributes.Append(typeAttribute);

                XmlAttribute idAttribute = document.CreateAttribute("Id");
                idAttribute.InnerText = entity.Id.ToString();
                node.Attributes.Append(idAttribute);

                string entityDescription = String.Empty;
                if (entity.Description != null)
                {
                    entityDescription = entity.Description;

                    Regex regex = new Regex(@"\</{0,}(\w+\:)\w+/{0,}\>");
                    entityDescription = regex.Replace(entityDescription, "");
                    entityDescription = Regex.Replace(entityDescription, @"\&nbsp\;", @"&#0160;");
                }
                node.InnerXml = entityDescription;

                rootElement.AppendChild(node);
            }
            document.Save(this.ReportFilePath);
        }

        #endregion

        #region Protected Methods

        protected override void ExecuteTask()
        {
            this.GenerateReport();
        }

        #endregion

    }
}
