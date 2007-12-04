// CIScript - A .NET build tool
// Copyright (C) 2001-2003 Gerry Shaw
// Copyright (c) 2007 Jay Flowers (jay.flowers@gmail.com)
// Gerry Shaw (gerry_shaw@yahoo.com)
// Scott Hernandez (ScottHernandez_at_HOtMail_dot_dot_dot_com?)

using System;

namespace CIScript.Core.Attributes {
    /// <summary>
    /// Indicates that class should be treated as a task.
    /// </summary>
    /// <remarks>
    /// Attach this attribute to a subclass of Task to have CIScript be able
    /// to recognize it.  The name should be short but must not confict
    /// with any other task already in use.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, Inherited=false, AllowMultiple=false)]
    public sealed class TaskNameAttribute : ElementNameAttribute {
        #region Public Instance Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskNameAttribute" /> 
        /// with the specified name.
        /// </summary>
        /// <param name="name">The name of the task.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="name" /> is a zero-length <see cref="string" />.</exception>
        public TaskNameAttribute(string name) : base(name) {
        }

        #endregion Public Instance Constructors
    }
}
