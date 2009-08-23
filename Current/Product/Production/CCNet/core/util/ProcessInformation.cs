using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Management;
using System.Collections;
using System.Threading;
using System.Xml.Serialization;
using System.IO;

namespace ThoughtWorks.CruiseControl.Core.Util
{
    public class ProcessInformation
    {

        #region Fields

        private ProcessInformationList _Children;

        private int _Id;

        private long _PagedMemory;

        private long _PrivateMemory;

        private string _ProcessName;

        private String _ProcessorTime;

        private string _StartTime;

        private long _VirtualMemory;

        #endregion

        #region Constructors

        public ProcessInformation(ProcessInformationList children, int id, long pagedMemory, long privateMemory, string processName, String processorTime, long virtualMemory, string startTime)
        {
            _Children = children;
            _Id = id;
            _PagedMemory = pagedMemory;
            _PrivateMemory = privateMemory;
            _ProcessName = processName;
            _ProcessorTime = processorTime;
            _VirtualMemory = virtualMemory;
            _StartTime = startTime;
        }

        public ProcessInformation(Process process)
        {
            this.Id = process.Id;
            this.ProcessName = process.ProcessName;
            this.ProcessorTime = process.TotalProcessorTime.ToString();
            this.VirtualMemory = process.VirtualMemorySize64;
            this.PagedMemory = process.PagedMemorySize64;
            this.PrivateMemory = process.PrivateMemorySize64;
            this.StartTime = process.StartTime.ToString();

            Process[] allProcesses = Process.GetProcesses();
            foreach (Process processCandidate in allProcesses)
            {
                if (GetParentProcess(processCandidate.Id) == this.Id)
                {
                    this.Children.Add(new ProcessInformation(processCandidate, allProcesses));
                }
            }
        }

        public ProcessInformation()
        {
        }

        public ProcessInformation(Process process, Process[] allProcesses)
        {
            this.Id = process.Id;
            this.ProcessName = process.ProcessName;
            this.ProcessorTime = process.TotalProcessorTime.ToString();
            this.VirtualMemory = process.VirtualMemorySize64;
            this.PagedMemory = process.PagedMemorySize64;
            this.PrivateMemory = process.PrivateMemorySize64;
            this.StartTime = process.StartTime.ToString();

            foreach (Process processCandidate in allProcesses)
            {
                if (GetParentProcess(processCandidate.Id) == this.Id)
                {
                    this.Children.Add(new ProcessInformation(processCandidate, allProcesses));
                }
            }
        }

        #endregion

        #region Properties

        public ProcessInformationList Children
        {
            get
            {
                if (_Children == null)
                    _Children = new ProcessInformationList();
                return _Children;
            }
            set
            {
                _Children = value;
            }
        }

        public int Id
        {
            get { return _Id; }
            set
            {
                _Id = value;
            }
        }

        public long PagedMemory
        {
            get { return _PagedMemory; }
            set
            {
                _PagedMemory = value;
            }
        }

        public long PrivateMemory
        {
            get { return _PrivateMemory; }
            set
            {
                _PrivateMemory = value;
            }
        }

        public string ProcessName
        {
            get { return _ProcessName; }
            set
            {
                _ProcessName = value;
            }
        }

        public String ProcessorTime
        {
            get { return _ProcessorTime; }
            set
            {
                _ProcessorTime = value;
            }
        }

        public string StartTime
        {
            get { return _StartTime; }
            set
            {
                _StartTime = value;
            }
        }

        public long VirtualMemory
        {
            get { return _VirtualMemory; }
            set
            {
                _VirtualMemory = value;
            }
        }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return String.Format("ProcessInformation: Name='{0}', Pid='{1}', ProcessorTime='{2}', PrivateMemory='{3}', VirtualMemory='{4}', PagedMemory='{5}', StartTime='{6}'.",
                this.ProcessName,
                this.Id,
                this.ProcessorTime,
                this.PrivateMemory,
                this.VirtualMemory,
                this.PagedMemory,
                this.StartTime);
        }

        #endregion

        #region Private Methods

        private int GetParentProcess(int Id)
        {
            int parentPid = 0;
            using (ManagementObject mo = new ManagementObject("win32_process.handle='" + Id.ToString() + "'"))
            {
                try
                {
                    mo.Get();
                    parentPid = Convert.ToInt32(mo["ParentProcessId"]);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }
            return parentPid;
        }

        #endregion

    }
}
