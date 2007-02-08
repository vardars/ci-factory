using System;
using System.Collections;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using System.ComponentModel;

namespace CCNET.Extensions
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ReflectorType("projectFilter")]
    public class ProjectFilter
    {
        
#region Constants

        public const string DefaultServerUri = RemoteCruiseServer.DefaultUri;

#endregion
        
#region Fields

        private InclusionFilters _inclusionFilters;
        private ExclusionFilters _exclusionFilters;
        private string _Project;
        private string _ServerUri = DefaultServerUri;
        private IRemotingService _RemoteService;

#endregion
        
#region Properties

        public IRemotingService RemoteService
        {
            get
            {
                if (_RemoteService == null)
                    _RemoteService = new RemotingServiceAdapter();
                return _RemoteService;
            }
            set
            {
                _RemoteService = value;
            }
        }

        [ReflectorProperty("project")]
        public string Project
        {
            get
            {
                return _Project;
            }
            set
            {
                _Project = value;
            }
        }

        [ReflectorProperty("serverUri", Required = false)]
        public string ServerUri
        {
            get
            {
                return _ServerUri;
            }
            set
            {
                _ServerUri = value;
            }
        }

        [ReflectorProperty("exclusionFilters", Required = false)]
        public ExclusionFilters ExclusionFilters
        {
            get
            {
                if (_exclusionFilters == null)
                    _exclusionFilters = new ExclusionFilters();

                return _exclusionFilters;
            }
            set { _exclusionFilters = value; }
        }

        [ReflectorProperty("inclusionFilters", Required = false)]
        public InclusionFilters InclusionFilters
        {
            get
            {
                if (_inclusionFilters == null)
                    _inclusionFilters = new InclusionFilters();

                return _inclusionFilters;
            }
            set { _inclusionFilters = value; }
        }

#endregion
        
#region Methods

        public Boolean IsAllowed()
        {
            ProjectStatus CurrentStatus = this.GetCurrentProjectStatus();

            if (!this.InclusionFilters.IsAllowed(CurrentStatus))
                return false;

            if (!this.ExclusionFilters.IsAllowed(CurrentStatus))
                return false;

            return true;
        }

        private ProjectStatus GetCurrentProjectStatus()
        {
            Log.Debug("Retrieving ProjectStatus from server: " + ServerUri);
            ICruiseManager cruiseManager = (ICruiseManager)this.RemoteService.Connect(typeof(ICruiseManager), ServerUri);
            ProjectStatus[] currentStatuses = cruiseManager.GetProjectStatus();
            foreach (ProjectStatus currentStatus in currentStatuses)
            {
                if (currentStatus.Name == Project)
                {
                    return currentStatus;
                }
            }
            throw new NoSuchProjectException(Project);
        }

#endregion

        public void Test()
        {
            this.ExclusionFilters.Conditions = new IntegrationStatus[1] { IntegrationStatus.Failure };
            this.ExclusionFilters.Activities = new ProjectActivity[1] { ProjectActivity.Building };

            this.Project = "Project 1";

            string Serialized = Zation.Serialize("projectFilter", this);

            ProjectFilter Clone = (ProjectFilter)Zation.Deserialize(Serialized);
            if (Clone.ExclusionFilters.Activities[0] != ProjectActivity.Building)
                throw new Exception();
        }
    }

}
