// CIScript - A .NET build tool
// Copyright (C) 2001-2002 Gerry Shaw
// Copyright (c) 2007 Jay Flowers (jay.flowers@gmail.com)

// Ian MacLean (ian_maclean@another.com)

using System;
using System.Globalization;
using System.Reflection;
using System.Security.Permissions;

using CIScript.Core.Attributes;

namespace CIScript.Core {
    public class DataTypeBaseBuilder {
        #region Public Instance Constructors

        /// <summary>
        /// Creates a new instance of the <see cref="DataTypeBaseBuilder" /> class
        /// for the specified Element class.
        /// </summary>
        /// <param name="className">The class representing the Element.</param>
        public DataTypeBaseBuilder(string className) : this(className, null) {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="DataTypeBaseBuilder" /> class
        /// for the specified Element class in the assembly specified.
        /// </summary>
        /// <param name="className">The class representing the Element.</param>
        /// <param name="assemblyFileName">The assembly containing the Element.</param>/// 
        public DataTypeBaseBuilder(string className, string assemblyFileName) {
            _className = className;
            _assemblyFileName = assemblyFileName;

            Assembly assembly = GetAssembly();
            // get Element name from attribute
            ElementNameAttribute ElementNameAttribute = (ElementNameAttribute) 
                Attribute.GetCustomAttribute(assembly.GetType(ClassName), typeof(ElementNameAttribute));

            _elementName = ElementNameAttribute.Name;
        }

        #endregion Public Instance Constructors

        #region Public Instance Properties

        public string ClassName {
            get { return _className; }
        }

        public string AssemblyFileName {
            get { return _assemblyFileName; }
        }

        public string DataTypeName {
            get { return _elementName; }
        }

        #endregion Public Instance Properties

        #region Public Instance Methods

        [ReflectionPermission(SecurityAction.Demand, Flags=ReflectionPermissionFlag.NoFlags)]
        public DataTypeBase CreateDataTypeBase() {
            Assembly assembly = GetAssembly();
            return (DataTypeBase) assembly.CreateInstance(
                ClassName, 
                true, 
                BindingFlags.Public | BindingFlags.Instance,
                null,
                null,
                CultureInfo.InvariantCulture,
                null);
        }

        #endregion Public Instance Methods

        #region Private Instance Methods

        private Assembly GetAssembly() {
            Assembly assembly = null;

            if (AssemblyFileName == null) {
                assembly = Assembly.GetExecutingAssembly();
            } else {
                //check to see if it is loaded already
                Assembly[] ass = AppDomain.CurrentDomain.GetAssemblies();
                for (int i = 0; i < ass.Length; i++) {
                    try {
                        string assemblyLocation = ass[i].Location;

                        if (assemblyLocation != null && assemblyLocation == AssemblyFileName) {
                            assembly = ass[i];
                            break;
                        }
                    } catch (NotSupportedException) {
                        // dynamically loaded assemblies do not not have a location
                    }
                }
                //load if not loaded
                if (assembly == null) {
                    assembly = Assembly.LoadFrom(AssemblyFileName);
                }                
            }
            return assembly;
        }

        #endregion Private Instance Methods

        #region Private Instance Fields

        string _className;
        string _assemblyFileName;
        string _elementName;

        #endregion Private Instance Fields
    }
}
