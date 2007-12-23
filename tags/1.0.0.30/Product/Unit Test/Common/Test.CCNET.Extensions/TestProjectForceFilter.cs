using System;
using System.Collections;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using CCNET.Extensions.ForceFilters;
using MbUnit.Framework;
using MbUnit.Core.Framework;
using ThoughtWorks.CruiseControl.Remote.TestDoubles;
using TestDoubles;

namespace CCNET.Extensions.Test
{
    [TestFixture]
    public class TestProjectForceFilter
    {

        #region Serialization

        [Test]
        public void TestSerialization()
        {
            ProjectForceFilter TestSubject = new ProjectForceFilter();

            TestSubject.ProjectFilters = new ProjectFilterList();
            ((IList)TestSubject.ProjectFilters).Add(new ProjectFilter());
            TestSubject.ProjectFilters[0].ExclusionFilters.Conditions = new IntegrationStatus[1] { IntegrationStatus.Failure };
            TestSubject.ProjectFilters[0].ExclusionFilters.Activities = new ProjectActivity[2] { ProjectActivity.Building, ProjectActivity.CheckingModifications };

            TestSubject.ProjectFilters[0].Project = "experimental1";
            TestSubject.ProjectFilters[0].ServerUri = "tcp://localhost:21247/CruiseManager.rem";

            string Serialized = Zation.Serialize("projectForceFilter", TestSubject);

            Assert.IsNotNull(Serialized);

            ProjectForceFilter Clone = (ProjectForceFilter)Zation.Deserialize(Serialized);

            Assert.In(ProjectActivity.Building, Clone.ProjectFilters[0].ExclusionFilters.Activities);
            Assert.In(ProjectActivity.CheckingModifications, Clone.ProjectFilters[0].ExclusionFilters.Activities);
            Assert.In(IntegrationStatus.Failure, Clone.ProjectFilters[0].ExclusionFilters.Conditions);
            Assert.AreEqual("experimental1", Clone.ProjectFilters[0].Project);
            Assert.AreEqual("tcp://localhost:21247/CruiseManager.rem", Clone.ProjectFilters[0].ServerUri);
        }

        #endregion

        #region RequiresClientInformation

        [Test]
        public void TestRequiresClientInformation()
        {
            ProjectForceFilter TestSubject = new ProjectForceFilter();

            Assert.IsFalse(TestSubject.RequiresClientInformation);
        }

        #endregion


        #region ShouldRunIntegration

        #region AllowedByExclusionFilter

        [Test]
        public void TestShouldRunIntegrationAllowedByExclusionFilter()
        {
            ProjectForceFilter TestSubject = new ProjectForceFilter();

            TestSubject.ProjectFilters = new ProjectFilterList();

            ProjectFilter Project = new ProjectFilter();

            Project.Project = "TestProject";
            Project.ServerUri = "TestUri";

            Project.ExclusionFilters.Activities = new ProjectActivity[2] { ProjectActivity.Building, ProjectActivity.CheckingModifications };
            Project.ExclusionFilters.Conditions = new IntegrationStatus[1] { IntegrationStatus.Failure };

            RecorderIRemotingService RemotingRecorder = new RecorderIRemotingService();
            RecorderICruiseManager CruiseRecorder = new RecorderICruiseManager();
            ProjectStatus Stati = new ProjectStatus();

            Stati.Name = "TestProject";
            Stati.Activity = ProjectActivity.Sleeping;
            Stati.BuildStatus = IntegrationStatus.Success;

            CruiseRecorder.Recordings.GetProjectStatusRecording.ReturnValue = new ProjectStatus[1] { Stati };
            RemotingRecorder.Recordings.ConnectTypeStringRecording.ReturnValue = CruiseRecorder;
            Project.RemoteService = RemotingRecorder;

            TestSubject.ProjectFilters.Add(Project);

            bool Condition = TestSubject.ShouldRunIntegration(null, null);

            Assert.IsTrue(RemotingRecorder.Recordings.ConnectTypeStringRecording.Called);
            Assert.AreEqual("TestUri", RemotingRecorder.Recordings.ConnectTypeStringRecording.PassedStringuri);
            Assert.AreEqual(typeof(ICruiseManager), RemotingRecorder.Recordings.ConnectTypeStringRecording.PassedTypeproxyType);

            Assert.IsTrue(CruiseRecorder.Recordings.GetProjectStatusRecording.Called);

            Assert.IsTrue(Condition);
        }

        #endregion

        #region BlockedByExclusionFilterCauseBuilding

