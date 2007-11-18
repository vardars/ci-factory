using System;
using System.IO;
using NAnt.Core;
using NAnt.Core.Attributes;
using CIFactory.NAnt.Types;

namespace CIFactory.NAnt.Tasks
{
    [TaskName("saveproperties")]
    public class SaveProperties : Task
    {
        #region Enums

        public enum FileFormat
        {
            Include,
            CommandLine
        }

        #endregion

        #region Fields

        private bool _Append = false;

        private FileInfo _File;

        private FileFormat _Format;

        private string _ProjectName;

        private SaveProperty[] _PropertyList;

        #endregion

        #region Properties

        [TaskAttribute("append", Required = false), BooleanValidator()]
        public bool Append
        {
            get { return _Append; }
            set { _Append = value; }
        }

        [TaskAttribute("file", Required = true)]
        public FileInfo File
        {
            get { return _File; }
            set { _File = value; }
        }

        [TaskAttribute("format", Required = true)]
        public FileFormat Format
        {
            get { return _Format; }
            set { _Format = value; }
        }

        [TaskAttribute("projectname", Required = false)]
        public string ProjectName
        {
            get { return _ProjectName; }
            set { _ProjectName = value; }
        }

        [BuildElementArray("property", Required = true)]
        public SaveProperty[] PropertyList
        {
            get { return _PropertyList; }
            set { _PropertyList = value; }
        }

        #endregion

        #region Public Methods

        public void CreateCommandLineFile()
        {
            StreamWriter Writer = null;
            try
            {
                Writer = this.GetWriter();
                foreach (SaveProperty Property in this.PropertyList)
                {
                    Writer.WriteLine(string.Format("-D:{0}=\"{1}\"", Property.PropertyName, Property.PropertyValue));
                }
            }
            finally
            {
                if (Writer != null)
                {
                    Writer.Close();
                }
            }
        }

        public void CreateIncludeFile()
        {
            StreamWriter Writer = null;
            try
            {
                Writer = this.GetWriter();
                Writer.WriteLine("<?xml version='1.0' encoding='utf-8' ?>");
                if (string.IsNullOrEmpty(this.ProjectName))
                    Writer.WriteLine("<project xmlns='http://nant.sf.net/schemas/nant.xsd'>");
                else
                    Writer.WriteLine(string.Format("<project xmlns='http://nant.sf.net/schemas/nant.xsd'  name='{0}'>", this.ProjectName));
                foreach (SaveProperty Property in this.PropertyList)
                {
                    Writer.WriteLine(string.Format("<property name='{0}' value='{1}' />", Property.PropertyName, Property.PropertyValue));
                }
                Writer.WriteLine("</project>");
            }
            finally
            {
                if (Writer != null)
                {
                    Writer.Close();
                }
            }
        }

        #endregion

        #region Protected Methods

        protected override void ExecuteTask()
        {
            switch (this.Format)
            {
                case FileFormat.CommandLine:
                    this.CreateCommandLineFile();
                    break;
                case FileFormat.Include:
                    this.CreateIncludeFile();
                    break;
                default:
                    throw new BuildException(string.Format("Format {0} is not supported.", this.Format.ToString()));
            }
        }

        #endregion

        #region Private Methods

        private StreamWriter GetWriter()
        {
            if (this.Append)
            {
                return this.File.AppendText();
            }
            else
            {
                return this.File.CreateText();
            }
        }

        #endregion

    }

}