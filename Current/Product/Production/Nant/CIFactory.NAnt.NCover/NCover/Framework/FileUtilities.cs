namespace NCover.Framework
{
    using NCover.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;

    public static class FileUtilities
    {
        public static List<string> ConvertFileGlobListIntoFilePaths(List<string> globs, string defaultDirectory)
        {
            List<string> list = new List<string>();
            for (int i = 0; i < globs.Count; i++)
            {
                string[] files = new string[0];
                try
                {
                    string directoryName = Path.GetDirectoryName(globs[i]);
                    if (string.IsNullOrEmpty(directoryName))
                    {
                        directoryName = defaultDirectory;
                    }
                    files = Directory.GetFiles(directoryName, Path.GetFileName(globs[i]));
                }
                catch (Exception)
                {
                    Trace.WriteLine("No files matching '" + globs[i] + "' were found.");
                }
                if (files.Length > 0)
                {
                    foreach (string str2 in files)
                    {
                        string item = str2.ToLower();
                        if (!list.Contains(item))
                        {
                            list.Add(item);
                        }
                    }
                }
            }
            return list;
        }

        public static List<string> ConvertStringPathToAssemblyList(List<string> list)
        {
            List<string> list2 = new List<string>();
            try
            {
                for (int i = 0; i < list.Count; i++)
                {
                    string path = list[i].Trim();
                    if (path.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase) || path.EndsWith(".exe", StringComparison.InvariantCultureIgnoreCase))
                    {
                        list2.Add(Path.GetFileNameWithoutExtension(path));
                    }
                    else
                    {
                        list2.Add(Path.GetFileName(path));
                    }
                }
            }
            catch (Exception exception)
            {
                throw new ApplicationException("StartUpFolder: " + Environment.CurrentDirectory + " List:" + string.Join(";", list.ToArray()), exception);
            }
            return list2;
        }

        public static List<string> ConvertStringPathToAssemblyList(string list, char splitChar)
        {
            return ConvertStringPathToAssemblyList(new List<string>(list.Split(new char[] { splitChar }, StringSplitOptions.RemoveEmptyEntries)));
        }

        public static void EnsureDirectoryExists(string path)
        {
            string directoryName = path;
            if (Path.HasExtension(path))
            {
                directoryName = Path.GetDirectoryName(path);
            }
            else if (!path.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                directoryName = Path.GetDirectoryName(path + Path.DirectorySeparatorChar.ToString());
            }
            if (!Directory.Exists(directoryName) && !string.IsNullOrEmpty(directoryName))
            {
                Trace.WriteLine("EnsureDirectoryExists - Creating:" + directoryName);
                Directory.CreateDirectory(directoryName);
            }
        }

        public static Tuple<string, string> FindUncommonRootPath(string original, string replacement)
        {
            string[] strArray = original.Split(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
            string[] strArray2 = replacement.Split(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
            string str = @"\\";
            if (original.StartsWith(str))
            {
                strArray[0] = str + strArray[0];
            }
            if (replacement.StartsWith(str))
            {
                strArray2[0] = str + strArray2[0];
            }
            int index = strArray.Length - 1;
            int num2 = strArray2.Length - 1;
            while (((index >= 0) && (num2 >= 0)) && strArray[index].Equals(strArray2[num2], StringComparison.InvariantCultureIgnoreCase))
            {
                index--;
                num2--;
            }
            if (((index >= 0) && (num2 >= 0)) && ((index != (strArray.Length - 1)) && (num2 != (strArray2.Length - 1))))
            {
                return new Tuple<string, string>(string.Join(Path.DirectorySeparatorChar.ToString(), strArray, 0, index + 1) + Path.DirectorySeparatorChar.ToString(), string.Join(Path.DirectorySeparatorChar.ToString(), strArray2, 0, num2 + 1) + Path.DirectorySeparatorChar.ToString());
            }
            return new Tuple<string, string>(original, replacement);
        }

        public static string GetTempFileName(string extension)
        {
            string tempFileName;
            do
            {
                tempFileName = Path.GetTempFileName();
                File.Delete(tempFileName);
                tempFileName = tempFileName + extension;
            }
            while (File.Exists(tempFileName));
            return tempFileName;
        }

        public static bool SafeFilePathExists(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }
            try
            {
                return File.Exists(path);
            }
            catch
            {
                return false;
            }
        }

        public static void SaveMoveWithBackup(string sourcePath, string destinationPath)
        {
            string destinationBackupFileName = destinationPath + ".bak";
            if (File.Exists(destinationPath))
            {
                try
                {
                    File.Replace(sourcePath, destinationPath, destinationBackupFileName);
                    return;
                }
                catch (Exception exception)
                {
                    throw new ApplicationException(string.Format("Unable to replace '{0}' to '{1}' and create backup '{2}'", sourcePath, destinationPath, destinationBackupFileName), exception);
                }
            }
            try
            {
                File.Move(sourcePath, destinationPath);
            }
            catch (Exception exception2)
            {
                throw new ApplicationException(string.Format("Unable to move '{0}' to '{1}'", sourcePath, destinationPath), exception2);
            }
        }
    }
}
