using System;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using NAnt.Core;
using NAnt.Core.Types;
using NAnt.Core.Attributes;

namespace CIFactory.NAnt.Functions
{
    [FunctionSet("process", "process")]
    public class ProcessFunctions : FunctionSetBase
    {

        #region Constructors

        public ProcessFunctions(Project project, PropertyDictionary properties)
            : base(project, properties)
        {

        }

        #endregion

        [Function("isrunning")]
        public bool IsRunning(int pid)
        {
            try
            {
                Process.GetProcessById(pid);
                return true;
            }
            catch { }
            return false;
        }

    }
}
