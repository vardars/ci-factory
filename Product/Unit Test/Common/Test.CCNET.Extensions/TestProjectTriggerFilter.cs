using System;
using System.Collections;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using CCNET.Extensions.Triggers;
using MbUnit.Framework;
using MbUnit.Core.Framework;
using ThoughtWorks.CruiseControl.Remote.TestDoubles;
using TestDoubles;

namespace CCNET.Extensions.Test
{
    [TestFixture]
    public class TestProjectTriggerFilter
    {
        
#region Serialization

        [Test]
        public void TestSerialization()
        {
            ProjectTriggerFilter TestSubject = new ProjectTriggerFilter();

            TestSubject.ProjectFilters = new ProjectFilterList();
            ((IList)TestSubject.ProjectFilters).Add(new ProjectFilter());
            TestSubject.ProjectFilters[0].ExclusionFilters.Conditions = new IntegrationStatus[1] { IntegrationStatus.Failure };
            TestSubject.ProjectFilters[0].ExclusionFilters.Activities = new ProjectActivity[2] { ProjectActivity.Building, ProjectActivity.CheckingModifications };

            TestSubject.ProjectFilters[0].Project = "experimental1";
            TestSubject.ProjectFilters[0].ServerUri = "tcp://localhost:21247/CruiseManager.rem";

            TestSubject.InnerTrigger = new ThoughtWorks.CruiseControl.Core.Triggers.IntervalTrigger();
            ((ThoughtWorks.CruiseControl.Core.Triggers.IntervalTrigger)TestSubject.InnerTrigger).IntervalSeconds = 60;

            string Serialized = Zation.Serialize("projectTriggerFilter", TestSubject);

            Assert.IsNotNull(Serialized);

            ProjectTriggerFilter Clone = (ProjectTriggerFilter)Zation.Deserialize(Serialized);

            Assert.In(ProjectActivity.Building, Clone.ProjectFilters[0].ExclusionFilters.Activities);
            Assert.In(ProjectActivity.CheckingModifications, Clone.ProjectFilters[0].ExclusionFilters.Activities);
            Assert.In(IntegrationStatus.Failure, Clone.ProjectFilters[0].ExclusionFilters.Conditions);
            Assert.AreEqual("experimental1", Clone.ProjectFilters[0].Project);
            Assert.AreEqual("tcp://localhost:21247/CruiseManager.rem", Clone.ProjectFilters[0].ServerUri);
        }

#endregion
        
#region IntegrationCompleted

        [Test]
        public void TestIntegrationCompleted()
        {
            ProjectTriggerFilter TestSubject = new ProjectTriggerFilter();

            RecorderITrigger Recorder = new RecorderITrigger();

            TestSubject.InnerTrigger = Recorder;

            TestSubject.IntegrationCompleted();

            Assert.IsTrue(Recorder.Recordings.IntegrationCompletedRecording.Called);
        }

#endregion
        
#region NextBuild

        [Test]
        public void TestNextBuild()
        {
            ProjectTriggerFilter TestSubject = new ProjectTriggerFilter();

            RecorderITrigger Recorder = new RecorderITrigger();
            DateTime TimeForNextBuild = DateTime.Now;
            Recorder.SetNextBuild = TimeForNextBuild;

            TestSubject.InnerTrigger = Recorder;

            DateTime NextBuild;

            NextBuild = TestSubject.NextBuild;

            Assert.AreEqual(TimeForNextBuild, NextBuild);
        }

#endregion
        
#region ShouldRunIntegration
        
#region InnerNoBuild

        [Test]
        public void TestShouldRunIntegrationInnerNoBuild()
        {
            ProjectTriggerFilter TestSubject = new ProjectTriggerFilter();

            RecorderITrigger TriggerRecorder = new RecorderITrigger();
            TriggerRecorder.Recordings.ShouldRunIntegrationRecording.ReturnValue = BuildCondition.NoBuild;

            TestSubject.InnerTrigger = TriggerRecorder;

            BuildCondition Condition = TestSubject.ShouldRunIntegration();

            Assert.AreEqual(BuildCondition.NoBuild, Condition);
        }

#endregion
        
#region AllowedByExclusionFilter

        [Test]
        public void TestShouldRunIntegrationAllowedByExclusionFilter()
        {
            ProjectTriggerFilter TestSubject = new ProjectTriggerFilter();

            RecorderITrigger TriggerRecorder = new RecorderITrigger();
            TriggerRecorder.Recordings.ShouldRunIntegrationRecording.ReturnValue = BuildCondition.IfModificationExists;

            TestSubject.InnerTrigger = TriggerRecorder;

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

            BuildCondition Condition = TestSubject.ShouldRunIntegration();

            Assert.IsTrue(RemotingRecorder.Recordings.ConnectTypeStringRecording.Called);
            Assert.AreEqual("TestUri", RemotingRecorder.Recordings.ConnectTypeStringRecording.PassedStringuri);
            Assert.AreEqual(typeof(ICruiseManager), RemotingRecorder.Recordings.ConnectTypeStringRecording.PassedTypeproxyType);

            Assert.IsTrue(CruiseRecorder.Recordings.GetProjectStatusRecording.Called);

            Assert.AreEqual(BuildCondition.IfModificationExists, Condition);
        }

#endregion
        
#region BlockedByExclusionFilterCauseBuilding

