using System;
using System.Collections.Generic;
using System.Text;

namespace VSTS.Tasks
{
    public class TestContainerCommand : Command
    {
        private string _TestAssemblyPath;
        private object _WrappedSubject;

        protected override Object WrappedSubject
        {
            get
            {
                if (_WrappedSubject == null)
                {
                    _WrappedSubject = TestToolsHelper.CreateInstance("Microsoft.VisualStudio.TestTools.CommandLine.TestContainerCommand");
                }
                return _WrappedSubject;
            }
        }

        public string TestAssemblyPath
        {
            get
            {
                return _TestAssemblyPath;
            }
            set
            {
                _TestAssemblyPath = value;
            }
        }

        public TestContainerCommand(string testAssemblyPath)
        {
            _TestAssemblyPath = testAssemblyPath;
            this.Initialize(this.TestAssemblyPath);
        }
    }
}