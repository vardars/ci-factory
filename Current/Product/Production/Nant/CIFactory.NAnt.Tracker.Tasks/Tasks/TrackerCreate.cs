using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Xml;

using NAnt.Core;
using NAnt.Core.Attributes;

namespace Tracker.Tasks
{

    [TaskName("trackercreate")]
    public class TrackerCreate : BaseTrackerTask
    {
        // Methods
        public TrackerCreate()
        {
        }

        protected override void ExecuteTask()
        {
            this.Login();
            int RecordHandle = 0;
            this.TrackerServer.NewRecordBegin(ref RecordHandle);

            if (0 == RecordHandle)
            {
                Console.WriteLine("RecordHandle is NULL");
                return;
            }

            WriteField[] fieldArray = this.TrackerFields.Fields;
            for (int i = 0; i < fieldArray.Length; ++i)
            {
                WriteField field = fieldArray[i];
                this.TrackerServer.SaveStringFieldValue(field.FieldName, field.FieldValue, RecordHandle);
            }
            int Id = this.TrackerServer.NewRecordCommit(RecordHandle);

            this.Properties[this.NewSCRId] = Id.ToString();
            this.Logout();
        }

        public void Test()
        {
            try
            {
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
                throw;
            }
        }

        [TaskAttribute("newscrid", Required = false)]
        public string NewSCRId
        {
            get
            {
                return this._NewSCRId;
            }
            set
            {
                this._NewSCRId = value;
            }
        }

        [BuildElement("trackerfields", Required = true)]
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
        private string _NewSCRId = "0";
        private WriteFieldSet _TrackerFields;
    }
}

