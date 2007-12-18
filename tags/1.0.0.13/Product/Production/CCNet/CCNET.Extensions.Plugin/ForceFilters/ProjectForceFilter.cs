using System;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace CCNET.Extensions.ForceFilters
{
    [ReflectorType("projectForceFilter")]
    public class ProjectForceFilter : IForceFilter
    {

        private ProjectFilterList _ProjectFilters;

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

        #region IForceFilter Members

        public bool ShouldRunIntegration(ForceFilterClientInfo[] clientInfo, IIntegrationResult result)
        {
            foreach (ProjectFilter Project in this.ProjectFilters)
            {
                if (!Project.IsAllowed())
                    return false;
            }
            return true;
        }

        public ForceFilterClientInfo GetClientInfo()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool RequiresClientInformation
        {
            get { return false; }
        }

        #endregion

    }
}
