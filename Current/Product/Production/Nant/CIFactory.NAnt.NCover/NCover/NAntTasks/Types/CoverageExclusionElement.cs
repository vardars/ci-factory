namespace NCover.NAntTasks.Types
{
    using NAnt.Core;
    using NAnt.Core.Attributes;
    using System;

    [ElementName("exclusion"), Obsolete]
    public class CoverageExclusionElement : Element
    {
        private bool _enabled;
        private string _exclusionType;
        private bool _isRegex;
        private string _pattern;

        public CoverageExclusionElement() : this(null, null, false)
        {
        }

        public CoverageExclusionElement(string exclusionType, string pattern, bool isRegex)
        {
            this._exclusionType = exclusionType;
            this._pattern = pattern;
            this._isRegex = isRegex;
            this._enabled = true;
        }

        [BooleanValidator, TaskAttribute("enabled")]
        public bool Enabled
        {
            get
            {
                return this._enabled;
            }
            set
            {
                this._enabled = value;
            }
        }

        [TaskAttribute("type")]
        public string ExclusionType
        {
            get
            {
                return this._exclusionType;
            }
            set
            {
                this._exclusionType = value;
            }
        }

        [BooleanValidator, TaskAttribute("isRegex")]
        public bool IsRegex
        {
            get
            {
                return this._isRegex;
            }
            set
            {
                this._isRegex = value;
            }
        }

        [TaskAttribute("pattern")]
        public string Pattern
        {
            get
            {
                return this._pattern;
            }
            set
            {
                this._pattern = value;
            }
        }
    }
}
