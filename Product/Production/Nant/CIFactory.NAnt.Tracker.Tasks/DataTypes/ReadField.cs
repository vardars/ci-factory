using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Xml;

using NAnt.Core;
using NAnt.Core.Attributes;
namespace Tracker.Tasks
{
     
    public class ReadField : Element
    {
        // Methods
        public ReadField()
        {
        }

        public ReadField(string fieldName)
        {
            this.FieldName = fieldName;
        }

        public bool Equals(ReadField field)
        {
            return this.FieldName.Equals(field.FieldName);
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


        // Fields
        private string _FieldName;
    }
}