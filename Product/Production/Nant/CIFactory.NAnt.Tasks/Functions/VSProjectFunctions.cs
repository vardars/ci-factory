using System;
using System.IO;
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

        public VSProjectFunctions()
            : base(null, null)
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

        [Function("get-output-directory")]
        public string GetOutputDirectory(string projectFilePath, string config)
        {
            XmlDocument xd = new XmlDocument();
            xd.PreserveWhitespace = true;
            xd.Load(projectFilePath);

            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(xd.NameTable);
            namespaceManager.AddNamespace("b", @"http://schemas.microsoft.com/developer/msbuild/2003");

            XmlNode Node = null;
            Node = xd.SelectSingleNode(string.Format("//b:PropertyGroup[contains(@Condition, '{0}')]/b:OutputPath", config), namespaceManager);
            string OutputValue = Node.InnerText;
            OutputValue = OutputValue.TrimEnd(@"\".ToCharArray());
            
            if (Path.IsPathRooted(OutputValue))
                return Path.GetFullPath(OutputValue);

            return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(projectFilePath), OutputValue));
        }

        public void Adhoctest()
        {
            string Dir = this.GetOutputDirectory(@"c:\Projects\dasblogce\Current\Product\Production\Lesnikowski.Pawel.Mail.Pop3\Lesnikowski.Pawel.Mail.Pop3.csproj", "Release");
            System.Diagnostics.Debug.WriteLine(Dir);
        }

    }
}
