// CIScript - A .NET build tool
// Copyright (C) 2001-2003 Gerry Shaw
// Copyright (c) 2007 Jay Flowers (jay.flowers@gmail.com)
// Jaroslaw Kowalski (jkowalski@users.sourceforge.net)

using System;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Globalization;

using CIScript.Core;
using CIScript.Core.Attributes;
using CIScript.Core.Util;

namespace CIScript.Core {
    [FunctionSet("property", "CIScript")]
    public class ExpressionEvaluator : ExpressionEvalBase {
        #region Private Instance Fields

        private PropertyDictionary _properties;
        private Hashtable _state;
        private Stack _visiting;
        private Project _project;

        #endregion Private Instance Fields

        #region Public Instance Constructors

        public ExpressionEvaluator(Project project, PropertyDictionary properties, Hashtable state, Stack visiting) {
            _project = project;
            _properties = properties;
            _state = state;
            _visiting = visiting;
        }

        #endregion Public Instance Constructors

        #region Public Instance Properties

        public Project Project {
            get { return _project; }
        }

        #endregion Public Instance Properties

        #region Override implementation of ExpressionEvalBase

        protected override object EvaluateProperty(string propertyName) {
            return GetPropertyValue(propertyName);
        }

        protected override ParameterInfo[] GetFunctionParameters(string functionName) {
            MethodInfo methodInfo = TypeFactory.LookupFunction(functionName, Project);
            if (methodInfo == null) {
                throw new BuildException(string.Format(CultureInfo.InvariantCulture, 
                                                       ResourceUtils.GetString("NA1052"), functionName));
            }
            return methodInfo.GetParameters();
        }

        protected override object EvaluateFunction(string functionName, object[] args) {
            MethodInfo methodInfo = TypeFactory.LookupFunction(functionName, Project);
            if (methodInfo == null) {
                throw new BuildException(string.Format(CultureInfo.InvariantCulture, 
                            ResourceUtils.GetString("NA1052"), functionName));
            }

            try {
                if (methodInfo.IsStatic) {
                    return methodInfo.Invoke(null, args);
                } else if (methodInfo.DeclaringType.IsAssignableFrom(typeof(ExpressionEvaluator))) {
                    return methodInfo.Invoke(this, args);
                } else {
                    // create new instance.
                    ConstructorInfo constructor = methodInfo.DeclaringType.GetConstructor(new Type[] {typeof(Project), typeof(PropertyDictionary)});
                    object o = constructor.Invoke(new object[] {_project, _properties});

                    return methodInfo.Invoke(o, args);
                }
            } catch (TargetInvocationException ex) {
                if (ex.InnerException != null) {
                    // throw actual exception
                    throw ex.InnerException;
                }
                // re-throw exception
                throw;
            }
        }

        #endregion Override implementation of ExpressionEvalBase

        #region Public Instance Methods

        /// <summary>
        /// Gets the value of the specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property to get the value of.</param>
        /// <returns>
        /// The value of the specified property.
        /// </returns>
        [Function("get-value")]
        public string GetPropertyValue(string propertyName) {
            if (_properties.IsDynamicProperty(propertyName)) {
                string currentState = (string)_state[propertyName];

                // check for circular references
                if (currentState == PropertyDictionary.Visiting) {
                    // Currently visiting this node, so have a cycle
                    throw PropertyDictionary.CreateCircularException(propertyName, _visiting);
                }

                _visiting.Push(propertyName);
                _state[propertyName] = PropertyDictionary.Visiting;

                string propertyValue = _properties.GetPropertyValue(propertyName);
                if (propertyValue == null) {
                    throw new BuildException(string.Format(CultureInfo.InvariantCulture, 
                        ResourceUtils.GetString("NA1053"), propertyName));
                }

                Location propertyLocation = Location.UnknownLocation;

                // TODO - get the proper location of the property declaration
                
                propertyValue = _properties.ExpandProperties(propertyValue, 
                    propertyLocation, _state, _visiting);

                _visiting.Pop();
                _state[propertyName] = PropertyDictionary.Visited;
                return propertyValue;
            } else {
                string propertyValue = _properties.GetPropertyValue(propertyName);
                if (propertyValue == null) {
                    throw new BuildException(string.Format(CultureInfo.InvariantCulture, 
                        ResourceUtils.GetString("NA1053"), propertyName));
                }

                return propertyValue;
            }
        }

        #endregion Public Instance Methods
    }
}
