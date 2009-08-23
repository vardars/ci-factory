using System;
using System.Collections;
using NAnt.Core;
using NAnt.Core.Attributes;
using System.Text;
using System.Text.RegularExpressions;

namespace CIFactory.NAnt.Types
{
    [ElementName("targetset")]
    public class TargetSet : LoopItems
    {
        #region Fields

        private StringItem[] _exclude;

        private ArrayList _excludeRegexs;

        private StringItem[] _include;

        private ArrayList _includeRegexs;

        #endregion

        #region Properties

        [BuildElementArray("exclude", ElementType = typeof(StringItem), Required = false)]
        public StringItem[] Exclude
        {
            get { return _exclude; }
            set { _exclude = value; }
        }

        public ArrayList ExcludeRegexs
        {
            get
            {
                if (_excludeRegexs == null)
                {
                    _excludeRegexs = this.ConvertPatternsToRegexs(this.Exclude);
                }
                return _excludeRegexs;
            }
            set { _excludeRegexs = value; }
        }

        [BuildElementArray("include", ElementType = typeof(StringItem), Required = false)]
        public StringItem[] Include
        {
            get { return _include; }
            set { _include = value; }
        }

        public ArrayList IncludeRegexs
        {
            get
            {
                if (_includeRegexs == null)
                {
                    _includeRegexs = this.ConvertPatternsToRegexs(this.Include);
                }
                return _includeRegexs;
            }
            set { _includeRegexs = value; }
        }

        #endregion

        #region Protected Methods

        protected override System.Collections.IEnumerator GetStrings()
        {
            ArrayList TargetList = new ArrayList();
            foreach (Target Candidate in this.Project.Targets)
            {
                if (this.LookForMatch(Candidate.Name, this.IncludeRegexs) & !this.LookForMatch(Candidate.Name, this.ExcludeRegexs))
                {
                    TargetList.Add(Candidate.Name);
                }
            }
            return TargetList.GetEnumerator();
        }

        #endregion

        #region Private Methods

        private ArrayList ConvertPatternsToRegexs(StringItem[] patterns)
        {
            ArrayList Matchers = new ArrayList();
            foreach (StringItem RawPattern in patterns)
            {
                StringBuilder RegexPattern = new StringBuilder(RawPattern.StringValue);
                RegexPattern.Replace("\\", "\\\\");
                RegexPattern.Replace(".", "\\.");
                RegexPattern.Replace("*", ".*");
                RegexPattern.Replace("$", "\\$");
                RegexPattern.Replace("^", "\\^");
                RegexPattern.Replace("{", "\\{");
                RegexPattern.Replace("[", "\\[");
                RegexPattern.Replace("(", "\\(");
                RegexPattern.Replace(")", "\\)");
                RegexPattern.Replace("+", "\\+");
                if (RegexPattern.Length > 0)
                {
                    RegexPattern.Insert(0, "^");
                    RegexPattern.Append("$");
                }
                string PatternText = RegexPattern.ToString();
                if ((PatternText.StartsWith("^.*")))
                {
                    PatternText = PatternText.Substring(3);
                }
                if ((PatternText.EndsWith(".*$")))
                {
                    PatternText = PatternText.Substring(0, RegexPattern.Length - 3);
                }
                Matchers.Add(new Regex(PatternText));
            }
            return Matchers;
        }

        private bool LookForMatch(string name, ArrayList regexs)
        {
            foreach (Regex Matcher in regexs)
            {
                if (Matcher.IsMatch(name))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

    }

}