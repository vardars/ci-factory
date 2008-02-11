using System;
using System.Threading;
using NAnt.Core;
using NAnt.Core.Attributes;

namespace CIFactory.NAnt.Tasks
{
    [TaskName("waitfor")]
    public class WaitForTask : Task
    {
        private string _Condition;
        private int _timeOutInSeconds = 5;
        private int _pollingIntervalInSeconds = 5;

        [TaskAttribute("condition", ExpandProperties = false, Required = true)]
        public string Condition
        {
            get
            {
                return _Condition;
            }
            set
            {
                _Condition = value;
            }
        }

        [TaskAttribute("timeout", Required = false)]
        public int TimeOut
        {
            get { return _timeOutInSeconds; }
            set { _timeOutInSeconds = value; }
        }

        [TaskAttribute("pollinginterval", Required = false)]
        public int PollingInterval
        {
            get { return _pollingIntervalInSeconds; }
            set { _pollingIntervalInSeconds = value; }
        }

        protected override void ExecuteTask()
        {
            if (bool.Parse(this.Project.ExpandProperties(this.Condition, this.Location)))
                return;
            DateTime startTime = DateTime.Now;
            TimeSpan timeout = new TimeSpan(0, 0, TimeOut);
            while (true)
            {
                TimeSpan elapsed = DateTime.Now - startTime;
                if (elapsed >= timeout)
                    throw new BuildException("Timed-out", this.Location);
                Thread.Sleep(PollingInterval * 1000);

                if (bool.Parse(this.Project.ExpandProperties(this.Condition, this.Location)))
                    break;
            }
        }
    }
}
