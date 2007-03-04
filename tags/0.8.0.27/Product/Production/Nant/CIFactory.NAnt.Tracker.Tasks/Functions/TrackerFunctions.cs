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
        public string ExtractScrNumbers(string comment)
        {
            StringCollection ScrList = new StringCollection();

            foreach (Regex Pattern in this.Patterns)
            {
                if (Pattern.IsMatch(comment))
                {
                    foreach (Match Matched in Pattern.Matches(comment))
                    {
                        foreach (Capture Captured in Matched.Groups["scr"].Captures)
                        {
                            if (!ScrList.Contains(Captured.Value))
                                ScrList.Add(Captured.Value);
                        }
                    }
                }
            }

            if (ScrList.Count == 0)
                return string.Empty;

            StringBuilder Builder = new StringBuilder();
            Builder.Append(ScrList[0]);
            for (int i = 1; i < ScrList.Count; ++i)
            {
                Builder.Append(",");
                Builder.Append(ScrList[i]);
            }
            return Builder.ToString();
        }
    }
}
