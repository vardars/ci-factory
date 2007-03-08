// CIScript - A .NET build tool
// Copyright (C) 2001 Gerry Shaw
// Copyright (c) 2007 Jay Flowers (jay.flowers@gmail.com)
// Gert Driesen (gert.driesen@ardatis.com)

using System;

namespace CIScript.Core.Util {
    /// <summary>
    /// Allows control of command line parsing.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class CommandLineArgumentAttribute : Attribute {
        #region Public Instance Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandLineArgumentAttribute" /> class
        /// with the specified argument type.
        /// </summary>
        /// <param name="argumentType">Specifies the checking to be done on the argument.</param>
        public CommandLineArgumentAttribute(CommandLineArgumentTypes argumentType) {
            _argumentType = argumentType;
        }

        #endregion Public Instance Constructors

        #region Public Instance Properties

        /// <summary>
        /// Gets or sets the checking to be done on the argument.
        /// </summary>
        /// <value>The checking that should be done on the argument.</value>
        public CommandLineArgumentTypes Type {
            get { return _argumentType; }
        }

        /// <summary>
        /// Gets or sets the long name of the argument.
        /// </summary>
        /// <value>The long name of the argument.</value>
        public string Name {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets or sets the short name of the argument.
        /// </summary>
        /// <value>The short name of the argument.</value>
        public string ShortName {
            get { return _shortName; }
            set { _shortName = value; }
        }

        /// <summary>
        /// Gets or sets the description of the argument.
        /// </summary>
        /// <value>The description of the argument.</value>
        public string Description {
            get { return _description; }
            set { _description = value; }
        }

        #endregion Public Instance Properties

        #region Private Instance Fields

        private CommandLineArgumentTypes _argumentType;
        private string _name;
        private string _shortName;
        private string _description;

        #endregion Private Instance Fields
    }
}

