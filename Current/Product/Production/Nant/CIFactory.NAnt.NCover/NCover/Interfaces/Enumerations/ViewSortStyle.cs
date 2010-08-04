namespace NCover.Interfaces.Enumerations
{
    using NCover.Interfaces;
    using System;

    public enum ViewSortStyle
    {
        ClassLine = 1,
        [ForNCoverFeature(NCoverFeature.CoverageForClassic)]
        CoveragePercentageAscending = 2,
        [ForNCoverFeature(NCoverFeature.CoverageForClassic)]
        CoveragePercentageDescending = 3,
        [ForNCoverFeature(NCoverFeature.AlwaysEnabled | NCoverFeature.CoverageMethodVisits)]
        FunctionCoverageAscending = 8,
        [ForNCoverFeature(NCoverFeature.AlwaysEnabled | NCoverFeature.CoverageMethodVisits)]
        FunctionCoverageDescending = 9,
        Name = 0,
        [ForNCoverFeature(NCoverFeature.CoverageForClassic)]
        UnvisitedSequencePointsAscending = 4,
        [ForNCoverFeature(NCoverFeature.CoverageForClassic)]
        UnvisitedSequencePointsDescending = 5,
        [ForNCoverFeature(NCoverFeature.AlwaysEnabled | NCoverFeature.CoverageMethodVisits)]
        VisitCountAscending = 6,
        [ForNCoverFeature(NCoverFeature.AlwaysEnabled | NCoverFeature.CoverageMethodVisits)]
        VisitCountDescending = 7
    }
}
