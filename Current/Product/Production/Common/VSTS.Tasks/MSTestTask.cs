using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Text;
using NAnt.Core;
using NAnt.Core.Types;
using NAnt.Core.Attributes;

namespace VSTS.Tasks
{
    [TaskName("mstest")]
    public class MSTestTask : Task
    {

        private FileSet _Assemblies;
        private string _RunConfig;
        private string _ResultsFile;
        private bool _FailOnTestFailure = true;
        private string _TestMetaData;
        private System.Collections.Specialized.StringCollection _AssembliesFileNames;
        
        
        [TaskAttribute("failontestfailure"), BooleanValidatorAttribute()]
        public bool FailOnTestFailure
        {
            get
            {
                return _FailOnTestFailure;
            }
            set
            {
                _FailOnTestFailure = value;
            }
        }

        [TaskAttribute("resultsfile", Required = true)]
        public string ResultsFile
        {
            get
            {
                return _ResultsFile;
            }
            set
            {
                _ResultsFile = value;
            }
        }

        [TaskAttribute("runconfig")]
        public string RunConfig
        {
            get
            {
                return _RunConfig;
            }
            set
            {
                _RunConfig = value;
            }
        }

        [BuildElement("testcontainers", Required = true)]
        public FileSet Assemblies
        {
            get
            {
                return _Assemblies;
            }
            set
            {
                _Assemblies = value;
            }
        }

        [TaskAttribute("testmetadata")]
        public string TestMetaData
        {
            get
            {
                return _TestMetaData;
            }
            set
            {
                _TestMetaData = value;
            }
        }

        private System.Collections.Specialized.StringCollection AssembliesFileNames
        {
            get
            {
                if (_AssembliesFileNames == null)
                    _AssembliesFileNames = this.Assemblies.FileNames;
                return _AssembliesFileNames;
            }
        }

        protected override void ExecuteTask()
        {
            Boolean Result = false;
            this.SubvertConsoleOutput();
            try
            {
                Executor TestExecutor = new Executor(this.Verbose);

                foreach (String File in AssembliesFileNames)
                {
                    TestExecutor.Add(new TestContainerCommand(File));
                }

                TestExecutor.Add(new ResultsOutputCommand(this.ResultsFile));

                if (!String.IsNullOrEmpty(this.RunConfig))
                    TestExecutor.Add(new RunConfigCommand(this.RunConfig));

                if (!String.IsNullOrEmpty(this.TestMetaData))
                    TestExecutor.Add(new TestMetaDataCommand(this.TestMetaData));

                TestExecutor.Add(new NoIsolationCommand());

                TestExecutor.ValidateCommands();

                Result = TestExecutor.Execute();
            }
            catch(System.Reflection.TargetInvocationException ex)
            {
                throw new BuildException(ex.InnerException.Message, ex.InnerException);
            }
            finally
            {
                this.RestorConsoleOutput();
                this.LogCapturedOutput();
            }
            if (!Result && this.FailOnTestFailure)
                throw new BuildException("At least one test failed!");
        }

        public void Test()
        {
            this.Verbose = true;

            this._AssembliesFileNames = new System.Collections.Specialized.StringCollection();
            this.AssembliesFileNames.Add(@"C:\Projects\dod.ahlta\Current\Product\Unit Test\BusinessLayer\BusinessEntities.Test\bin\Dod.CHCSII.BusinessLayer.BusinessEntities.Test.dll");

            this.ResultsFile = @"C:\Projects\dod.ahlta\Current\Product\report.xml";

            this.TestMetaData = @"C:\Projects\dod.ahlta\Current\Product\Dod.Ahlta1.vsmdi";

            this.ExecuteTask();
        }

        private void LogCapturedOutput()
        {
            using (StringReader Reader = new StringReader(this.Captured.ToString()))
            {
                String Line = Reader.ReadLine();
                while (Line != null)
                {
                    this.Log(Level.Info, Line);
                    Line = Reader.ReadLine();
                }
            }
            this.Captured.Close();
        }
        private TextWriter _StandardOut;
        private TextWriter StandardOut
        {
            get
            {
                return _StandardOut;
            }
            set
            {
                _StandardOut = value;
            }
        }

        private StringWriter _Captured;
        private StringWriter Captured
        {
            get
            {
                if (_Captured == null)
                    _Captured = new StringWriter();
                return _Captured;
            }
            set
            {
                _Captured = value;
            }
        }

        private void SubvertConsoleOutput()
        {
            this.StandardOut = Console.Out;
            FieldInfo OutFieldInfo = typeof(Console).GetField("_out", BindingFlags.Static | BindingFlags.NonPublic);
            OutFieldInfo.SetValue(null, this.Captured);
        }

        private void RestorConsoleOutput()
        {
            FieldInfo OutFieldInfo = typeof(Console).GetField("_out", BindingFlags.Static | BindingFlags.NonPublic);
            OutFieldInfo.SetValue(null, this.StandardOut);
        }
    }
}
