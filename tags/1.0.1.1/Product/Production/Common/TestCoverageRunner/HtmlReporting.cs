using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Reflection;
using TestCoverage;

namespace TestCoverageRunner
{
    public class HtmlReporting : IReporting
    {

        #region Fields

        private StringCollection _TestAssemblies;
        private StringCollection _ProductionAssemblies;
        private Stream _ReportStream;
        private string _ProjectName;

        #endregion

        #region Properties

        public string ProjectName
        {
            get
            {
                return _ProjectName;
            }
            set
            {
                _ProjectName = value;
            }
        }

        public StringCollection TestAssemblies
        {
            get
            {
                if (_TestAssemblies == null)
                    _TestAssemblies = new StringCollection();
                return _TestAssemblies;
            }
            set
            {
                _TestAssemblies = value;
            }
        }
        public StringCollection ProductionAssemblies
        {
            get
            {
                if (_ProductionAssemblies == null)
                    _ProductionAssemblies = new StringCollection();
                return _ProductionAssemblies;
            }
            set
            {
                _ProductionAssemblies = value;
            }
        }
        public Stream ReportStream
        {
            get
            {
                return _ReportStream;
            }
            set
            {
                _ReportStream = value;
            }
        }

        #endregion
       
#region Constructors

        public HtmlReporting()
        {

        }

        public HtmlReporting(StringCollection testAssemblies, StringCollection productionAssemblies, Stream reportStream, string projectName)
        {
            _TestAssemblies = testAssemblies;
            _ProductionAssemblies = productionAssemblies;
            _ReportStream = reportStream;
            _ProjectName = projectName;
        }

#endregion

        private Stream GetXslStream()
        {
            Assembly ThisAssembly = this.GetType().Assembly;
            Stream XslStream = ThisAssembly.GetManifestResourceStream(this.GetType(), "CoverageDetail.xsl");
            return XslStream;
        }

        public void GenerateReport()
        {
            MemoryStream RawXmlStream = new MemoryStream();
            XmlReporting XmlReporter = new XmlReporting(this.TestAssemblies, this.ProductionAssemblies, RawXmlStream, this.ProjectName);
            
            XmlReporter.GenerateReport();
            RawXmlStream.Position = 0;
            XPathDocument XPathReportReader = new XPathDocument(RawXmlStream);

            XmlReader XslReader = new XmlTextReader(this.GetXslStream());


            XslTransform XslTransformer = new XslTransform();
            XslTransformer.Load(XslReader);
            XslTransformer.Transform(XPathReportReader, null, this.ReportStream, null);
        }

        public void Test()
        {
            try
            {
                this.ReportStream = new FileStream(@"C:\temp\Report.html", FileMode.Create);
                this.ProjectName = "Test";
                this.TestAssemblies.Add(@"C:\Projects\EF_COTS\1.6.1\Product\Unit Test\Test.EF.Pluggin\bin\Test.EF.Pluggin.dll");
                this.GenerateReport();
                this.ReportStream.Close();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
