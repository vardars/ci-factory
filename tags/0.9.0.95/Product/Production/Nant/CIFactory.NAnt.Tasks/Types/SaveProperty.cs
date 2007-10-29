using System;
using System.Collections;
using System.Collections.Generic;
using NAnt.Core;
using NAnt.Core.Attributes;

namespace CIFactory.NAnt.Types
{
    public class SaveProperty : Element
    {
        #region Fields

        private string _PropertyName;

        private string _PropertyValue;

        #endregion

        #region Properties

        [TaskAttribute("name", Required = true)]
        public string PropertyName
        {
            get { return _PropertyName; }
            set { _PropertyName = value; }
        }

        public string PropertyValue
        {
            get
            {
                if (this._PropertyValue != null)
                {
                    return this._PropertyValue;
                }
                return this.Properties[this.PropertyName];
            }
        }

        [TaskAttribute("value", Required = false)]
        public string SetProperty
        {
            set { this._PropertyValue = value; }
        }

        #endregion

    }
}