using System;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using NAnt.Core;
using NAnt.Core.Types;
using NAnt.Core.Attributes;
using System.Management;

namespace CIFactory.NAnt.Functions
{
    [FunctionSet("process", "process")]
    public class ProcessFunctions : FunctionSetBase
    {

        #region Constructors

        public ProcessFunctions(Project project, Location location, PropertyDictionary properties)
            : base(project, location, properties)
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

        [Function("get-current-pid")]
        public int GetCurrentPid()
        {
            return Process.GetCurrentProcess().Id;
        }

        [Function("get-parent-pid")]
        public int GetParentPid(int pid)
        {
            using (ManagementObject mo = new ManagementObject("win32_process.handle='" + pid.ToString() + "'"))
            {
                mo.Get();
                return Convert.ToInt32(mo["ParentProcessId"]);
            }
        }
        
        public int GetParentPid()
        {
            int Id = this.GetCurrentPid();
            return this.GetParentPid(Id);
        }

        [Function("get-command-line")]
        public string GetCommandLine(int pid)
        {
            using (ManagementObject mo = new ManagementObject("win32_process.handle='" + pid.ToString() + "'"))
            {
                mo.Get();
                return (String)mo["Commandline"];
            }
        }

        public string GetCommandLine()
        {
            return this.GetCommandLine(GetCurrentPid());
        }

    }
}
