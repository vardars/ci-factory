using NAnt.Core;
using NAnt.Core.Attributes;
using System;

namespace Macrodef
{
    [ElementName("elementgroup")]
    public class MacroElementGroup : Element
    {
        #region Fields

        private string _name;

        private string _Type;

        #endregion

        #region Properties

        /// <summary>
        /// The name of the element.
        /// </summary>
        [TaskAttribute("name", Required = true)]
        public string name
        {
            get { return _name; }
            set { _name = value; }
        }

        [TaskAttribute("type", Required = true)]
        public string Type
        {
            get { return _Type; }
            set
            {
                _Type = value;
            }
        }

        private bool _Require = false;

        [TaskAttribute("require", Required = false)]
        public bool Require
        {
            get { return _Require; }
            set
            {
                _Require = value;
            }
        }

        private string _ElementName;
        [TaskAttribute("elementname", Required = true)]
        public string ElementName
        {
            get { return _ElementName; }
            set
            {
                _ElementName = value;
            }
        }
        
        #endregion

    }
}
