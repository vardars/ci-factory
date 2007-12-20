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
    public abstract class BuildInstallShieldBase : ExternalProgramBase
    {
        private const string UNINSTALL_KEY = "Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\";

        protected string m_sIsmFile;
        private string m_sRelease;
        private string m_sStandAloneBuildExe;
        private string m_sBuildLocation;
        private string m_sUpdateProductVersion;
        private bool m_bBuildSilently = false;
        private bool m_bStopOnError = false;
        private bool m_bWarningAsError = false;
        private bool m_bNoCompile = false;

        [TaskAttribute("p", Required = true)]
        public string p
        {
            get { return m_sIsmFile; }
            set { m_sIsmFile = value; }
        }

        [TaskAttribute("r", Required = false)]
        public string r
        {
            get { return m_sRelease; }
            set { m_sRelease = value; }
        }

        [TaskAttribute("standalonebuildexe", Required = true)]
        public string standalonebuildexe
        {
            get { return m_sStandAloneBuildExe; }
            set { m_sStandAloneBuildExe = value; }
        }

        [TaskAttribute("b", Required = false)]
        public string b
        {
            get { return m_sBuildLocation; }
            set { m_sBuildLocation = value; }
        }

        [TaskAttribute("updateproductversion", Required = false)]
        public string updateproductversion
        {
            get { return m_sUpdateProductVersion; }
            set { m_sUpdateProductVersion = value; }
        }

        [TaskAttribute("s", Required = false)]
        [BooleanValidator()]
        public bool s
        {
            get { return m_bBuildSilently; }
            set { m_bBuildSilently = value; }
        }

        [TaskAttribute("x", Required = false)]
        [BooleanValidator()]
        public bool x
        {
            get { return m_bStopOnError; }
            set { m_bStopOnError = value; }
        }

        [TaskAttribute("w", Required = false)]
        [BooleanValidator()]
        public bool w
        {
            get { return m_bWarningAsError; }
            set { m_bWarningAsError = value; }
        }

        [TaskAttribute("n", Required = false)]
        [BooleanValidator()]
        public bool n
        {
            get { return m_bNoCompile; }
            set { m_bNoCompile = value; }
        }

        public override string ProgramFileName
        {
            // The path and file name to the InstallShield stand alone build.
            get { return GetISSAExe(); }
        }

        // The base class calls this to build the command-line string.
        public abstract override string ProgramArguments
        {
            get;
        }

        protected override void ExecuteTask()
        {
            // we’ll let the base task do all the work.
            base.ExecuteTask();
        }

        protected string GetBaseArguments()
        {
            if (updateproductversion != null)
            {
                ChangeProductVersion();
            }

            string sCmdLine;
            sCmdLine = "-p " + "\"" + p + "\"";

            if (r != null)
            {
                sCmdLine += " -r " + "\"" + r + "\"";
            }

            if (b != null)
            {
                sCmdLine += " -b " + "\"" + b + "\"";
            }

            if (s)
            {
                sCmdLine += " -s";
            }

            if (w)
            {
                sCmdLine += " -w";
            }

            if (x)
            {
                sCmdLine += " -x";
            }

            if (n)
            {
                sCmdLine += " -n";
            }

            return sCmdLine;
        }

        private string GetISSAExe()
        {
            string sExe = m_sStandAloneBuildExe;
            if ('{' == sExe[0])
            {
                string sKey = Path.Combine(UNINSTALL_KEY, sExe);

                RegistryKey reg = Registry.LocalMachine.OpenSubKey(sKey);
                string sValue = (string)reg.GetValue("InstallLocation");

                sExe = Path.Combine(sValue, "IsSABld.exe");
                reg.Close();

                // format it
                sExe = " " + sExe + " ";
            }
            return sExe;
        }

        protected void ChangeProductVersion()
        {
            if (!File.Exists(m_sIsmFile))
            {
                return;
            }

            System.IO.FileAttributes fileAttribs = File.GetAttributes(m_sIsmFile);

            const string TEMP_FILE_EXT = ".tmp";
            string sTempFile = m_sIsmFile + TEMP_FILE_EXT;

            if (File.Exists(sTempFile))
            {
                File.SetAttributes(sTempFile, System.IO.FileAttributes.Normal);
                File.Delete(sTempFile);
            }

            bool bReplaced = false;

            try
            {
                using (StreamWriter sw = new StreamWriter(sTempFile))
                {
                    using (StreamReader sr = new StreamReader(m_sIsmFile))
                    {
                        const string BEGIN_PROPERTY_TABLE = "<table name=\"Property\">";
                        const string END_TABLE = "</table>";
                        const string END_OF_ELEMENT_START_OF_NEW = "</td><td>";
                        const string ELEM_VALUE_TO_REPLACE = "ProductVersion";

                        bool bFoundPropertySection = false;
                        bool bFoundEndOfPropertySection = false;

                        int nPos = -1;
                        string sLine;
                        while ((sLine = sr.ReadLine()) != null)
                        {
                            if (!bReplaced && !bFoundEndOfPropertySection)
                            {
                                if (bFoundPropertySection)
                                {
                                    bFoundEndOfPropertySection = (-1 < sLine.LastIndexOf(END_TABLE));
                                }
                                else
                                {
                                    bFoundPropertySection = (-1 < sLine.LastIndexOf(BEGIN_PROPERTY_TABLE));
                                }

                                if (!bFoundEndOfPropertySection && bFoundPropertySection && -1 < (nPos = sLine.LastIndexOf(ELEM_VALUE_TO_REPLACE)))
                                {
                                    // Found "ProductVersion"

                                    nPos += ELEM_VALUE_TO_REPLACE.Length + END_OF_ELEMENT_START_OF_NEW.Length;
                                    string sBegin = sLine.Substring(0, nPos);

                                    // Find the end of the ProductVersion value
                                    // (if the version is "1.0.0.0", find the
                                    // position of the last '0')
                                    while (sLine[nPos++] != '<') ;

                                    --nPos;
                                    string sEnd = sLine.Substring(nPos, sLine.Length - nPos);


                                    sLine = sBegin + m_sUpdateProductVersion + sEnd;
                                    bReplaced = true;
                                }
                            }
                            sw.WriteLine(sLine);
                        }
                    }
                }
                if (bReplaced)
                {
                    File.SetAttributes(m_sIsmFile, System.IO.FileAttributes.Normal);
                    File.Delete(m_sIsmFile);
                    File.Move(sTempFile, m_sIsmFile);
                    File.SetAttributes(m_sIsmFile, fileAttribs);
                }
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.ToString(), ex.Message);
                return;
            }

            Debug.Assert(bReplaced);
        }
    }
}
