using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAnt.Core;

using java.lang;
using java.lang.reflect;

namespace CIFactory.NAnt.Groovy.Tasks
{
    public class GroovyTaskBuilder : TaskBuilder
    {
        string _name;
        Method _method;
        Class _clazz;
        string[] _args;

        public GroovyTaskBuilder(string name, Class clazz, Method method, string[] args)
        {
            this._args = args;
            this._clazz = clazz;
            this._method = method;
            this._name = name;
            this.TaskName = method.getName();
        }

        public override Task CreateTask()
        {
            return new GroovyTask(_clazz, _method, _args);
        }
    }
}
