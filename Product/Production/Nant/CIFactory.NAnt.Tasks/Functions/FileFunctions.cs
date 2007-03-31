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

    [FunctionSet("file", "IO")]
    public class FileFunctions : FunctionSetBase
    {
        #region Constructors

        public FileFunctions(Project project, PropertyDictionary properties)
            : base(project, properties)
        {

        }

        #endregion

        #region Public Methods

        [Function("get-version")]
        public string GetVersion(string path)
        {
            return FileVersionInfo.GetVersionInfo(path).FileVersion;
        }

        #endregion

    }
}
