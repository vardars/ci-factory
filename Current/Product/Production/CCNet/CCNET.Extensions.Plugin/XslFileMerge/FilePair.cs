using System;
using System.IO;
using System.Collections;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Core.Tasks;

namespace CCNET.Extensions.XslFileMerge
{

    [ReflectorType("filepair")]
    public class FilePair
    {
        private string _XmlFile;
        private string _XslFile;

        [ReflectorProperty("xslfile")]
        public string XslFile
        {
            get
            {
                return _XslFile;
            }
            set
            {
                _XslFile = value;
            }
        }

        [ReflectorProperty("xmlfile")]
        public string XmlFile
        {
            get
            {
                return _XmlFile;
            }
            set
            {
                _XmlFile = value;
            }
        }
    }

}
