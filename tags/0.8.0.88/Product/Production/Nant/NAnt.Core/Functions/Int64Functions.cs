// NAnt - A .NET build tool
// Copyright (C) 2001-2004 Gerry Shaw
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// Gert Driesen (gert.driesen@ardatis.com)

using System;
using System.Globalization;

using NAnt.Core;
using NAnt.Core.Types;
using NAnt.Core.Attributes;

namespace NAnt.Core.Functions {
    [FunctionSet("long", "Conversion")]
    public class Int64ConversionFunctions : FunctionSetBase {
        #region Public Instance Constructors

        public Int64ConversionFunctions(Project project, PropertyDictionary properties) : base(project, properties) {
        }

        #endregion Public Instance Constructors

        #region Public Static Methods

        /// <summary>
        /// Converts the specified string representation of a number to its 
        /// 64-bit signed integer equivalent.
        /// </summary>
        /// <param name="s">A string containing a number to convert.</param>
        /// <returns>
        /// A 64-bit signed integer equivalent to the number contained in 
        /// <paramref name="s" />.
        /// </returns>
        /// <exception cref="FormatException"><paramref name="s" /> is not of the correct format.</exception>
        /// <exception cref="OverflowException"><paramref name="s" /> represents a number less than <see cref="long.MinValue" /> or greater than <see cref="long.MaxValue" />.</exception>
        /// <remarks>
        /// The <see cref="NumberFormatInfo" /> for the invariant culture is 
        /// used to supply formatting information about <paramref name="s" />.
        /// </remarks>
        [Function("parse")]
        public static long Parse(string s) {
            return long.Parse(s, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts the specified <see cref="long" /> to its equivalent string
        /// representation.
        /// </summary>
        /// <param name="value">A <see cref="long" /> to convert.</param>
        /// <returns>
        /// The string representation of <paramref name="value" />, consisting 
        /// of a negative sign if the value is negative, and a sequence of 
        /// digits ranging from 0 to 9 with no leading zeroes.
        /// </returns>
        /// <remarks>
        /// <paramref name="value" /> is formatted with the 
        /// <see cref="NumberFormatInfo" /> for the invariant culture.
        /// </remarks>
        [Function("to-string")]
        public static string ToString(long value) {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        #endregion Public Static Methods
    }
}
