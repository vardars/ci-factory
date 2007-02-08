using System;
using System.IO;
using System.Collections;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Core.Tasks;

namespace CCNET.Extensions.XslFileMerge
{


    public class XslMergerTaskResult : ITaskResult
    {

        private string _Data;

        public XslMergerTaskResult(string data)
        {
            _Data = data;
        }
        public string Data
        {
            get { return _Data; }
        }

        public bool Succeeded()
        {
            return true;
        }

        public bool Failed()
        {
            return false;
        }

    }

}
