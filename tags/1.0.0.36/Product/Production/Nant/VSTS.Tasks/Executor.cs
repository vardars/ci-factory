using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

namespace VSTS.Tasks
{
    public class Executor
    {

        private Object _WrappedSubject;
        private MethodInfo _AddCommandMethod;
        private MethodInfo _ExecuteMethod;
        private MethodInfo _ValidateCommandsMethod;
        private PropertyInfo _OutputProperty;

        private PropertyInfo OutputProperty
        {
            get
            {
                if (_OutputProperty == null)
                {
                    _OutputProperty = this.WrappedSubject.GetType().GetProperty("Output");
                }
                return _OutputProperty;
            }
        }

        private object Output
        {
            set
            {
                this.OutputProperty.SetValue(this.WrappedSubject, value, null);
            }
        }

        private MethodInfo ValidateCommandsMethod
        {
            get
            {
                if (_ValidateCommandsMethod == null)
                {
                    _ValidateCommandsMethod = this.WrappedSubject.GetType().GetMethod("ValidateCommands");
                }
                return _ValidateCommandsMethod;
            }
        }

        public void ValidateCommands()
        {
            this.ValidateCommandsMethod.Invoke(this.WrappedSubject, null);
        }

        private MethodInfo ExecuteMethod
        {
            get
            {
                if (_ExecuteMethod == null)
                {
                    _ExecuteMethod = this.WrappedSubject.GetType().GetMethod("Execute");
                }
                return _ExecuteMethod;
            }
        }

        public Boolean Execute()
        {
            return (Boolean)this.ExecuteMethod.Invoke(this.WrappedSubject, null);
        }

        private MethodInfo AddCommandMethod
        {
            get
            {
                if (_AddCommandMethod == null)
                {
                    _AddCommandMethod = this.WrappedSubject.GetType().GetMethod("Add");
                }
                return _AddCommandMethod;
            }
        }

        public void Add(Command command)
        {
            this.AddCommandMethod.Invoke(this.WrappedSubject, new object[1] { command.UnWrapObject });
        }

        private Object WrappedSubject
        {
            get
            {
                if (_WrappedSubject == null)
                {
                    foreach (Type Canidate in TestToolsHelper.CommandLineAssembly.GetTypes())
                    {
                        if (Canidate.FullName == "Microsoft.VisualStudio.TestTools.CommandLine.Executor")
                        {
                            _WrappedSubject = Activator.CreateInstance(Canidate, null);
                            break;
                        }
                    }
                }
                return _WrappedSubject;
            }
        }

        public Executor()
        {
            
        }

        public Executor(Boolean verbose)
        {
            if (!verbose)
                this.Output = TestToolsHelper.CreateInstance("Microsoft.VisualStudio.TestTools.CommandLine.EmptyOutput");
        }
    }
}
