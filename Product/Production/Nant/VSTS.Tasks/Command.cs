using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

namespace VSTS.Tasks
{
    public class Command
    {

        private object _WrappedSubject;

        protected virtual object WrappedSubject
        {
            get
            {
                return _WrappedSubject;
            }
        }

        public object UnWrapObject
        {
            get
            {
                return this.WrappedSubject;
            }
        }

        internal Command(object wrappedSubject)
        {
            _WrappedSubject = wrappedSubject;
        }

        public Command()
        {
        }

        private MethodInfo _InitializeMethod;

        private MethodInfo InitializeMethod
        {
            get
            {
                if (_InitializeMethod == null)
                {
                    _InitializeMethod = this.WrappedSubject.GetType().GetMethod("Initialize");
                }
                return _InitializeMethod;
            }
        }

        protected void Initialize(string commandArgument)
        {
            this.InitializeMethod.Invoke(this.WrappedSubject, new object[1] { commandArgument });
        }


    }
}
