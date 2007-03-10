using System;
using System.Collections;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace Tracker.CCNET.Plugin.IntegrationFilters
{

    [ReflectorType("query")]
    public class Query
    {
        private string _Name;

        [ReflectorProperty("name", Required = true)]
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }
    }

}
