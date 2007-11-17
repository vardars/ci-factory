// CIScript - A .NET build tool
// Copyright (C) 2001 Gerry Shaw
// Copyright (c) 2007 Jay Flowers (jay.flowers@gmail.com)
// Gerry Shaw (gerry_shaw@yahoo.com)

using System;
using System.Reflection;
using System.Globalization;
using CIScript.Core.Util;

namespace CIScript.Core.Attributes {
    /// <summary>
    /// Indicates that property should be able to be converted into a <see cref="Int32" /> 
    /// within the given range.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited=true)]
    public sealed class Int32ValidatorAttribute : ValidatorAttribute {
        #region Public Instance Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Int32ValidatorAttribute" /> 
        /// class.
        /// </summary>
        public Int32ValidatorAttribute() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Int32ValidatorAttribute" /> 
        /// class with the specied minimum and maximum values.
        /// </summary>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="maxValue">The maximum value.</param>
        public Int32ValidatorAttribute(int minValue, int maxValue) {
            MinValue = minValue;
            MaxValue = maxValue;
        }

        #endregion Public Instance Constructors

        #region Public Instance Properties

        /// <summary>
        /// Gets or sets the minimum value.
        /// </summary>
        /// <value>
        /// The minimum value. The default is <see cref="Int32.MinValue" />.
        /// </value>
        public int MinValue {
            get { return _minValue; }
            set { _minValue = value; }
        }

        /// <summary>
        /// Gets or sets the maximum value.
        /// </summary>
        /// <value>
        /// The maximum value. The default is <see cref="Int32.MaxValue" />.
        /// </value>
        public int MaxValue {
            get { return _maxValue; }
            set { _maxValue = value; }
        }

        /// <summary>
        /// The base of the number to validate, which must be 2, 8, 10, or 16.
        /// </summary>
        /// <value>
        /// The base of the number to validate.
        /// </value>
        /// <remarks>
        /// The default is 10.
        /// </remarks>
        public int Base {
            get { return _base; }
            set { _base = value; }
        }

        #endregion Public Instance Properties

        #region Override implementation of ValidatorAttribute

        /// <summary>
        /// Checks whether the specified value can be converted to an <see cref="Int32" /> 
        /// and whether the value lies within the range defined by the <see cref="MinValue" /> 
        /// and <see cref="MaxValue" /> properties.
        /// </summary>
        /// <param name="value">The value to be checked.</param>
        /// <exception cref="ValidationException">
        ///   <para>
        ///   <paramref name="value" /> cannot be converted to an <see cref="Int32" />.
        ///   </para>
        ///   <para>-or-</para>
        ///   <para>
        ///   <paramref name="value" /> is not in the range defined by <see cref="MinValue" />
        ///   and <see cref="MaxValue" />.
        ///   </para>
        /// </exception>
        public override void Validate(object value) {
            Int32 intValue;

            try {
                if (value is String) {
                    intValue = Convert.ToInt32((string) value, Base);
                } else {
                    intValue = Convert.ToInt32(value, CultureInfo.InvariantCulture);
                }
            } catch (Exception ex) {
                throw new ValidationException(string.Format(CultureInfo.InvariantCulture, 
                                                            ResourceUtils.GetString("NA1091"), value.ToString()), ex);
            }

            if (intValue < MinValue || intValue > MaxValue) {
                throw new ValidationException(string.Format(CultureInfo.InvariantCulture, 
                    ResourceUtils.GetString("NA1090"), value.ToString(), 
                    MinValue, MaxValue));
            }
        }

        #endregion Override implementation of ValidatorAttribute

        #region Private Instance Fields

        private int _minValue = Int32.MinValue;
        private int _maxValue = Int32.MaxValue;
        private int _base = 10;

        #endregion Private Instance Fields
    }
}