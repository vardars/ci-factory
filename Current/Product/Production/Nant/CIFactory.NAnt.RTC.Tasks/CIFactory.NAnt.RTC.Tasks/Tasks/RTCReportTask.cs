using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using NAnt.Core;
using NAnt.Core.Tasks;
using NAnt.Core.Attributes;
using NAnt.Core.Util;
using CIFactory.NAnt.Types;
using CIFactory.RTC.NAnt.DataTypes;
using System.Xml;
using System.Text.RegularExpressions;

namespace CIFactory.RTC.NAnt.Tasks
{
    [TaskName("rtcreport")]
    class RTCReportTask : Task
    {
        #region Fields

        private StringList _BugIds;
        private StringList _StoryIds;
        private StringList _TaskIds;
        private string _Host;
        private string _Cookies;
        private string _CurlExe;
        private string _ReportFilePath;
                
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

        [TaskAttribute("host", Required = true)]
        public string Host
        {
            get
            {
                return this._Host;
            }
            set
            {
                this._Host = value;
            }
        }
                
        [TaskAttribute("cookies", Required = true)]
        public string Cookies
        {
            get { return this._Cookies; }
            set { this._Cookies = value; }
        }

        [TaskAttribute("curlexe", Required = true)]
        public string CurlExe
        {
            get { return this._CurlExe; }
            set { this._CurlExe = value; }
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

        #endregion

        #region Public Methods

        public void GenerateReport()
        {
            Process p = new Process();

            //Debugger.Break();

            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = _CurlExe;
            
            List<RTCItem> items = new List<RTCItem>();

            foreach (string id in StoryIds)
            {
                try
                {
                    p.StartInfo.Arguments = "-k  -b \"" + Cookies + "\" -H \"Accept:application/x-oslc-cm-change-request+xml\" " +
                                            Host + "/resource/itemName/com.ibm.team.workitem.WorkItem/" + id;
                    p.Start();
                    string curlStandardOutput = p.StandardOutput.ReadToEnd();
                    p.WaitForExit();

                    RTCItem StoryItem = new RTCItem() { EntityId = id };
                    StoryItem.HyperLink = Host + "/resource/itemName/com.ibm.team.workitem.WorkItem/" + id;
                    StoryItem.Cookies = Cookies;
                    StoryItem.CurlExe = CurlExe;
                    StoryItem.RTCQueryResult = curlStandardOutput;
                    if (StoryItem.EntityName != null)
                    {
                        items.Add(StoryItem);
                    }
                }
                catch (Exception ex)
                {
                    Log(Level.Error, ex.Message);
                }                
            }

            foreach (string id in TaskIds)
            {
                try
                {
                    p.StartInfo.Arguments = "-k  -b \"" + Cookies + "\" -H \"Accept:application/x-oslc-cm-change-request+xml\" " +
                                            Host + "/resource/itemName/com.ibm.team.workitem.WorkItem/" + id;
                    p.Start();
                    string curlStandardOutput = p.StandardOutput.ReadToEnd();
                    p.WaitForExit();

                    RTCItem TaskItem = new RTCItem() { EntityId = id };
                    TaskItem.HyperLink = Host + "/resource/itemName/com.ibm.team.workitem.WorkItem/" + id;
                    TaskItem.Cookies = Cookies;
                    TaskItem.CurlExe = CurlExe;
                    TaskItem.RTCQueryResult = curlStandardOutput;
                    if (TaskItem.EntityName != null)
                    {
                        items.Add(TaskItem);
                    }
                }
                catch (Exception ex)
                {
                    Log(Level.Error, ex.Message);
                }
            }

            foreach (string id in BugIds)
            {
                try
                {
                    p.StartInfo.Arguments = "-k  -b \"" + Cookies + "\" -H \"Accept:application/x-oslc-cm-change-request+xml\" " +
                                            Host + "/resource/itemName/com.ibm.team.workitem.WorkItem/" + id;
                    p.Start();
                    string curlStandardOutput = p.StandardOutput.ReadToEnd();
                    p.WaitForExit();

                    RTCItem BugItem = new RTCItem() { EntityId = id };
                    BugItem.HyperLink = Host + "/resource/itemName/com.ibm.team.workitem.WorkItem/" + id;
                    BugItem.Cookies = Cookies;
                    BugItem.CurlExe = CurlExe;
                    BugItem.RTCQueryResult = curlStandardOutput;
                    if (BugItem.EntityName != null)
                    {
                        items.Add(BugItem);
                    }
                }
                catch (Exception ex)
                {
                    Log(Level.Error, ex.Message);
                }
            }
                        
            XmlDocument document = new XmlDocument();
            document.LoadXml(document.CreateElement("RTC").OuterXml);
            //XmlDeclaration xmldec = document.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement rootElement = document.DocumentElement;
            //document.InsertBefore(xmldec, rootElement);

            foreach (RTCItem item in items)
            {
                XmlDocument reportPart = item.GenerateReport();
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
