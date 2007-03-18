using System;
using System.Collections.Generic;
using System.Text;

namespace VSTS.Tasks
{
    public class NoIsolationCommand : Command
    {
        private object _WrappedSubject;

        protected override Object WrappedSubject
        {
            get
            {
                if (_WrappedSubject == null)
                {
                    _WrappedSubject = TestToolsHelper.CreateInstance("Microsoft.VisualStudio.TestTools.CommandLine.NoIsolationCommand");
                }
                return _WrappedSubject;
            }
        }


        public NoIsolationCommand()
        {
            this.Initialize(null);
        }
    }
}
