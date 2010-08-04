namespace NCover.Framework.Configuration
{
    using NCover.Framework.Exceptions;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    public abstract class AbstractConfigFile : IConfig, ICloneable
    {
        protected AbstractConfigFile()
        {
        }

        public object Clone()
        {
            object obj2;
            XmlWriterSettings settings = new XmlWriterSettings {
                CheckCharacters = true,
                CloseOutput = false,
                Encoding = Encoding.UTF8,
                Indent = false,
                OmitXmlDeclaration = true,
                NewLineOnAttributes = false
            };
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            MemoryStream output = new MemoryStream();
            using (XmlWriter writer = XmlWriter.Create(output, settings))
            {
                XmlSerializer serializer = new XmlSerializer(base.GetType());
                serializer.Serialize(writer, this, namespaces);
                writer.Flush();
                output.Seek(0L, SeekOrigin.Begin);
                using (XmlReader reader = XmlReader.Create(output))
                {
                    obj2 = serializer.Deserialize(reader);
                }
            }
            return obj2;
        }

        private static bool IsValidFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return false;
            }
            try
            {
                Path.GetDirectoryName(fileName);
                Path.GetFileName(fileName);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public SaveFileResult Save()
        {
            return this.Save(this.LastFilePath);
        }

        public SaveFileResult Save(Stream dataStream)
        {
            try
            {
                new XmlSerializerNamespaces().Add(string.Empty, string.Empty);
                XmlWriterSettings settings = new XmlWriterSettings {
                    CheckCharacters = true,
                    CloseOutput = false,
                    Indent = true,
                    OmitXmlDeclaration = true
                };
                using (XmlWriter writer = XmlWriter.Create(dataStream, settings))
                {
                    new XmlSerializer(base.GetType()).Serialize(writer, this);
                    writer.Flush();
                }
            }
            catch (Exception exception)
            {
                string completeInnerStackTrace = ExceptionWriter.GetCompleteInnerStackTrace(exception);
                Trace.WriteLine(exception.Message);
                Trace.WriteLine(completeInnerStackTrace);
                return new SaveFileResult(exception);
            }
            return SaveFileResult.SuccessfulSave;
        }

        public SaveFileResult Save(string fileName)
        {
            SaveFileResult result;
            if (!IsValidFileName(fileName))
            {
                return new SaveFileResult(new InvalidOperationException(string.Format("Invalid filename: '{0}'", fileName ?? "NULL")));
            }
            string destFileName = fileName + ".bkp";
            bool flag = false;
            try
            {
                if (FileStub.Exists(fileName))
                {
                    FileStub.Copy(fileName, destFileName);
                }
                string directoryName = PathStub.GetDirectoryName(fileName);
                if (!string.IsNullOrEmpty(directoryName) && !DirectoryStub.Exists(directoryName))
                {
                    DirectoryStub.CreateDirectory(directoryName);
                }
                flag = true;
                using (Stream stream = FileStub.Open(fileName, FileMode.Create, FileAccess.Write))
                {
                    result = this.Save(stream);
                }
                if (!result.Success)
                {
                    return result;
                }
            }
            catch (Exception exception)
            {
                if (flag && FileStub.Exists(destFileName))
                {
                    FileStub.Copy(destFileName, fileName);
                }
                return new SaveFileResult(exception);
            }
            finally
            {
                if (FileStub.Exists(destFileName))
                {
                    FileStub.Delete(destFileName);
                }
            }
            this.LastFilePath = fileName;
            return result;
        }

        [XmlAttribute("version")]
        public string FileVersion { get; set; }

        [XmlIgnore]
        public string LastFilePath { get; set; }
    }
}
