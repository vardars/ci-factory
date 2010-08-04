namespace NCover.NAntTasks
{
    using NAnt.Core;
    using NAnt.Core.Attributes;
    using NAnt.Core.Types;
    using System;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Xml;

    [TaskName("nunitproject")]
    public class NUnitProjectTask : Task
    {
        private string _appBase = string.Empty;
        private string _appConfig = string.Empty;
        private FileSet _assemblyFiles = new FileSet();
        private FileInfo _project;

        private void _CreateNUnitProjectFile(string nunitProjectFileName, FileSet testFileSet, string appConfig, string appBase)
        {
            XmlTextWriter writer = null;
            try
            {
                if (File.Exists(nunitProjectFileName))
                {
                    File.Delete(nunitProjectFileName);
                }
                bool flag = (appBase != null) && (appBase.Length > 0);
                DirectoryInfo info = null;
                if (flag)
                {
                    info = new DirectoryInfo(appBase);
                }
                writer = new XmlTextWriter(nunitProjectFileName, Encoding.Default) {
                    Formatting = Formatting.Indented
                };
                writer.WriteStartElement("NUnitProject");
                writer.WriteStartElement("Settings");
                writer.WriteAttributeString("activeConfig", "Debug");
                if (flag)
                {
                    writer.WriteAttributeString("appbase", appBase);
                }
                writer.WriteEndElement();
                writer.WriteStartElement("Config");
                writer.WriteAttributeString("name", "Debug");
                if ((appConfig != null) && (appConfig.Length > 0))
                {
                    writer.WriteAttributeString("configfile", appConfig);
                }
                writer.WriteAttributeString("binpathtype", "Auto");
#if (UNDECOMPILABLE)
            	using (StringEnumerator enumerator = testFileSet.FileNames.GetEnumerator())
#else
				StringEnumerator enumerator = testFileSet.FileNames.GetEnumerator();
#endif
                {
                    while (enumerator.MoveNext())
                    {
                        FileInfo info2 = new FileInfo(enumerator.Current);
                        if (info2.Exists)
                        {
                            writer.WriteStartElement("assembly");
                            if (flag)
                            {
                                writer.WriteAttributeString("path", info2.FullName.Substring(info.FullName.Length + 1));
                            }
                            else
                            {
                                writer.WriteAttributeString("path", info2.Name);
                            }
                            writer.WriteEndElement();
                        }
                    }
                }
                writer.WriteEndElement();
                writer.WriteStartElement("Config");
                writer.WriteAttributeString("name", "Release");
                writer.WriteAttributeString("binpathtype", "Auto");
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
            catch (Exception exception)
            {
                this.Log(Level.Error, exception.Message);
                throw new BuildException(string.Format(CultureInfo.InvariantCulture, "An error occurred while creating file: {0}", new object[] { nunitProjectFileName }), this.Location, exception);
            }
            finally
            {
                if (writer != null)
                {
                    writer.Flush();
                    writer.Close();
                }
            }
        }

        protected override void ExecuteTask()
        {
            this.Log(Level.Verbose, "Creating {0} for test assemblies.", new object[] { this.ProjectFile.FullName });
            this._CreateNUnitProjectFile(this.ProjectFile.FullName, this.TestingFileSet, this.AppConfig, this.AppBase);
            this.Log(Level.Info, "{0} created.", new object[] { this.ProjectFile.FullName });
        }

        protected override void InitializeTask(XmlNode taskNode)
        {
            if (this.ProjectFile == null)
            {
                throw new BuildException(string.Format(CultureInfo.InvariantCulture, "The .nunit project file must be specified.", new object[0]), this.Location);
            }
            if ((this.TestingFileSet == null) || (this.TestingFileSet.Includes.Count == 0))
            {
                throw new BuildException(string.Format(CultureInfo.InvariantCulture, "The test assembly fileset must be specified.", new object[0]), this.Location);
            }
        }

        [TaskAttribute("appBase", Required=false)]
        public string AppBase
        {
            get
            {
                return this._appBase;
            }
            set
            {
                this._appBase = value;
            }
        }

        [TaskAttribute("appConfig")]
        public string AppConfig
        {
            get
            {
                return this._appConfig;
            }
            set
            {
                this._appConfig = value;
            }
        }

        [TaskAttribute("project")]
        public FileInfo ProjectFile
        {
            get
            {
                return this._project;
            }
            set
            {
                this._project = value;
            }
        }

        [BuildElement("fileset")]
        public FileSet TestingFileSet
        {
            get
            {
                return this._assemblyFiles;
            }
            set
            {
                this._assemblyFiles = value;
            }
        }
    }
}
