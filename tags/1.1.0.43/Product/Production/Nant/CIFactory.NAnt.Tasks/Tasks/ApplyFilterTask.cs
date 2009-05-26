using System;
using System.IO;
using NAnt.Core;
using NAnt.Core.Filters;
using NAnt.Core.Attributes;
using CIFactory.NAnt.Types;
using NAnt.Core.Types;

namespace CIFactory.NAnt.Tasks
{
    [TaskName("applyfilter")]
    public class ApplyFilterTask : Task
    {
        #region Fields

        private FilterChain _Filter;

        private string _OutProperty;

        private TextElement _Text;

        #endregion

        #region Properties

        [BuildElement("filterchain", Required = true)]
        public FilterChain Filter
        {
            get { return _Filter; }
            set { _Filter = value; }
        }

        [TaskAttribute("propertyname", Required = true)]
        public string OutProperty
        {
            get { return _OutProperty; }
            set { _OutProperty = value; }
        }

        [BuildElement("text", Required = true)]
        public TextElement Text
        {
            get { return _Text; }
            set { _Text = value; }
        }

        #endregion

        #region Protected Methods

        protected override void ExecuteTask()
        {
            this.WriteWithFilters();
        }

        #endregion

        #region Private Methods

        private TextWriter GetWriter()
        {
            return new StringWriter();
        }

        private void WriteWithFilters()
        {
            TextWriter Writer = null;
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
                this.Properties[this.OutProperty] = Writer.ToString();
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