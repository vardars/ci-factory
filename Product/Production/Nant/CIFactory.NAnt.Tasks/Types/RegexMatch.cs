using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.IO;
using NAnt.Core.Attributes;

namespace CIFactory.NAnt.Types
{
    [ElementName("regexmatch")]
    public class RegexMatch : LoopItems
    {
        #region Fields

        private FileInfo _File;

        private string _Pattern;

        #endregion

        #region Properties

        [TaskAttribute("file", Required = true)]
        public FileInfo File
        {
            get { return _File; }
            set { _File = value; }
        }

        [TaskAttribute("pattern", Required = false)]
        public string Pattern
        {
            get { return _Pattern; }
            set { _Pattern = value; }
        }

        #endregion

        #region Protected Methods

        protected override IEnumerator GetStrings()
        {
            LineEnumerator Enumerator = new LineEnumerator(this.File.OpenText(), new Regex(this.Pattern));
            return Enumerator;
        }

        #endregion

    }
}