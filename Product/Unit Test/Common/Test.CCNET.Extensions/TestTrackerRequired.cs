using System;
using System.IO;
using System.Xml;
using System.Collections;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.TestDoubles;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using MbUnit.Framework;
using MbUnit.Core.Framework;
using ThoughtWorks.CruiseControl.Remote.TestDoubles;
using TestDoubles;
using ThoughtWorks.CruiseControl.Core.IntegrationFilters;
using Tracker.CCNET.Plugin.IntegrationFilters;
using Test.Tracker.Stubs;
using Tracker.Common;

namespace CCNET.Extensions.Test
{
    [TestFixture]
    public class TestTrackerRequired
    {
        [Test]
        public void TestShouldRunBuild()
        {
            TrackerRequired TestSubject = new TrackerRequired();

            ToolKitStub ToolKitStub = new ToolKitStub();
            TestSubject.ToolKit = ToolKitStub;

            int[] IdList = new int[2] { 1234, 2345 };
            ToolKitStub.QueryIdList = IdList;

            TestSubject.Condition = true;
            TestSubject.WithModifications = false;

            TestSubject.ConnectionInformation = new ConnectionInformation();
            TestSubject.ConnectionInformation.dbmsLoginMode = 2;
            TestSubject.ConnectionInformation.dbmsPassword = "p";
            TestSubject.ConnectionInformation.dbmsServer = "s";
            TestSubject.ConnectionInformation.dbmsType = "t";
            TestSubject.ConnectionInformation.dbmsUserName = "n";
            TestSubject.ConnectionInformation.Password = "p";
            TestSubject.ConnectionInformation.ProjectName = "n";
            TestSubject.ConnectionInformation.UserName = "n";

            TestSubject.QueryInforation = new Query();
            TestSubject.QueryInforation.Name = "q";

            RecorderIIntegrationResult ResultRecorder = new RecorderIIntegrationResult();
            ResultRecorder.Recordings.HasModificationsRecording.ReturnValue = false;

            bool ShouldRun = TestSubject.ShouldRunBuild(ResultRecorder);

            Assert.IsTrue(ShouldRun);
        }
    }
}
