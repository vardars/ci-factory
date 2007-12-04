using System;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Text;
using System.Collections.Generic;
using NAnt;
using NAnt.Core;
using NAnt.Core.Util;
using NAnt.Core.Types;
using NAnt.Core.Functions;
using NAnt.Core.Attributes;

namespace CIFactory.NAnt.Functions
{
    [FunctionSet("xpath", "xml")]
    public class XPathFunctions : FunctionSetBase
    {
        #region Constructors

        public XPathFunctions()
            : base(null, null)
        {
        }

        public XPathFunctions(Project project, PropertyDictionary properties)
            : base(project, properties)
        {
        }

        #endregion

        #region Public Methods

        public bool AreBindingsCorrect(string projectFilePath)
        {
            XmlDocument xd = new XmlDocument();
            xd.PreserveWhitespace = true;
            xd.Load(projectFilePath);

            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(xd.NameTable);
            namespaceManager.AddNamespace("b", @"http://schemas.microsoft.com/developer/msbuild/2003");

            List<string> Queries = new List<string>();
            Queries.Add("//b:SccProjectName");
            Queries.Add("//b:SccLocalPath");
            Queries.Add("//b:SccAuxPath");
            Queries.Add("//b:SccProvider");

            foreach (string Query in Queries)
            {
                XmlNode Node = null;
                Node = xd.SelectSingleNode(Query, namespaceManager);
                if (Node == null || Node.InnerText != "SAK")
                    return false;
            }

            return true;
        }

        public void FixBindings(string projectFilePath)
        {
            XmlDocument xd = new XmlDocument();
            xd.PreserveWhitespace = true;
            xd.Load(projectFilePath);

            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(xd.NameTable);
            namespaceManager.AddNamespace("b", @"http://schemas.microsoft.com/developer/msbuild/2003");

            XmlNode ParentNode = xd.SelectSingleNode(@"/b:Project/b:PropertyGroup[boolean(@Condition) = false]", namespaceManager);

            Dictionary<string, string> Queries = new Dictionary<string, string>();
            Queries.Add("SccProjectName", "//b:SccProjectName");
            Queries.Add("SccLocalPath", "//b:SccLocalPath");
            Queries.Add("SccAuxPath", "//b:SccAuxPath");
            Queries.Add("SccProvider", "//b:SccProvider");

            foreach (KeyValuePair<string, string> Pair in Queries)
            {
                string Query = Pair.Value;
                XmlNode Node = null;
                Node = xd.SelectSingleNode(Query, namespaceManager);
                if (Node != null || Node.InnerText != "SAK")
                    Node.InnerText = "SAK";
                if (Node == null)
                {
                    XmlElement Element = xd.CreateElement(Pair.Key);
                    Element.InnerText = "SAK";
                    ParentNode.AppendChild(Element);
                }
            }

            xd.Save(projectFilePath);
        }

        public void Test()
        {
            this.FixBindings(@"C:\Projects\dod.ahlta\Current\Product\Production\Nex\ServerService\ServerService.vbproj");
        }

        #endregion

    }
}
