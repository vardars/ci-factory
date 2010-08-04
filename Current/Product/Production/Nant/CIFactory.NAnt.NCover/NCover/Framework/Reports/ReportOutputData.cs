using NCover.Interfaces;

namespace NCover.Framework.Reports
{
    using NCover.Interfaces.Enumerations;
    using NCover.Interfaces.Reporting;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Xml.Serialization;

    public class ReportOutputData : IReportOutputData
    {
        public ReportOutputData()
        {
            this.Format = ReportOutputFormats.Html;
            this.ReportType = CoverageReportType.None;
            this.XmlParams = new List<NameValuePair>();
        }

        public bool ParamExists(string key)
        {
            if (this.Params == null)
            {
                return false;
            }
            return (this.Params.FirstOrDefault<INameValuePair>(s => s.Name.Equals(key, StringComparison.InvariantCultureIgnoreCase)) != null);
        }

        public string ParamValue(string key)
        {
            if (this.Params != null)
            {
                INameValuePair pair = this.Params.FirstOrDefault<INameValuePair>(s => s.Name.Equals(key, StringComparison.InvariantCultureIgnoreCase));
                if (pair != null)
                {
                    return pair.Value;
                }
            }
            return null;
        }

        public override string ToString()
        {
            return (this.ReportType.ToString() + ":" + this.Format.ToString() + ":" + (string.IsNullOrEmpty(this.OutputPath) ? "" : this.OutputPath));
        }

        public ReportOutputFormats Format { get; set; }

        public string OutputPath { get; set; }

        [XmlIgnore]
        public IEnumerable<INameValuePair> Params
        {
            get
            {
                if (this.XmlParams == null)
                {
                    return null;
                }
                return this.XmlParams.Cast<INameValuePair>();
            }
        }

        public CoverageReportType ReportType { get; set; }

        [XmlArray("params"), XmlArrayItem("param")]
        public List<NameValuePair> XmlParams { get; set; }
    }
}
