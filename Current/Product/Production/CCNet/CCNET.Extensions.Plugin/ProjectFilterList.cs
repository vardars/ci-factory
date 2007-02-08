using System;
using System.Collections;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using System.ComponentModel;

namespace CCNET.Extensions
{

    [TypeConverter(typeof(ExpandableObjectConverter))]
    [ReflectorType("projectFilters")]
    public class ProjectFilterList : CollectionBase
    {
        public ProjectFilter this[int index]
        {
            get
            {
                return (ProjectFilter)this.InnerList[index];
            }
            set
            {
                this.InnerList[index] = value;
            }
        }

        public void Add(ProjectFilter filter)
        {
            this.InnerList.Add(filter);
        }

    }

}
