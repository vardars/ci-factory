using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAnt.Core;

using java.lang;
using java.lang.reflect;

namespace CIFactory.NAnt.Groovy.Tasks
{
    public class GroovyTask : Task
    {
        static Dictionary<java.lang.Class, object> groovyTargetCache = new Dictionary<java.lang.Class, object>();
        java.lang.Class _script;
        java.lang.reflect.Method _method;

        public GroovyTask(Class groovyScript, Method method)
        {
            this._script = groovyScript;
            this._method = method;
        }

        protected override void ExecuteTask()
        {
            object script = this._script.newInstance();
            try
            {
                this._method.invoke(script);
            }
            catch (java.lang.Exception e)
            {
                e.printStackTrace();
            }
        }
    }
}
