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
using CIFactory.TargetProcess.NAnt.Helpers;
using CIFactory.TargetProcess.NAnt.DataTypes;

namespace CIFactory.TargetProcess.NAnt.Tasks
{

    [TaskName("targetprocessreport")]
    public class TargetProcessReportTask : Task
    {
        #region Fields

        private StringList _BugIds;
        private StringList _StoryIds;
        private StringList _TaskIds;
        private string _ReportFilePath = String.Empty;
        private ConnectionInformation _ConnectionInformation;

        #endregion

        #region Properties

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
        #endregion

        #region Public Methods

        public void GenerateReport()
        {
            ServicesCF.ConnectionInformation = this.ConnectionInformation;

            List<TargetProcessEntity> entities = new List<TargetProcessEntity>();

            foreach (string id in this.TaskIds)
            {
                TargetProcessTask task = new TargetProcessTask() { TaskId = int.Parse(id) };
                entities.Add(task);
            }

            foreach (string id in this.StoryIds)
            {
                TargetProcessUserStory story = new TargetProcessUserStory() { UserStoryId = int.Parse(id) };
                entities.Add(story);
            }

            foreach (string id in this.BugIds)
            {
                TargetProcessBug bug = new TargetProcessBug() { BugId = int.Parse(id) };
                entities.Add(bug);
            }

            XmlDocument document = new XmlDocument();
            document.LoadXml(document.CreateElement("TargetProcess").OuterXml);
            XmlElement rootElement = document.DocumentElement;

            foreach (TargetProcessEntity entity in entities)
            {
                XmlDocument reportPart = entity.GenerateReport();
                XmlNode node = document.ImportNode(reportPart.DocumentElement, true);
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
