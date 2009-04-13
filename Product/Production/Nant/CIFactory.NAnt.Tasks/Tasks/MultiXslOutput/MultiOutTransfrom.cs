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

  
namespace Common.Functions.MultiXslOutput
{
    [TaskName("xslmultiout")]
	public class MultiOutTransform : Task
    {
        private string _XmlInputFilePath;
        private string _XslTransformFilePath;
        private string _OutputFilePath;
        private bool _Append;
        private XsltParameterCollection _xsltParameters = new XsltParameterCollection();

        [TaskAttribute("xmlInputFilePath")]
        public string XmlInputFilePath
        {
            get
            {
                return _XmlInputFilePath;
            }
            set
            {
                if (_XmlInputFilePath == value)
                    return;
                _XmlInputFilePath = value;
            }
        }

        [TaskAttribute("xslTransformFilePath")]
        public string XslTransformFilePath
        {
            get
            {
                return _XslTransformFilePath;
            }
            set
            {
                if (_XslTransformFilePath == value)
                    return;
                _XslTransformFilePath = value;
            }
        }

        [TaskAttribute("outputFilePath")]
        public string OutputFilePath
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

        [TaskAttribute("append")]
        public bool Append
        {
            get
            {
                return _Append;
            }
            set
            {
                if (_Append == value)
                    return;
                _Append = value;
            }
        }

        [BuildElementCollection("parameters", "parameter")]
        public XsltParameterCollection Parameters
        {
            get { return _xsltParameters; }
        }
        
        public void Test()
        {
            this.XmlInputFilePath = @"C:\Temp\History.xml";
            this.XslTransformFilePath = @"C:\Projects\dod.ahlta\Current\Build\Packages\Analytics\SuccessProgress.xsl";
            this.OutputFilePath = @"C:\Temp\Main.xml";
            this.Append = true;

            this.ExecuteTask();
        }


        protected override void ExecuteTask()
        {
            using (TextReader InputStream = new StreamReader(this.XmlInputFilePath))
            {
                XPathDocument doc = new XPathDocument(InputStream);
                XslTransform xslt = new XslTransform();

                using (XmlReader TransformStream = new XmlTextReader(this.XslTransformFilePath))
                {
                    xslt.Load(TransformStream);
                    using (StreamWriter writer = new StreamWriter(this.OutputFilePath, this.Append, Encoding.UTF8))
                    {
                        XsltArgumentList xsltArgs = new XsltArgumentList();

                        // set the xslt parameters
                        foreach (XsltParameter parameter in Parameters)
                        {
                            if (IfDefined && !UnlessDefined)
                            {
                                Log(Level.Info, string.Format("Name {0}, NameSpaceUri {1}, Value {2}", parameter.ParameterName,
                                    parameter.NamespaceUri, parameter.Value));
                                xsltArgs.AddParam(parameter.ParameterName,
                                    parameter.NamespaceUri, parameter.Value);
                            }
                        }

                        MultiXmlTextWriter multiWriter = new MultiXmlTextWriter(writer);
                        multiWriter.Formatting = Formatting.Indented;
                        xslt.Transform(doc, xsltArgs, multiWriter);
                    }
                }
            }
	    }
	}
}
