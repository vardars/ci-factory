namespace NCover.Framework.Scripting
{
    using NCover.Framework;
    using NCover.Interfaces.Reporting;
    using System;
    using System.IO;

    public static class NCoverReportingSettingsFileWriter
    {
        private static void _BuildReportSettingsFile(ICoverageReportSettings consoleSetting, string settingsFileName)
        {
            consoleSetting.SaveToFile(settingsFileName);
        }

        public static void BuildTempSettingsXmlFile(ICoverageReportSettings consoleSetting, string settingsFileName)
        {
            _BuildReportSettingsFile(consoleSetting, settingsFileName);
        }

        public static string CreateSettingsFile(ICoverageReportSettings consoleSetting)
        {
            string tempFileName = FileUtilities.GetTempFileName(".config");
            BuildTempSettingsXmlFile(consoleSetting, tempFileName);
            return tempFileName;
        }

        public static string GetSettingsFileContent(ICoverageReportSettings consoleSetting)
        {
            string path = CreateSettingsFile(consoleSetting);
            string str2 = File.ReadAllText(path);
            File.Delete(path);
            return str2;
        }
    }
}
