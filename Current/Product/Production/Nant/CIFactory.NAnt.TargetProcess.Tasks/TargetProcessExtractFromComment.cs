using System;
using System.Collections.Generic;
using System.Text;
using NAnt.Core;
using NAnt.Core.Tasks;
using NAnt.Core.Attributes;
using NAnt.Core.Util;
using CIFactory.NAnt.Types;
using System.Text.RegularExpressions;

namespace CIFactory.TargetProcess.NAnt
{
    [TaskName("targetprocessextractfromcomment")]
    public class TargetProcessExtractFromComment : Task
    {
        private const string CAPTURE = "num";

        #region Fields

        private string _Comment;

        private string _EntityPrefix;

        private String _ListRefID;

        private List<Regex> _Patterns;

        #endregion

        #region Properties

        [TaskAttribute("comment", Required = true)]
        public string Comment
        {
            get
            {
                return _Comment;
            }
            set
            {
                _Comment = value;
            }
        }

        [TaskAttribute("entityprefix", Required = true)]
        public string EntityPrefix
        {
            get { return _EntityPrefix; }
            set
            {
                _EntityPrefix = value;
            }
        }

        [TaskAttribute("listrefid", Required = true)]
        public String ListRefID
        {
            get
            {
                return _ListRefID;
            }
            set
            {
                _ListRefID = value;
            }
        }

        public List<Regex> Patterns
        {
            get
            {
                if (_Patterns == null)
                {
                    _Patterns = new List<Regex>();
                    _Patterns.Add(new Regex(this.EntityPrefix + @"{1,1}\s{0,}\#{0,}\s{0,}(?<" + CAPTURE + @">\d{4,})", RegexOptions.IgnoreCase | RegexOptions.Multiline));
                }
                return _Patterns;
            }
        }

        #endregion

        #region Public Methods

        public void GetEntityNumbers(string comment, StringList RefStringList)
        {
            foreach (Regex Pattern in this.Patterns)
            {
                if (Pattern.IsMatch(comment))
                {
                    foreach (Match Matched in Pattern.Matches(comment))
                    {
                        foreach (Capture Captured in Matched.Groups[CAPTURE].Captures)
                        {
                            if (!RefStringList.StringItems.Contains(Captured.Value))
                                RefStringList.StringItems.Add(Captured.Value, new StringItem(Captured.Value));
                        }
                    }
                }
            }
        }

        #endregion

        #region Protected Methods

        protected override void ExecuteTask()
        {
            if (!this.Project.DataTypeReferences.Contains(ListRefID))
                throw new BuildException(String.Format("The refid {0} is not defined.", ListRefID));

            StringList RefStringList = (StringList)this.Project.DataTypeReferences[ListRefID];

            this.GetEntityNumbers(Comment, RefStringList);
        }

        #endregion

    }
}
