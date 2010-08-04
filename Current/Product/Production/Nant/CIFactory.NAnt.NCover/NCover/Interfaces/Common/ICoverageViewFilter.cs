namespace NCover.Interfaces.Common
{
    using NCover.Interfaces.Enumerations;
    using System;

    public interface ICoverageViewFilter : IEquatable<ICoverageViewFilter>
    {
        string FilterRuleData { get; }

        CoverageFilterType FilterType { get; }

        bool Include { get; }

        bool RegexRule { get; }

        FilterVersion Version { get; }
    }
}
