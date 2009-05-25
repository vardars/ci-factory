using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using NAnt.Core;
using NAnt.Core.Filters;
using NAnt.Core.Attributes;
using CIFactory.NAnt.Types;
using System.Text;
using NAnt.Core.Types;

namespace CIFactory.NAnt.Tasks
{
    [TaskName("write")]
    public class WriteTask : Task
    {
        #region Fields

        private bool _Append = false;

        private FilterChain _Filter;

        private FileInfo _OutFile;

        private TextElement _Text;
        private Encoding _encoding;

        #endregion

        #region Properties

        [TaskAttributeAttribute("append", Required = false), BooleanValidator()]
        public bool Append
        {
            get { return _Append; }
            set { _Append = value; }
        }

        [BuildElement("filterchain", Required = false)]
        public FilterChain Filter
        {
            get { return _Filter; }
            set { _Filter = value; }
        }

        [TaskAttributeAttribute("file", Required = true)]
        public FileInfo OutFile
        {
            get { return _OutFile; }
            set { _OutFile = value; }
        }

        [BuildElement("text", Required = true)]
        public TextElement Text
        {
            get { return _Text; }
            set { _Text = value; }
        }

        [TaskAttribute("encoding")]
        public Encoding Encoding
        {
            get { return _encoding; }
            set { _encoding = value; }
        }

        #endregion

        #region Protected Methods

        protected override void ExecuteTask()
        {
            if (this.Filter == null || this.Filter.Filters == null || this.Filter.Filters.Count == 0)
            {
                this.Write();
            }
            else
            {
                this.WriteWithFilters();
            }
        }

        #endregion

        #region Private Methods

        private StreamWriter GetWriter()
        {
            Encoding encoding = (Encoding != null) ? Encoding : Encoding.Default;
            return new StreamWriter(this.OutFile.FullName, this.Append, encoding);
        }

        private void Write()
        {
            StreamWriter Writer = null;
            try
            {
                Writer = this.GetWriter();
                Writer.Write(this.Text.Value);
            }
            finally
            {
                if (Writer != null)
                {
                    Writer.Close();
                }
            }
        }

        private void WriteWithFilters()
        {
            StreamWriter Writer = null;
            StringReader Reader = null;
            PhysicalTextReader ChainedReader = null;
            try
            {
                Writer = this.GetWriter();
                Reader = new StringReader(this.Text.Value);
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