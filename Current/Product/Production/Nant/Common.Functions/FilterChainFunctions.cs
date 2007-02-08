using System;
using System.Collections;
using System.Text;

using NAnt.Core;
using NAnt.Core.Filters;
using NAnt.Core.Types;
using NAnt.Core.Attributes;
using NAnt.Core.Functions;

namespace Common.Functions
{
    [FunctionSet("filterchain", "DataTypes")]
    public class FilterChainFunctions : FunctionSetBase
    {

        public FilterChainFunctions(Project project, PropertyDictionary properties)
            : base(project, properties)
        {
        }

        [Function("replacestring-count")]
        public int ReplaceStringCount(String refID)
        {
            if (!this.Project.DataTypeReferences.Contains(refID))
                throw new BuildException(String.Format("The refid {0} is not defined.", refID));

            FilterChain RefFilterChain = (FilterChain)this.Project.DataTypeReferences[refID];
            return RefFilterChain.Filters.Count;
        }

        [Function("replacestring-add")]
        public void AddReplaceString(String refID, String oldValue, String newValue)
        {
            if (!this.Project.DataTypeReferences.Contains(refID))
                throw new BuildException(String.Format("The refid {0} is not defined.", refID));

            FilterChain RefFilterChain = (FilterChain)this.Project.DataTypeReferences[refID];
            ReplaceString ReplaceStringFilter = new ReplaceString();
            ReplaceStringFilter.From = oldValue;
            ReplaceStringFilter.To = newValue;
            RefFilterChain.Filters.Add(ReplaceStringFilter);
        }

        [Function("replacestring-remove")]
        public void RemoveReplaceString(String refID, String oldValue, String newValue)
        {
            if (!this.Project.DataTypeReferences.Contains(refID))
                throw new BuildException(String.Format("The refid {0} is not defined.", refID));

            FilterChain RefFilterChain = (FilterChain)this.Project.DataTypeReferences[refID];
            Boolean Found = false;
            ReplaceString ToRemove = null;
            foreach (Filter ReplacementFilter in RefFilterChain.Filters)
            {
                if (ReplacementFilter is ReplaceString && 
                    ((ReplaceString)ReplacementFilter).From == oldValue &&
                    ((ReplaceString)ReplacementFilter).To== newValue)
                {
                    ToRemove = (ReplaceString)ReplacementFilter;
                    Found = true;
                    break;
                }
            }
            if (Found)
                RefFilterChain.Filters.Remove(ToRemove);
        }

        [Function("replacestring-contains")]
        public Boolean ContainsReplaceString(String refID, String oldValue, String newValue)
        {
            if (!this.Project.DataTypeReferences.Contains(refID))
                throw new BuildException(String.Format("The refid {0} is not defined.", refID));

            FilterChain RefFilterChain = (FilterChain)this.Project.DataTypeReferences[refID];
            foreach (Filter ReplacementFilter in RefFilterChain.Filters)
            {
                if (ReplacementFilter is ReplaceString &&
                    ((ReplaceString)ReplacementFilter).From == oldValue &&
                    ((ReplaceString)ReplacementFilter).To == newValue)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
