using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using System.Diagnostics;

namespace ThoughtWorks.CruiseControl.Core.Triggers
{
	[ReflectorType("urlTrigger")]
	public class UrlTrigger : IntervalTrigger
	{
		private HttpWrapper httpRequest;
		private DateTime lastModified;
        private DateTime oldLastModifiedDate;
		private Uri uri;

		public UrlTrigger() : this(new DateTimeProvider(), new HttpWrapper())
		{}

		public UrlTrigger(DateTimeProvider dtProvider, HttpWrapper httpWrapper) : base(dtProvider)
		{
			this.httpRequest = httpWrapper;
		}

		[ReflectorProperty("url", Required=true)]
		public virtual string Url
		{
			get { return uri.ToString(); }
			set { uri = new Uri(value); }
		}

		public override BuildCondition ShouldRunIntegration()
		{
			BuildCondition condition = base.ShouldRunIntegration();
			if (condition == BuildCondition.NoBuild) return condition;

			Log.Debug(string.Format("More than {0} seconds since last integration, checking url.", IntervalSeconds));
			if (HasUrlChanged())
			{
				return BuildCondition;
			}
			return BuildCondition.NoBuild;
		}

		private bool HasUrlChanged()
		{
			try
			{
				DateTime newModifiedTime = httpRequest.GetLastModifiedTimeFor(uri, lastModified);
                Log.Info(string.Format("lastModified: {0}", lastModified.ToString()));
                Log.Info(string.Format("newModifiedTime: {0}", newModifiedTime.ToString()));
                Log.Info(string.Format("oldLastModifiedDate: {0}", oldLastModifiedDate.ToString()));
                if (lastModified == new DateTime() || newModifiedTime > lastModified)
				{
                    oldLastModifiedDate = lastModified;
                    lastModified = newModifiedTime;
                    Log.Info("Modifying Dates...");
                    Log.Info(string.Format("lastModified: {0}", lastModified.ToString()));
                    Log.Info(string.Format("newModifiedTime: {0}", newModifiedTime.ToString()));
                    Log.Info(string.Format("oldLastModifiedDate: {0}", oldLastModifiedDate.ToString()));
                    return true;
				}

			}
			catch (Exception e)
			{
				Log.Error("Error accessing url: " + uri);
				Log.Error(e);
			}
			return false;
		}


        public override void IntegrationNotRun()
        {
            Log.Info("IntegrationNotRun...");
            Log.Info(string.Format("lastModified: {0}", lastModified.ToString()));
            Log.Info(string.Format("oldLastModifiedDate: {0}", oldLastModifiedDate.ToString()));
            base.IntegrationNotRun();
            lastModified = oldLastModifiedDate;
            Log.Info(string.Format("lastModified: {0}", lastModified.ToString()));
            Log.Info(string.Format("oldLastModifiedDate: {0}", oldLastModifiedDate.ToString()));
        }

        public override void IntegrationCompleted()
        {
            Log.Info("IntegrationCompleted...");

            StackTrace stackTrace = new StackTrace();           // get call stack
            StackFrame[] stackFrames = stackTrace.GetFrames();  // get method calls (frames)

            // write call stack method names
            foreach (StackFrame stackFrame in stackFrames)
            {
                Log.Info(string.Format("{0}:({1}){2}", stackFrame.GetFileName(), stackFrame.GetFileLineNumber(), stackFrame.GetMethod().Name));   // write method name
            }

            Log.Info(string.Format("lastModified: {0}", lastModified.ToString()));
            Log.Info(string.Format("oldLastModifiedDate: {0}", oldLastModifiedDate.ToString()));
            oldLastModifiedDate = lastModified;
            base.IntegrationCompleted();
            Log.Info(string.Format("lastModified: {0}", lastModified.ToString()));
            Log.Info(string.Format("oldLastModifiedDate: {0}", oldLastModifiedDate.ToString()));
        }
	}
   
}