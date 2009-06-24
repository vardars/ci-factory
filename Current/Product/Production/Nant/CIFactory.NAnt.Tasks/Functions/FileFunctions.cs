using System;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using NAnt.Core;
using NAnt.Core.Types;
using NAnt.Core.Attributes;
using System.IO;

namespace CIFactory.NAnt.Functions
{
    [FunctionSet("file", "IO")]
    public class FileFunctions : FunctionSetBase
    {
        #region Constructors

        public FileFunctions(Project project, Location location, PropertyDictionary properties)
            : base(project, location, properties)
        {

        }

        #endregion

        #region Public Methods

        [Function("get-version")]
        public string GetVersion(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException(string.Format("File not found: {0}", path), path);

            return FileVersionInfo.GetVersionInfo(path).FileVersion;
        }

        [Function("get-product-version")]
        public string GetProductVersion(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException(string.Format("File not found: {0}", path), path);

            return FileVersionInfo.GetVersionInfo(path).ProductVersion;
        }

        #endregion

    }
}
