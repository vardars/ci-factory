// CIScript - A .NET build tool
// Copyright (C) 2001 Gerry Shaw
// Copyright (c) 2007 Jay Flowers (jay.flowers@gmail.com)
// Ian MacLean ( ian@maclean.ms )
// Scott Hernandez (ScottHernandez_at_HOtMail_dot_dot_dot_com?)

using System;

namespace CIScript.Core.Attributes {
    /// <summary>
    /// Indicates that property should be treated as a XML arrayList
    /// </summary>
    /// <remarks>
    /// <para>
    /// Should only be applied to properties exposing strongly typed arrays or 
    /// strongly typed collections.
    /// </para>
    /// <para>
    /// The XML format is like this:
    /// <code>
    ///     <![CDATA[
    /// <task>
    ///     <elementName ... />
    ///     <elementName ... />
    ///     <elementName ... />
    ///     <elementName ... />
    /// </task>
    ///     ]]>
    /// </code>
    /// </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property, Inherited=true)]
    public class BuildElementArrayAttribute : BuildElementAttribute {
        #region Public Instance Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildElementArrayAttribute" /> 
        /// with the specified name.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <exception cref="ArgumentNullException"><paramref name="name" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="name" /> is a zero-length <see cref="string" />.</exception>
        public BuildElementArrayAttribute(string name) : base(name) {
        }

        #endregion Public Instance Constructors

        #region Public Instance Properties

        /// <summary>
        /// Gets or sets the type of objects that this container holds.
        /// </summary>
        /// <value>
        /// The type of the elements that this container holds.
        /// </value>
        /// <remarks>
        /// <para>
        /// This can be used for validation and schema generation.
        /// </para>
        /// <para>
        /// If not specified, the type of the elements will be determined using
        /// reflection.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="name" /> is <see langword="null" />.</exception>
        public Type ElementType {
            get { return _elementType; }
            set { 
                if (value == null) {
                    throw new ArgumentNullException("ElementType");
                }
                _elementType = value; 
            }
        }

        #endregion Public Instance Properties

        #region Private Instance Fields

        private Type _elementType;

        #endregion Private Instance Fields
    }
}