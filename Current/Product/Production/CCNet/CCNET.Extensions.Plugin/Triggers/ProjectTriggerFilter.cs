using System;
using System.Collections;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace CCNET.Extensions.Triggers
{
    [ReflectorType("projectTriggerFilter")]
    public class ProjectTriggerFilter : ITrigger
    {
        
#region Fields

        private ITrigger _InnerTrigger;
        private ProjectFilterList _ProjectFilters;

#endregion
        
#region Properties

        [ReflectorCollection("projectFilters", InstanceType = typeof(ProjectFilterList), Required = true)]
        public ProjectFilterList ProjectFilters
        {
            get
            {
                return _ProjectFilters;
            }
            set
            {
                _ProjectFilters = value;
            }
        }

        [ReflectorProperty("trigger", InstanceTypeKey = "type")]
        public ITrigger InnerTrigger
        {
            get
            {
                return _InnerTrigger;
            }
            set
            {
                _InnerTrigger = value;
            }
        }

#endregion

#region ITrigger Members

        public void IntegrationCompleted()
        {
            this.InnerTrigger.IntegrationCompleted();
        }

        public DateTime NextBuild
        {
            get
            {
                return this.InnerTrigger.NextBuild;
            }
        }

		public BuildCondition ShouldRunIntegration()
		{
			BuildCondition ShouldRun = this.InnerTrigger.ShouldRunIntegration();
			if (ShouldRun == BuildCondition.NoBuild)
				return BuildCondition.NoBuild;

			foreach (ProjectFilter Project in this.ProjectFilters)
			{
				if (!Project.IsAllowed())
				{
					return BuildCondition.NoBuild;
				}
			}

			return ShouldRun;
		}
        
#endregion


		#region ITrigger Members


		public void IntegrationNotRun()
		{
			this.InnerTrigger.IntegrationNotRun();
		}

		#endregion
	}
}