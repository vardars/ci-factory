namespace NCover.NAntTasks.Types
{
    using NAnt.Core;
    using NAnt.Core.Attributes;
    using NCover.Framework.Reports;
    using NCover.Interfaces.Enumerations;
    using NCover.Interfaces.Extensions;
    using System;

    [ElementName("threshold")]
    public class CoverageThresholdElement : Element
    {
        private CoverageThreshold _backingField = new CoverageThreshold();

        [StringValidator(AllowEmpty=false), TaskAttribute("coverageMetric", Required=true)]
        public string CoverageType
        {
            get
            {
                return this._backingField.CoverageType.ToString();
            }
            set
            {
                CoverageMetric metric;
                if (!ParseExtensions.TryParseEnum<CoverageMetric>(value, out metric))
                {
                    throw new ArgumentOutOfRangeException("value", value, "Not a valid Coverage Metric.");
                }
                this._backingField.CoverageType = metric;
            }
        }

        [TaskAttribute("type"), StringValidator(AllowEmpty=false)]
        public string ElementType
        {
            get
            {
                return this._backingField.ElementType.ToString();
            }
            set
            {
                CoverageViewElementType type;
                if (!ParseExtensions.TryParseEnum<CoverageViewElementType>(value, out type))
                {
                    throw new ArgumentOutOfRangeException("value", value, "Not a valid Node Type.");
                }
                this._backingField.ElementType = type;
            }
        }

        [TaskAttribute("isMax")]
        public bool IsMax
        {
            get
            {
                return this._backingField.IsMaxThreshold;
            }
            set
            {
                this._backingField.IsMaxThreshold = value;
            }
        }

        [TaskAttribute("pattern"), StringValidator(AllowEmpty=false)]
        public string Pattern
        {
            get
            {
                return this._backingField.Pattern;
            }
            set
            {
                this._backingField.Pattern = value;
            }
        }

        public CoverageThreshold Threshold
        {
            get
            {
                return this._backingField;
            }
        }

        [TaskAttribute("value")]
        public float Value
        {
            get
            {
                return this._backingField.Value;
            }
            set
            {
                this._backingField.Value = value;
            }
        }
    }
}
