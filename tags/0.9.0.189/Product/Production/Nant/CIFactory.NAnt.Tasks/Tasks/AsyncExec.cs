using System;
using System.Diagnostics;
using System.IO;
using NAnt.Core;
using NAnt.Core.Tasks;
using NAnt.Core.Attributes;

namespace CIFactory.NAnt.Tasks
{
    [TaskName("asyncexec")]
    public class AsyncExec : ExecTask
    {
        #region Fields

        private TextWriter _ErrorWriter;

        private TextWriter _OutputWriter;

        private Process _Process;

        private string _taskName = string.Empty;

        private bool _waitForExit = true;

        #endregion

        #region Properties

        public override System.IO.TextWriter ErrorWriter
        {
            get
            {
                if (_ErrorWriter == null)
                {
                    _ErrorWriter = TextWriter.Null;
                }
                return _ErrorWriter;
            }
            set { }
        }

        public override FileInfo Output
        {
            get { return null; }
            set { Log(Level.Warning, "The output attribute is not used for the asyncexec task.  Please do something like pipe the output to a file."); }
        }

        public override System.IO.TextWriter OutputWriter
        {
            get
            {
                if (_OutputWriter == null)
                {
                    _OutputWriter = TextWriter.Null;
                }
                return _OutputWriter;
            }
            set { }
        }

        private Process Process
        {
            get { return _Process; }
            set { _Process = value; }
        }

        [TaskAttribute("taskname", Required = false)]
        public string TaskName
        {
            get { return _taskName; }
            set { _taskName = value; }
        }

        [TaskAttribute("waitforexit", Required = false)]
        public bool WaitForExit
        {
            get { return _waitForExit; }
            set { _waitForExit = value; }
        }

        #endregion

        #region Public Methods

        public void Wait()
        {
            try
            {
                this.Process.WaitForExit(this.TimeOut);
                if (!this.Process.HasExited)
                {
                    try
                    {
                        this.Process.Kill();
                    }
                    catch
                    {
                    }
                    throw new BuildException(string.Format("External Program {0} did not finish within {1} milliseconds.", new object[] { this.ProgramFileName, this.TimeOut }), this.Location);
                }
                if ((this.Process.ExitCode != 0))
                {
                    throw new BuildException(string.Format("External Program Failed: {0} (return code was {1})", new object[] { this.ProgramFileName, this.Process.ExitCode }), this.Location);
                }
            }
            catch (BuildException exception1)
            {
                if (base.FailOnError)
                {
                    throw;
                }
                this.Log(Level.Error, exception1.Message);
            }
            finally
            {
                if ((this.ResultProperty != null && this.WaitForExit))
                {
                    this.Properties[this.ResultProperty] = this.Process.ExitCode.ToString();
                }
            }
        }

        #endregion

        #region Protected Methods

        protected override void ExecuteTask()
        {
            this.Process = this.StartProcess();
            if (this.TaskName != string.Empty && this.WaitForExit == false)
            {
                Log(Level.Warning, "You set the attribute taskname to {0} and waitforexit to false.  You will not be able to call the waitforexit task with the task name {0} with an error.  If you wanted to wait for this to exit please set waitforexit to true.", this.TaskName);
            }
            if (this.TaskName != string.Empty && this.WaitForExit)
            {
                AsyncExecList.Add(this.TaskName, this);
            }
            if (this.TaskName == string.Empty && this.WaitForExit)
            {
                this.Wait();
            }
        }

        #endregion

    }

}