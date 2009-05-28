using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.IO;
using NAnt.Core.Attributes;
using System.Collections.Generic;

namespace CIFactory.NAnt.Types
{
    [ElementName("propertystructureiterator")]
    public class PropertyStructureIterator : LoopItems
    {

        private string _Pattern;

        [TaskAttribute("pattern")]
        public string Pattern
        {
            get { return _Pattern; }
            set
            {
                _Pattern = value;
            }
        }


        protected override IEnumerator GetStrings()
        {
            List<String> Keys = new List<string>();

            Regex regex = new Regex(this.Pattern);

            foreach (String Key in this.Properties.Keys)
            {
                if (regex.IsMatch(Key))
                    Keys.Add(Key);
            }

            return Keys.GetEnumerator();
        }
    }
}
