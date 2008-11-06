using System;
using NAnt.Core;
using NAnt.Core.Attributes;

namespace CIFactory.NAnt.Types
{
    [ElementName("case")]
    public class CaseElement : ElementTaskContainer
    {
        #region Fields

        private bool _Break;

        private string _RightValue;

        #endregion

        #region Properties

        [TaskAttribute("break"), BooleanValidator()]
        public bool Break
        {
            get { return _Break; }
            set { _Break = value; }
        }

        [TaskAttribute("value", Required = true)]
        public string RightValue
        {
            get { return _RightValue; }
            set { _RightValue = value; }
        }

        #endregion

    }
}