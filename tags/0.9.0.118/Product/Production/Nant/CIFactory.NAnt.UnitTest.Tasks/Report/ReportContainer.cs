using System;
using System.Collections.ObjectModel;
using System.Text;
using CIFactory.NAnt.Types;
using NAnt.Core;
using NAnt.Core.Tasks;
using NAnt.Core.Attributes;
using NAnt.Core.Types;
using MbUnit;
using MbUnit.Framework;
using System.Runtime.Serialization;

namespace CIFactory.NAnt.UnitTest.Report
{
    [Serializable]
    public class ReportContainer
    {

        private Collection<FixtureReport> _FixtureReports;
        public Collection<FixtureReport> FixtureReports
        {
            get 
            {
                if (_FixtureReports == null)
                    _FixtureReports = new Collection<FixtureReport>();
                return _FixtureReports; 
            }
            set
            {
                _FixtureReports = value;
            }
        }
    }

    [Serializable]
    public class FixtureReport
    {
        private string _Name;
        [System.Xml.Serialization.XmlAttribute()]
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
            }
        }

        private Collection<TestReport> _TestReports;
        public Collection<TestReport> TestReports
        {
            get 
            {
                if (_TestReports == null)
                    _TestReports = new Collection<TestReport>();
                return _TestReports; 
            }
            set
            {
                _TestReports = value;
            }
        }

        protected FixtureReport(SerializationInfo info, StreamingContext context)
        {
            _Name = info.GetString("_Name");
            _TestReports = (Collection<TestReport>)info.GetValue("_TestReports", typeof(Collection<TestReport>));
        }
        /// <summary>
        /// Initializes a new instance of the FixtureReport class.
        /// </summary>
        /// <param name="name"></param>
        public FixtureReport(string name)
        {
            _Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the FixtureReport class.
        /// </summary>
        public FixtureReport()
        {
        }
    }

    public class TestReport
    {
        private string _Name;
        [System.Xml.Serialization.XmlAttribute()]
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
            }
        }

        private bool _Passed;

        [System.Xml.Serialization.XmlAttribute()]
        public bool Passed
        {
            get { return _Passed; }
            set
            {
                _Passed = value;
            }
        }

        private String _FailureException;
        public String FailureException
        {
            get { return _FailureException; }
            set
            {
                _FailureException = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the TestReport class.
        /// </summary>
        /// <param name="name"></param>
        public TestReport(string name)
        {
            _Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the TestReport class.
        /// </summary>
        public TestReport()
        {
        }
    }
}
