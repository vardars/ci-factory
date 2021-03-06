// NAnt - A .NET build tool
// Copyright (C) 2001 Gerry Shaw
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

using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;

using NAnt.Core.Filters;
using NAnt.Core.Types;

namespace NAnt.Core.Util {
    /// <summary>
    /// Provides modified version for Copy and Move from the File class that 
    /// allow for filter chain processing.
    /// </summary>
    public sealed class FileUtils {
        private FileUtils() {
        }

        #region Public Static Methods

        /// <summary>
        /// Copies a file filtering its content through the filter chain.
        /// </summary>
        /// <param name="sourceFileName">The file to copy</param>
        /// <param name="destFileName">The file to copy to</param>
        /// <param name="filterChain">Chain of filters to apply when copying, or <see langword="null" /> is no filters should be applied.</param>
        /// <param name="inputEncoding">The encoding used to read the soure file.</param>
        /// <param name="outputEncoding">The encoding used to write the destination file.</param>
        public static void CopyFile(string sourceFileName, string destFileName, FilterChain filterChain, Encoding inputEncoding, Encoding outputEncoding) {
            // determine if filters are available
            bool filtersAvailable = filterChain != null && filterChain.Filters.Count > 0;

            // if no filters have been defined, and no input or output encoding
            // is set, we can just use the File.Copy method
            if (!filtersAvailable && inputEncoding == null && outputEncoding == null) {
                File.Copy(sourceFileName, destFileName, true);
            } else {
                // determine actual input encoding to use. if no explicit input
                // encoding is specified, we'll use the system's current ANSI
                // code page
                Encoding actualInputEncoding = (inputEncoding != null) ? 
                inputEncoding : Encoding.Default;

                // get base filter built on the file's reader. Use a 8k buffer.
                using (StreamReader sourceFileReader = new StreamReader(sourceFileName, actualInputEncoding, true, 8192)) {
                    Encoding actualOutputEncoding = outputEncoding;
                    if (actualOutputEncoding == null) {
                        // if no explicit output encoding is specified, we'll
                        // just use the encoding of the input file as determined
                        // by the runtime
                        // 
                        // Note : the input encoding as specified on the filterchain
                        // might not match the current encoding of the streamreader
                        //
                        // eg. when specifing an ANSI encoding, the runtime might
                        // still detect the file is using UTF-8 encoding, because 
                        // we use BOM detection
                        actualOutputEncoding = sourceFileReader.CurrentEncoding;
                    }

                    // writer for destination file
                    using (StreamWriter destFileWriter = new StreamWriter(destFileName, false, actualOutputEncoding, 8192)) {
                        if (filtersAvailable) {
                            Filter baseFilter = filterChain.GetBaseFilter(new PhysicalTextReader(sourceFileReader));

                            bool atEnd = false;
                            int character;
                            while (!atEnd) {
                                character = baseFilter.Read();
                                if (character > -1) {
                                    destFileWriter.Write((char)character);
                                } else {
                                    atEnd = true;
                                }
                            }
                        } else {
                            char[] buffer = new char[8192];

                            while (true) {
                                int charsRead = sourceFileReader.Read(buffer, 0, buffer.Length);
                                if (charsRead == 0) {
                                    break;
                                }
                                destFileWriter.Write(buffer, 0, charsRead);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Moves a file filtering its content through the filter chain.
        /// </summary>
        /// <param name="sourceFileName">The file to move</param>
        /// <param name="destFileName">The file to move move to</param>
        /// <param name="filterChain">Chain of filters to apply when moving, or <see langword="null" /> is no filters should be applied.</param>
        /// <param name="inputEncoding">The encoding used to read the soure file.</param>
        /// <param name="outputEncoding">The encoding used to write the destination file.</param>
        public static void MoveFile(string sourceFileName, string destFileName, FilterChain filterChain, Encoding inputEncoding, Encoding outputEncoding) {
            // if no filters have been defined, and no input or output encoding
            // is set, we can just use the File.Move method
            if ((filterChain == null || filterChain.Filters.Count == 0) && inputEncoding == null && outputEncoding == null) {
                File.Move(sourceFileName, destFileName);
            } else {
                CopyFile(sourceFileName, destFileName, filterChain, inputEncoding, outputEncoding);
                File.Delete(sourceFileName);
            }
        }

        /// <summary>
        /// Returns a uniquely named empty temporary directory on disk.
        /// </summary>
        /// <value>
        /// A <see cref="DirectoryInfo" /> representing the temporary directory.
        /// </value>
        public static DirectoryInfo GetTempDirectory() {
            // create a uniquely named zero-byte file
            string tempFile = Path.GetTempFileName();
            // remove the temporary file
            File.Delete(tempFile);
            // create a directory named after the unique temporary file
            Directory.CreateDirectory(tempFile);
            // return the 
            return new DirectoryInfo(tempFile);
        }

        /// <summary>
        /// Combines two path strings.
        /// </summary>
        /// <param name="path1">The first path.</param>
        /// <param name="path2">The second path.</param>
        /// <returns>
        /// A string containing the combined paths. If one of the specified 
        /// paths is a zero-length string, this method returns the other path. 
        /// If <paramref name="path2" /> contains an absolute path, this method 
        /// returns <paramref name="path2" />.
        /// </returns>
        /// <remarks>
        ///   <para>
        ///   On *nix, processing is delegated to <see cref="Path.Combine(string, string)" />.
        ///   </para>
        ///   <para>
        ///   On Windows, this method normalized the paths to avoid running into
        ///   the 260 character limit of a path and converts forward slashes in 
        ///   both <paramref name="path1" /> and <paramref name="path2" /> to 
        ///   the platform's directory separator character.
        ///   </para>
        /// </remarks>
        public static string CombinePaths(string path1, string path2) {
            if (PlatformHelper.IsUnix) {
                return Path.Combine(path1, path2);
            }

            if (path1 == null) {
                throw new ArgumentNullException("path1");
            }
            if (path2 == null) {
                throw new ArgumentNullException("path2");
            }

            char separatorChar = Path.DirectorySeparatorChar;
            char[] splitChars = new char[] {'/', separatorChar};

            // Now we split the Path by the Path Separator
            String[] path2Parts = path2.Split(splitChars);

            ArrayList arList = new ArrayList();
            
            // for each Item in the path that differs from ".." we just add it to the ArrayList
            for (int iCount = 0; iCount < path2Parts.Length; iCount++) {
                // If we get a ".." Try to remove the last item added (as if going up in the Directory Structure)
                if (path2Parts[iCount] == "..") {
                    if (arList.Count > 0 && ((string) arList[arList.Count - 1] != "..")) {
                        arList.RemoveAt(arList.Count -1);
                    } else {
                        arList.Add(path2Parts[iCount]);
                    }
                } else {
                    arList.Add(path2Parts[iCount]);
                }
            }

            string[] path1Parts = path1.Split(splitChars);
            int counter = path1Parts.Length;

            // if the second path starts with parts to move up the directory tree, 
            // then remove corresponding parts in the first path
            //
            // eg. path1 = d:\whatever\you\want\to\do 
            //     path2 = ../../test
            //     
            //     ->
            //
            //     path1 = d:\whatever\you\want
            //     path2 = test
            ArrayList arList2 = (ArrayList) arList.Clone();
            for (int i = 0; i < arList2.Count; i++) {
                if ((string) arList2[i] != ".." || counter < 3) {
                    break;
                }

                // skip part of current directory
                counter--;

                arList.RemoveAt(0);
            }

            string separatorString = separatorChar.ToString(CultureInfo.InvariantCulture);

            return Path.Combine(string.Join(separatorString, path1Parts,
                0, counter), string.Join(separatorString, (String[]) arList.ToArray(typeof(String))));
        }

        /// <summary>
        /// Returns Absolute Path (Fix for 260 Char Limit of Path.GetFullPath(...))
        /// </summary>
        /// <param name="path">The file or directory for which to obtain absolute path information.</param>
        /// <returns>Path Resolved</returns>
        /// <exception cref="ArgumentException">path is a zero-length string, contains only white space or contains one or more invalid characters as defined by <see cref="Path.InvalidPathChars" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="path" /> is <see langword="null" />.</exception>
        public static string GetFullPath(string path) {
            if (path == null) {
                throw new ArgumentNullException("path");
            }

            if (PlatformHelper.IsUnix || Path.IsPathRooted(path)) {
                return Path.GetFullPath(path);
            }

            if (path.Length == 0 || path.Trim().Length == 0 || path.IndexOfAny(Path.InvalidPathChars) != -1) {
                throw new ArgumentException("The path is not of a legal form.");
            }

            string combinedPath = FileUtils.CombinePaths(
                Directory.GetCurrentDirectory(), path);

            return Path.GetFullPath(combinedPath);
        }

        #endregion Public Static Methods
    }
}
