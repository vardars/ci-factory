using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Config;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{
    class FileSvnQueue
    {
        private Configuration cfg = Configuration.Instance();
        private string directory = Directory.GetCurrentDirectory();
        private Project project;

        private string _lastRevisionBuilt;
        
        public string LastRevisionBuilt
        {
            get { return _lastRevisionBuilt; }
            set { _lastRevisionBuilt = value; }
        }

        public FileSvnQueue(Project p)
        {
            project = p;
        }
                                
        public string ReadLastSvnRevision()
        {
            string svnQueueFilePath = GetFilePath(project.Name);
            if (!File.Exists(svnQueueFilePath)) return string.Empty;

            using (TextReader reader = CreateTextReader(svnQueueFilePath))
            {
                _lastRevisionBuilt = reader.ReadToEnd();
                return _lastRevisionBuilt;
            }
        }

        public void SaveLastSvnRevision()
        {
            using (TextWriter writer = CreateTextWriter(GetFilePath(project.Name)))
            {
                writer.Write(_lastRevisionBuilt);
                writer.Flush();
            }
        }

        private string GetFilePath(string project)
        {
            return Path.Combine(directory, SvnQueueFilename(project));
        }

        private string SvnQueueFilename(string project)
        {
            StringBuilder strBuilder = new StringBuilder();
            foreach (string token in project.Split(' '))
            {
                strBuilder.Append(token.Substring(0, 1).ToUpper());
                if (token.Length > 1)
                {
                    strBuilder.Append(token.Substring(1));
                }
            }
            return strBuilder.Append(".svn.queue").ToString();
        }

        private TextReader CreateTextReader(string path)
        {
            try
            {
                return new StreamReader(path);
            }
            catch (IOException ex)
            {
                throw new CruiseControlException(string.Format("Unable to read the specified subversion queue file: {0}.  The path may be invalid.", path), ex);
            }
        }

        private TextWriter CreateTextWriter(string path)
        {
            try
            {
                return new StreamWriter(path, false, Encoding.Unicode);
            }
            catch (SystemException ex)
            {
                throw new CruiseControlException(string.Format("Unable to save the IntegrationResult to the specified directory: {0}", path), ex);
            }
        }
    }

    
}
