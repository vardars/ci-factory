#region Copyright © 2006 Grant Drake. All rights reserved.
/*
Copyright © 2006 Grant Drake. All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions
are met:

1. Redistributions of source code must retain the above copyright
   notice, this list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright
   notice, this list of conditions and the following disclaimer in the
   documentation and/or other materials provided with the distribution.
3. The name of the author may not be used to endorse or promote products
   derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE AUTHOR "AS IS" AND ANY EXPRESS OR
IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 
*/
#endregion

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using NAnt.Core;
using NAnt.Core.Tasks;
using NAnt.Core.Attributes;
using NAnt.Core.Types;
using NAnt.Core.Util;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace NCoverExplorer.NAntTasks
{
    public static class NCoverUtilities
    {
        // Fields
        private const string NCOVER_CLSID_KEY_NAME = @"Software\Classes\CLSID\{6287B5F9-08A1-45e7-9498-B5B2E7B02995}";
        private const string NCOVER_PROFILER_CLSID = "{6287B5F9-08A1-45e7-9498-B5B2E7B02995}";
        private const string NCOVER_REFCOUNT = "NCoverRefCount";
        private const string NCOVEREXPLORER_KEY_NAME = @"Software\Gnoso\NCoverExplorer";

        // Methods
        private static bool _ConsoleCtrlCheck(CtrlTypes ctrlType)
        {
            UnregisterNCover();
            return false;
        }

        private static string _GetNCoverPath(string ncoverPath)
        {
            if (ncoverPath == null)
            {
                ncoverPath = @"C:\Program Files\NCover";
            }
            if (!Directory.Exists(ncoverPath))
            {
                string[] strArray = PathSearch.Search("ncover.console.exe");
                if (strArray.Length <= 0)
                {
                    throw new FileNotFoundException("Could not find NCover folder in your path.");
                }
                ncoverPath = Path.GetDirectoryName(strArray[0]);
            }
            return ncoverPath;
        }

        private static void _RegisterNCoverPre20InHKCURegistry(string ncoverPath)
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Classes\CLSID\{6287B5F9-08A1-45e7-9498-B5B2E7B02995}"))
            {
                key.SetValue(null, "NCover Profiler Object");
                using (RegistryKey key2 = key.CreateSubKey("InprocServer32"))
                {
                    string str = Path.Combine(ncoverPath, "CoverLib.dll");
                    key2.SetValue(null, str);
                    key2.SetValue("ThreadingModel", "Both");
                }
            }
        }

        public static void RegisterNCover(string ncoverPath, int versionNumber)
        {
            ncoverPath = _GetNCoverPath(ncoverPath);
            int num = 0;
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Gnoso\NCoverExplorer"))
            {
                if (key.GetValue("NCoverRefCount") != null)
                {
                    num = (int)key.GetValue("NCoverRefCount");
                }
                num++;
                key.SetValue("NCoverRefCount", num);
            }
            if (versionNumber < 200)
            {
                _RegisterNCoverPre20InHKCURegistry(ncoverPath);
            }
            SetConsoleCtrlHandler(new HandlerRoutine(NCoverUtilities._ConsoleCtrlCheck), true);
        }

        public static bool Running32BitArchitecture()
        {
            return (4 == IntPtr.Size);
        }

        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool Add);
        public static void UnregisterNCover()
        {
            SetConsoleCtrlHandler(new HandlerRoutine(NCoverUtilities._ConsoleCtrlCheck), false);
            int num = 0;
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Gnoso\NCoverExplorer"))
            {
                if (key.GetValue("NCoverRefCount") != null)
                {
                    num = ((int)key.GetValue("NCoverRefCount")) - 1;
                    num = Math.Max(num, 0);
                }
                key.SetValue("NCoverRefCount", num);
            }
            if (num == 0)
            {
                RegistryKey key2 = Registry.CurrentUser.OpenSubKey(@"Software\Classes\CLSID\{6287B5F9-08A1-45e7-9498-B5B2E7B02995}");
                if (key2 != null)
                {
                    key2.Close();
                    Registry.CurrentUser.DeleteSubKeyTree(@"Software\Classes\CLSID\{6287B5F9-08A1-45e7-9498-B5B2E7B02995}");
                }
            }
        }

        // Nested Types
        public enum CtrlTypes
        {
            CTRL_BREAK_EVENT = 1,
            CTRL_C_EVENT = 0,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_VENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        private delegate bool HandlerRoutine(NCoverUtilities.CtrlTypes CtrlType);
    }
}
