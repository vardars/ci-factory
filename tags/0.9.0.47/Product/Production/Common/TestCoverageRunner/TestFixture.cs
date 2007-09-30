using System;
using System.Collections;
using System.Text;

namespace TestCoverageRunner
{
    public class TestFixture : DictionaryBase
    {

        private string _FullName;

        public string FullName
        {
            get
            {
                return _FullName;
            }
            set
            {
                _FullName = value;
            }
        }

        public string this[string memberName]
        {
            get
            {
                return (string)this.Dictionary[memberName];
            }
            set
            {
                this.Dictionary[memberName] = value;
            }
        }

        public void Add(string memberName, string testMethodName)
        {
            this.Dictionary.Add(memberName, testMethodName);
        }

        public bool Contains(string memberName)
        {
            return this.Dictionary.Contains(memberName);
        }

        public void Remove(string memberName)
        {
            this.Dictionary.Remove(memberName);
        }


        /// <summary>
        /// Creates a new instance of TestFixture
        /// </summary>
        /// <param name="fullName"></param>
        public TestFixture(string fullName)
        {
            _FullName = fullName;
        }
    }
}
