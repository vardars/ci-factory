namespace NCover.Framework.Configuration
{
    using System;
    using System.IO;

    public interface IConfig : ICloneable
    {
        SaveFileResult Save();
        SaveFileResult Save(Stream dataStream);
        SaveFileResult Save(string fileName);

        string FileVersion { get; }

        string LastFilePath { get; set; }
    }
}
