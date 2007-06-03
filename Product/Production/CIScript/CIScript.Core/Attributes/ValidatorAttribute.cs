// CIScript - A .NET build tool
// Copyright (C) 2001-2003 Gerry Shaw
// Copyright (c) 2007 Jay Flowers (jay.flowers@gmail.com)
// Gerry Shaw (gerry_shaw@yahoo.com)

using System;

namespace CIScript.Core.Attributes {
    /// <summary>
    /// Base class for all validator attributes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited=true)]
    public abstract class ValidatorAttribute : Attribute {
        #region Public Instance Methods

        /// <summary>
        /// Validates the specified value.
        /// </summary>
        /// <param name="value">The value to be validated.</param>
        /// <exception cref="ValidationException">The validation fails.</exception>
        public abstract void Validate(object value);

        #endregion Public Instance Methods
    }
}