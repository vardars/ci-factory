// CIScript - A .NET build tool
// Copyright (C) 2001 Gerry Shaw
// Copyright (c) 2007 Jay Flowers (jay.flowers@gmail.com)
// Gert Driesen (gert.driesen@ardatis.com)

using System;
using System.Runtime.Serialization;

namespace CIScript.Core.Util {
    /// <summary>
    /// The exception that is thrown when one of the command-line arguments provided 
    /// is not valid.
    /// </summary>
    [Serializable()]
    public sealed class CommandLineArgumentException : ArgumentException {
        #region Public Instance Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLineArgumentException" /> class.
        /// </summary>
        public CommandLineArgumentException() : base() {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLineArgumentException" /> class
        /// with a descriptive message.
        /// </summary>
        /// <param name="message">A descriptive message to include with the exception.</param>
        public CommandLineArgumentException(string message) : base(message) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLineArgumentException" /> class
        /// with a descriptive message and an inner exception.
        /// </summary>
        /// <param name="message">A descriptive message to include with the exception.</param>
        /// <param name="innerException">A nested exception that is the cause of the current exception.</param>
        public CommandLineArgumentException(string message, Exception innerException) : base(message, innerException) {
        }

        #endregion Public Instance Constructors

        #region Private Instance Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLineArgumentException" /> class 
        /// with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext" /> that contains contextual information about the source or destination.</param>
        private CommandLineArgumentException(SerializationInfo info, StreamingContext context) : base(info, context) {
        }

        #endregion Private Instance Constructors
    }
}
