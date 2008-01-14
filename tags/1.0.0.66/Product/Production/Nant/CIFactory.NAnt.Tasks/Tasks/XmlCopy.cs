using System;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Xml;
using System.Text;
using System.IO;

using NAnt.Core;
using NAnt.Core.Types;
using NAnt.Core.Attributes;
using NAnt.Core.Tasks;

namespace Xml.Tasks
{
    [TaskName("xmlcopy")]
    public class XmlCopy : Task
    {
        private String _InputFilePath;
        private String _OutputFilePath;
        private FileSet _InputFiles;
        private string _OutputDirectoryPath;

        [TaskAttribute("outputDirectoryPath")]
        public string OutputDirectoryPath
        {
            get
            {
                return _OutputDirectoryPath;
            }
            set
            {
                _OutputDirectoryPath = value;
            }
        }

        [BuildElement("inputFiles")]
        public FileSet InputFiles
        {
            get
            {
                return _InputFiles;
            }
            set
            {
                _InputFiles = value;
            }
        }

        [TaskAttribute("inputFilePath")]
        public String InputFilePath
        {
            get
            {
                return _InputFilePath;
            }
            set
            {
                if (_InputFilePath == value)
                    return;
                _InputFilePath = value;
            }
        }

        [TaskAttribute("outputFilePath")]
        public String OutputFilePath
        {
            get
            {
                return _OutputFilePath;
            }
            set
            {
                if (_OutputFilePath == value)
                    return;
                _OutputFilePath = value;
            }
        }

        protected override void ExecuteTask()
        {
            if (this.InputFilePath != null && this.OutputDirectoryPath != null
                    && this.InputFilePath != string.Empty && this.OutputDirectoryPath != string.Empty)
            {
                this.OutputFilePath = Path.Combine(this.OutputDirectoryPath, Path.GetFileName(this.InputFilePath));
            }

            if (this.InputFilePath != null && this.OutputFilePath != null 
                    && this.InputFilePath != string.Empty && this.OutputFilePath != string.Empty)
            {
                this.CopyXmlFile(this.InputFilePath, this.OutputFilePath);
                return;
            }

            if (this.OutputDirectoryPath != null && this.InputFiles != null && this.OutputDirectoryPath != String.Empty && this.InputFiles.FileNames.Count > 0)
            {
                foreach (string InputFilePath in this.InputFiles.FileNames)
                {
                    string FileName = Path.GetFileName(InputFilePath);
                    string OutputFilePath = Path.Combine(this.OutputDirectoryPath, FileName);
                    this.CopyXmlFile(InputFilePath, OutputFilePath);
                }
                return;
            }
        }

        public void CopyXmlFile(String inputFilePath, String outputFilePath)
        {
            if (this.Verbose)
                Log(Level.Info, string.Format("Copying {0} to {1}.", inputFilePath, outputFilePath));

            XmlValidatingReader reader = new XmlValidatingReader(new XmlTextReader(inputFilePath));
            XmlTextWriter writer = new XmlTextWriter(outputFilePath, Encoding.UTF8);
            reader.ValidationType = ValidationType.None;
            reader.EntityHandling = EntityHandling.ExpandEntities;
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.DocumentType:
                        break;
                    case XmlNodeType.Whitespace:
                        break;
                    default:
                        writer.WriteNode(reader, true);
                        break;
                }
            }
            writer.Close();
            reader.Close();
        }
    }
}

public class MyClass : TextWriter
{
    public override void Write(string value)
    {
        
    }

    public override Encoding Encoding
    {
        get { throw new Exception("The method or operation is not implemented."); }
    }
}