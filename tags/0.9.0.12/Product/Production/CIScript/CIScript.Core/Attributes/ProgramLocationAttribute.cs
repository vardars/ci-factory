// CIScript - A .NET build tool
// Copyright (C) 2001 Gerry Shaw
// Copyright (c) 2007 Jay Flowers (jay.flowers@gmail.com)
// Ian MacLean ( ian@maclean.ms )

using System;

namespace CIScript.Core.Attributes {

    /// <summary>
    /// Defines possible locations in which a task executable can be located.
    /// </summary>
    public enum LocationType {
        /// <summary>
        /// Locates the task executable in the current Framework directory.
        /// </summary>
        FrameworkDir,

        /// <summary>
        /// Locates the task executable in the current Framework SDK directory.
        /// </summary>
        FrameworkSdkDir
    }
    
    /// <summary>
    /// Indicates the location that a task executable can be located in.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited=false, AllowMultiple=false)]
    public class ProgramLocationAttribute : Attribute {
        #region Protected Instance Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgramLocationAttribute" /> 
        /// with the specified location.
        /// </summary>
        /// <param type="type">The <see cref="LocationType" /> of the attribute.</param>
        public ProgramLocationAttribute(LocationType type) {
            LocationType = type;
        }

        #endregion Protected Instance Constructors

        #region Public Instance Properties

        /// <summary>
        /// Gets or sets the <see cref="LocationType" /> of the task.
        /// </summary>
        /// <value>
        /// The location type of the task to which the attribute is assigned.
        /// </value>
        public LocationType LocationType {
            get { return _locationType; }
            set { _locationType = value; }
        }

        #endregion Public Instance Properties

        #region Private Instance Fields

        private LocationType _locationType;

        #endregion Private Instance Fields
    }
}