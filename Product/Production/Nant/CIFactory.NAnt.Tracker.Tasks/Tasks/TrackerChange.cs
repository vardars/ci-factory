using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Xml;

using NAnt.Core;
using NAnt.Core.Attributes;

namespace Tracker.Tasks
{
     
    [TaskName("trackerchange")]
    public class TrackerChange : BaseTrackerTask
    {
        // Methods
        public TrackerChange()
        {
        }

        protected override void ExecuteTask()
        {
            this.Login();
            WriteField[] fieldArray1 = this.TrackerFields.Fields;
            for (int num1 = 0; num1 < fieldArray1.Length; num1++)
            {
                WriteField field1 = fieldArray1[num1];
                this.TrackerServer.SaveStringFieldValue(this.Id, field1.FieldName, field1.FieldValue);
            }
            this.Logout();
        }

        public void Test()
        {
            this.Id = 0x389;
            this.TrackerFields = new WriteFieldSet();
            WriteField[] fieldArray1 = new WriteField[] { new WriteField("Priority", "5") } ;
            this.TrackerFields.Fields = fieldArray1;
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
        }


        // Properties
        [TaskAttribute("scrid", Required=true)]
        public int Id
        {
            get
            {
                return this._Id;
            }
            set
            {
                this._Id = value;
            }
        }

        [BuildElement("trackerfields", Required=false)]
        public WriteFieldSet TrackerFields
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
        private int _Id;
        private WriteFieldSet _TrackerFields;
    }
}