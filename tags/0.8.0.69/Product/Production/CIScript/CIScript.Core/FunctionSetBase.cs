// CIScript - A .NET build tool
// Copyright (C) 2001-2003 Gerry Shaw
// Copyright (c) 2007 Jay Flowers (jay.flowers@gmail.com)
// Ian MacLean (ian_maclean@another.com)

using System;

using CIScript.Core;
using CIScript.Core.Attributes;


namespace CIScript.Core {
    public abstract class FunctionSetBase {
        #region Protected Instance Constructors

        protected FunctionSetBase(Project project, PropertyDictionary properties) {
            _project = project;
        }
        
        #endregion Protected Instance Constructors

        #region Public Instance Properties

        /// <summary>
        /// Gets or sets the <see cref="Project" /> that this functionset will 
        /// reference.
        /// </summary>
        /// <value>
        /// The <see cref="Project" /> that this functionset will reference.
        /// </value>
        public virtual Project Project {
            get { return _project; }
            set { _project = value; }
        }

        #endregion Public Instance Properties

        #region Private Instance Fields

        private Project _project = null;

        #endregion Private Instance Fields
    }
}
