namespace System.IO
{
    using NCover.Interfaces.SystemAdapters;
    using System;
    using System.Security.AccessControl;
    using System.Text;

    public static class FileStub
    {
        private static IFileProvider _provider;

        public static void AppendAllText(string path, string contents)
        {
            if (_provider == null)
            {
                File.AppendAllText(path, contents);
            }
            else
            {
                _provider.AppendAllText(path, contents);
            }
        }

        public static void AppendAllText(string path, string contents, Encoding encoding)
        {
            if (_provider == null)
            {
                File.AppendAllText(path, contents, encoding);
            }
            else
            {
                _provider.AppendAllText(path, contents, encoding);
            }
        }

        public static StreamWriter AppendText(string path)
        {
            if (_provider == null)
            {
                return File.AppendText(path);
            }
            return _provider.AppendText(path);
        }

        public static void Copy(string sourceFileName, string destFileName)
        {
            if (_provider == null)
            {
                File.Copy(sourceFileName, destFileName);
            }
            else
            {
                _provider.Copy(sourceFileName, destFileName);
            }
        }

        public static void Copy(string sourceFileName, string destFileName, bool overwrite)
        {
            if (_provider == null)
            {
                File.Copy(sourceFileName, destFileName, overwrite);
            }
            else
            {
                _provider.Copy(sourceFileName, destFileName, overwrite);
            }
        }

        public static Stream Create(string path)
        {
            if (_provider == null)
            {
                return File.Create(path);
            }
            return _provider.Create(path);
        }

        public static Stream Create(string path, int bufferSize)
        {
            if (_provider == null)
            {
                return File.Create(path, bufferSize);
            }
            return _provider.Create(path, bufferSize);
        }

        public static Stream Create(string path, int bufferSize, FileOptions options)
        {
            if (_provider == null)
            {
                return File.Create(path, bufferSize, options);
            }
            return _provider.Create(path, bufferSize, options);
        }

        public static Stream Create(string path, int bufferSize, FileOptions options, FileSecurity fileSecurity)
        {
            if (_provider == null)
            {
                return File.Create(path, bufferSize, options, fileSecurity);
            }
            return _provider.Create(path, bufferSize, options, fileSecurity);
        }

        public static StreamWriter CreateText(string path)
        {
            if (_provider == null)
            {
                return File.CreateText(path);
            }
            return _provider.CreateText(path);
        }

        public static void Decrypt(string path)
        {
            if (_provider == null)
            {
                File.Decrypt(path);
            }
            else
            {
                _provider.Decrypt(path);
            }
        }

        public static void Delete(string path)
        {
            if (_provider == null)
            {
                File.Delete(path);
            }
            else
            {
                _provider.Delete(path);
            }
        }

        public static void Encrypt(string path)
        {
            if (_provider == null)
            {
                File.Encrypt(path);
            }
            else
            {
                _provider.Encrypt(path);
            }
        }

        public static bool Exists(string path)
        {
            if (_provider == null)
            {
                return File.Exists(path);
            }
            return _provider.Exists(path);
        }

        public static FileSecurity GetAccessControl(string path)
        {
            if (_provider == null)
            {
                return File.GetAccessControl(path);
            }
            return _provider.GetAccessControl(path);
        }

        public static FileSecurity GetAccessControl(string path, AccessControlSections includeSections)
        {
            if (_provider == null)
            {
                return File.GetAccessControl(path, includeSections);
            }
            return _provider.GetAccessControl(path, includeSections);
        }

        public static FileAttributes GetAttributes(string path)
        {
            if (_provider == null)
            {
                return File.GetAttributes(path);
            }
            return _provider.GetAttributes(path);
        }

        public static DateTime GetCreationTime(string path)
        {
            if (_provider == null)
            {
                return File.GetCreationTime(path);
            }
            return _provider.GetCreationTime(path);
        }

        public static DateTime GetCreationTimeUtc(string path)
        {
            if (_provider == null)
            {
                return File.GetCreationTimeUtc(path);
            }
            return _provider.GetCreationTimeUtc(path);
        }

        public static DateTime GetLastAccessTime(string path)
        {
            if (_provider == null)
            {
                return File.GetLastAccessTime(path);
            }
            return _provider.GetLastAccessTime(path);
        }

        public static DateTime GetLastAccessTimeUtc(string path)
        {
            if (_provider == null)
            {
                return File.GetLastAccessTimeUtc(path);
            }
            return _provider.GetLastAccessTimeUtc(path);
        }

        public static DateTime GetLastWriteTime(string path)
        {
            if (_provider == null)
            {
                return File.GetLastWriteTime(path);
            }
            return _provider.GetLastWriteTime(path);
        }

        public static DateTime GetLastWriteTimeUtc(string path)
        {
            if (_provider == null)
            {
                return File.GetLastWriteTimeUtc(path);
            }
            return _provider.GetLastWriteTimeUtc(path);
        }

        public static void Move(string sourceFileName, string destFileName)
        {
            if (_provider == null)
            {
                File.Move(sourceFileName, destFileName);
            }
            else
            {
                _provider.Move(sourceFileName, destFileName);
            }
        }

        public static Stream Open(string path, FileMode mode)
        {
            if (_provider == null)
            {
                return File.Open(path, mode);
            }
            return _provider.Open(path, mode);
        }

