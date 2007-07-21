using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using NAnt.Core;
using NAnt.Core.Attributes;

namespace CIFactory.NAnt.Types
{

    [ElementName("text")]
    public class TextElement : Element
    {
        #region Fields

        private bool _Expand = false;

        private string _Value;

        private bool _Xml = false;

        #endregion

        #region Properties

        protected override bool CustomXmlProcessing
        {
            get { return true; }
        }

        [TaskAttribute("expand", Required = false), BooleanValidator()]
        public bool Expand
        {
            get { return _Expand; }
            set { _Expand = value; }
        }

        public string Value
        {
            get
            {
                if (this._Value == null)
                {
                    if (this.Xml)
                    {
						StringBuilder Builder = new StringBuilder();
						XmlNoNamespaceWriter Writer = new XmlNoNamespaceWriter(new StringWriter(Builder));
						
						Writer.WriteNode(
							new XmlTextReader(new StringReader(this.XmlNode.InnerXml)), false);

						Writer.Flush();
						Writer.Close();

                        this._Value = Builder.ToString();
                    }
                    else
                    {
                        this._Value = this.XmlNode.InnerText;
                    }
                    if (this.Expand)
                    {
                        this._Value = this.Project.ExpandProperties(this._Value, this.Location);
                    }
                }
                return this._Value;
            }
        }

        [TaskAttribute("xml", Required = false), BooleanValidator()]
        public bool Xml
        {
            get { return _Xml; }
            set { _Xml = value; }
        }

        #endregion

        #region Protected Methods

        protected override void InitializeXml(System.Xml.XmlNode elementNode, PropertyDictionary properties, FrameworkInfo framework)
        {
            base.InitializeXml(elementNode, properties, framework);
        }

        #endregion


		private class XmlNoNamespaceWriter : System.Xml.XmlTextWriter
		{
			bool skipAttribute = false;

			public XmlNoNamespaceWriter(System.IO.TextWriter writer)
				: base(writer)
			{
			}

			public override void WriteStartElement(string prefix, string localName, string ns)
			{
				base.WriteStartElement(null, localName, null);
			}


			public override void WriteStartAttribute(string prefix, string localName, string ns)
			{
				//If the prefix or localname are "xmlns", don't write it.
				if (prefix.CompareTo("xmlns") == 0 || localName.CompareTo("xmlns") == 0)
				{
					skipAttribute = true;
				}
				else
				{
					base.WriteStartAttribute(null, localName, null);
				}
			}

			public override void WriteString(string text)
			{
				//If we are writing an attribute, the text for the xmlns
				//or xmlns:prefix declaration would occur here.  Skip
				//it if this is the case.
				if (!skipAttribute)
				{
					base.WriteString(text);
				}
			}

			public override void WriteEndAttribute()
			{
				//If we skipped the WriteStartAttribute call, we have to
				//skip the WriteEndAttribute call as well or else the XmlWriter
				//will have an invalid state.
				if (!skipAttribute)
				{
					base.WriteEndAttribute();
				}
				//reset the boolean for the next attribute.
				skipAttribute = false;
			}


			public override void WriteQualifiedName(string localName, string ns)
			{
				//Always write the qualified name using only the
				//localname.
				base.WriteQualifiedName(localName, null);
			}
		}

    }

}