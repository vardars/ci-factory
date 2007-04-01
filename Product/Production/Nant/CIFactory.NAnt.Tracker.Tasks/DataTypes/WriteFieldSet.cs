using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Xml;

using NAnt.Core;
using NAnt.Core.Attributes;

namespace Tracker.Tasks
{
     
    [Serializable, ElementName("writefieldset")]
    public class WriteFieldSet : DataTypeBase
    {
        // Methods
        public WriteFieldSet()
        {
        }


        // Properties
        [BuildElementArray("field")]
        public WriteField[] Fields
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


        // Fields
        private WriteField[] _Fields;
    }
}