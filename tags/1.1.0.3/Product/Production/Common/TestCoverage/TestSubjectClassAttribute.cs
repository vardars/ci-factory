using System;
using System.Collections;
using System.Text;

namespace TestCoverage
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    public class TestSubjectClassAttribute : Attribute
    {
        private Type _TestSubject;

        public Type TestSubject
        {
            get
            {
                return _TestSubject;
            }
            set
            {
                _TestSubject = value;
            }
        }

        public TestSubjectClassAttribute(Type testSubject)
        {
            _TestSubject = testSubject;
        }

        public TestSubjectClassAttribute()
        {
            
        }
    }
}
