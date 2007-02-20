using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Xml;

using NAnt.Core;
using NAnt.Core.Attributes;

namespace Tracker.Tasks
{
     
    public class WriteField : Element
    {
        // Methods
        public WriteField()
        {
        }

        public WriteField(string fieldName, string fieldValue)
        {
            this.FieldName = fieldName;
            this.FieldValue = fieldValue;
        }


        // Properties
        [TaskAttribute("name", Required=true)]
        public string FieldName
        {
            get
            {
                return this._FieldName;
            }
            set
            {
                if (value.ToLower() == "id")
                {
                    throw new BuildException("Id is not allowed as a field name.");
                }
                this._FieldName = value;
            }
        }

        [TaskAttribute("value", Required=true)]
        public string FieldValue
        {
            get
            {
                return this._FieldValue;
            }
            set
            {
                this._FieldValue = value;
            }
        }


        // Fields
        private string _FieldName;
        private string _FieldValue;
    }
}