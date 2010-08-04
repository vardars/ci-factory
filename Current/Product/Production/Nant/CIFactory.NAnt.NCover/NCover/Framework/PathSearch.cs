namespace NCover.Framework
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Text.RegularExpressions;

    public static class PathSearch
    {
        private static string[] _pathExtensions = new string[0];

        private static string[] _GetPathsSplit(string path)
        {
            string[] strArray = path.Split(new char[] { Path.PathSeparator });
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

        private static DirectoryInfo _SafeGetDirectoryInfo(string path)
        {
            DirectoryInfo info = null;
            try
            {
                info = new DirectoryInfo(path);
            }
            catch (Exception exception)
            {
                Trace.WriteLine(string.Format("Caught an exception while in _SafeGetDirectoryInfo. Path[{0}]. Exception:{1}{3}{3}{2}", new object[] { path, exception.Message, exception.StackTrace, Environment.NewLine }));
            }
            return info;
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
            if (Assembly.GetEntryAssembly() != null)
            {
                Uri uri = new Uri(Assembly.GetEntryAssembly().CodeBase);
                string str = Path.Combine(Path.GetDirectoryName(uri.AbsolutePath), executableName);
                if (File.Exists(str))
                {
                    return new string[] { str };
                }
            }
            string path = "";
            List<string> list = new List<string>();
            foreach (DictionaryEntry entry in Environment.GetEnvironmentVariables())
            {
                if (entry.Key.ToString().ToUpper().Equals("PATH"))
                {
                    path = entry.Value.ToString().Trim();
                }
                if (entry.Key.ToString().ToUpper().Equals("PATHEXT"))
                {
                    _pathExtensions = entry.Value.ToString().Trim().Split(new char[] { ';' });
                }
            }
            Regex regex = new Regex(_GetRegExString(executableName), RegexOptions.IgnoreCase);
            foreach (string str3 in _GetPathsSplit(path))
            {
                string str4 = str3.Trim();
                if (str4.Length != 0)
                {
                    DirectoryInfo info = _SafeGetDirectoryInfo(str4);
                    if ((info != null) && info.Exists)
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
