using System;
using System.Collections;
using System.Text;

namespace TestCoverage
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class TestSubjectMemberAttribute : Attribute
    {
        private string _MemeberName;

        public string MemeberName
        {
            get
            {
                return _MemeberName;
            }
            set
            {
                _MemeberName = value;
            }
        }

        public TestSubjectMemberAttribute(string memeberName)
        {
            _MemeberName = memeberName;
        }

        public TestSubjectMemberAttribute()
        {
            
        }
    }
}
