using System;
using System.Collections;
using System.Text;

namespace TestCoverage
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = true)]
    public class TestSubjectAssemblyAttribute : Attribute
    {
        private string _Location;

        public string Location
        {
            get
            {
                return _Location;
            }
            set
            {
                _Location = value;
            }
        }

        public TestSubjectAssemblyAttribute(string location)
        {
            _Location = location;
        }

        public TestSubjectAssemblyAttribute()
        {
            
        }
    }
}