        [Test]
        public void TestShouldRunIntegrationBlockedByExclusionFilterCauseBuilding()
        {
            ProjectTriggerFilter TestSubject = new ProjectTriggerFilter();

            RecorderITrigger TriggerRecorder = new RecorderITrigger();
            TriggerRecorder.Recordings.ShouldRunIntegrationRecording.ReturnValue = BuildCondition.IfModificationExists;

            TestSubject.InnerTrigger = TriggerRecorder;

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

            BuildCondition Condition = TestSubject.ShouldRunIntegration();

            Assert.IsTrue(RemotingRecorder.Recordings.ConnectTypeStringRecording.Called);
            Assert.AreEqual("TestUri", RemotingRecorder.Recordings.ConnectTypeStringRecording.PassedStringuri);
            Assert.AreEqual(typeof(ICruiseManager), RemotingRecorder.Recordings.ConnectTypeStringRecording.PassedTypeproxyType);

            Assert.IsTrue(CruiseRecorder.Recordings.GetProjectStatusRecording.Called);

            Assert.AreEqual(BuildCondition.NoBuild, Condition);
        }

#endregion
        
#region BlockedByExclusionFilterCauseFailed

        [Test]
        public void TestShouldRunIntegrationBlockedByExclusionFilterCauseFailed()
        {
            ProjectTriggerFilter TestSubject = new ProjectTriggerFilter();

            RecorderITrigger TriggerRecorder = new RecorderITrigger();
            TriggerRecorder.Recordings.ShouldRunIntegrationRecording.ReturnValue = BuildCondition.IfModificationExists;

            TestSubject.InnerTrigger = TriggerRecorder;

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

            BuildCondition Condition = TestSubject.ShouldRunIntegration();

            Assert.IsTrue(RemotingRecorder.Recordings.ConnectTypeStringRecording.Called);
            Assert.AreEqual("TestUri", RemotingRecorder.Recordings.ConnectTypeStringRecording.PassedStringuri);
            Assert.AreEqual(typeof(ICruiseManager), RemotingRecorder.Recordings.ConnectTypeStringRecording.PassedTypeproxyType);

            Assert.IsTrue(CruiseRecorder.Recordings.GetProjectStatusRecording.Called);

            Assert.AreEqual(BuildCondition.NoBuild, Condition);
        }
        
#endregion
        
#region AllowedByInclusionFilter

        [Test]
        public void TestShouldRunIntegrationAllowedByInclusionFilter()
        {
            ProjectTriggerFilter TestSubject = new ProjectTriggerFilter();

            RecorderITrigger TriggerRecorder = new RecorderITrigger();
            TriggerRecorder.Recordings.ShouldRunIntegrationRecording.ReturnValue = BuildCondition.IfModificationExists;

            TestSubject.InnerTrigger = TriggerRecorder;

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

            BuildCondition Condition = TestSubject.ShouldRunIntegration();

            Assert.IsTrue(RemotingRecorder.Recordings.ConnectTypeStringRecording.Called);
            Assert.AreEqual("TestUri", RemotingRecorder.Recordings.ConnectTypeStringRecording.PassedStringuri);
            Assert.AreEqual(typeof(ICruiseManager), RemotingRecorder.Recordings.ConnectTypeStringRecording.PassedTypeproxyType);

            Assert.IsTrue(CruiseRecorder.Recordings.GetProjectStatusRecording.Called);

            Assert.AreEqual(BuildCondition.IfModificationExists, Condition);
        }

#endregion
        
#region BlockedByInclusionFilterCauseFailed

        [Test]
        public void TestShouldRunIntegrationBlockedByInclusionFilterCauseFailed()
        {
            ProjectTriggerFilter TestSubject = new ProjectTriggerFilter();

            RecorderITrigger TriggerRecorder = new RecorderITrigger();
            TriggerRecorder.Recordings.ShouldRunIntegrationRecording.ReturnValue = BuildCondition.IfModificationExists;

            TestSubject.InnerTrigger = TriggerRecorder;

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

            BuildCondition Condition = TestSubject.ShouldRunIntegration();

            Assert.IsTrue(RemotingRecorder.Recordings.ConnectTypeStringRecording.Called);
            Assert.AreEqual("TestUri", RemotingRecorder.Recordings.ConnectTypeStringRecording.PassedStringuri);
            Assert.AreEqual(typeof(ICruiseManager), RemotingRecorder.Recordings.ConnectTypeStringRecording.PassedTypeproxyType);

            Assert.IsTrue(CruiseRecorder.Recordings.GetProjectStatusRecording.Called);

            Assert.AreEqual(BuildCondition.NoBuild, Condition);
        }

#endregion
        
#region BlockedByInclusionFilterCauseBuilding

        [Test]
        public void TestShouldRunIntegrationBlockedByInclusionFilterCauseBuilding()
        {
            ProjectTriggerFilter TestSubject = new ProjectTriggerFilter();

            RecorderITrigger TriggerRecorder = new RecorderITrigger();
            TriggerRecorder.Recordings.ShouldRunIntegrationRecording.ReturnValue = BuildCondition.IfModificationExists;

            TestSubject.InnerTrigger = TriggerRecorder;

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

            BuildCondition Condition = TestSubject.ShouldRunIntegration();

            Assert.IsTrue(RemotingRecorder.Recordings.ConnectTypeStringRecording.Called);
            Assert.AreEqual("TestUri", RemotingRecorder.Recordings.ConnectTypeStringRecording.PassedStringuri);
            Assert.AreEqual(typeof(ICruiseManager), RemotingRecorder.Recordings.ConnectTypeStringRecording.PassedTypeproxyType);

            Assert.IsTrue(CruiseRecorder.Recordings.GetProjectStatusRecording.Called);

            Assert.AreEqual(BuildCondition.NoBuild, Condition);
        }

#endregion

#endregion

    }
}