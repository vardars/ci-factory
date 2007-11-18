using System;
using System.IO;
using NAnt.Core;
using NAnt.Core.Filters;
using NAnt.Core.Attributes;

namespace CIFactory.NAnt.Tasks
{
    [TaskName("replace")]
    public class Replace : Task
    {
        #region Fields

        private FileInfo _File;

        private FilterChain _Filter;

        #endregion

        #region Properties

        [TaskAttributeAttribute("file", Required = true)]
        public FileInfo File
        {
            get { return _File; }
            set { _File = value; }
        }

        [BuildElement("filterchain", Required = true)]
        public FilterChain Filter
        {
            get { return _Filter; }
            set { _Filter = value; }
        }

        #endregion

        #region Protected Methods

        protected override void ExecuteTask()
        {
            this.WriteWithFilters();
        }

        #endregion

        #region Private Methods

        private void WriteWithFilters()
        {
            StreamReader FileReader = null;
            StreamWriter Writer = null;
            StringReader Reader = null;
            PhysicalTextReader ChainedReader = null;
            try
            {
                FileReader = this.File.OpenText();
                Reader = new StringReader(FileReader.ReadToEnd());
                FileReader.Close();
                FileReader = null;
                Writer = this.File.CreateText();
                ChainedReader = new PhysicalTextReader(Reader);
                Filter FilterReader = this.Filter.GetBaseFilter(ChainedReader);
                while (!false)
                {
                    int Focus = FilterReader.Read();
                    if (!(Focus > -1))
                    {
                        break; // TODO: might not be correct. Was : Exit Do
                    }
                    Writer.Write((Char)Focus);
                }
            }
            finally
            {
                if (FileReader != null)
                {
                    FileReader.Close();
                }
                if (Writer != null)
                {
                    Writer.Close();
                }
                if (ChainedReader != null)
                {
                    ChainedReader.Close();
                }
                if (Reader != null)
                {
                    Reader.Close();
                }
            }
        }

        #endregion

    }

}