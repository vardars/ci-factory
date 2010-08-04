namespace NCover.Framework.Scripting
{
    using NCover.Interfaces.Enumerations;
    using System;

    internal class ConvertReportOutputFormatEnumAttribute : ConvertValueAttribute
    {
        private static char[] _splits = new char[] { ',', ';', '|' };

        public ConvertReportOutputFormatEnumAttribute(Type throwaway)
        {
        }

        public override object ConvertBack(string fromVal)
        {
            if (fromVal == null)
            {
                throw new ArgumentNullException("fromVal");
            }
            if (string.IsNullOrEmpty(fromVal))
            {
                throw new ArgumentOutOfRangeException("fromVal", "Expected a non-empty string");
            }
            string[] strArray = fromVal.Split(_splits, StringSplitOptions.RemoveEmptyEntries);
            if (strArray.Length <= 0)
            {
                throw new ArgumentOutOfRangeException("fromVal", "Unable to find elements of the CoverageType enum in '" + fromVal + "'");
            }
            ReportOutputFormats formats = (ReportOutputFormats) Enum.Parse(typeof(ReportOutputFormats), strArray[0]);
            for (int i = 1; i < strArray.Length; i++)
            {
                formats |= (ReportOutputFormats) Enum.Parse(typeof(ReportOutputFormats), strArray[i].Trim());
            }
            return formats;
        }

        public override string ConvertFrom(object fromVal)
        {
            if (fromVal == null)
            {
                throw new ArgumentNullException("fromVal");
            }
            return fromVal.ToString();
        }
    }
}
