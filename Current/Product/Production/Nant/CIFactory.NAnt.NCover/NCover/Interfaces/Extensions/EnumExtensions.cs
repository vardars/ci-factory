namespace NCover.Interfaces.Extensions
{
    using NCover.Interfaces.Enumerations;
    using System;
    using System.Runtime.CompilerServices;

    public static class EnumExtensions
    {
        public static bool ValueIncluded(this CoverageFilterType c, CoverageFilterType t)
        {
            return ((c & t) == t);
        }

        public static bool ValueIncluded(this CoverageReportType c, CoverageReportType t)
        {
            return ((c & t) == t);
        }

        public static bool ValueIncluded(this CoverageType c, CoverageType t)
        {
            return ((c & t) == t);
        }

        public static bool ValueIncluded(this NCoverFeature c, NCoverFeature t)
        {
            return ((c & t) == t);
        }

        public static bool ValueIncluded(this SymbolSearchPolicy c, SymbolSearchPolicy t)
        {
            return ((c & t) == t);
        }
    }
}
