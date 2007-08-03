using System;
using System.Xml;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using NAnt.Core;
using NAnt.Core.Types;
using NAnt.Core.Attributes;
using NAnt.Core.Tasks;

namespace UpdateVersion.Tasks
{
    [TaskName("GenerateAssemblyInfoList")]
    public class GenerateAssemblyInfoList : Task
    {

        public const string VBASSEMBLYINFO = "AssemblyInfo.vb";
        public const string CSASSEMBLYINFO = "AssemblyInfo.cs";

        #region Fields

        private FileSet _AssemblyInfoFileSet;

        private FileSet _Changeset;

        private List<String> _ProcessedPaths;

        private string _RootDirectory;

        #endregion

        #region Properties

        [TaskAttribute("rootdirectory", Required = true)]
        public string RootDirectory
        {
        	get 
        	{
        		return _RootDirectory; 
        	}
        	set
        	{
        		_RootDirectory = value;
        	}
        }

        [BuildElement("assemblyinfoset", Required = true)]
        public FileSet AssemblyInfoFileSet
        {
            get
            {
                return _AssemblyInfoFileSet;
            }
            set
            {
                _AssemblyInfoFileSet = value;
            }
        }

        [BuildElement("changeset", Required = true)]
        public FileSet Changeset
        {
            set
            {
                this._Changeset = value;
            }
            get
            {
                return this._Changeset;
            }
        }

        public List<String> ProcessedPaths
        {
            get
            {
                if (_ProcessedPaths == null)
                    _ProcessedPaths = new List<string>();
                return _ProcessedPaths;
            }
            set
            {
                _ProcessedPaths = value;
            }
        }

        #endregion

        #region Public Methods

        public void ProcessChangeset()
        {
            foreach (string ChangedFilePath in this.Changeset.FileNames)
            {
                this.ProcessPath(Path.GetDirectoryName(ChangedFilePath));
            }
        }

        public void ProcessPath(string directoryPath)
        {
        	if (this.ProcessedPaths.Contains(directoryPath))
                return;

            this.ProcessedPaths.Add(directoryPath);

            if (File.Exists(Path.Combine(directoryPath, VBASSEMBLYINFO)))
            {
                this.AssemblyInfoFileSet.Includes.Add(Path.Combine(directoryPath, VBASSEMBLYINFO));
                return;
            }

            if (File.Exists(Path.Combine(directoryPath, CSASSEMBLYINFO)))
            {
                this.AssemblyInfoFileSet.Includes.Add(Path.Combine(directoryPath, CSASSEMBLYINFO));
                return;
            }

            string[] ProjectFiles = Directory.GetFiles(directoryPath, "*.*proj", SearchOption.TopDirectoryOnly);

            if (ProjectFiles.Length == 0)
            {
                if (this.RootDirectory.ToLower() == directoryPath.ToLower())
                    return;

            	this.ProcessPath(Directory.GetParent(directoryPath).FullName);
                return;
            }

            this.ProcessProjectFile(ProjectFiles[0]);
        }

        public void ProcessProjectFile(string projectFilePath)
        {
            Log(Level.Info, "Looking in project file for assemblyinfo location: {0}", projectFilePath);

            string FileExt = Path.GetExtension(projectFilePath);

            switch (FileExt.ToLower())
            {
                case ".csproj":
                    FileExt = ".cs";
                    break;
                case ".vbproj":
                    FileExt = ".vb";
                    break;
				default:
					return;
            }

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

                Node = xd.SelectSingleNode(string.Format("b:Project/b:ItemGroup/b:Compile[contains(@Include,'AssemblyInfo{0}')]/@Include", FileExt), namespaceManager);
            }
            else
            {
                Node = xd.SelectSingleNode(string.Format("//File[contains(@RelPath, 'AssemblyInfo{0}')]/@RelPath", FileExt));
            }

            if (Node == null)
            {
                Log(Level.Warning, "Could not find an AssemblyInfo file in the project file: {0}.", projectFilePath);
                return;
            }

            string AssemblyInfoPartialPath = Node.InnerText;
            string AssemblyInfoFullPath = Path.Combine(Path.GetDirectoryName(projectFilePath), AssemblyInfoPartialPath);
            this.AssemblyInfoFileSet.Includes.Add(AssemblyInfoFullPath);
        }

        #endregion

        public void AdHocTest()
        {
            string ProjectFilePath = @"c:\Projects\VersioningTestProject\Current\Product\Production\2003-abnormal\Test.csproj";
            this.ProcessProjectFile(ProjectFilePath);
            Console.WriteLine(this.AssemblyInfoFileSet.Includes[0]);
        }

        #region Protected Methods

        protected override void ExecuteTask()
        {
            this.ProcessChangeset();
        }

        #endregion

    }
}
