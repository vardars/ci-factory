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

        private string _StructureName;

        [TaskAttribute("name")]
        public string StructureName
        {
            get { return _StructureName; }
            set
            {
                _StructureName = value;
            }
        }


        protected override IEnumerator GetStrings()
        {
            List<String> Keys = new List<string>();

            foreach (String Key in this.Properties.Keys)
            {
                if (Key.StartsWith(this.StructureName))
                    Keys.Add(Key);
            }

            return Keys.GetEnumerator();
        }
    }
}
