using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using NAnt.Core;
using NAnt.Core.Attributes;

namespace CIFactory.NAnt.Functions
{
    [FunctionSet("vsproject", "visualstudio")]
    public class VSProjectFunctions : FunctionSetBase
    {

        #region Constructors

        public VSProjectFunctions(Project project, PropertyDictionary properties)
            : base(project, properties)
        {
        }

        #endregion

        [Function("get-assemblyname")]
        public string GetSssemblyName(string projectFilePath)
        {
            XmlDocument xd = new XmlDocument();
            xd.PreserveWhitespace = true;
            xd.Load(projectFilePath);

            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(xd.NameTable);
            namespaceManager.AddNamespace("b", @"http://schemas.microsoft.com/developer/msbuild/2003");

            XmlNode Node = null;
            Node = xd.SelectSingleNode("//b:AssemblyName", namespaceManager);
            return Node.InnerText;
        }
    }
}
