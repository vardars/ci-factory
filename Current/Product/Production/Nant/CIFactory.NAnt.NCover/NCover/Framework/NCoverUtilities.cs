namespace NCover.Framework
{
    using Microsoft.Win32;
    using System;
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static class NCoverUtilities
    {
        private const string NCOVER_CLSID_KEY_NAME = @"Software\Classes\CLSID\{6287B5F9-08A1-45e7-9498-B5B2E7B02995}";
        private const string NCOVER_PROFILER_CLSID = "{6287B5F9-08A1-45e7-9498-B5B2E7B02995}";
        private const string NCOVER_REFCOUNT = "NCoverRefCount";
        private const string NCOVEREXPLORER_KEY_NAME = @"Software\Gnoso\NCoverExplorer";

        private static bool _ConsoleCtrlCheck(CtrlTypes ctrlType)
        {
            UnregisterNCover();
            return false;
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

        public static string GetNCoverPath(string ncoverPath)
        {
            if (ncoverPath == null)
            {
                ncoverPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                if (!File.Exists(ncoverPath + @"\ncover.console.exe"))
                {
                    ncoverPath = @"C:\Program Files\NCover";
                }
            }
            if (!Directory.Exists(ncoverPath) || (Directory.Exists(ncoverPath) && !File.Exists(ncoverPath + @"\ncover.console.exe")))
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

        public static void RegisterNCover(string ncoverPath, int versionNumber)
        {
            ncoverPath = GetNCoverPath(ncoverPath);
            int num = 0;
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Gnoso\NCoverExplorer"))
            {
                if (key.GetValue("NCoverRefCount") != null)
                {
                    num = (int) key.GetValue("NCoverRefCount");
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
                    num = ((int) key.GetValue("NCoverRefCount")) - 1;
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
