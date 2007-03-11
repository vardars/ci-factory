using System;
using System.IO;
using System.Collections;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Core.Tasks;

namespace CCNET.Extensions.XslFileMerge
{
    [ReflectorType("xslmerger")]
    public class XslMerger : ITask
    {

        private FilePairList _FilePairs;

        [ReflectorCollection("filepairs", InstanceType = typeof(FilePairList), Required = true)]
        public FilePairList FilePairs
        {
            get
            {
                return _FilePairs;
            }
            set
            {
                _FilePairs = value;
            }
        }

        public void Run(IIntegrationResult result)
        {
            foreach (FilePair Pair in this.FilePairs)
            {
                string XmlFilePath = Pair.XmlFile;
                if (!Path.IsPathRooted(XmlFilePath))
                {
                    XmlFilePath = Path.Combine(result.WorkingDirectory, XmlFilePath);
                }

                string XslFilePath = Pair.XslFile;
                if (!Path.IsPathRooted(XslFilePath))
                {
                    XslFilePath = Path.Combine(result.WorkingDirectory, XslFilePath);
                }

                string XslFileName = Path.GetFileName(XslFilePath);
                if (!File.Exists(XslFilePath))
                {
                    Log.Warning("File not Found: " + XslFileName);
                }

                WildCardPath Pattern = new WildCardPath(XmlFilePath);
                FileInfo[] Files = Pattern.GetFiles();
                foreach (FileInfo XmlFileInfo in Files)
                {
                    Log.Info(String.Format("Merging file {0} through {1}", XmlFileInfo, XslFileName));
                    if (XmlFileInfo.Exists)
                    {
                        string Data;
                        String Contents;
                        using (TextReader Reader = XmlFileInfo.OpenText())
                        {
                            Contents = Reader.ReadToEnd();
                        }
                        XslTransformer Transformer = new XslTransformer();
                        Data = Transformer.Transform(Contents, XslFilePath);
                        result.AddTaskResult((new XslMergerTaskResult(Data)));
                    }
                    else
                    {
                        Log.Warning("File not Found: " + XmlFileInfo);
                    }
                }
            }
        }
    }
}
