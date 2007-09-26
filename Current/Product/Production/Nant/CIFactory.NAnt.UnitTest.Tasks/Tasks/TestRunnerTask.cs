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

namespace CIFactory.NAnt.UnitTest.Tasks
{
    
    [TaskName("testrunner")]
    public class TestRunnerTask : Task
    {
        #region Constants

        private const string FixtureSetUp = "FixtureSetUp";

        private const string FixtureTearDown = "FixtureTearDown";

        private const string SetUpName = "SetUp";

        private const string TearDownName = "TearDown";

        #endregion

        #region Fields

        private StringList _Fixtures;

        #endregion

        #region Properties

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

            foreach (String Fixture in Fixtures)
            {
                SubProject FixtureProject = this.Project.SubProjects[Fixture];

                try
                {
                    this.TryRun(string.Format("{0}.{1}", FixtureProject.ProjectName, FixtureSetUp), FixtureProject);

                    foreach (Target FixtureTarget in FixtureProject.Targets)
                    {
                        if (FixtureTarget.Name.EndsWith("Test"))
                        {
                            try
                            {
                                TestCount++;
                                this.TryRun(string.Format("{0}.{1}", FixtureProject.ProjectName, SetUpName), FixtureProject);
                                FixtureTarget.Execute();
                                TestPassingCount++;
                            }
                            catch (BuildException bx)
                            {
                                string FailureInfo = string.Format("{0} at:{1}{2}", bx.Message, Environment.NewLine, bx.Location.ToString());
                                Log(Level.Error, FailureInfo);
                                FailingTestInfo.AppendLine(FailureInfo);
                            }
                            catch (Exception ex)
                            {
                                string FailureInfo = string.Format("{0} in {1}", ex.Message, FixtureTarget.Name);
                                Log(Level.Error, FailureInfo);
                                FailingTestInfo.AppendLine(FailureInfo);
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
            Log(Level.Error, FailingTestInfo.ToString());
            
            if (TestFailingCount > 0)
                throw new BuildException(string.Format("{0} tests failed." , TestFailingCount));
        }

        #endregion

    }
}
