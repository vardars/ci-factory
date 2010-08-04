namespace NCover.Interfaces.Reporting
{
    using NCover.Interfaces.Enumerations;
    using System;
    using System.Text.RegularExpressions;

    public interface ICoverageThreshold : IEquatable<ICoverageThreshold>
    {
        bool AppliesToAllOfType { get; }

        Regex CompiledPattern { get; }

        CoverageMetric CoverageType { get; set; }

        CoverageViewElementType ElementType { get; set; }

        bool IsMaxThreshold { get; set; }

        string Pattern { get; set; }

        float Value { get; set; }
    }
}
