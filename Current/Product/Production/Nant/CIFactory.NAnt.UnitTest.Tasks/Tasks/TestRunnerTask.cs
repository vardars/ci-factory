using System;
using System.Collections.Generic;
using System.Text;
using CIFactory.NAnt.Types;
using NAnt.Core;
using NAnt.Core.Tasks;
using NAnt.Core.Attributes;
using NAnt.Core.Types;

using MbUnit;
using MbUnit.Framework;
using CIFactory.NAnt.UnitTest.Report;
using System.Xml.Serialization;
using System.IO;

namespace CIFactory.NAnt.UnitTest.Tasks
{
    
    [TaskName("testrunner")]
    public class TestRunnerTask : Task
    {
        #region Constants

        private string _FilePath;

        private const string FixtureSetUp = "FixtureSetUp";

        private const string FixtureTearDown = "FixtureTearDown";

        private const string SetUpName = "SetUp";

        private const string TearDownName = "TearDown";

        #endregion

        #region Fields

        private StringList _Fixtures;

        #endregion

        #region Properties

        [TaskAttribute("reportfilepath", Required = true)]
        public string FilePath
        {
            get
            {
                return _FilePath;
            }
            set
            {
                _FilePath = value;
            }
        }

        [BuildElement("fixtures", Required = true)]
        public StringList Fixtures
        {
            get
            {
                return _Fixtures;
            }
            set
            {
                _Fixtures = value;
            }
        }

        #endregion

        #region Public Methods

        public void TryRun(string targetName, SubProject fixtureProject)
        {
            if (fixtureProject.Targets.Contains(targetName))
            {
                fixtureProject.Targets[targetName].Execute();
            }
        }

        #endregion

        #region Protected Methods

        protected override void ExecuteTask()
        {
            int TestCount = 0;
            int TestPassingCount = 0;
            StringBuilder FailingTestInfo = new StringBuilder();

            ReportContainer Report = new ReportContainer();


            foreach (String Fixture in Fixtures)
            {
                if (!this.Project.SubProjects.Contains(Fixture))
                    throw new BuildException(string.Format("There is no fixture loaded for {0}.", Fixture), this.Location);
                SubProject FixtureProject = this.Project.SubProjects[Fixture];

                FixtureReport fixtureReport = new FixtureReport(Fixture);
                Report.FixtureReports.Add(fixtureReport);

                try
                {
                    this.TryRun(string.Format("{0}.{1}", FixtureProject.ProjectName, FixtureSetUp), FixtureProject);

                    foreach (Target FixtureTarget in FixtureProject.Targets)
                    {
                        if (FixtureTarget.Name.EndsWith("Test"))
                        {
                            TestReport testReport = new TestReport(FixtureTarget.Name);
                            fixtureReport.TestReports.Add(testReport);
                            try
                            {
                                TestCount++;
                                this.TryRun(string.Format("{0}.{1}", FixtureProject.ProjectName, SetUpName), FixtureProject);
                                FixtureTarget.Execute();
                                TestPassingCount++;
                                testReport.Passed = true;
                            }
                            catch (BuildException bx)
                            {
                                string FailureInfo = string.Format("{0} at:{1}{2}", bx.Message, Environment.NewLine, bx.Location.ToString());
                                Log(Level.Error, FailureInfo);
                                FailingTestInfo.AppendLine(FailureInfo);

                                testReport.Passed = false;
                                testReport.FailureException = bx.ToString();
                            }
                            catch (Exception ex)
                            {
                                string FailureInfo = string.Format("{0} in {1}", ex.Message, FixtureTarget.Name);
                                Log(Level.Error, FailureInfo);
                                FailingTestInfo.AppendLine(FailureInfo);

                                testReport.Passed = false;
                                testReport.FailureException = ex.ToString();
                            }
                            finally
                            {
                                this.TryRun(string.Format("{0}.{1}", FixtureProject.ProjectName, TearDownName), FixtureProject);
                            }
                        }
                    }
                }
                finally
                {
                    this.TryRun(string.Format("{0}.{1}", FixtureProject.ProjectName, FixtureTearDown), FixtureProject);
                }
            }

            int TestFailingCount = TestCount - TestPassingCount;
            Log(Level.Info, "Tests Run: {0}", TestCount);
            Log(Level.Info, "Tests Passing: {0}", TestPassingCount);
            Log(Level.Info, "Tests Failing: {0}", TestFailingCount);
            Log(Level.Info, "Test Assertions Executed: {0}", MbUnit.Framework.Assert.AssertCount);
            Console.WriteLine(FailingTestInfo.ToString());

            if (File.Exists(FilePath))
                File.Delete(FilePath);

            XmlSerializer Serializer = new XmlSerializer(typeof(ReportContainer));
            using (FileStream ReportStream = File.OpenWrite(FilePath))
            {
                Serializer.Serialize(ReportStream, Report);
            }

            if (TestFailingCount > 0)
                throw new BuildException(string.Format("{0} tests failed.", TestFailingCount));
        }

        #endregion

    }
}
