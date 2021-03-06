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
using System.IO;
using System.Collections;
using System.Reflection;
using System.Globalization;

using NAnt.Core;
using NAnt.Core.Attributes;
using NAnt.Core.Types;
using NAnt.Core.Util;

namespace NAnt.Core.Functions {
    [FunctionSet("directory", "Directory")]
    public class DirectoryFunctions : FunctionSetBase {
        #region Public Instance Constructors

        public DirectoryFunctions(Project project, PropertyDictionary properties) : base(project, properties) {
        }

        #endregion Public Instance Constructors

        #region Public Instance Methods

        /// <summary>
        /// Returns the creation date and time of the specified directory.
        /// </summary>
        /// <param name="path">The directory for which to obtain creation date and time information.</param>
        /// <returns>
        /// The creation date and time of the specified directory.
        /// </returns>
        /// <exception cref="IOException">The specified directory does not exist.</exception>
        /// <exception cref="ArgumentException"><paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters.</exception>
        /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length.</exception>
        [Function("get-creation-time")]
        public DateTime GetCreationTime(string path) {
            return Directory.GetCreationTime(Project.GetFullPath(path));
        }       
        /// <summary>
        /// Gets the current working directory.
        /// </summary>
        /// <returns>
        /// A <see cref="string" /> containing the path of the current working 
        /// directory.
        ///</returns>
        [Function("get-current-directory")]
        public static string GetCurrentDirectory() {
            return Directory.GetCurrentDirectory();
        }

        /// <summary>
        /// Returns the date and time the specified directory was last written to.
        /// </summary>
        /// <param name="path">The directory for which to obtain write date and time information.</param>
        /// <returns>
        /// The date and time the specified directory was last written to.
        /// </returns>
        /// <exception cref="IOException">The specified directory does not exist.</exception>
        /// <exception cref="ArgumentException"><paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters.</exception>
        /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length.</exception>
        [Function("get-last-write-time")]
        public DateTime GetLastWriteTime(string path) {
            return Directory.GetLastWriteTime(Project.GetFullPath(path));
        }

        /// <summary>
        /// Returns the date and time the specified directory was last accessed.
        /// </summary>
        /// <param name="path">The directory for which to obtain access date and time information.</param>
        /// <returns>
        /// The date and time the specified directory was last accessed.
        /// </returns>
        /// <exception cref="IOException">The specified directory does not exist.</exception>
        /// <exception cref="ArgumentException"><paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters.</exception>
        /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length.</exception>
        /// <exception cref="NotSupportedException">The <paramref name="path" /> parameter is in an invalid format.</exception>
        [Function("get-last-access-time")]
        public DateTime GetLastAccessTime(string path) {
            return Directory.GetLastAccessTime(Project.GetFullPath(path));
        }

        /// <summary>
        /// Retrieves the parent directory of the specified path.
        /// </summary>
        /// <param name="path">The path for which to retrieve the parent directory.</param>
        /// <returns>
        /// The parent directory, or an empty <see cref="string" /> if 
        /// <paramref name="path" /> is the root directory, including the root 
        /// of a UNC server or share name.
        /// </returns>
        /// <exception cref="IOException">The directory specified by <paramref name="path" /> is read-only.</exception>
        /// <exception cref="ArgumentException"><paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters.</exception>
        /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length.</exception>
        /// <exception cref="DirectoryNotFoundException">The specified path was not found.</exception>
        /// <example>
        ///   <para>
        ///   Copy &quot;readme.txt&quot; from the current working directory to 
        ///   its parent directory.
        ///   </para>
        ///   <code>
        ///     <![CDATA[
        /// <property name="current.dir" value="${directory::get-current-directory()}" />
        /// <property name="current.dir.parent" value="${directory::get-parent-directory(current.dir)}" />
        /// <copy file="${path::combine(current.dir, 'readme.txt')} todir="${current.dir.parent}" />
        ///     ]]>
        ///   </code>
        /// </example>
        [Function("get-parent-directory")]
        public string GetParentDirectory(string path) {
            // do not use Directory.GetParent() as that will not return the
            // parent if the directory ends with the directory separator
            DirectoryInfo directory = new DirectoryInfo(Project.GetFullPath(path));
            DirectoryInfo parentDirectory = directory.Parent;
            return parentDirectory != null ? parentDirectory.FullName 
                : string.Empty;
        }

        /// <summary>
        /// Returns the volume information, root information, or both for the 
        /// specified path.
        /// </summary>
        /// <param name="path">The path for which to retrieve the parent directory.</param>
        /// <returns>
        /// A string containing the volume information, root information, or 
        /// both for the specified path.
        /// </returns>
        /// <exception cref="ArgumentException"><paramref name="path" /> is a zero-length string, contains only white space, or contains one or more invalid characters.</exception>
        /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length.</exception>
        [Function("get-directory-root")]
        public string GetDirectoryRoot(string path) {
            string directoryRoot = Directory.GetDirectoryRoot(
                Project.GetFullPath(path));
            return StringUtils.ConvertNullToEmpty(directoryRoot);
        }

        /// <summary>
        /// Determines whether the given path refers to an existing directory 
        /// on disk.
        /// </summary>
        /// <param name="path">The path to test.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="path" /> refers to an
        /// existing directory; otherwise, <see langword="false" />.
        /// </returns>
        /// <example>
        ///   <para>Remove directory "test", if it exists.</para>
        ///   <code>
        ///     <![CDATA[
        /// <delete dir="test" if="${directory::exists('test')}" />
        ///     ]]>
        ///   </code>
        /// </example>
        [Function("exists")]
        public bool Exists(string path) {
            return Directory.Exists(Project.GetFullPath(path));
        }

        #endregion Public Instance Methods
    }
}
