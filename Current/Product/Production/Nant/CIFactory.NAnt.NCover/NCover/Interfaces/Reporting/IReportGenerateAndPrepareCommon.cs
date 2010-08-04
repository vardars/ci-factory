namespace NCover.Interfaces.Reporting
{
    using System.Collections.Generic;

    public interface IReportGenerateAndPrepareCommon
    {
        IEnumerable<ICoverageThreshold> SatisfactoryThresholds { get; }
    }
}
