namespace NCover.Interfaces.Enumerations
{
    using System;

    public enum CoverageMetric
    {
        BranchCoverage = 1,
        BranchPoints = 1,
        CyclomaticComplexity = 5,
        DecisionCoverage = 2,
        LineCoverage = 4,
        MethodCoverage = 3,
        SequencePoints = 0,
        SymbolCoverage = 0
    }
}