        [Test]
        public void TestShouldRunIntegrationBlockedByExclusionFilterCauseBuilding()
        {
            ProjectForceFilter TestSubject = new ProjectForceFilter();

            TestSubject.ProjectFilters = new ProjectFilterList();

            ProjectFilter Project = new ProjectFilter();

            Project.Project = "TestProject";
            Project.ServerUri = "TestUri";

            Project.ExclusionFilters.Activities = new ProjectActivity[2] { ProjectActivity.Building, ProjectActivity.CheckingModifications };
            Project.ExclusionFilters.Conditions = new IntegrationStatus[1] { IntegrationStatus.Failure };

            RecorderIRemotingService RemotingRecorder = new RecorderIRemotingService();
            RecorderICruiseManager CruiseRecorder = new RecorderICruiseManager();
            ProjectStatus Stati = new ProjectStatus();

            Stati.Name = "TestProject";
            Stati.Activity = ProjectActivity.Building;
            Stati.BuildStatus = IntegrationStatus.Success;

            CruiseRecorder.Recordings.GetProjectStatusRecording.ReturnValue = new ProjectStatus[1] { Stati };
            RemotingRecorder.Recordings.ConnectTypeStringRecording.ReturnValue = CruiseRecorder;
            Project.RemoteService = RemotingRecorder;

            TestSubject.ProjectFilters.Add(Project);

            bool Condition = TestSubject.ShouldRunIntegration(null, null);

            Assert.IsTrue(RemotingRecorder.Recordings.ConnectTypeStringRecording.Called);
            Assert.AreEqual("TestUri", RemotingRecorder.Recordings.ConnectTypeStringRecording.PassedStringuri);
            Assert.AreEqual(typeof(ICruiseManager), RemotingRecorder.Recordings.ConnectTypeStringRecording.PassedTypeproxyType);

            Assert.IsTrue(CruiseRecorder.Recordings.GetProjectStatusRecording.Called);

            Assert.IsFalse(Condition);
        }

        #endregion

        #region BlockedByExclusionFilterCauseFailed

        [Test]
        public void TestShouldRunIntegrationBlockedByExclusionFilterCauseFailed()
        {
            ProjectForceFilter TestSubject = new ProjectForceFilter();

            TestSubject.ProjectFilters = new ProjectFilterList();

            ProjectFilter Project = new ProjectFilter();

            Project.Project = "TestProject";
            Project.ServerUri = "TestUri";

            Project.ExclusionFilters.Activities = new ProjectActivity[2] { ProjectActivity.Building, ProjectActivity.CheckingModifications };
            Project.ExclusionFilters.Conditions = new IntegrationStatus[1] { IntegrationStatus.Failure };

            RecorderIRemotingService RemotingRecorder = new RecorderIRemotingService();
            RecorderICruiseManager CruiseRecorder = new RecorderICruiseManager();
            ProjectStatus Stati = new ProjectStatus();

            Stati.Name = "TestProject";
            Stati.Activity = ProjectActivity.Sleeping;
            Stati.BuildStatus = IntegrationStatus.Failure;

            CruiseRecorder.Recordings.GetProjectStatusRecording.ReturnValue = new ProjectStatus[1] { Stati };
            RemotingRecorder.Recordings.ConnectTypeStringRecording.ReturnValue = CruiseRecorder;
            Project.RemoteService = RemotingRecorder;

            TestSubject.ProjectFilters.Add(Project);

            bool Condition = TestSubject.ShouldRunIntegration(null, null);

            Assert.IsTrue(RemotingRecorder.Recordings.ConnectTypeStringRecording.Called);
            Assert.AreEqual("TestUri", RemotingRecorder.Recordings.ConnectTypeStringRecording.PassedStringuri);
            Assert.AreEqual(typeof(ICruiseManager), RemotingRecorder.Recordings.ConnectTypeStringRecording.PassedTypeproxyType);

            Assert.IsTrue(CruiseRecorder.Recordings.GetProjectStatusRecording.Called);

            Assert.IsFalse(Condition);
        }

        #endregion

        #region AllowedByInclusionFilter

        [Test]
        public void TestShouldRunIntegrationAllowedByInclusionFilter()
        {
            ProjectForceFilter TestSubject = new ProjectForceFilter();

            TestSubject.ProjectFilters = new ProjectFilterList();

            ProjectFilter Project = new ProjectFilter();

            Project.Project = "TestProject";
            Project.ServerUri = "TestUri";

            Project.InclusionFilters.Activities = new ProjectActivity[1] { ProjectActivity.Sleeping };
            Project.InclusionFilters.Conditions = new IntegrationStatus[1] { IntegrationStatus.Success };

            RecorderIRemotingService RemotingRecorder = new RecorderIRemotingService();
            RecorderICruiseManager CruiseRecorder = new RecorderICruiseManager();
            ProjectStatus Stati = new ProjectStatus();

            Stati.Name = "TestProject";
            Stati.Activity = ProjectActivity.Sleeping;
            Stati.BuildStatus = IntegrationStatus.Success;

            CruiseRecorder.Recordings.GetProjectStatusRecording.ReturnValue = new ProjectStatus[1] { Stati };
            RemotingRecorder.Recordings.ConnectTypeStringRecording.ReturnValue = CruiseRecorder;
            Project.RemoteService = RemotingRecorder;

            TestSubject.ProjectFilters.Add(Project);

            bool Condition = TestSubject.ShouldRunIntegration(null, null);

            Assert.IsTrue(RemotingRecorder.Recordings.ConnectTypeStringRecording.Called);
            Assert.AreEqual("TestUri", RemotingRecorder.Recordings.ConnectTypeStringRecording.PassedStringuri);
            Assert.AreEqual(typeof(ICruiseManager), RemotingRecorder.Recordings.ConnectTypeStringRecording.PassedTypeproxyType);

            Assert.IsTrue(CruiseRecorder.Recordings.GetProjectStatusRecording.Called);

            Assert.IsTrue(Condition);
        }

