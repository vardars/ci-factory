using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using NAnt.Core;
using NAnt.Core.Attributes;
using CIFactory.NAnt.Types;

namespace CIFactory.NAnt.Functions
{
    [FunctionSet("vsproject", "visualstudio")]
    public class VSProjectFunctions : FunctionSetBase
    {

        #region Constructors

        public VSProjectFunctions(Project project, Location location, PropertyDictionary properties)
            : base(project, location, properties)
        {
        }

        public VSProjectFunctions()
            : base(null, null, null)
        {
        }

        #endregion

        [Function("get-assemblyname")]
        public string GetAssemblyName(string projectFilePath)
        {
            XmlDocument xd = new XmlDocument();
            xd.PreserveWhitespace = true;
            xd.Load(projectFilePath);

            string NameSpaceFor2005 = @"http://schemas.microsoft.com/developer/msbuild/2003";
            bool Is2005 = xd.DocumentElement.HasAttribute("xmlns");
            if (Is2005)
            {
                XmlNamespaceManager namespaceManager = new XmlNamespaceManager(xd.NameTable);
                namespaceManager.AddNamespace("b", NameSpaceFor2005);

                XmlNode Node = null;
                Node = xd.SelectSingleNode("//b:AssemblyName", namespaceManager);
                if (Node == null) return null;
                return Node.InnerText;
            }
            else
            {
                XmlNode Node = null;
                Node = xd.SelectSingleNode("//Settings/@AssemblyName");
                if (Node == null) return null;
                return Node.InnerText;
            }
        }

        [Function("get-output-directory")]
        public string GetOutputDirectory(string projectFilePath, string config)
        {
            string OutputValue;
            XmlDocument xd = new XmlDocument();
            xd.PreserveWhitespace = true;
            xd.Load(projectFilePath);

            string NameSpaceFor2005 = @"http://schemas.microsoft.com/developer/msbuild/2003";
            bool Is2005 = xd.DocumentElement.HasAttribute("xmlns");
            if (Is2005)
            {
                XmlNamespaceManager namespaceManager = new XmlNamespaceManager(xd.NameTable);
                namespaceManager.AddNamespace("b", NameSpaceFor2005);

                XmlNode Node = null;
                Node = xd.SelectSingleNode(string.Format("//b:PropertyGroup[contains(@Condition, '{0}')]/b:OutputPath", config), namespaceManager);
                if (Node == null) return null;
                OutputValue = Node.InnerText;
                OutputValue = OutputValue.TrimEnd(@"\".ToCharArray());
            }
            else
            {
                XmlNode Node = null;
                Node = xd.SelectSingleNode(string.Format("//Config[@Name = '{0}']/@OutputPath", config));
                if (Node == null) return null;
                OutputValue = Node.InnerText;
                OutputValue = OutputValue.TrimEnd(@"\".ToCharArray());
            }
            
            if (Path.IsPathRooted(OutputValue))
                return Path.GetFullPath(OutputValue);

            return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(projectFilePath), OutputValue));
        }

        [Function("get-hint-paths")]
        public void GetHintPaths(string stringListRef, string projectFilePath)
        {
            if (!this.Project.DataTypeReferences.Contains(stringListRef))
                throw new BuildException(String.Format("The refid {0} is not defined.", stringListRef));

            StringList RefStringList = (StringList)this.Project.DataTypeReferences[stringListRef];

            XmlDocument xd = new XmlDocument();
            xd.PreserveWhitespace = true;
            xd.Load(projectFilePath);

            string NameSpaceFor2005 = @"http://schemas.microsoft.com/developer/msbuild/2003";
            bool Is2005 = xd.DocumentElement.HasAttribute("xmlns");

            XmlNodeList Nodes = null;
            if (Is2005)
            {
                XmlNamespaceManager namespaceManager = new XmlNamespaceManager(xd.NameTable);
                namespaceManager.AddNamespace("b", NameSpaceFor2005);

                Nodes = xd.SelectNodes(@"b:Project/b:ItemGroup/b:Reference/b:HintPath", namespaceManager);
            }
            else
            {
                Nodes = xd.SelectNodes(@"/VisualStudioProject//Build/References/Reference/@HintPath");
            }
            foreach (XmlNode xmlNode in Nodes)
            {
                RefStringList.StringItems.Add(xmlNode.InnerText, new StringItem(xmlNode.InnerText));
            }
        }


        [Function("replace-hint-path")]
        public void ReplaceHintPath(string projectFilePath, string oldHintPath, string newHintPath)
        {
            XmlDocument xd = new XmlDocument();
            xd.PreserveWhitespace = true;
            xd.Load(projectFilePath);

            string NameSpaceFor2005 = @"http://schemas.microsoft.com/developer/msbuild/2003";
            bool Is2005 = xd.DocumentElement.HasAttribute("xmlns");

            XmlNode Node = null;
            if (Is2005)
            {
                XmlNamespaceManager namespaceManager = new XmlNamespaceManager(xd.NameTable);
                namespaceManager.AddNamespace("b", NameSpaceFor2005);

                Node = xd.SelectSingleNode(string.Format(@"b:Project/b:ItemGroup/b:Reference/b:HintPath[text() = '{0}']", oldHintPath), namespaceManager);
            }
            else
            {
                Node = xd.SelectSingleNode(string.Format(@"/VisualStudioProject//Build/References/Reference[@HintPath = '{0}']", oldHintPath));
            }

            Node.InnerText = newHintPath;
            xd.Save(projectFilePath);
        }

        public void Adhoctest()
        {
            string Dir = this.GetOutputDirectory(@"c:\Projects\dasblogce\Current\Product\Production\Lesnikowski.Pawel.Mail.Pop3\Lesnikowski.Pawel.Mail.Pop3.csproj", "Debug");
            System.Diagnostics.Debug.WriteLine(Dir);
            Dir = this.GetOutputDirectory(@"c:\Projects\dasblogce\Current\Product\Production\Lesnikowski.Pawel.Mail.Pop3\Lesnikowski.Pawel.Mail.Pop3.csproj", "Release");
            System.Diagnostics.Debug.WriteLine(Dir);
            Dir = this.GetOutputDirectory(@"C:\Projects\VS.NETTestProject\Current\Product\Production\Test Subject\Test Subject.csproj", "Debug");
            System.Diagnostics.Debug.WriteLine(Dir);
            Dir = this.GetOutputDirectory(@"C:\Projects\VS.NETTestProject\Current\Product\Production\Test Subject\Test Subject.csproj", "Release");
            System.Diagnostics.Debug.WriteLine(Dir);
        }

    }
}
