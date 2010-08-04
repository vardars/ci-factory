namespace NCover.NAntTasks.Types
{
    using NAnt.Core;
    using NAnt.Core.Attributes;
    using NCover.Framework.Views;
    using NCover.Interfaces.Enumerations;
    using NCover.Interfaces.Extensions;
    using System;

    [ElementName("filter")]
    public class FilterElement : Element
    {
        private CoverageViewFilterBase _backingField = new CoverageViewFilterBase();

        public CoverageViewFilterBase Filter
        {
            get
            {
                return this._backingField;
            }
        }

        [TaskAttribute("pattern", Required=true), StringValidator(AllowEmpty=false)]
        public string FilterRuleData
        {
            get
            {
                return this._backingField.FilterRuleData;
            }
            set
            {
                this._backingField.FilterRuleData = value;
            }
        }

        [StringValidator(AllowEmpty=false), TaskAttribute("type", Required=true)]
        public string FilterType
        {
            get
            {
                return this._backingField.FilterType.ToString();
            }
            set
            {
                CoverageFilterType type;
                if (!ParseExtensions.TryParseEnum<CoverageFilterType>(value, out type))
                {
                    throw new ArgumentOutOfRangeException("value", value, "Invalid value for the Filter Type.");
                }
                this._backingField.FilterType = type;
            }
        }

        [TaskAttribute("isInclude"), BooleanValidator]
        public bool Include
        {
            get
            {
                return this._backingField.Include;
            }
            set
            {
                this._backingField.Include = value;
            }
        }

        [TaskAttribute("isRegex"), BooleanValidator]
        public bool RegexRule
        {
            get
            {
                return this._backingField.RegexRule;
            }
            set
            {
                this._backingField.RegexRule = value;
            }
        }
    }
}
