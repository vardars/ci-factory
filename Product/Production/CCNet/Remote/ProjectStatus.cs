using System;

namespace ThoughtWorks.CruiseControl.Remote
{
	/// <summary>
	/// Value type that contains extensive details about a project's most recent
	/// integration.
	/// </summary>
	/// <remarks>
	/// This class is serialized to persist CruiseControl.NET's state for a
	/// particular project, hence is is marked with a <see cref="SerializableAttribute"/>.
	/// </remarks>
	[Serializable]
	public class ProjectStatus
	{
		private ProjectIntegratorState status;
		private IntegrationStatus buildStatus;
		private ProjectActivity activity;
		private string name;
		private string webURL;
		private DateTime lastBuildDate;
		private string lastBuildLabel;
		private string lastSuccessfulBuildLabel;
		private readonly DateTime nextBuildTime;
        private string _Forcee;
        private Modification[] _Modifications;
        private DateTime currentBuildStartTime;
        private BuildCondition buildCondition;
        private TimeSpan lastBuildDuration;

		public ProjectStatus() { }

		public ProjectStatus(ProjectIntegratorState status, IntegrationStatus buildStatus, 
            ProjectActivity activity, string name, string webURL, DateTime lastBuildDate, TimeSpan lastBuildDuration,
            string lastBuildLabel, string lastSuccessfulBuildLabel, DateTime nextBuildTime,
            string forcee, Modification[] modifications, DateTime currentBuildStartTime, BuildCondition buildCondition) 
		{
			this.status = status;
			this.buildStatus = buildStatus;
			this.activity = activity;
			this.name = name;
			this.webURL = webURL;
			this.lastBuildDate = lastBuildDate;
			this.lastBuildLabel = lastBuildLabel;
			this.lastSuccessfulBuildLabel = lastSuccessfulBuildLabel;
			this.nextBuildTime = nextBuildTime;
            this._Forcee = forcee;
            this._Modifications = modifications;
            this.currentBuildStartTime = currentBuildStartTime;
            this.buildCondition = buildCondition;
            this.lastBuildDuration = lastBuildDuration;
		}


        public TimeSpan LastBuildDuration
        {
            get
            {
                return lastBuildDuration;
            }
            set
            {
                lastBuildDuration = value;
            }
        }
        public BuildCondition BuildCondition
        {
            get
            {
                return buildCondition;
            }
            set
            {
                buildCondition = value;
            }
        }

        public DateTime CurrentBuildStartTime
        {
            get
            {
                return currentBuildStartTime;
            }
            set
            {
                currentBuildStartTime = value;
            }
        }

        public Modification[] Modifications
        {
            get { return _Modifications; }
            set
            {
                _Modifications = value;
            }
        }
        
        public string Forcee
        {
            get { return _Forcee; }
            set
            {
                _Forcee = value;
            }
        }

		public ProjectIntegratorState Status 
		{
			get { return status; }
			set { status = value; }
		}

		public IntegrationStatus BuildStatus 
		{
			get { return buildStatus; }
			set { buildStatus = value; }
		}

		public ProjectActivity Activity 
		{
			get { return activity; }
			set { activity = value; }
		}

		public string Name 
		{
			get { return name; }
			set { name = value; }
		}

		public string WebURL
		{
			get { return webURL; }
			set { webURL = value; }
		}

		public DateTime LastBuildDate
		{
			get { return lastBuildDate; }
			set { lastBuildDate = value; }
		}

		public string LastBuildLabel
		{
			get { return lastBuildLabel; }
			set { lastBuildLabel = value; }
		}

		public string LastSuccessfulBuildLabel
		{
			get { return lastSuccessfulBuildLabel; }
			set { lastSuccessfulBuildLabel = value; }
		}

		public DateTime NextBuildTime
		{
			get { return nextBuildTime; }
		}
	}
}