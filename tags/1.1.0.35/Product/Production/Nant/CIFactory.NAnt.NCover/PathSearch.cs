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
    public static class PathSearch
    {
        // Fields
        private static string[] _pathExtensions = new string[0];

        // Methods
        private static string[] _GetPathsSplit(string pathString)
        {
            string[] strArray = pathString.Split(new char[] { Path.PathSeparator });
            string str = Environment.CurrentDirectory.ToLower();
            List<string> list = new List<string>();
            foreach (string str2 in strArray)
            {
                if (!str2.ToLower().Equals(str) && !str2.Equals("."))
                {
                    list.Add(str2);
                }
            }
            list.Insert(0, ".");
            return list.ToArray();
        }

        private static string _GetRegExString(string executableName)
        {
            executableName = executableName.ToLower();
            string str = "^" + executableName;
            bool flag = false;
            foreach (string str2 in _pathExtensions)
            {
                if (executableName.EndsWith(str2.ToLower()))
                {
                    flag = true;
                    break;
                }
            }
            if ((flag || (_pathExtensions == null)) || (_pathExtensions.Length == 0))
            {
                return (str + "$");
            }
            str = str + "(?:";
            foreach (string str3 in _pathExtensions)
            {
                str = str + @"\" + str3 + "|";
            }
            return (str + ")$");
        }

        public static string GetVersionForExecutable(string executablePath, bool throwExceptionIfNotFound)
        {
            bool flag = false;
            if (executablePath.Length != 0)
            {
                if (!File.Exists(executablePath))
                {
                    string[] strArray = Search(Path.GetFileName(executablePath));
                    if (strArray.Length > 0)
                    {
                        executablePath = strArray[0];
                        flag = true;
                    }
                }
                else
                {
                    flag = true;
                }
            }
            if (flag)
            {
                FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(executablePath);
                return string.Concat(new object[] { versionInfo.FileMajorPart, ".", versionInfo.FileMinorPart, ".", versionInfo.FileBuildPart });
            }
            if (throwExceptionIfNotFound)
            {
                throw new FileNotFoundException("Executable could not be found. Please specify a full path or add folder to your path.");
            }
            return string.Empty;
        }

        public static string[] Search(string executableName)
        {
            string pathString = "";
            List<string> list = new List<string>();
            foreach (DictionaryEntry entry in Environment.GetEnvironmentVariables())
            {
                if (entry.Key.ToString().ToUpper().Equals("PATH"))
                {
                    pathString = entry.Value.ToString().Trim();
                }
                if (entry.Key.ToString().ToUpper().Equals("PATHEXT"))
                {
                    _pathExtensions = entry.Value.ToString().Trim().Split(new char[] { ';' });
                }
            }
            Regex regex = new Regex(_GetRegExString(executableName), RegexOptions.IgnoreCase);
            foreach (string str2 in _GetPathsSplit(pathString))
            {
                string path = str2.Trim();
                if (path.Length != 0)
                {
                    DirectoryInfo info = new DirectoryInfo(path);
                    if (info.Exists)
                    {
                        foreach (FileInfo info2 in info.GetFiles())
                        {
                            if (regex.IsMatch(info2.Name))
                            {
                                list.Add(info2.FullName);
                            }
                        }
                    }
                }
            }
            return list.ToArray();
        }
    }
}
