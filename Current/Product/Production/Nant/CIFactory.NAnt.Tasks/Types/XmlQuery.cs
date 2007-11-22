using System;
using System.Xml;
using System.Collections;
using NAnt.Core.Types;
using NAnt.Core.Attributes;

namespace CIFactory.NAnt.Types
{
    [ElementName("xmlquery")]
    public class XmlQuery : LoopItems
    {
        #region Fields

        private string _File;

        private XmlNamespaceCollection _Namespaces;

        private string _Query;

        #endregion

        #region Properties

        [TaskAttribute("file", Required = true)]
        public string File
        {
            get { return _File; }
            set { _File = value; }
        }

        [BuildElementCollection("namespaces", "namespace")]
        public XmlNamespaceCollection Namespaces
        {
            get { return this._Namespaces; }
            set { this._Namespaces = value; }
        }

        [TaskAttribute("query", Required = true)]
        public string Query
        {
            get { return _Query; }
            set { _Query = value; }
        }

        #endregion

        #region Protected Methods

        protected override System.Collections.IEnumerator GetStrings()
        {
            ArrayList Strings = new ArrayList();
            XmlDocument XmlDoc = new XmlDocument();
            XmlDoc.Load(this.File);
            XmlNamespaceManager Manager = new XmlNamespaceManager(XmlDoc.NameTable);

            foreach (XmlNamespace NameSpace in this.Namespaces)
            {
                if ((NameSpace.IfDefined && !NameSpace.UnlessDefined))
                {
                    Manager.AddNamespace(NameSpace.Prefix, NameSpace.Uri);
                }
            }
            foreach (XmlNode Node in XmlDoc.SelectNodes(this.Query, Manager))
            {
                if (Node.Value != null)
                    Strings.Add(Node.Value);
                else
                    Strings.Add(Node.InnerXml);
            }
            return ((string[])Strings.ToArray(typeof(string))).GetEnumerator();
        }

        #endregion

    }
}