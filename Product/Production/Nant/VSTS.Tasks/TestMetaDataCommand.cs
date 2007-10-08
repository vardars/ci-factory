using System;
using System.Collections.Generic;
using System.Text;

namespace VSTS.Tasks
{

    public class TestMetaDataCommand : Command
    {
        private string _VsmidPath;
        private object _WrappedSubject;

        protected override Object WrappedSubject
        {
            get
            {
                if (_WrappedSubject == null)
                {
                    _WrappedSubject = TestToolsHelper.CreateInstance("Microsoft.VisualStudio.TestTools.CommandLine.TestMetaDataCommand");
                }
                return _WrappedSubject;
            }
        }

        public string VsmidPath
        {
            get
            {
                return _VsmidPath;
            }
            set
            {
                _VsmidPath = value;
            }
        }

        public TestMetaDataCommand(string vsmidPath)
        {
            _VsmidPath = vsmidPath;
            this.Initialize(this.VsmidPath);
        }
    }

}
