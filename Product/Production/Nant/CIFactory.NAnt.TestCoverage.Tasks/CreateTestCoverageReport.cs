using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using NAnt.Core;
using NAnt.Core.Types;
using NAnt.Core.Attributes;
using System.IO;
using TestCoverageRunner;

namespace TestCoverage.Tasks
{
    [TaskName("createtestcoveragereport")]
    public class CreateTestCoverageReport :Task
    {
        
#region Enums

        public enum OutPutFormat
        {
            Xml,
            Html
        }

#endregion
        
#region Fields

        private OutPutFormat _Format;
        private FileInfo _File;
        private NAnt.Core.Types.FileSet _ProductionSet;
        private NAnt.Core.Types.FileSet _TestSet;
        private string _ProjectName;

#endregion
        
#region Properties

        [TaskAttribute("projectname", Required=true)]
        public string ProjectName
        {
            get
            {
                return _ProjectName;
            }
            set
            {
                _ProjectName = value;
            }
        }

        [TaskAttribute("format", Required = true)]
        public OutPutFormat Format
        {
            get
            {
                return _Format;
            }
            set
            {
                _Format = value;
            }
        }

        [TaskAttribute("reportfile", Required = true)]
        public FileInfo File
        {
            get
            {
                return _File;
            }
            set
            {
                _File = value;
            }
        }

        [BuildElement("productionassemblies")]
        public NAnt.Core.Types.FileSet ProductionSet
        {
            get
            {
                if (_ProductionSet == null)
                    _ProductionSet = new FileSet();
                return _ProductionSet;
            }
            set
            {
                _ProductionSet = value;
            }
        }

        [BuildElement("testasseblies", Required = true)]
        public NAnt.Core.Types.FileSet TestSet
        {
            get
            {
                return _TestSet;
            }
            set
            {
                _TestSet = value;
            }
        }

        #endregion

        private IReporting ReporterFactory(FileStream ReportStream)
        {
            IReporting Reporter = null;
            if (this.Format == OutPutFormat.Html)
            {
                Reporter = new HtmlReporting(this.TestSet.FileNames, this.ProductionSet.FileNames, ReportStream, this.ProjectName);
            }
            else if (this.Format == OutPutFormat.Xml)
            {
                Reporter = new XmlReporting(this.TestSet.FileNames, this.ProductionSet.FileNames, ReportStream, this.ProjectName);
            }
            return Reporter;
        }

        protected override void ExecuteTask()
        {
            IReporting Reporter;
            FileStream ReportStream = this.File.Create();
            Reporter = this.ReporterFactory(ReportStream);
            Reporter.GenerateReport();
            ReportStream.Close();
        }

    }
}
