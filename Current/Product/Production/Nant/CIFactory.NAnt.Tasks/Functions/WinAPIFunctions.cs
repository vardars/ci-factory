using System;
using System.Text;
using System.Runtime.InteropServices;
using NAnt.Core;
using NAnt.Core.Types;
using NAnt.Core.Attributes;

namespace CIFactory.NAnt.Functions
{
    [FunctionSet("winapi", "WinAPI")]
    class WinAPIFunctions : FunctionSetBase
    {
        [DllImport("User32.dll")]
        public static extern Int32 FindWindow(String lpClassName, String lpWindowName);
        [DllImport("User32.dll")]
        public static extern Int32 SetWindowText(int hWnd, string s);

        #region Constructors

        public WinAPIFunctions(Project project, Location location, PropertyDictionary properties)
            : base(project, location, properties)
        {

        }

        public WinAPIFunctions()
            : base(null, null, null)
        {
        }

        #endregion

        [Function("set-winapp-title")]
        public bool SetWinAppTitle(string currentTitle, string toTitle)
        {
            int hWnd = FindWindow(null, currentTitle);
            if (hWnd == 0) return false;
            if (hWnd > 0) SetWindowText(hWnd, toTitle);
            return true;
        }
    }
}
