namespace NCover.Interfaces.SystemAdapters
{
    using System;

    public interface IPathProvider
    {
        string ChangeExtension(string path, string extension);
        string Combine(string path1, string path2);
        string GetDirectoryName(string path);
        string GetExtension(string path);
        string GetFileName(string path);
        string GetFileNameWithoutExtension(string path);
        string GetFullPath(string path);
        char[] GetInvalidFileNameChars();
        char[] GetInvalidPathChars();
        string GetPathRoot(string path);
        string GetRandomFileName();
        string GetTempFileName();
        string GetTempPath();
        bool HasExtension(string path);
        bool IsPathRooted(string path);

        char AltDirectorySeparatorChar { get; }

        char DirectorySeparatorChar { get; }

        [Obsolete("use GetInvalidPathChars() instead")]
        char[] InvalidPathChars { get; }

        char PathSeparator { get; }

        char VolumeSeparatorChar { get; }
    }
}