        #endregion

        #region BlockedByInclusionFilterCauseFailed

        [Test]
        public void TestShouldRunIntegrationBlockedByInclusionFilterCauseFailed()
        {
            ProjectForceFilter TestSubject = new ProjectForceFilter();

            TestSubject.ProjectFilters = new ProjectFilterList();

            ProjectFilter Project = new ProjectFilter();

            Project.Project = "TestProject";
            Project.ServerUri = "TestUri";

            Project.InclusionFilters.Activities = new ProjectActivity[1] { ProjectActivity.Sleeping };
            Project.InclusionFilters.Conditions = new IntegrationStatus[1] { IntegrationStatus.Success };

            RecorderIRemotingService RemotingRecorder = new RecorderIRemotingService();
            RecorderICruiseManager CruiseRecorder = new RecorderICruiseManager();
            ProjectStatus Stati = new ProjectStatus();

            Stati.Name = "TestProject";
            Stati.Activity = ProjectActivity.Sleeping;
            Stati.BuildStatus = IntegrationStatus.Failure;

            CruiseRecorder.Recordings.GetProjectStatusRecording.ReturnValue = new ProjectStatus[1] { Stati };
            RemotingRecorder.Recordings.ConnectTypeStringRecording.ReturnValue = CruiseRecorder;
            Project.RemoteService = RemotingRecorder;

            TestSubject.ProjectFilters.Add(Project);

            bool Condition = TestSubject.ShouldRunIntegration(null, null);

            Assert.IsTrue(RemotingRecorder.Recordings.ConnectTypeStringRecording.Called);
            Assert.AreEqual("TestUri", RemotingRecorder.Recordings.ConnectTypeStringRecording.PassedStringuri);
            Assert.AreEqual(typeof(ICruiseManager), RemotingRecorder.Recordings.ConnectTypeStringRecording.PassedTypeproxyType);

            Assert.IsTrue(CruiseRecorder.Recordings.GetProjectStatusRecording.Called);

            Assert.IsFalse(Condition);
        }

        #endregion

        #region BlockedByInclusionFilterCauseBuilding

        [Test]
        public void TestShouldRunIntegrationBlockedByInclusionFilterCauseBuilding()
        {
            ProjectForceFilter TestSubject = new ProjectForceFilter();

            TestSubject.ProjectFilters = new ProjectFilterList();

            ProjectFilter Project = new ProjectFilter();

            Project.Project = "TestProject";
            Project.ServerUri = "TestUri";

            Project.InclusionFilters.Activities = new ProjectActivity[1] { ProjectActivity.Sleeping };
            Project.InclusionFilters.Conditions = new IntegrationStatus[1] { IntegrationStatus.Success };

            RecorderIRemotingService RemotingRecorder = new RecorderIRemotingService();
            RecorderICruiseManager CruiseRecorder = new RecorderICruiseManager();
            ProjectStatus Stati = new ProjectStatus();

            Stati.Name = "TestProject";
            Stati.Activity = ProjectActivity.Building;
            Stati.BuildStatus = IntegrationStatus.Success;

            CruiseRecorder.Recordings.GetProjectStatusRecording.ReturnValue = new ProjectStatus[1] { Stati };
            RemotingRecorder.Recordings.ConnectTypeStringRecording.ReturnValue = CruiseRecorder;
            Project.RemoteService = RemotingRecorder;

            TestSubject.ProjectFilters.Add(Project);

            bool Condition = TestSubject.ShouldRunIntegration(null, null);

            Assert.IsTrue(RemotingRecorder.Recordings.ConnectTypeStringRecording.Called);
            Assert.AreEqual("TestUri", RemotingRecorder.Recordings.ConnectTypeStringRecording.PassedStringuri);
            Assert.AreEqual(typeof(ICruiseManager), RemotingRecorder.Recordings.ConnectTypeStringRecording.PassedTypeproxyType);

            Assert.IsTrue(CruiseRecorder.Recordings.GetProjectStatusRecording.Called);

            Assert.IsFalse(Condition);
        }

        #endregion

        #endregion

    }
}
