using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using NAnt.Core;

using com.thoughtworks.paranamer;
using java.lang;
using java.lang.reflect;

namespace CIFactory.NAnt.Groovy.Tasks
{
    public class GroovyTask : Task
    {
        static Dictionary<java.lang.Class, object> groovyTargetCache = new Dictionary<java.lang.Class, object>();
        java.lang.Class _script;
        java.lang.reflect.Method _method;
        string[] _argumentNames;
        string[] _parameters;

        public GroovyTask(Class groovyScript, Method method, string[] argumentNames)
        {
            this._script = groovyScript;
            this._method = method;
            this._argumentNames = argumentNames;
        }

        public override void Initialize(XmlNode elementNode)
        {
            this._parameters = new string[_argumentNames.Length];
            foreach (XmlAttribute attr in elementNode.Attributes)
            {
                if( !_argumentNames.Contains(attr.Name) )
                    throw new BuildException("Error: Groovy Task " + this._method.getName() + " does not accept the parameter " + attr.Name);

                for (int i = 0; i < _argumentNames.Length; i++)
                {
                    if (_argumentNames[i] == attr.Name)
                    {
                        _parameters[i] = Project.ExpandProperties(attr.Value, this.Location);
                        break;
                    }
                }
            }

            for (int i = 0; i < _argumentNames.Length; i++)
                if (_parameters[i] == null)
                    throw new BuildException("Parameter " + _argumentNames[i] + " for Groovy task " + this._method.getName() + " was not specified!");
        }


        protected override void ExecuteTask()
        {
            object script = this._script.newInstance();
            try
            {
                this._method.invoke(script, _parameters);
            }
            catch (java.lang.reflect.InvocationTargetException e)
            {
                throw new BuildException("Failed while attempting to execute task '" + this._method.getName() + "'. Exception follows:\n" + e.getCause().Message + "\n" + e.getCause().StackTrace, e);
            }
        }
    }
}
