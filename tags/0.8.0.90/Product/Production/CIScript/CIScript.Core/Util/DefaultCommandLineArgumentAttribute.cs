// CIScript - A .NET build tool
// Copyright (C) 2001 Gerry Shaw
// Copyright (c) 2007 Jay Flowers (jay.flowers@gmail.com)
// Gert Driesen (gert.driesen@ardatis.com)

using System;

namespace CIScript.Core.Util {
    /// <summary>
    /// Marks a command-line option as being the default option.  When the name of 
    /// a command-line argument is not specified, this option will be assumed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class DefaultCommandLineArgumentAttribute : CommandLineArgumentAttribute {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLineArgumentAttribute" /> class
        /// with the specified argument type.
        /// </summary>
        /// <param name="argumentType">Specifies the checking to be done on the argument.</param>
        public DefaultCommandLineArgumentAttribute(CommandLineArgumentTypes argumentType) : base(argumentType) {
        }
    }
}
