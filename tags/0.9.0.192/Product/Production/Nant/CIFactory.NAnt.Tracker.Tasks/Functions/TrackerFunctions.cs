using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using NAnt;
using NAnt.Core;
using NAnt.Core.Util;
using NAnt.Core.Types;
using NAnt.Core.Functions;
using NAnt.Core.Attributes;
using CIFactory.NAnt.Types;

namespace CIFactory.NAnt.Tracker.Functions
{
    [FunctionSet("tracker", "tracker")]
    public class TrackerFunctions : FunctionSetBase
    {

        #region Constructors

        public TrackerFunctions()
            : base(null, null)
        {
        }

        public TrackerFunctions(Project project, PropertyDictionary properties)
            : base(project, properties)
        {
        }

        #endregion

        private List<Regex> _Patterns;

        public List<Regex> Patterns
        {
            get
            {
                if (_Patterns == null)
                {
                    _Patterns = new List<Regex>();
                    _Patterns.Add(new Regex(@"\#\s{0,}(?<scr>\d{4,})", RegexOptions.IgnoreCase | RegexOptions.Multiline));
                    _Patterns.Add(new Regex(@"scr\s{0,}\#{0,}\s{0,}(?<scr>\d{4,})", RegexOptions.IgnoreCase | RegexOptions.Multiline));
                    _Patterns.Add(new Regex(@"^(\s{0,}\#{0,}\s{0,}(?<scr>\d{4,})\s{0,}\,{0,})+$", RegexOptions.IgnoreCase | RegexOptions.Multiline));
                }
                return _Patterns;
            }
        }

        [Function("extract-scr-numbers")]
        public void ExtractScrNumbers(String refID, string comment)
        {

            if (!this.Project.DataTypeReferences.Contains(refID))
                throw new BuildException(String.Format("The refid {0} is not defined.", refID));

            StringList RefStringList = (StringList)this.Project.DataTypeReferences[refID];

            this.GetScrNumbers(comment, RefStringList);
        }

        public void GetScrNumbers(string comment, StringList RefStringList)
        {
            foreach (Regex Pattern in this.Patterns)
            {
                if (Pattern.IsMatch(comment))
                {
                    foreach (Match Matched in Pattern.Matches(comment))
                    {
                        foreach (Capture Captured in Matched.Groups["scr"].Captures)
                        {
                            if (!RefStringList.StringItems.Contains(Captured.Value))
                                RefStringList.StringItems.Add(Captured.Value, new StringItem(Captured.Value));
                        }
                    }
                }
            }
        }
    }
}
