using System;
using System.Collections.Generic;
using System.Text;

namespace VSTS.Tasks
{
    public class RunConfigCommand : Command
    {
        private object _WrappedSubject;
        private string _RunConfigPath;
        public string RunConfigPath
        {
            get
            {
                return _RunConfigPath;
            }
            set
            {
                _RunConfigPath = value;
            }
        }

        protected override Object WrappedSubject
        {
            get
            {
                if (_WrappedSubject == null)
                {
                    _WrappedSubject = TestToolsHelper.CreateInstance("Microsoft.VisualStudio.TestTools.CommandLine.RunConfigCommand");
                }
                return _WrappedSubject;
            }
        }


        public RunConfigCommand(string runConfig)
        {
            _RunConfigPath = runConfig;
            this.Initialize(this.RunConfigPath);
        }
    }
}
