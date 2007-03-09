using System;
using NAnt.Core;
using NAnt.Core.Attributes;
using CIFactory.NAnt.Types;

namespace CIFactory.NAnt.Functions
{
    
    [FunctionSet("stringlist", "DataTypes")]
    public class StringListFunctions : FunctionSetBase
    {
        #region Constructors

        public StringListFunctions(Project project, PropertyDictionary properties)
            : base(project, properties)
        {
        }

        #endregion

        #region Public Methods

        [Function("add")]
        public void AddInclude(String refID, String name)
        {
            if (!this.Project.DataTypeReferences.Contains(refID))
                throw new BuildException(String.Format("The refid {0} is not defined.", refID));

            StringList RefStringList = (StringList)this.Project.DataTypeReferences[refID];
            RefStringList.StringItems.Add(name, new StringItem(name));
        }

        [Function("contains")]
        public Boolean ContainsInclude(String refID, String name)
        {
            if (!this.Project.DataTypeReferences.Contains(refID))
                throw new BuildException(String.Format("The refid {0} is not defined.", refID));

            StringList RefStringList = (StringList)this.Project.DataTypeReferences[refID];
            return RefStringList.StringItems.Contains(name);
        }

        [Function("item")]
        public String Count(String refID, int index)
        {
            if (!this.Project.DataTypeReferences.Contains(refID))
                throw new BuildException(String.Format("The refid {0} is not defined.", refID));

            StringList RefStringList = (StringList)this.Project.DataTypeReferences[refID];
            return RefStringList.StringItems.Values[index];
        }

        [Function("count")]
        public int Count(String refID)
        {
            if (!this.Project.DataTypeReferences.Contains(refID))
                throw new BuildException(String.Format("The refid {0} is not defined.", refID));

            StringList RefStringList = (StringList)this.Project.DataTypeReferences[refID];
            return RefStringList.StringItems.Count;
        }

        [Function("remove")]
        public void RemoveInclude(String refID, String name)
        {
            if (!this.Project.DataTypeReferences.Contains(refID))
                throw new BuildException(String.Format("The refid {0} is not defined.", refID));

            StringList RefStringList = (StringList)this.Project.DataTypeReferences[refID];
            RefStringList.StringItems.Remove(name);
        }

        [Function("sort-assending")]
        public void SortAssending(String refID)
        {
            if (!this.Project.DataTypeReferences.Contains(refID))
                throw new BuildException(String.Format("The refid {0} is not defined.", refID));

            StringList RefStringList = (StringList)this.Project.DataTypeReferences[refID];
            RefStringList.StringItems.Sort();
        }

        [Function("sort-dessending")]
        public void SortDessending(String refID)
        {
            if (!this.Project.DataTypeReferences.Contains(refID))
                throw new BuildException(String.Format("The refid {0} is not defined.", refID));

            StringList RefStringList = (StringList)this.Project.DataTypeReferences[refID];
            RefStringList.StringItems.ReverseSort();
        }

        #endregion

    }
}
