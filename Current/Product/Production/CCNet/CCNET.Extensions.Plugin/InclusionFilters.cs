using System;
using System.Xml;
using System.IO;
using System.Collections;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using System.ComponentModel;

namespace CCNET.Extensions
{

    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ReflectorType("inclusionFilters")]
    public class InclusionFilters
    {
        
#region Fields

        private ProjectActivity[] _Activities;
        private IntegrationStatus[] _Conditions;
        
#endregion
        
#region Properties

        [ReflectorArray("conditions", Required = false)]
        public IntegrationStatus[] Conditions
        {
            get
            {
                return _Conditions;
            }
            set
            {
                _Conditions = value;
            }
        }

        [ReflectorArray("activities", Required = false)]
        public ProjectActivity[] Activities
        {
            get
            {
                return _Activities;
            }
            set
            {
                _Activities = value;
            }
        }

#endregion
        
#region Methods

        public Boolean IsAllowed(ProjectStatus status)
        {
            Boolean ToAllow = true;

            if (Conditions != null && Conditions.Length != 0)
            {
                foreach (IntegrationStatus Condition in this.Conditions)
                {
                    if (Condition != status.BuildStatus)
                        return false;
                }
            }

            if (Activities != null && Activities.Length != 0)
            {
                foreach (ProjectActivity Activity in this.Activities)
                {
                    if (Activity != status.Activity)
                        return false;
                }
            }

            return ToAllow;
        }

#endregion

        public void Test()
        {
            this.Conditions = new IntegrationStatus[1] { IntegrationStatus .Failure };
            this.Activities = new ProjectActivity[1] { ProjectActivity.Building };

            string Serialized = Zation.Serialize("inclusionFilters", this);

            InclusionFilters Clone = (InclusionFilters)Zation.Deserialize(Serialized);
            if (Clone.Activities[0] != ProjectActivity.Building)
                throw new Exception();
        }
    }

}
