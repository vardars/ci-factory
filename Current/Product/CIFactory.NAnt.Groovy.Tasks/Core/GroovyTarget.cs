using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAnt.Core;
using java.lang.reflect;
using java.lang;

namespace CIFactory.NAnt.Groovy.Tasks
{
    public class GroovyTarget : Target
    {
        static Dictionary<java.lang.Class, object> groovyTargetCache = new Dictionary<java.lang.Class, object>();
        java.lang.Class _script;
        java.lang.reflect.Method _method;

        public GroovyTarget(Project project, Class groovyScript, Method method)
        {
            this.Project = project;
            this._script = groovyScript;
            this._method = method;
        }

        public override void Execute()
        {
            if (!groovyTargetCache.ContainsKey(this._script))
                groovyTargetCache.Add(this._script, this._script.newInstance());
            object instance = groovyTargetCache[this._script];

            try
            {
                Project.OnTargetStarted(this, new BuildEventArgs(this));

                this._method.invoke(instance);
            }
            catch (java.lang.reflect.InvocationTargetException e)
            {
                throw new BuildException("Groovy target " + this._method.getName() + " failed: " + e.getCause() + e.getMessage());
            }
            finally
            {
                Project.OnTargetFinished(this, new BuildEventArgs(this));
            }
        }
    }
}
