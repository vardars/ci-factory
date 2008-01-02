using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;
using Microsoft.Win32;
using NAnt;
using NAnt.Core;
using NAnt.Core.Attributes;
using NAnt.Core.Tasks;
using NAnt.Core.Types;
using NAnt.Contrib.Types;

namespace CIFactory.NAnt.InstallShield.Tasks
{
    [TaskName("buildinstallshieldmsi")]
    public class BuildInstallShieldMsi : BuildInstallShieldBase
    {
        private string m_sProductConfiguration;
        private string m_sReleaseConfiguration;
        private string m_sMergeModuleSearchPath;
        private bool m_bSkipUpgrade = false;
        private string m_sDotNetFrameworkPath;
        private string m_sMinimumTargetMsiVersion;
        private string m_sMinimumTargetDotNetFrameworkVersion;
        private bool m_bCreateSetupExe = false;
        private string m_sReleaseFlags;
        private bool m_bQ1 = false;
        private bool m_bQ2 = false;
        private bool m_bQ3 = false;

        [TaskAttribute("a", Required = false)]
        public string a
        {
            get { return m_sProductConfiguration; }
            set { m_sProductConfiguration = value; }
        }

        [TaskAttribute("c", Required = false)]
        public string c
        {
            get { return m_sReleaseConfiguration; }
            set { m_sReleaseConfiguration = value; }
        }

        [TaskAttribute("o", Required = false)]
        public string o
        {
            get { return m_sMergeModuleSearchPath; }
            set { m_sMergeModuleSearchPath = value; }
        }

        [TaskAttribute("h", Required = false)]
        [BooleanValidator()]
        public bool h
        {
            get { return m_bSkipUpgrade; }
            set { m_bSkipUpgrade = value; }
        }

        [TaskAttribute("t", Required = false)]
        public string t
        {
            get { return m_sDotNetFrameworkPath; }
            set { m_sDotNetFrameworkPath = value; }
        }

        [TaskAttribute("g", Required = false)]
        public string g
        {
            get { return m_sMinimumTargetMsiVersion; }
            set { m_sMinimumTargetMsiVersion = value; }
        }

        [TaskAttribute("j", Required = false)]
        public string j
        {
            get { return m_sMinimumTargetDotNetFrameworkVersion; }
            set { m_sMinimumTargetDotNetFrameworkVersion = value; }
        }

        [TaskAttribute("e", Required = false)]
        [BooleanValidator()]
        public bool e
        {
            get { return m_bCreateSetupExe; }
            set { m_bCreateSetupExe = value; }
        }

        [TaskAttribute("f", Required = false)]
        public string f
        {
            get { return m_sReleaseFlags; }
            set { m_sReleaseFlags = value; }
        }

        [TaskAttribute("q1", Required = false)]
        [BooleanValidator()]
        public bool q1
        {
            get { return m_bQ1; }
            set { m_bQ1 = value; }
        }

        [TaskAttribute("q2", Required = false)]
        [BooleanValidator()]
        public bool q2
        {
            get { return m_bQ2; }
            set { m_bQ2 = value; }
        }

        [TaskAttribute("q3", Required = false)]
        [BooleanValidator()]
        public bool q3
        {
            get { return m_bQ3; }
            set { m_bQ3 = value; }
        }

        // The base class calls this to build the command-line string.
        public override string ProgramArguments
        {
            get
            {
                string sCmdLine = base.GetBaseArguments();

                if (null != a)
                {
                    sCmdLine += " -a " + "\"" + a + "\"";
                }

                if (null != o)
                {
                    sCmdLine += " -o " + "\"" + o + "\"";
                }

                if (null != c)
                {
                    sCmdLine += " -c " + "\"" + c + "\"";
                }

                if (h)
                {
                    sCmdLine += " -h";
                }

                if (null != t)
                {
                    sCmdLine += " -t " + "\"" + t + "\"";
                }

                if (null != g)
                {
                    sCmdLine += " -g " + "\"" + g + "\"";
                }

                if (null != j)
                {
                    sCmdLine += " -j " + "\"" + j + "\"";
                }

                if (e)
                {
                    sCmdLine += " -e y";
                }
                else
                {
                    sCmdLine += " -e n";
                }

                if (null != f)
                {
                    sCmdLine += " -f " + "\"" + f + "\"";
                }

                if (q1)
                {
                    sCmdLine += " -q1";
                }

                if (q2)
                {
                    sCmdLine += " -q2";
                }

                if (q3)
                {
                    sCmdLine += " -q3";
                }

                return sCmdLine;
            }
        }
    }
}
