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
            Log.Info(string.Format("{0}:Begin", System.Reflection.MethodBase.GetCurrentMethod().Name));
            this.InnerTrigger.IntegrationCompleted();
            Log.Info(string.Format("{0}:End", System.Reflection.MethodBase.GetCurrentMethod().Name));
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
            Log.Info(string.Format("{0}:Begin", System.Reflection.MethodBase.GetCurrentMethod().Name));

			BuildCondition ShouldRun = this.InnerTrigger.ShouldRunIntegration();
			if (ShouldRun == BuildCondition.NoBuild)
				return BuildCondition.NoBuild;

			foreach (ProjectFilter Project in this.ProjectFilters)
			{
				if (!Project.IsAllowed())
				{
                    Log.Info(string.Format("{0}:this.InnerTrigger.IntegrationNotRun()", System.Reflection.MethodBase.GetCurrentMethod().Name));
                    this.InnerTrigger.IntegrationNotRun();
					return BuildCondition.NoBuild;
				}
			}

            Log.Info(string.Format("{0}:ShouldRun={1}", System.Reflection.MethodBase.GetCurrentMethod().Name, ShouldRun.ToString()));
            return ShouldRun;
		}
        
#endregion


		#region ITrigger Members


		public void IntegrationNotRun()
		{
            Log.Info(string.Format("{0}:Begin", System.Reflection.MethodBase.GetCurrentMethod().Name));
			this.InnerTrigger.IntegrationNotRun();
            Log.Info(string.Format("{0}:End", System.Reflection.MethodBase.GetCurrentMethod().Name));
        }

		#endregion
	}
}