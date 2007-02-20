using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Xml;

using NAnt.Core;
using NAnt.Core.Attributes;

namespace Tracker.Tasks
{
     
    [TaskName("trackergetfield")]
    public class TrackerGetField : BaseTrackerTask
    {
        // Methods
        public TrackerGetField()
        {
        }

        protected override void ExecuteTask()
        {
            this.Login();
            this.Properties[this.FieldValueProperty] = this.TrackerServer.GetFieldValueString(this.Id, this.FieldName);
            this.Logout();
        }


        // Properties
        [TaskAttribute("fieldname", Required=true)]
        public string FieldName
        {
            get
            {
                return this._FieldName;
            }
            set
            {
                this._FieldName = value;
            }
        }

        [TaskAttribute("fieldvalueproperty", Required=true)]
        public string FieldValueProperty
        {
            get
            {
                return this._FieldValueProperty;
            }
            set
            {
                this._FieldValueProperty = value;
            }
        }

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


        // Fields
        private string _FieldName;
        private string _FieldValueProperty;
        private int _Id;
    }
}