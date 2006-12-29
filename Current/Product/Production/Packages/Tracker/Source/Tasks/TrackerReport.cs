using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Xml;

using NAnt.Core;
using NAnt.Core.Attributes;

namespace Tracker.Tasks
{
     
    [TaskName("trackerreport")]
    public class TrackerReport : BaseTrackerTask
    {
        // Methods
        public TrackerReport()
        {
        }

        private void AddMetaDataToReport(XmlDocument document, XmlNode fieldsNode)
        {
            XmlNode node1 = document.CreateNode(XmlNodeType.Element, null, "Field", null);
            node1.InnerText = "ID";
            fieldsNode.AppendChild(node1);
            ReadField[] fieldArray1 = this.TrackerFields.Fields;
            for (int num1 = 0; num1 < fieldArray1.Length; num1++)
            {
                ReadField field1 = fieldArray1[num1];
                node1 = document.CreateNode(XmlNodeType.Element, null, "Field", null);
                node1.InnerText = field1.FieldName.Replace(" ", "");
                fieldsNode.AppendChild(node1);
            }
            if (this.TrackerFields.Description)
            {
                XmlNode node2 = document.CreateNode(XmlNodeType.Element, null, "Field", null);
                node2.InnerText = "Description";
                fieldsNode.AppendChild(node2);
            }
            if (this.TrackerFields.LastNote)
            {
                XmlNode node3 = document.CreateNode(XmlNodeType.Element, null, "Field", null);
                node3.InnerText = "LastNote";
                fieldsNode.AppendChild(node3);
            }
        }

        private void AddScrToReport(int id, XmlDocument document, XmlNode node)
        {
            XmlNode node1 = document.CreateNode(XmlNodeType.Element, null, "ID", null);
            node1.InnerText = id.ToString();
            node.AppendChild(node1);
            ReadField[] fieldArray1 = this.TrackerFields.Fields;
            for (int num1 = 0; num1 < fieldArray1.Length; num1++)
            {
                ReadField field1 = fieldArray1[num1];
                node1 = document.CreateNode(XmlNodeType.Element, null, field1.FieldName.Replace(" ", ""), null);
                node1.InnerText = this.TrackerServer.GetFieldValueString(id, field1.FieldName);
                node.AppendChild(node1);
            }
            if (this.TrackerFields.Description)
            {
                XmlNode node2 = document.CreateNode(XmlNodeType.Element, null, "Description", null);
                node2.InnerText = this.TrackerServer.GetDescription(id);
                node.AppendChild(node2);
            }
            if (this.TrackerFields.LastNote)
            {
                XmlNode node3 = document.CreateNode(XmlNodeType.Element, null, "LastNote", null);
                node3.InnerText = this.GetLatestNote(id);
                node.AppendChild(node3);
            }
        }

        private string GetLatestNote(int id)
        {
            string text2 = string.Empty;
            StringCollection collection1 = this.TrackerServer.GetNoteList(id);
            if (collection1.Count > 0)
            {
                text2 = collection1[collection1.Count - 1];
            }
            return text2;
        }

        protected override void ExecuteTask()
        {
            if (((this.TrackerFields == null) || (this.TrackerFields.Fields == null)) || (this.TrackerFields.Fields.Length == 0))
            {
                throw new BuildException("No fields were specified in a fieldset for the task trackerquery so no report can be generated.");
            }
            int[] numArray1 = this.Inflate(this.FlatScrIdList);
            this.Login();
            this.GenerateReportFile(numArray1);
            this.Logout();
        }

        private void GenerateReportFile(int[] ListOfSCRIDs)
        {
            XmlDocument document1 = new XmlDocument();
            document1.LoadXml(document1.CreateElement("TrackerQuery").OuterXml);
            XmlElement element1 = document1.DocumentElement;
            XmlNode node1 = document1.CreateNode(XmlNodeType.Element, null, "TrackerFields", null);
            this.AddMetaDataToReport(document1, node1);
            element1.AppendChild(node1);
            int[] numArray1 = ListOfSCRIDs;
            for (int num2 = 0; num2 < numArray1.Length; num2++)
            {
                int num1 = numArray1[num2];
                XmlNode node2 = document1.CreateNode(XmlNodeType.Element, null, "Tracker", null);
                this.AddScrToReport(num1, document1, node2);
                element1.AppendChild(node2);
            }
            document1.Save(this.ReportFile);
        }

        private int[] Inflate(string FlatList)
        {
            ArrayList list1 = new ArrayList();
            string[] textArray1 = FlatList.Split(",".ToCharArray());
            for (int num1 = 0; num1 < textArray1.Length; num1++)
            {
                string text1 = textArray1[num1];
                list1.Add(int.Parse(text1));
            }
            return (int[]) list1.ToArray(typeof(int));
        }

        public void Test()
        {
            this.FlatScrIdList = "1000,905";
            this.TrackerFields = new ReadFieldSet();
            ReadField[] fieldArray1 = new ReadField[] { new ReadField("Submit Type"), new ReadField("Title") } ;
            this.TrackerFields.Fields = fieldArray1;
            this.ReportFile = @"C:\temp\trackers.xml";
            this.ConnectionInformation = new ConnectionInformation();
            ConnectionInformation information1 = this.ConnectionInformation;
            information1.DBMSLoginMode = 2;
            information1.DBMSServer = "Jupiter";
            information1.DBMSType = "Tracker SQL Server Sys";
            information1.DBMSPassword = "tracker12";
            information1.DBMSUserName = "tracker";
            information1.ProjectName = "EF";
            information1.UserName = "jflowers";
            information1.UserPWD = "password";
            information1 = null;
            try
            {
                this.ExecuteTask();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
                throw;
            }
            try
            {
                XmlDocument document1 = new XmlDocument();
                document1.Load(this.ReportFile);
            }
            catch (Exception exception)
            {
               Console.WriteLine(exception.ToString());
               throw;
            }
        }


        // Properties
        [TaskAttribute("ScrIdList", Required=true)]
        public string FlatScrIdList
        {
            get
            {
                return this._FlatScrIdList;
            }
            set
            {
                this._FlatScrIdList = value;
            }
        }

        [TaskAttribute("reportfile", Required=true)]
        public string ReportFile
        {
            get
            {
                return this._ReportFile;
            }
            set
            {
                this._ReportFile = value;
            }
        }

        [BuildElement("trackerfields", Required=false)]
        public ReadFieldSet TrackerFields
        {
            get
            {
                return this._TrackerFields;
            }
            set
            {
                this._TrackerFields = value;
            }
        }


        // Fields
        private string _FlatScrIdList;
        private string _ReportFile;
        private ReadFieldSet _TrackerFields;
    }
}