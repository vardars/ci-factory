using System;
using System.Collections.Generic;
using System.Text;

namespace VSTS.Tasks
{
    public class ResultsOutputCommand : Command
    {
        private object _WrappedSubject;
        private string _ReportFilePath;
        public string ReportFilePath
        {
            get
            {
                return _ReportFilePath;
            }
            set
            {
                _ReportFilePath = value;
            }
        }

        protected override Object WrappedSubject
        {
            get
            {
                if (_WrappedSubject == null)
                {
                    _WrappedSubject = TestToolsHelper.CreateInstance("Microsoft.VisualStudio.TestTools.CommandLine.ResultsOutputCommand");
                }
                return _WrappedSubject;
            }
        }


        public ResultsOutputCommand(string reportFilePath)
        {
            _ReportFilePath = reportFilePath;
            this.Initialize(this.ReportFilePath);
        }
    }
}
