using System;
using NAnt.Core;


namespace NAnt.Core
{
    /// <summary>
    /// Custom build listener for NAnt. This listener writes the X last messages to a file
    /// for viewing the build progress in CCNET.
    /// This assembly has to be copied in the Nant folder, where the NAnt.exe resides
    /// The extra listenfile is XML-based, consisting in the hierarchy of the NAnt project
    /// and the last X messages under the last target of the NAnt project.
    /// </summary>
    public class AppendListener : IBuildListener
    {
        private const string CCNetListenerFile_PropertyName = "CCNetListenerFile";
        private const Int32 _MessageQueueLength = 10;

        private string _traceFileName;
        private Boolean _tracingEnabled;

        private System.Collections.Queue _MessageQueue = new System.Collections.Queue();
        private System.Collections.Stack _ProjectTargetStack = new System.Collections.Stack();


        #region IBuildListener Members

        public void TargetFinished(object sender, BuildEventArgs e)
        {
            this._ProjectTargetStack.Pop();
        }

        public void MessageLogged(object sender, BuildEventArgs e)
        {
            string Data = string.Format("{0}: {1}", GetTimeStamp(), e.Message);

            if (this._MessageQueue.Count > _MessageQueueLength)
            { this._MessageQueue.Dequeue(); }

            this._MessageQueue.Enqueue(Data);

            WriteQueueData();
        }

        public void BuildStarted(object sender, BuildEventArgs e)
        {
            this._tracingEnabled = e.Project.Properties.Contains(CCNetListenerFile_PropertyName);

            this._traceFileName = e.Project.Properties[CCNetListenerFile_PropertyName];

            string Data = String.Format("{0}: Starting Build {1}", GetTimeStamp(), e.Project.ProjectName);
            this._ProjectTargetStack.Push(Data);

            this._MessageQueue.Clear();
        }

        public void BuildFinished(object sender, BuildEventArgs e)
        {
            // log the error in the queue, so it displays in the dashboard 
            // this way the error is available in the extra listen file, so it can be 
            // shown to CCTray, Dashboard if wanted
            if (e.Exception != null)
            {
                string Data = string.Format("{0}: Finished Build {1}", GetTimeStamp(), e.Exception.ToString());
                this._MessageQueue.Enqueue(Data);
                WriteQueueData();
            }
            this._ProjectTargetStack.Pop();
        }

        public void TaskFinished(object sender, BuildEventArgs e)
        {
        }

        public void TargetStarted(object sender, BuildEventArgs e)
        {
            string Data = string.Format("{0}: Starting Target {1}", GetTimeStamp(), e.Target.Name);
            this._ProjectTargetStack.Push(Data);
            this._MessageQueue.Clear();
        }

        public void TaskStarted(object sender, BuildEventArgs e)
        {
        }

        #endregion

        private string GetTimeStamp()
        {
            return System.DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
        }

        private void WriteQueueData()
        {
            if (!this._tracingEnabled) return;

            System.IO.StreamWriter TraceFile;

            System.Collections.ArrayList ProjectsTargets;
            ProjectsTargets = new System.Collections.ArrayList();

            // empty file
            try
            {
                TraceFile = new System.IO.StreamWriter(this._traceFileName, false);
            }
            catch
            {
                return;
            }

            TraceFile.AutoFlush = false;

            // write hierarchy
            ProjectsTargets.AddRange(this._ProjectTargetStack.ToArray());
            ProjectsTargets.Reverse();

            foreach (string data in ProjectsTargets)
            {
                TraceFile.WriteLine("{0}", data);
            }

            // write messages
            foreach (string data in this._MessageQueue.ToArray())
            {
                TraceFile.WriteLine("{0}", data);
            }

            TraceFile.Close();
        }
    }
}