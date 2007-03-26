using System;
using System.IO;
using System.Collections;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Core.Tasks;

namespace CCNET.Extensions.XslFileMerge
{

    [ReflectorType("filepairs")]
    public class FilePairList : CollectionBase
    {
        public FilePair this[int index]
        {
            get
            {
                return (FilePair)this.InnerList[index];
            }
            set
            {
                this.InnerList[index] = value;
            }
        }

        public void Add(FilePair filter)
        {
            this.InnerList.Add(filter);
        }

    }

}
