// NAnt - A .NET build tool
// Copyright (C) 2001-2003 Gerry Shaw
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// Ian Maclean (ian_maclean@another.com)
// Jaroslaw Kowalski (jkowalski@users.sourceforge.net)

using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security;

using NAnt.Core;
using NAnt.Core.Types;
using NAnt.Core.Attributes;

namespace NAnt.Core.Functions {
    [FunctionSet("file", "File")]
    public class FileFunctions : FunctionSetBase {
        #region Public Instance Constructors

        public FileFunctions(Project project, PropertyDictionary properties) : base(project, properties) {
        }

        #endregion Public Instance Constructors

        #region Public Instance Methods

        /// <summary>
        /// Returns the creation date and time of the specified file.
        /// </summary>
        /// <param name="path">The file for which to obtain creation date and time information.</param>
        /// <returns>
        /// The creation date and time of the specified file.
        /// </returns>
        /// <exception cref="IOException">The specified file does not exist.</exception>
        /// <exception cref="ArgumentException"><paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters.</exception>
        /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length.</exception>
        /// <exception cref="NotSupportedException">The <paramref name="path" /> parameter is in an invalid format.</exception>
        [Function("get-creation-time")]
        public DateTime GetCreationTime(string path) {
            return File.GetCreationTime(Project.GetFullPath(path));
        }

        /// <summary>
        /// Returns the date and time the specified file was last written to.
        /// </summary>
        /// <param name="path">The file for which to obtain write date and time information.</param>
        /// <returns>
        /// The date and time the specified file was last written to.
        /// </returns>
        /// <exception cref="IOException">The specified file does not exist.</exception>
        /// <exception cref="ArgumentException"><paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters.</exception>
        /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length.</exception>
        [Function("get-last-write-time")]
        public DateTime GetLastWriteTime(string path) {
            return File.GetLastWriteTime(Project.GetFullPath(path));
        }

        /// <summary>
        /// Returns the date and time the specified file was last accessed.
        /// </summary>
        /// <param name="path">The file for which to obtain access date and time information.</param>
        /// <returns>
        /// The date and time the specified file was last accessed.
        /// </returns>
        /// <exception cref="IOException">The specified file does not exist.</exception>
        /// <exception cref="ArgumentException"><paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters.</exception>
        /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length.</exception>
        /// <exception cref="NotSupportedException">The <paramref name="path" /> parameter is in an invalid format.</exception>
        [Function("get-last-access-time")]
        public DateTime GetLastAccessTime(string path) {
            return File.GetLastAccessTime(Project.GetFullPath(path));
        }

        /// <summary>
        /// Determines whether the specified file exists.
        /// </summary>
        /// <param name="file">The file to check.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="file" /> refers to an 
        /// existing file; otherwise, <see langword="false" />.
        /// </returns>
        /// <example>
        ///   <para>Execute a set of tasks, if file "output.xml" does not exist.</para>
        ///   <code>
        ///     <![CDATA[
        /// <if test="${not file::exists('output.xml')}">
        ///     ...
        /// </if>
        ///     ]]>
        ///   </code>
        /// </example>
        [Function("exists")]
        public bool Exists(string file) {
            return File.Exists(Project.GetFullPath(file));
        }

        /// <summary>
        /// Determines whether <paramref name="targetFile" /> is more or equal 
        /// up-to-date than <paramref name="srcFile" />.
        /// </summary>
        /// <param name="srcFile">The file to check against the target file.</param>
        /// <param name="targetFile">The file for which we want to determine the status.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="targetFile" /> is more 
        /// or equal up-to-date than <paramref name="srcFile" />; otherwise,
        /// <see langword="false" />.
        /// </returns>
        /// <exception cref="ArgumentException"><paramref name="srcFile" /> or <paramref name="targetFile" /> is a zero-length string, contains only white space, or contains one or more invalid characters.</exception>
        /// <exception cref="PathTooLongException">The specified path, file name, or both of either <paramref name="srcFile" /> or <paramref name="targetFile" /> exceed the system-defined maximum length.</exception>
        [Function("up-to-date")]
        public bool UpToDate(string srcFile, string targetFile) {
            string srcPath = Project.GetFullPath(srcFile);
            string targetPath = Project.GetFullPath(targetFile);

            if (!File.Exists(targetPath)) {
                // if targetFile does not exist, we consider it out-of-date
                return false;
            }

            // get lastwritetime of targetFile
            DateTime targetLastWriteTime = File.GetLastWriteTime(targetPath);

            // determine whether lastwritetime of srcFile is more recent
            // than lastwritetime or targetFile
            string newerFile = FileSet.FindMoreRecentLastWriteTime(
                srcPath, targetLastWriteTime);

            // return true if srcFile is not newer than target file
            return newerFile == null;
        }

        /// <summary>
        /// Gets the length of the file.
        /// </summary>
        /// <param name="file">filename</param>
        /// <returns>
        /// Length in bytes, of the file named <paramref name="file" />.
        /// </returns>
        /// <exception cref="FileNotFoundException">The file specified cannot be found.</exception>
        [Function("get-length")]
        public long GetLength(string file) {
            FileInfo fi = new FileInfo(Project.GetFullPath(file));
            return fi.Length;
        }

        /// <summary>
        /// Checks if a given file is an assembly.
        /// </summary>
        /// <param name="assemblyFile">The name or path of the file to be checked.</param>
        /// <returns>True if the file is a valid assembly, false if it's not or if the assembly seems corrupted (invalid headers or metadata).</returns>
        /// <exception cref="ArgumentNullException"><paramref name="assemblyFile" /> is a null <see cref="string"/>.</exception>
        /// <exception cref="ArgumentException"><paramref name="assemblyFile" /> is an empty <see cref="string"/>.</exception>
        /// <exception cref="FileNotFoundException"><paramref name="assemblyFile" /> is not found, or the file you are trying to check does not specify a filename extension.</exception>
        /// <exception cref="SecurityException">The caller does not have path discovery permission.</exception>
        /// <exception cref="FileLoadException">The file could not be loaded for some reason.</exception>
        [Function("is-assembly")]
        public bool IsAssembly(string assemblyFile) {
            try {
                AssemblyName.GetAssemblyName(Project.GetFullPath(assemblyFile));
                //no exception occurred, this is an assembly
                return true;
            } catch (FileLoadException fle) {
                if (fle.InnerException != null && fle.InnerException.GetType() == typeof(BadImageFormatException)) {
                    // this is probably not an assembly, or it has invalid headers / metadata
                    return false;
                }
                // other exceptions should be thrown to the end-user
                throw;
            } catch (BadImageFormatException) {
                return false;
            } catch (Exception) {
                // other exceptions should be thrown to the end-user
                throw;
            }
        }

        #endregion Public Instance Methods
    }
}
