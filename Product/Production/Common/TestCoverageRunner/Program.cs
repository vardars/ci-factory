using System;
using System.IO;
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using Params;

namespace TestCoverageRunner
{
    class Program
    {
        
#region Fields

        private string _TestAssembly;
        private string _ReportFile;
        private string _ReportFormat;
        private string _ProjectName;

#endregion

#region Properties

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

        public string TestAssembly
        {
            get
            {
                return _TestAssembly;
            }
            set
            {
                _TestAssembly = value;
            }
        }
        
        public string ReportFile
        {
            get
            {
                return _ReportFile;
            }
            set
            {
                _ReportFile = value;
            }
        }
        
        public string ReportFormat
        {
            get
            {
                return _ReportFormat;
            }
            set
            {
                _ReportFormat = value;
            }
        }

#endregion

        static void Main(string[] args)
        {
            Program Self = new Program();
            ParamCollection p = ParamCollection.ApplicationParameters;
            Self.TestAssembly = (string)p["TestAssembly"].Value;
            Self.ReportFile = (string)p["ReportFile"].Value;
            Self.ReportFormat = (string)p["ReportFormat"].Value;
            Self.ProjectName = (string)p["ProjectName"].Value;
            Self.ChangeDirFromObjToBin();
            Self.GenerateReport();
            Console.WriteLine();
            Console.WriteLine(string.Format(@"file://{0}", Self.ReportFile));
            Console.WriteLine();
        }

        private IReporting ReporterFactory(FileStream ReportStream)
        {
            IReporting Reporter = null;
            StringCollection TestAssemblies = new StringCollection();
            TestAssemblies.Add(this.TestAssembly);

            if (this.ReportFormat == "Html")
            {
                Reporter = new HtmlReporting(TestAssemblies, new StringCollection(), ReportStream, this.ProjectName);
            }
            else if (this.ReportFormat == "Xml")
            {
                Reporter = new XmlReporting(TestAssemblies, new StringCollection(), ReportStream, this.ProjectName);
            }
            return Reporter;
        }

        private void GenerateReport()
        {
            IReporting Reporter;
            FileStream ReportStream = new FileStream(this.ReportFile, FileMode.Create);
            Reporter = this.ReporterFactory(ReportStream);
            Reporter.GenerateReport();
            ReportStream.Close();
        }

        public void ChangeDirFromObjToBin()
        {
            DirectoryInfo Dir = new DirectoryInfo(Path.GetDirectoryName(this.TestAssembly));
            if (Dir.Parent.Name == "obj")
            {
                string SharpPath = Path.Combine(Dir.Parent.Parent.FullName, String.Format(@"bin\{0}", Dir.Name));
                if (Directory.Exists(SharpPath))
                {
                    this.TestAssembly = Path.Combine(SharpPath, Path.GetFileName(this.TestAssembly));
                }
                else
                {
                    this.TestAssembly = Path.Combine(String.Format(@"{0}\bin", Dir.Parent.Parent.FullName), Path.GetFileName(this.TestAssembly));
                }
            }
            else if (Dir.Parent.Parent.Name == "obj")
            {
                string SharpPath = Path.Combine(Dir.Parent.Parent.Parent.FullName, String.Format(@"bin\{0}", Dir.Name));
                if (Directory.Exists(SharpPath))
                {
                    this.TestAssembly = Path.Combine(SharpPath, Path.GetFileName(this.TestAssembly));
                }
                else
                {
                    this.TestAssembly = Path.Combine(String.Format(@"{0}\bin\{1}", Dir.Parent.Parent.Parent.FullName, Dir.Name), Path.GetFileName(this.TestAssembly));
                }
            }
        }
    }
}
