using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using CCNET.Extensions.Plugin.ForceFilters;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Core.TestDoubles;

namespace CCNET.Extensions.Test
{
    [TestFixture]
    public class TestHostForceFilter
    {
        [Row("frog", "frog", true)]
        [Row("frog", "toad", false)]
        [RowTest]
        public void ShouldRunIntegration(string stubName, string includedName, bool expected)
        {
            HostForceFilter TestSubject = new HostForceFilter();
            TestSubject.Hosts = new string[] { includedName };
            TestSubject.HostHelper = new HostNameHelperStub(stubName);
            TestSubject.Logger = new LogHelperFake();

            RecorderIIntegrationResult IntegrationResult = new RecorderIIntegrationResult();
            IntegrationResult.SetProjectName = "bogas";

            ForceFilterClientInfo ClientInfo = TestSubject.GetClientInfo();

            bool Actual = TestSubject.ShouldRunIntegration(new ForceFilterClientInfo[] { ClientInfo }, IntegrationResult);

            Assert.AreEqual(expected, Actual, "Got the opposite result as expected from ShouldRunIntegration.");
        }
    }

    public class LogHelperFake : ILogHelper
    {
        
        #region ILogHelper Members

        public void LogInfo(string message)
        {
            
        }

        #endregion
    }

    public class HostNameHelperStub : IHostNameHelper
    {
        private string _HostName;
        
        public HostNameHelperStub(string hostName)
        {
            this._HostName = hostName;
        }

        #region IHostNameHelper Members

        public string GetHostName()
        {
            return this._HostName;
        }

        #endregion
    }
}
