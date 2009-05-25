using System;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using NAnt.Core;
using NAnt.Core.Types;
using NAnt.Core.Attributes;
using NDepend.Helpers.FileDirectoryPath;

namespace CIFactory.NAnt.Functions
{
    [FunctionSet("path", "IO")]
    public class PathFunctions : FunctionSetBase
    {
        #region Constructors

        public PathFunctions(Project project, Location location, PropertyDictionary properties)
            : base(project, location, properties)
        {
        }

        #endregion

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern uint GetShortPathName(
           [MarshalAs(UnmanagedType.LPTStr)]
		           string lpszLongPath,
           [MarshalAs(UnmanagedType.LPTStr)]
		           StringBuilder lpszShortPath,
           uint cchBuffer);

        [Function("get-short-path")]
        public string GetShortPath(string longName)
        {
            StringBuilder shortNameBuffer = new StringBuilder(256);
            uint bufferSize = (uint)shortNameBuffer.Capacity;

            uint result = GetShortPathName(longName, shortNameBuffer, bufferSize);

            if (result != 0)
                return shortNameBuffer.ToString();

            throw new BuildException("Failed to convert to short name.");
        }

        [Function("get-relative-path")]
        public string GetRelativePath(string absoluteFilePath, string absoluteDirectoryPath)
        {
            FilePathAbsolute filePathAbsolute1 = new FilePathAbsolute(absoluteFilePath);
            DirectoryPathAbsolute directoryPathAbsolute1 = new DirectoryPathAbsolute(absoluteDirectoryPath);
            FilePathRelative filePathRelative1 = filePathAbsolute1.GetPathRelativeFrom(directoryPathAbsolute1);
            return filePathRelative1.Path;
        }
    }
}
