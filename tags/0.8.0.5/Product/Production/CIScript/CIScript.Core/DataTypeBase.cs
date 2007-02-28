// CIScript - A .NET build tool
// Copyright (C) 2001-2002 Gerry Shaw
// Copyright (c) 2007 Jay Flowers (jay.flowers@gmail.com)
// Ian MacLean (ian_maclean@another.com)

using System;
using System.Globalization;
using System.Reflection;
using System.Xml;

using CIScript.Core.Attributes;
using CIScript.Core.Util;

namespace CIScript.Core {
    /// <summary>
    /// Provides the abstract base class for types.
    /// </summary>
    [Serializable()]
    public abstract class DataTypeBase : Element {
        #region Private Instance Fields

        private string _id;
        private string _refID;

        #endregion Private Instance Fields

        #region Public Instance Properties

        /// <summary>        /// The ID used to be referenced later.        /// </summary>
        [TaskAttribute("id" )]
        public string ID {
            get { return _id; }
            set { _id = StringUtils.ConvertEmptyToNull(value); }
        }

        /// <summary>        /// The ID to use as the reference.        /// </summary>
        [TaskAttribute("refid")]
        public string RefID {
            get { return _refID; }
            set { _refID = StringUtils.ConvertEmptyToNull(value); }
        }

        /// <summary>
        /// Gets a value indicating whether a reference to the type can be
        /// defined.
        /// </summary>
        /// <remarks>
        /// Only types with an <see cref="ElementNameAttribute" /> assigned 
        /// to it, can be referenced.
        /// </remarks>
        public bool CanBeReferenced {
            get { return Name != null; }
        }

        #endregion Public Instance Properties

        #region Override implementation of Element
        
        /// <summary>
        /// Gets the name of the datatype.
        /// </summary>
        /// <value>
        /// The name of the datatype.
        /// </value>
        public override string Name {
            get {
                string name = null;
                ElementNameAttribute elementName = (ElementNameAttribute) Attribute.GetCustomAttribute(GetType(), typeof(ElementNameAttribute));
                if (elementName != null) {
                    name = elementName.Name;
                }
                return name;
            }
        }

        protected override void InitializeElement(XmlNode elementNode) {
            if (Parent == null) {
                // output warning message
                Log(Level.Warning, "Parent property should be set on types" 
                    + " deriving from DataTypeBase to determine whether" 
                    + " the type is declared on a valid level.");

                // skip further tests
                return;
            }

            if (Parent.GetType() == typeof(Project) || Parent.GetType() == typeof(Target)) {
                if (StringUtils.IsNullOrEmpty(ID)) {
                    throw new BuildException(string.Format(CultureInfo.InvariantCulture, 
                        ResourceUtils.GetString("NA1010"), 
                        Name), Location);
                }
                if (!StringUtils.IsNullOrEmpty(RefID)) {
                    throw new BuildException(string.Format(CultureInfo.InvariantCulture, 
                        ResourceUtils.GetString("NA1009"), 
                        Name), Location);
                }
            } else {
                  if (!StringUtils.IsNullOrEmpty(ID)) {
                    throw new BuildException(string.Format(CultureInfo.InvariantCulture, 
                        ResourceUtils.GetString("NA1008") 
                        + " can only be declared at Project or Target level.", 
                        Name), Location);
                }
            }
        }

        #endregion Override implementation of Element

        #region Public Instance Methods

        /// <summary>
        /// Should be overridden by derived classes. clones the referenced types 
        /// data into the current instance.
        /// </summary>
        public virtual void Reset( ) {
        }

        #endregion Public Instance Methods

        #region Protected Instance Methods

        /// <summary>
        /// Copies all instance data of the <see cref="DataTypeBase" /> to a given
        /// <see cref="DataTypeBase" />.
        /// </summary>
        protected void CopyTo(DataTypeBase clone) {
            base.CopyTo(clone);

            clone._id = _id;
            clone._refID = _refID;
        }

        #endregion Protected Instance Methods
    }
}