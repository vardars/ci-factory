using System;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System.Text;
using NAnt.Core;
using NAnt.Core.Attributes;
using NAnt.Core.Types;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.TeamFoundation.Client;
using TF.Tasks.SourceControl.Types;

namespace TF.Tasks.SourceControl.Helpers
{

    public class XmlWriterHelper
    {
        private XmlTextWriter _Writer;

        public XmlTextWriter Writer
        {
            get
            {
                return _Writer;
            }
            set
            {
                _Writer = value;
            }
        }

        public XmlWriterHelper(XmlTextWriter writer)
        {
            _Writer = writer;
        }

        public void Attribute(String name, String value)
        {
            this.Writer.WriteStartAttribute(name);
            this.String(value);
            this.Writer.WriteEndAttribute();
        }

        public void ElementBegining(String name)
        {
            this.Writer.WriteStartElement(name);
        }

        public void ElementEnd()
        {
            this.Writer.WriteEndElement();
        }

        public void String(String text)
        {
            this.Writer.WriteString(text);
        }
    }

}
