using System;
using NAnt.Core;
using NAnt.Core.Filters;
using NAnt.Core.Attributes;
using CIFactory.NAnt.Filters;

namespace CIFactory.NAnt.Functions
{
    [FunctionSet("filterchain", "DataTypes")]
    public class FilterChainFunctions : FunctionSetBase
    {
        #region Constructors

        public FilterChainFunctions(Project project, PropertyDictionary properties)
            : base(project, properties)
        {
        }

        #endregion

        #region Public Methods

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

        [Function("regexreplace-add")]
        public void AddReplaceString(String refID, String replacment, String pattern, int lines)
        {
            if (!this.Project.DataTypeReferences.Contains(refID))
                throw new BuildException(String.Format("The refid {0} is not defined.", refID));

            FilterChain RefFilterChain = (FilterChain)this.Project.DataTypeReferences[refID];

            RegexFilter Regex = new RegexFilter();
            Regex.Lines = lines;
            Regex.Pattern = pattern;
            Regex.Replacment = replacment;

            RefFilterChain.Filters.Add(Regex);
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
                    ((ReplaceString)ReplacementFilter).To == newValue)
                {
                    ToRemove = (ReplaceString)ReplacementFilter;
                    Found = true;
                    break;
                }
            }
            if (Found)
                RefFilterChain.Filters.Remove(ToRemove);
        }

        [Function("replacestring-count")]
        public int ReplaceStringCount(String refID)
        {
            if (!this.Project.DataTypeReferences.Contains(refID))
                throw new BuildException(String.Format("The refid {0} is not defined.", refID));

            FilterChain RefFilterChain = (FilterChain)this.Project.DataTypeReferences[refID];
            return RefFilterChain.Filters.Count;
        }

        #endregion

    }
}
