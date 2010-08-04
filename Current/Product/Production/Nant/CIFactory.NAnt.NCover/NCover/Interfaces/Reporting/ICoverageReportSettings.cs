namespace NCover.Interfaces.Reporting
{
    using System;
    using System.IO;

    public interface ICoverageReportSettings : IReportPrepareSettings, IReportGenerateSettings, IReportGenerateAndPrepareCommon
    {
        void SaveToFile(string fileName);
        void SaveToStream(Stream stream);

        string FileNameLoadedFrom { get; set; }
    }
}
