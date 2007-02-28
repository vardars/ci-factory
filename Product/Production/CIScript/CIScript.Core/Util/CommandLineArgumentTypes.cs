// CIScript - A .NET build tool
// Copyright (C) 2001 Gerry Shaw
// Copyright (c) 2007 Jay Flowers (jay.flowers@gmail.com)
// Gert Driesen (gert.driesen@ardatis.com)

using System;

namespace CIScript.Core.Util {
    /// <summary>
    /// Used to control parsing of command-line arguments.
    /// </summary>
    [Flags]    
    public enum CommandLineArgumentTypes {
        /// <summary>
        /// Indicates that this field is required. An error will be displayed
        /// if it is not present when parsing arguments.
        /// </summary>
        Required    = 0x01,

        /// <summary>
        /// Only valid in conjunction with Multiple.
        /// Duplicate values will result in an error.
        /// </summary>
        Unique      = 0x02,

        /// <summary>
        /// Inidicates that the argument may be specified more than once.
        /// Only valid if the argument is a collection
        /// </summary>
        Multiple    = 0x04,

        /// <summary>
        /// Inidicates that if this argument is specified, no other arguments may be specified.
        /// </summary>
        Exclusive    = 0x08,

        /// <summary>
        /// The default type for non-collection arguments.
        /// The argument is not required, but an error will be reported if it is specified more than once.
        /// </summary>
        AtMostOnce  = 0x00,
        
        /// <summary>
        /// The default type for collection arguments.
        /// The argument is permitted to occur multiple times, but duplicate 
        /// values will cause an error to be reported.
        /// </summary>
        MultipleUnique  = Multiple | Unique,
    }
}
