// CIScript - A .NET build tool
// Copyright (C) 2001-2005 Gerry Shaw
// Copyright (c) 2007 Jay Flowers (jay.flowers@gmail.com)
// Gerry Shaw (gerry_shaw@yahoo.com)
// Ian MacLean ( ian@maclean.ms )
// Scott Hernandez (ScottHernandez_at_HOtMail_dot_dot_dot_com?)

using System;

namespace CIScript.Core.Attributes {
    /// <summary>
    /// Indicates that a property should be treated as a XML file set for the 
    /// task.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited=true)]
    [Obsolete("Please use a BuildElement attribute instead. This class will be removed soon.", true)]
    public sealed class FileSetAttribute : BuildElementAttribute {
        #region Public Instance Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSetAttribute" /> with the
        /// specified name.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="name" /> is a zero-length <see cref="string" />.</exception>
        public FileSetAttribute(string name) : base(name) {
        }

        #endregion Public Instance Constructors
    }
}