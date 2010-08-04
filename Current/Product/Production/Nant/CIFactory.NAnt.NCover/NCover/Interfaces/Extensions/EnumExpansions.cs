namespace NCover.Interfaces.Extensions
{
    using NCover.Interfaces.Enumerations;
    using System;

    public static class EnumExpansions
    {
        public static CoverageFilterType ExpandFilter(CoverageFilterType filter)
        {
            CoverageFilterType type = filter & CoverageFilterType.Document;
            if ((filter & CoverageFilterType.Method) == CoverageFilterType.Method)
            {
                return ((((type | CoverageFilterType.Assembly) | CoverageFilterType.Namespace) | CoverageFilterType.Class) | CoverageFilterType.Method);
            }
            if ((filter & CoverageFilterType.Class) == CoverageFilterType.Class)
            {
                return (((type | CoverageFilterType.Assembly) | CoverageFilterType.Namespace) | CoverageFilterType.Class);
            }
            if ((filter & CoverageFilterType.Namespace) == CoverageFilterType.Namespace)
            {
                return ((type | CoverageFilterType.Assembly) | CoverageFilterType.Namespace);
            }
            return filter;
        }
    }
}
