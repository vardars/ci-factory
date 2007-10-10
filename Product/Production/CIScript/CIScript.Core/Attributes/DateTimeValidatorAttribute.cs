// CIScript - A .NET build tool
// Copyright (C) 2001 Gerry Shaw
// Copyright (c) 2007 Jay Flowers (jay.flowers@gmail.com)
// Clayton Harbour (claytonharbour@sporadicism.com)

using System;
using System.Globalization;
using CIScript.Core.Util;

namespace CIScript.Core.Attributes {
    /// <summary>
    /// Used to indicate that a property should be able to be converted into a 
    /// <see cref="DateTime" />.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited=true)]
    public sealed class DateTimeValidatorAttribute : ValidatorAttribute {
        #region Public Instance Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeValidatorAttribute" /> 
        /// class.
        /// </summary>
        public DateTimeValidatorAttribute() {
        }

        #endregion Public Instance Constructors

        #region Override implementation of ValidatorAttribute

        /// <summary>
        /// Checks if the specified value can be converted to a <see cref="DateTime" />.
        /// </summary>
        /// <param name="value">The value to be checked.</param>
        /// <exception cref="ValidationException"><paramref name="value" /> cannot be converted to a <see cref="DateTime" />.</exception>
        public override void Validate(object value) {
            try {
                Convert.ToDateTime(value, CultureInfo.InvariantCulture);
            } catch (Exception ex) {
                throw new ValidationException(string.Format(CultureInfo.InvariantCulture, 
                                                            ResourceUtils.GetString("NA1089"), value.ToString()), ex);
            }
        }

        #endregion Override implementation of ValidatorAttribute
    }
}