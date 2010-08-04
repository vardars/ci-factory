namespace NCover.Interfaces.Enumerations
{
    using NCover.Interfaces;
    using System;

    public enum ViewReportStyle
    {
        [ForNCoverFeature(NCoverFeature.CoverageForComplete), CoverageTypeMapping(CoverageMetric.SequencePoints)]
        All = 11,
        [CoverageTypeMapping(CoverageMetric.BranchPoints), ForNCoverFeature(NCoverFeature.AlwaysEnabled | NCoverFeature.CoverageBranch)]
        BranchPointCoveragePercentage = 8,
        [ForNCoverFeature(NCoverFeature.AlwaysEnabled | NCoverFeature.CoverageBranch), CoverageTypeMapping(CoverageMetric.BranchPoints)]
        BranchPointCoveragePercentageAndUnvisitedPoints = 9,
        [ForNCoverFeature(NCoverFeature.AlwaysEnabled | NCoverFeature.CoverageBranch), CoverageTypeMapping(CoverageMetric.BranchPoints)]
        BranchPointCoverageUnvisitedBranchPoints = 10,
        [ForNCoverFeature(NCoverFeature.AlwaysEnabled | NCoverFeature.CoverageComplexity), CoverageTypeMapping(CoverageMetric.CyclomaticComplexity)]
        CyclomaticComplexity = 7,
        [CoverageTypeMapping(CoverageMetric.MethodCoverage), ForNCoverFeature(NCoverFeature.AlwaysEnabled | NCoverFeature.CoverageMethodVisits)]
        FunctionCoverage = 3,
        [CoverageTypeMapping(CoverageMetric.MethodCoverage), ForNCoverFeature(NCoverFeature.AlwaysEnabled | NCoverFeature.CoverageMethodVisits)]
        FunctionCoveragePercentage = 4,
        [ForNCoverFeature(NCoverFeature.AlwaysEnabled | NCoverFeature.CoverageMethodVisits), CoverageTypeMapping(CoverageMetric.MethodCoverage)]
        FunctionCoveragePercentageUnvisited = 5,
        [ForNCoverFeature(NCoverFeature.AlwaysEnabled | NCoverFeature.CoverageMethodVisits), CoverageTypeMapping(CoverageMetric.MethodCoverage)]
        FunctionCoverageUnvisited = 6,
        [ForNCoverFeature(NCoverFeature.CoverageForClassic), CoverageTypeMapping(CoverageMetric.SequencePoints)]
        SequencePointCoveragePercentage = 0,
        [CoverageTypeMapping(CoverageMetric.SequencePoints), ForNCoverFeature(NCoverFeature.CoverageForClassic)]
        SequencePointCoveragePercentageAndUnvisitedSeqPts = 1,
        [ForNCoverFeature(NCoverFeature.CoverageForClassic), CoverageTypeMapping(CoverageMetric.SequencePoints)]
        SequencePointCoverageUnvisitedSequencePoints = 2,
        [CoverageTypeMapping(CoverageMetric.SequencePoints), ForNCoverFeature(NCoverFeature.CoverageForClassic | NCoverFeature.CoverageComplexity)]
        SymbolCoverageAndCyclomaticComplexity = 12
    }
}
