using System;
using System.Collections;
using System.Text;

namespace TestCoverageRunner
{
    public class FixtureTable : DictionaryBase
    {
        public TestFixture this[string className]
        {
            get
            {
                return (TestFixture)this.Dictionary[className];
            }
            set
            {
                this.Dictionary[className] = value;
            }
        }

        public void Add(string className, TestFixture fixture)
        {
            this.Dictionary.Add(className, fixture);
        }

        public bool Contains(string className)
        {
            return this.Dictionary.Contains(className);
        }

        public void Remove(string className)
        {
            this.Dictionary.Remove(className);
        }
    }
}
