namespace NCover.Framework.Views
{
    using NCover.Framework.Scripting;
    using NCover.Interfaces.Common;
    using NCover.Interfaces.Enumerations;
    using System;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    [Serializable]
    public sealed class CoverageViewFilterBase : ICoverageViewFilter, IEquatable<ICoverageViewFilter>
    {
        public bool Equals(ICoverageViewFilter other)
        {
            return ((((other.FilterType == this.FilterType) && (other.Include == this.Include)) && (other.RegexRule == this.RegexRule)) && (other.FilterRuleData == this.FilterRuleData));
        }

        public static CoverageViewFilterBase Parse(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                throw new ArgumentNullException("data");
            }
            CoverageViewFilterBase base2 = new CoverageViewFilterBase();
            string[] strArray = data.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (strArray.Length < 1)
            {
                throw new ArgumentOutOfRangeException("data", "Splitting on ':' returned an empty list");
            }
            if (strArray.Length < 2)
            {
                throw new ArgumentOutOfRangeException("data", "The first two sections ('filterruledata' and 'filtertype') are required.");
            }
            if (strArray.Length > 0)
            {
                base2.FilterRuleData = strArray[0];
            }
            if (strArray.Length > 1)
            {
                base2.FilterType = (CoverageFilterType) Enum.Parse(typeof(CoverageFilterType), strArray[1]);
                if ((base2.FilterType == CoverageFilterType.None) || (base2.FilterType == CoverageFilterType.Threshold))
                {
                    throw new ArgumentOutOfRangeException("filtertype", "'None' and 'Threshold' are not allowed");
                }
            }
            if (strArray.Length > 2)
            {
                base2.RegexRule = bool.Parse(strArray[2]);
            }
            if (strArray.Length > 3)
            {
                base2.Include = bool.Parse(strArray[3]);
            }
            return base2;
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}:{2}:{3}:{4}", new object[] { this.FilterRuleData, this.FilterType.ToString(), this.Include, this.RegexRule, this.Version });
        }

        [XmlAttribute("filterruledata"), ScriptArgument("pattern", ScriptOutputType.NAntNCover), ScriptArgument("Pattern", ScriptOutputType.MSBuildNCover)]
        public string FilterRuleData { get; set; }

        [ScriptArgument("Type", ScriptOutputType.MSBuildNCover), ScriptArgument("type", ScriptOutputType.NAntNCover), XmlAttribute("filtertype")]
        public CoverageFilterType FilterType { get; set; }

        [XmlAttribute("include"), ScriptArgument("IsInclude", ScriptOutputType.MSBuildNCover), ScriptArgument("isInclude", ScriptOutputType.NAntNCover)]
        public bool Include { get; set; }

        [XmlAttribute("regexrule"), ScriptArgument("IsRegex", ScriptOutputType.MSBuildNCover), ScriptArgument("isRegex", ScriptOutputType.NAntNCover)]
        public bool RegexRule { get; set; }

        [XmlAttribute("version")]
        public FilterVersion Version { get; set; }
    }
}
