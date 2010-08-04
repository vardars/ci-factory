namespace System.IO
{
    using NCover.Interfaces.SystemAdapters;
    using System;
    using System.Security.AccessControl;

    public static class DirectoryStub
    {
        private static IDirectoryProvider _provider;

        public static DirectoryInfo CreateDirectory(string path)
        {
            if (_provider == null)
            {
                return Directory.CreateDirectory(path);
            }
            return _provider.CreateDirectory(path);
        }

        public static DirectoryInfo CreateDirectory(string path, DirectorySecurity directorySecurity)
        {
            if (_provider == null)
            {
                return Directory.CreateDirectory(path, directorySecurity);
            }
            return _provider.CreateDirectory(path, directorySecurity);
        }

        public static void Delete(string path)
        {
            if (_provider == null)
            {
                Directory.Delete(path);
            }
            else
            {
                _provider.Delete(path);
            }
        }

        public static void Delete(string path, bool recursive)
        {
            if (_provider == null)
            {
                Directory.Delete(path, recursive);
            }
            else
            {
                _provider.Delete(path, recursive);
            }
        }

        public static bool Exists(string path)
        {
            if (_provider == null)
            {
                return Directory.Exists(path);
            }
            return _provider.Exists(path);
        }

        public static DirectorySecurity GetAccessControl(string path)
        {
            if (_provider == null)
            {
                return Directory.GetAccessControl(path);
            }
            return _provider.GetAccessControl(path);
        }

        public static DirectorySecurity GetAccessControl(string path, AccessControlSections includeSections)
        {
            if (_provider == null)
            {
                return Directory.GetAccessControl(path, includeSections);
            }
            return _provider.GetAccessControl(path, includeSections);
        }

        public static DateTime GetCreationTime(string path)
        {
            if (_provider == null)
            {
                return Directory.GetCreationTime(path);
            }
            return _provider.GetCreationTime(path);
        }

        public static DateTime GetCreationTimeUtc(string path)
        {
            if (_provider == null)
            {
                return Directory.GetCreationTimeUtc(path);
            }
            return _provider.GetCreationTimeUtc(path);
        }

        public static string GetCurrentDirectory()
        {
            if (_provider == null)
            {
                return Directory.GetCurrentDirectory();
            }
            return _provider.GetCurrentDirectory();
        }

        public static string[] GetDirectories(string path)
        {
            if (_provider == null)
            {
                return Directory.GetDirectories(path);
            }
            return _provider.GetDirectories(path);
        }

        public static string[] GetDirectories(string path, string searchPattern)
        {
            if (_provider == null)
            {
                return Directory.GetDirectories(path, searchPattern);
            }
            return _provider.GetDirectories(path, searchPattern);
        }

        public static string[] GetDirectories(string path, string searchPattern, SearchOption searchOption)
        {
            if (_provider == null)
            {
                return Directory.GetDirectories(path, searchPattern, searchOption);
            }
            return _provider.GetDirectories(path, searchPattern, searchOption);
        }

        public static string GetDirectoryRoot(string path)
        {
            if (_provider == null)
            {
                return Directory.GetDirectoryRoot(path);
            }
            return _provider.GetDirectoryRoot(path);
        }

        public static string[] GetFiles(string path)
        {
            if (_provider == null)
            {
                return Directory.GetFiles(path);
            }
            return _provider.GetFiles(path);
        }

        public static string[] GetFiles(string path, string searchPattern)
        {
            if (_provider == null)
            {
                return Directory.GetFiles(path, searchPattern);
            }
            return _provider.GetFiles(path, searchPattern);
        }

        public static string[] GetFiles(string path, string searchPattern, SearchOption searchOption)
        {
            if (_provider == null)
            {
                return Directory.GetFiles(path, searchPattern, searchOption);
            }
            return _provider.GetFiles(path, searchPattern, searchOption);
        }

        public static string[] GetFileSystemEntries(string path)
        {
            if (_provider == null)
            {
                return Directory.GetFileSystemEntries(path);
            }
            return _provider.GetFileSystemEntries(path);
        }

        public static string[] GetFileSystemEntries(string path, string searchPattern)
        {
            if (_provider == null)
            {
                return Directory.GetFileSystemEntries(path, searchPattern);
            }
            return _provider.GetFileSystemEntries(path, searchPattern);
        }

        public static DateTime GetLastAccessTime(string path)
        {
            if (_provider == null)
            {
                return Directory.GetLastAccessTime(path);
            }
            return _provider.GetLastAccessTime(path);
        }

        public static DateTime GetLastAccessTimeUtc(string path)
        {
            if (_provider == null)
            {
                return Directory.GetLastAccessTimeUtc(path);
            }
            return _provider.GetLastAccessTimeUtc(path);
        }

        public static DateTime GetLastWriteTime(string path)
        {
            if (_provider == null)
            {
                return Directory.GetLastWriteTime(path);
            }
            return _provider.GetLastWriteTime(path);
        }

        public static DateTime GetLastWriteTimeUtc(string path)
        {
            if (_provider == null)
            {
                return Directory.GetLastWriteTimeUtc(path);
            }
            return _provider.GetLastWriteTimeUtc(path);
        }

        public static string[] GetLogicalDrives()
        {
            if (_provider == null)
            {
                return Directory.GetLogicalDrives();
            }
            return _provider.GetLogicalDrives();
        }

        public static DirectoryInfo GetParent(string path)
        {
            if (_provider == null)
            {
                return Directory.GetParent(path);
            }
            return _provider.GetParent(path);
        }

        public static void Move(string sourceDirName, string destDirName)
        {
            if (_provider == null)
            {
                Directory.Move(sourceDirName, destDirName);
            }
            else
            {
                _provider.Move(sourceDirName, destDirName);
            }
        }

        public static void SetAccessControl(string path, DirectorySecurity directorySecurity)
        {
            if (_provider == null)
            {
                Directory.SetAccessControl(path, directorySecurity);
            }
            else
            {
                _provider.SetAccessControl(path, directorySecurity);
            }
        }

        public static void SetCreationTime(string path, DateTime creationTime)
        {
            if (_provider == null)
            {
                Directory.SetCreationTime(path, creationTime);
            }
            else
            {
                _provider.SetCreationTime(path, creationTime);
            }
        }

        public static void SetCreationTimeUtc(string path, DateTime creationTimeUtc)
        {
            if (_provider == null)
            {
                Directory.SetCreationTimeUtc(path, creationTimeUtc);
            }
            else
            {
                _provider.SetCreationTimeUtc(path, creationTimeUtc);
            }
        }

        public static void SetCurrentDirectory(string path)
        {
            if (_provider == null)
            {
                Directory.SetCurrentDirectory(path);
            }
            else
            {
                _provider.SetCurrentDirectory(path);
            }
        }

        public static void SetLastAccessTime(string path, DateTime lastAccessTime)
        {
            if (_provider == null)
            {
                Directory.SetLastAccessTime(path, lastAccessTime);
            }
            else
            {
                _provider.SetLastAccessTime(path, lastAccessTime);
            }
        }

        public static void SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc)
        {
            if (_provider == null)
            {
                Directory.SetLastAccessTimeUtc(path, lastAccessTimeUtc);
            }
            else
            {
                _provider.SetLastAccessTimeUtc(path, lastAccessTimeUtc);
            }
        }

        public static void SetLastWriteTime(string path, DateTime lastWriteTime)
        {
            if (_provider == null)
            {
                Directory.SetLastWriteTime(path, lastWriteTime);
            }
            else
            {
                _provider.SetLastWriteTime(path, lastWriteTime);
            }
        }

        public static void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc)
        {
            if (_provider == null)
            {
                Directory.SetLastWriteTimeUtc(path, lastWriteTimeUtc);
            }
            else
            {
                _provider.SetLastWriteTimeUtc(path, lastWriteTimeUtc);
            }
        }
    }
}