        public static Stream Open(string path, FileMode mode, FileAccess access)
        {
            if (_provider == null)
            {
                return File.Open(path, mode, access);
            }
            return _provider.Open(path, mode, access);
        }

        public static Stream Open(string path, FileMode mode, FileAccess access, FileShare share)
        {
            if (_provider == null)
            {
                return File.Open(path, mode, access, share);
            }
            return _provider.Open(path, mode, access, share);
        }

        public static Stream OpenRead(string path)
        {
            if (_provider == null)
            {
                return File.OpenRead(path);
            }
            return _provider.OpenRead(path);
        }

        public static StreamReader OpenText(string path)
        {
            if (_provider == null)
            {
                return File.OpenText(path);
            }
            return _provider.OpenText(path);
        }

        public static Stream OpenWrite(string path)
        {
            if (_provider == null)
            {
                return File.OpenWrite(path);
            }
            return _provider.OpenWrite(path);
        }

        public static byte[] ReadAllBytes(string path)
        {
            if (_provider == null)
            {
                return File.ReadAllBytes(path);
            }
            return _provider.ReadAllBytes(path);
        }

        public static string[] ReadAllLines(string path)
        {
            if (_provider == null)
            {
                return File.ReadAllLines(path);
            }
            return _provider.ReadAllLines(path);
        }

        public static string[] ReadAllLines(string path, Encoding encoding)
        {
            if (_provider == null)
            {
                return File.ReadAllLines(path, encoding);
            }
            return _provider.ReadAllLines(path, encoding);
        }

        public static string ReadAllText(string path)
        {
            if (_provider == null)
            {
                return File.ReadAllText(path);
            }
            return _provider.ReadAllText(path);
        }

        public static string ReadAllText(string path, Encoding encoding)
        {
            if (_provider == null)
            {
                return File.ReadAllText(path, encoding);
            }
            return _provider.ReadAllText(path, encoding);
        }

        public static void Replace(string sourceFileName, string destinationFileName, string destinationBackupFileName)
        {
            if (_provider == null)
            {
                File.Replace(sourceFileName, destinationFileName, destinationBackupFileName);
            }
            else
            {
                _provider.Replace(sourceFileName, destinationFileName, destinationBackupFileName);
            }
        }

        public static void Replace(string sourceFileName, string destinationFileName, string destinationBackupFileName, bool ignoreMetadataErrors)
        {
            if (_provider == null)
            {
                File.Replace(sourceFileName, destinationFileName, destinationBackupFileName, ignoreMetadataErrors);
            }
            else
            {
                _provider.Replace(sourceFileName, destinationFileName, destinationBackupFileName, ignoreMetadataErrors);
            }
        }

        public static void SetAccessControl(string path, FileSecurity fileSecurity)
        {
            if (_provider == null)
            {
                File.SetAccessControl(path, fileSecurity);
            }
            else
            {
                _provider.SetAccessControl(path, fileSecurity);
            }
        }

        public static void SetAttributes(string path, FileAttributes fileAttributes)
        {
            if (_provider == null)
            {
                File.SetAttributes(path, fileAttributes);
            }
            else
            {
                _provider.SetAttributes(path, fileAttributes);
            }
        }

        public static void SetCreationTime(string path, DateTime creationTime)
        {
            if (_provider == null)
            {
                File.SetCreationTime(path, creationTime);
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
                File.SetCreationTimeUtc(path, creationTimeUtc);
            }
            else
            {
                _provider.SetCreationTimeUtc(path, creationTimeUtc);
            }
        }

        public static void SetLastAccessTime(string path, DateTime lastAccessTime)
        {
            if (_provider == null)
            {
                File.SetLastAccessTime(path, lastAccessTime);
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
                File.SetLastAccessTimeUtc(path, lastAccessTimeUtc);
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
                File.SetLastWriteTime(path, lastWriteTime);
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
                File.SetLastWriteTimeUtc(path, lastWriteTimeUtc);
            }
            else
            {
                _provider.SetLastWriteTimeUtc(path, lastWriteTimeUtc);
            }
        }

        public static void WriteAllBytes(string path, byte[] bytes)
        {
            if (_provider == null)
            {
                File.WriteAllBytes(path, bytes);
            }
            else
            {
                _provider.WriteAllBytes(path, bytes);
            }
        }

        public static void WriteAllLines(string path, string[] contents)
        {
            if (_provider == null)
            {
                File.WriteAllLines(path, contents);
            }
            else
            {
                _provider.WriteAllLines(path, contents);
            }
        }

        public static void WriteAllLines(string path, string[] contents, Encoding encoding)
        {
            if (_provider == null)
            {
                File.WriteAllLines(path, contents, encoding);
            }
            else
            {
                _provider.WriteAllLines(path, contents, encoding);
            }
        }

        public static void WriteAllText(string path, string contents)
        {
            if (_provider == null)
            {
                File.WriteAllText(path, contents);
            }
            else
            {
                _provider.WriteAllText(path, contents);
            }
        }

        public static void WriteAllText(string path, string contents, Encoding encoding)
        {
            if (_provider == null)
            {
                File.WriteAllText(path, contents, encoding);
            }
            else
            {
                _provider.WriteAllText(path, contents, encoding);
            }
        }
    }
}
