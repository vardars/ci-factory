using System;
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
                        this._Value = this.XmlNode.InnerXml;
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

    }

}