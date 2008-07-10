using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Xml;

using NAnt.Core;
using NAnt.Core.Attributes;

namespace Tracker.Tasks
{
     
    [Serializable, ElementName("readfieldset")]
    public class ReadFieldSet : DataTypeBase
    {
        // Methods
        public ReadFieldSet()
        {
            this._Description = true;
            this._LastNote = true;
        }


        // Properties
        [TaskAttribute("description", Required=false)]
        public bool Description
        {
            get
            {
                return this._Description;
            }
            set
            {
                this._Description = value;
            }
        }

        [BuildElementArray("field")]
        public ReadField[] Fields
        {
            get
            {
                return this._Fields;
            }
            set
            {
                this._Fields = value;
            }
        }

        [TaskAttribute("lastnote", Required=false)]
        public bool LastNote
        {
            get
            {
                return this._LastNote;
            }
            set
            {
                this._LastNote = value;
            }
        }


        // Fields
        private bool _Description;
        private ReadField[] _Fields;
        private bool _LastNote;
    }
}