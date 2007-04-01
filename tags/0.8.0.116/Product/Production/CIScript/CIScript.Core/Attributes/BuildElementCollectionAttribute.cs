// CIScript - A .NET build tool
// Copyright (C) 2001-2003 Gerry Shaw
// Copyright (c) 2007 Jay Flowers (jay.flowers@gmail.com)
// Gert Driesen (gert.driesen@ardatis.com)
// Scott Hernandez (ScottHernandez_at_HOtMail_dot_dot_dot_com?)

using System;

namespace CIScript.Core.Attributes {
    /// <summary>
    /// Indicates that the property should be treated as a container for a 
    /// collection of build elements.
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
    ///     <collectionName>
    ///         <elementName ... />
    ///         <elementName ... />
    ///         <elementName ... />
    ///         <elementName ... />
    ///     </collectionName>
    /// </task>
    ///     ]]>
    /// </code>
    /// </para>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property, Inherited=true)]
    public sealed class BuildElementCollectionAttribute : BuildElementArrayAttribute{
        #region Public Instance Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildElementCollectionAttribute" /> with the 
        /// specified name and child element name.
        /// </summary>
        /// <param name="collectionName">The name of the collection.</param>
        /// <param name="childName">The name of the child elements in the collection</param>
        /// <exception cref="ArgumentNullException"><paramref name="childName" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="childName" /> is a zero-length <see cref="string" />.</exception>
        public BuildElementCollectionAttribute(string collectionName, string childName) : base(collectionName) {
            if (childName == null) { 
                throw new ArgumentNullException("childName"); 
            }

            _elementName = childName.Trim();

            if (_elementName.Length == 0) {
                throw new ArgumentOutOfRangeException("childName", childName, "A zero-length string is not an allowed value.");
            }
        }

        #endregion Public Instance Constructors

        #region Public Instance Constructors

        /// <summary>
        /// The name of the child element within the collection.
        /// </summary>
        /// <value>
        /// The name to check for in the XML of the elements in the collection.
        /// </value>
        /// <remarks>
        /// This can be used for validation and schema generation.
        /// </remarks>
        public string ChildElementName {
            get { return _elementName; }
        }

        #endregion Public Instance Constructors

        #region Private Instance Fields

        private string _elementName = null;

        #endregion Private Instance Fields
    }
}