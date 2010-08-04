namespace NCover.NAntTasks
{
    using NAnt.Core;
    using NAnt.Core.Attributes;
    using NAnt.Core.Tasks;
    using NAnt.Core.Types;
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Xml;

    [TaskName("ndoc2")]
    public class NDoc2Task : ExternalProgramBase
    {
        private FileSet _assemblies = new FileSet();
        private readonly string _configName = "NAnt.NDoc2.ndoc";
        private XmlNodeList _docNodes;
        private RawXml _documenters;
        private readonly StringBuilder _programArguments = new StringBuilder();
        private DirSet _referencePaths = new DirSet();
        private FileSet _summaries = new FileSet();
        private const string DefaultApplicationName = "NDocConsole.exe";
        private const string DefaultConfigName = "NAnt.NDoc2.ndoc";

        public NDoc2Task()
        {
            this.ExeName = "NDocConsole.exe";
        }

        private void _BuildArguments()
        {
            this._programArguments.AppendFormat(" -project=\"{0}\"", this._GetConfigFilename());
            if (this.Verbose)
            {
                this._programArguments.Append(" -verbose");
            }
        }

        private void _BuildTempConfigXmlFile()
        {
            string path = this._GetConfigFilename();
            this.Log(Level.Verbose, "Writing project settings to '{0}'.", new object[] { path });
            using (Stream stream = File.Create(path))
            {
                XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8) {
                    Indentation = 2,
                    Formatting = Formatting.Indented
                };
                writer.WriteStartDocument();
                writer.WriteStartElement("project");
                writer.WriteAttributeString("SchemaVersion", "2.0");
                writer.WriteStartElement("assemblies");
                foreach (string str2 in this.Assemblies.FileNames)
                {
                    string str3 = Path.ChangeExtension(str2, ".xml");
                    writer.WriteStartElement("assembly");
                    writer.WriteAttributeString("location", str2);
                    if (File.Exists(str3))
                    {
                        writer.WriteAttributeString("documentation", str3);
                    }
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                StringBuilder builder = new StringBuilder();
                foreach (string str4 in this.Summaries.FileNames)
                {
                    try
                    {
                        XmlTextReader reader = new XmlTextReader(str4);
                        reader.MoveToContent();
                        builder.Append(reader.ReadOuterXml());
                        reader.Close();
                        continue;
                    }
                    catch (IOException exception)
                    {
                        throw new BuildException(string.Format(CultureInfo.InvariantCulture, "Failed to read ndoc namespace summary file '{0}'.", new object[] { str4 }), this.Location, exception);
                    }
                }
                writer.WriteRaw(builder.ToString());
                writer.WriteStartElement("documenters");
                foreach (XmlNode node in this._docNodes)
                {
                    if ((node.NodeType == XmlNodeType.Element) && node.NamespaceURI.Equals(base.NamespaceManager.LookupNamespace("nant")))
                    {
                        writer.WriteRaw(node.OuterXml);
                    }
                }
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.Flush();
                stream.Close();
            }
            if (base.IsLogEnabledFor(Level.Verbose))
            {
                string message = File.ReadAllText(this._GetConfigFilename());
                this.Log(Level.Verbose, "Contents of NDoc settings file:");
                this.Log(Level.Verbose, message);
            }
        }

        private void _ExpandPropertiesInNodes(XmlNodeList nodes)
        {
            foreach (XmlNode node in nodes)
            {
                if (node.NodeType == XmlNodeType.Element)
                {
                    this._ExpandPropertiesInNodes(node.ChildNodes);
                    foreach (XmlAttribute attribute in node.Attributes)
                    {
                        attribute.Value = this.Project.ExpandProperties(attribute.Value, this.Location);
                    }
                    XmlNode node2 = node.SelectSingleNode("property[@name='OutputDirectory']");
                    if (node2 != null)
                    {
                        XmlAttribute namedItem = (XmlAttribute) node2.Attributes.GetNamedItem("value");
                        if (namedItem != null)
                        {
                            namedItem.Value = this.Project.GetFullPath(namedItem.Value);
                        }
                    }
                }
            }
        }

        private string _GetConfigFilename()
        {
            return Path.Combine(Path.GetTempPath(), this._configName);
        }

        private void _OnProcessExited(object sender, EventArgs e)
        {
            string path = this._GetConfigFilename();
            if (File.Exists(path))
            {
                this.Log(Level.Verbose, "Deleting config file: " + path);
                File.Delete(path);
            }
        }

        protected override void InitializeTask(XmlNode taskNode)
        {
            this._docNodes = this.Documenters.Xml.Clone().SelectNodes("nant:documenter", base.NamespaceManager);
            this._ExpandPropertiesInNodes(this._docNodes);
        }

        protected override void PrepareProcess(Process process)
        {
            if (this.Assemblies.BaseDirectory == null)
            {
                this.Assemblies.BaseDirectory = new DirectoryInfo(this.Project.BaseDirectory);
            }
            if (this.Summaries.BaseDirectory == null)
            {
                this.Summaries.BaseDirectory = new DirectoryInfo(this.Project.BaseDirectory);
            }
            if (this.ReferencePaths.BaseDirectory == null)
            {
                this.ReferencePaths.BaseDirectory = new DirectoryInfo(this.Project.BaseDirectory);
            }
            if (this.Assemblies.FileNames.Count == 0)
            {
                throw new BuildException("There must be at least one included assembly.", this.Location);
            }
            this._BuildTempConfigXmlFile();
            this._BuildArguments();
            this.Log(Level.Verbose, "Working directory: {0}", new object[] { this.BaseDirectory });
            this.Log(Level.Verbose, "Arguments: {0}", new object[] { this.ProgramArguments });
            base.PrepareProcess(process);
        }

        protected override Process StartProcess()
        {
            if (!Path.IsPathRooted(this.ExeName))
            {
                this.ExeName = this.Project.GetFullPath(this.ExeName);
            }
            Process process = base.StartProcess();
            process.Exited += new EventHandler(this._OnProcessExited);
            return process;
        }

        [BuildElement("assemblies", Required=true)]
        public FileSet Assemblies
        {
            get
            {
                return this._assemblies;
            }
            set
            {
                this._assemblies = value;
            }
        }

        [BuildElement("documenters", Required=true)]
        public RawXml Documenters
        {
            get
            {
                return this._documenters;
            }
            set
            {
                this._documenters = value;
            }
        }

        [TaskAttribute("program", Required=false), StringValidator(AllowEmpty=false)]
        public override string ExeName
        {
            get
            {
                return base.ExeName;
            }
            set
            {
                base.ExeName = value;
            }
        }

        public override string ProgramArguments
        {
            get
            {
                return this._programArguments.ToString();
            }
        }

        [BuildElement("referencepaths")]
        public DirSet ReferencePaths
        {
            get
            {
                return this._referencePaths;
            }
            set
            {
                this._referencePaths = value;
            }
        }

        [BuildElement("summaries")]
        public FileSet Summaries
        {
            get
            {
                return this._summaries;
            }
            set
            {
                this._summaries = value;
            }
        }
    }
}
