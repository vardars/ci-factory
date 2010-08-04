namespace NCover.Framework.Reports
{
    using NCover.Framework.Scripting;
    using NCover.Interfaces.Enumerations;
    using NCover.Interfaces.Extensions;
    using NCover.Interfaces.Reporting;
    using System;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;
    using System.Xml.Serialization;

    public class CoverageThreshold : ICoverageThreshold, IEquatable<ICoverageThreshold>
    {
        private Regex _internalRegex;

        public CoverageThreshold() : this(CoverageViewElementType.View, CoverageMetric.SequencePoints, 95f)
        {
        }

        public CoverageThreshold(CoverageViewElementType type, CoverageMetric stat, float statValue)
        {
            if (statValue < 0f)
            {
                throw new ArgumentOutOfRangeException("statValue", statValue, "Parameter 'statValue' must be greater than or equal to 0.0 Was: " + statValue.ToString());
            }
            if ((stat != CoverageMetric.CyclomaticComplexity) && (statValue > 100f))
            {
                throw new ArgumentOutOfRangeException("statValue", statValue, "Parameter 'statValue' must be less than or equal to 100.0 Was: " + statValue.ToString());
            }
            if (((type == CoverageViewElementType.Unknown) || (type == CoverageViewElementType.InstrumentedPoint)) || ((type == CoverageViewElementType.Method) || (type == CoverageViewElementType.Property)))
            {
                throw new ArgumentOutOfRangeException("type", type, "Parameter 'type' had an invalid value: " + type.ToString());
            }
            this.Value = statValue;
            this.ElementType = type;
            this.CoverageType = stat;
        }

        public CoverageThreshold(CoverageViewElementType type, CoverageMetric stat, float statValue, string name) : this(type, stat, statValue)
        {
            if ((name != null) && (name.Trim() == "*"))
            {
                name = string.Empty;
            }
            this.Pattern = name;
        }

        public bool Equals(ICoverageThreshold other)
        {
            return ((((other.ElementType == this.ElementType) && (other.CoverageType == this.CoverageType)) && ((other.IsMaxThreshold == this.IsMaxThreshold) && other.Value.Within(0.0001f, other.Value))) && (other.Pattern == this.Pattern));
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as ICoverageThreshold);
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public static CoverageThreshold Parse(string threshold)
        {
            if (string.IsNullOrEmpty(threshold))
            {
                throw new ArgumentNullException("threshold");
            }
            string[] strArray = threshold.Split(new char[] { ':' }, StringSplitOptions.None);
            CoverageThreshold threshold2 = new CoverageThreshold();
            if (strArray.Length > 0)
            {
                threshold2.CoverageType = (CoverageMetric) Enum.Parse(typeof(CoverageMetric), strArray[0]);
                if (strArray.Length <= 1)
                {
                    return threshold2;
                }
                float actualValue = float.Parse(strArray[1]);
                if (actualValue < 0f)
                {
                    throw new ArgumentOutOfRangeException("statValue", actualValue, "Parameter 'statValue' must be greater than or equal to 0.0 Was: " + actualValue.ToString());
                }
                if ((threshold2.CoverageType != CoverageMetric.CyclomaticComplexity) && (actualValue > 100f))
                {
                    throw new ArgumentOutOfRangeException("statValue", actualValue, "Parameter 'statValue' must be less than or equal to 100.0 Was: " + actualValue.ToString());
                }
                threshold2.Value = actualValue;
                if (strArray.Length <= 2)
                {
                    return threshold2;
                }
                CoverageViewElementType type = (CoverageViewElementType) Enum.Parse(typeof(CoverageViewElementType), strArray[2]);
                switch (type)
                {
                    case CoverageViewElementType.Unknown:
                    case CoverageViewElementType.InstrumentedPoint:
                    case CoverageViewElementType.Method:
                    case CoverageViewElementType.Property:
                        throw new ArgumentOutOfRangeException("type", type, "Parameter 'type' had an invalid value: " + type.ToString());
                }
                threshold2.ElementType = type;
                if (strArray.Length > 3)
                {
                    string pattern = strArray[3].Trim(new char[] { '\'', '"' });
                    new Regex(pattern);
                    threshold2.Pattern = pattern;
                    if (strArray.Length > 4)
                    {
                        threshold2.IsMaxThreshold = !string.IsNullOrEmpty(strArray[4]) && strArray[4].Equals("true", StringComparison.InvariantCultureIgnoreCase);
                    }
                }
            }
            return threshold2;
        }

        bool IEquatable<ICoverageThreshold>.Equals(ICoverageThreshold other)
        {
            if (other == null)
            {
                return false;
            }
            return (object.ReferenceEquals(this, other) || ((((this.CoverageType == other.CoverageType) && (this.ElementType == other.ElementType)) && ((this.IsMaxThreshold == other.IsMaxThreshold) && this.Pattern.CultureAgnosticEquals(other.Pattern))) && this.Value.Within(0.001f, other.Value)));
        }

        public override string ToString()
        {
            return (this.CoverageType.ToString() + ":" + this.Value.ToString() + ":" + this.ElementType.ToString() + ":" + this.IsMaxThreshold.ToString() + ":" + (string.IsNullOrEmpty(this.Pattern) ? ":" : this.Pattern.ToString()));
        }

        [XmlIgnore]
        public bool AppliesToAllOfType
        {
            get
            {
                return string.IsNullOrEmpty(this.Pattern);
            }
        }

        [XmlIgnore]
        public Regex CompiledPattern
        {
            get
            {
                if (this._internalRegex == null)
                {
                    this._internalRegex = new Regex(this.Pattern);
                }
                return this._internalRegex;
            }
        }

        [ScriptArgument("CoverageMetric", ScriptOutputType.MSBuildNCover), XmlElement("CoverageMetric"), ScriptArgument("coverageMetric", ScriptOutputType.NAntNCover)]
        public CoverageMetric CoverageType { get; set; }

        [ScriptArgument("Type", ScriptOutputType.MSBuildNCover), ScriptArgument("type", ScriptOutputType.NAntNCover), XmlElement("Type")]
        public CoverageViewElementType ElementType { get; set; }

        [XmlElement("IsMax"), ScriptArgument("isMax", ScriptOutputType.NAntNCover), ScriptArgument("IsMax", ScriptOutputType.MSBuildNCover)]
        public bool IsMaxThreshold { get; set; }

        [XmlElement("Pattern"), ScriptArgument("Pattern", ScriptOutputType.MSBuildNCover), ScriptArgument("pattern", ScriptOutputType.NAntNCover)]
        public string Pattern { get; set; }

        [ScriptArgument("Value", ScriptOutputType.MSBuildNCover), ScriptArgument("value", ScriptOutputType.NAntNCover), XmlElement("Value")]
        public float Value { get; set; }
    }
}
