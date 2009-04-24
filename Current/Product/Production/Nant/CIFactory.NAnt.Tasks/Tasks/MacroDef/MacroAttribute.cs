using NAnt.Core;
using NAnt.Core.Attributes;
using System;

namespace Macrodef
{
    /// <summary>
    /// Describes a parameter/attribute of the macro. Can be accessed as properties within the macro definition (i.e. ${prop}).
    /// </summary>
    [ElementName("attribute")]
    public class MacroAttribute : Element
    {
        #region Fields

        private string _default;

        private string _name;

        private string _property;

        private string _Type;

        #endregion

        #region Properties

        /// <summary>
        /// Default value - the property will be set to this if the attribute is not present.
        /// </summary>
        [TaskAttribute("default", Required = false)]
        public string defaultValue
        {
            get { return _default; }
            set { _default = value; }
        }

        public string LocalPropertyName
        {
            get
            {
                if (_property == null)
                    return _name;
                return _property;
            }
        }

        /// <summary>
        /// The name of the attribute.
        /// </summary>
        [TaskAttribute("name", Required = true)]
        public string name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Property name to store the value in - defaults to the name of the attribute.
        /// </summary>
        [TaskAttribute("property", Required = false)]
        public string property
        {
            get { return _property; }
            set { _property = value; }
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
        
        #endregion

    }
}
