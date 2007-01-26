using System;
using System.Collections.Generic;
using System.Text;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Core.Tasks;

namespace CCNET.TFS.Plugin
{
    public class RollBackTask : ITask
    {
        [ReflectorType("xslmerger")]
        #region ITask Members

        public void Run(IIntegrationResult result)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }
}
