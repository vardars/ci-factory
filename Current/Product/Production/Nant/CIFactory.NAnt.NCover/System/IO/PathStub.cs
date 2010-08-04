namespace System.IO
{
    using NCover.Interfaces.SystemAdapters;
    using System;

    public static class PathStub
    {
        private static IPathProvider _provider;

        public static string ChangeExtension(string path, string extension)
        {
            if (_provider == null)
            {
                return Path.ChangeExtension(path, extension);
            }
            return _provider.ChangeExtension(path, extension);
        }

        public static string Combine(string path1, string path2)
        {
            if (_provider == null)
            {
                return Path.Combine(path1, path2);
            }
            return _provider.Combine(path1, path2);
        }

        public static string GetDirectoryName(string path)
        {
            if (_provider == null)
            {
                return Path.GetDirectoryName(path);
            }
            return _provider.GetDirectoryName(path);
        }

        public static string GetExtension(string path)
        {
            if (_provider == null)
            {
                return Path.GetExtension(path);
            }
            return _provider.GetExtension(path);
        }

        public static string GetFileName(string path)
        {
            if (_provider == null)
            {
                return Path.GetFileName(path);
            }
            return _provider.GetFileName(path);
        }

        public static string GetFileNameWithoutExtension(string path)
        {
            if (_provider == null)
            {
                return Path.GetFileNameWithoutExtension(path);
            }
            return _provider.GetFileNameWithoutExtension(path);
        }

        public static string GetFullPath(string path)
        {
            if (_provider == null)
            {
                return Path.GetFullPath(path);
            }
            return _provider.GetFullPath(path);
        }

        public static char[] GetInvalidFileNameChars()
        {
            if (_provider == null)
            {
                return Path.GetInvalidFileNameChars();
            }
            return _provider.GetInvalidFileNameChars();
        }

        public static char[] GetInvalidPathChars()
        {
            if (_provider == null)
            {
                return Path.GetInvalidPathChars();
            }
            return _provider.GetInvalidPathChars();
        }

        public static string GetPathRoot(string path)
        {
            if (_provider == null)
            {
                return Path.GetPathRoot(path);
            }
            return _provider.GetPathRoot(path);
        }

        public static string GetRandomFileName()
        {
            if (_provider == null)
            {
                return Path.GetRandomFileName();
            }
            return _provider.GetRandomFileName();
        }

        public static string GetTempFileName()
        {
            if (_provider == null)
            {
                return Path.GetTempFileName();
            }
            return _provider.GetTempFileName();
        }

        public static string GetTempPath()
        {
            if (_provider == null)
            {
                return Path.GetTempPath();
            }
            return _provider.GetTempPath();
        }

        public static bool HasExtension(string path)
        {
            if (_provider == null)
            {
                return Path.HasExtension(path);
            }
            return _provider.HasExtension(path);
        }

        public static bool IsPathRooted(string path)
        {
            if (_provider == null)
            {
                return Path.IsPathRooted(path);
            }
            return _provider.IsPathRooted(path);
        }

        public static char AltDirectorySeparatorChar
        {
            get
            {
                if (_provider == null)
                {
                    return Path.AltDirectorySeparatorChar;
                }
                return _provider.AltDirectorySeparatorChar;
            }
        }

        public static char DirectorySeparatorChar
        {
            get
            {
                if (_provider == null)
                {
                    return Path.DirectorySeparatorChar;
                }
                return _provider.DirectorySeparatorChar;
            }
        }

        [Obsolete("use GetInvalidPathChars() instead")]
        public static char[] InvalidPathChars
        {
            get
            {
                if (_provider == null)
                {
                    return Path.InvalidPathChars;
                }
                return _provider.InvalidPathChars;
            }
        }

        public static char PathSeparator
        {
            get
            {
                if (_provider == null)
                {
                    return Path.PathSeparator;
                }
                return _provider.PathSeparator;
            }
        }

        public static char VolumeSeparatorChar
        {
            get
            {
                if (_provider == null)
                {
                    return Path.VolumeSeparatorChar;
                }
                return _provider.VolumeSeparatorChar;
            }
        }
    }
}
