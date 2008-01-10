#region Copyright © 2006 Grant Drake. All rights reserved.
/*
Copyright © 2006 Grant Drake. All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions
are met:

1. Redistributions of source code must retain the above copyright
   notice, this list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright
   notice, this list of conditions and the following disclaimer in the
   documentation and/or other materials provided with the distribution.
3. The name of the author may not be used to endorse or promote products
   derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE AUTHOR "AS IS" AND ANY EXPRESS OR
IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE. 
*/
#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;

using NAnt.Core;
using NAnt.Core.Tasks;
using NAnt.Core.Attributes;
using NAnt.Core.Types;
using NAnt.Core.Util;

using NCoverExplorer.NAntTasks.Types;

namespace NCoverExplorer.NAntTasks
{
	[TaskName("ncoverexplorer")]
    public class NCoverExplorerTask : ExternalProgramBase
    {
        // Fields
        private string _configName = string.Empty;
        private readonly CoverageExclusionElementCollection _coverageExclusionElements = new CoverageExclusionElementCollection();
        private FileSet _coverageFiles = new FileSet();
        private bool _failCombinedMinimum;
        private bool _failMinimum;
        private string _htmlReportName;
        private string _mergeFileName;
        private float _minimumCoverage = 100f;
        private readonly ModuleThresholdElementCollection _moduleThresholdElements = new ModuleThresholdElementCollection();
        private string _outputDir;
        private readonly StringBuilder _programArguments = new StringBuilder();
        private string _projectName;
        private CoverageReportType _reportType = CoverageReportType.ModuleSummary;
        private float _satisfactoryCoverage = 100f;
        private bool _showExcluded;
        private TreeFilterStyle _treeFilterStyle = TreeFilterStyle.None;
        private TreeSortStyle _treeSortStyle = TreeSortStyle.Name;
        private string _xmlReportName;
        private const string DefaultApplicationName = "NCoverExplorer.Console.exe";

        // Methods
        public NCoverExplorerTask()
        {
            this.ExeName = "NCoverExplorer.Console.exe";
        }

        private void _BuildLegacyArguments()
        {
            this._programArguments.AppendFormat("/r:{0} ", this._reportType);
            if (!string.IsNullOrEmpty(this._htmlReportName))
            {
                if (!Path.IsPathRooted(this._htmlReportName) && !string.IsNullOrEmpty(this._outputDir))
                {
                    this._htmlReportName = Path.Combine(this._outputDir, this._htmlReportName);
                }
                this._programArguments.AppendFormat("/h:\"{0}\" ", this._htmlReportName);
            }
            if (!string.IsNullOrEmpty(this._xmlReportName))
            {
                if (!Path.IsPathRooted(this._xmlReportName) && !string.IsNullOrEmpty(this._outputDir))
                {
                    this._xmlReportName = Path.Combine(this._outputDir, this._xmlReportName);
                }
                this._programArguments.AppendFormat("/x:\"{0}\" ", this._xmlReportName);
            }
            if (!string.IsNullOrEmpty(this._mergeFileName))
            {
                if (!Path.IsPathRooted(this._mergeFileName) && !string.IsNullOrEmpty(this._outputDir))
                {
                    this._mergeFileName = Path.Combine(this._outputDir, this._mergeFileName);
                }
                this._programArguments.AppendFormat("/s:\"{0}\" ", this._mergeFileName);
            }
            if (this._showExcluded)
            {
                this._programArguments.Append("/e ");
            }
            if (this._failMinimum)
            {
                this._programArguments.Append("/f ");
            }
            foreach (string str in this._coverageFiles.FileNames)
            {
                this._programArguments.AppendFormat("\"{0}\" ", str);
            }
        }

        private void _BuildLegacyTempConfigXmlFile(string settingsFileName)
        {
            using (Stream stream = File.Create(settingsFileName))
            {
                XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8);
                writer.Indentation = 2;
                writer.Formatting = Formatting.Indented;
                writer.WriteStartDocument();
                writer.WriteStartElement("NCoverExplorer");
                writer.WriteElementString("ProjectName", this._projectName);
                writer.WriteElementString("SatisfactoryCoverageThreshold", XmlConvert.ToString(this._satisfactoryCoverage));
                writer.WriteElementString("TreeSortStyle", this._treeSortStyle.ToString());
                writer.WriteElementString("TreeFilterStyle", this._treeFilterStyle.ToString());
                writer.WriteStartElement("CoverageExclusions");
                foreach (CoverageExclusionElement element in this._coverageExclusionElements)
                {
                    writer.WriteStartElement("CoverageExclusion");
                    writer.WriteElementString("ExclusionType", element.ExclusionType);
                    writer.WriteElementString("Pattern", element.Pattern);
                    writer.WriteElementString("IsRegex", XmlConvert.ToString(element.IsRegex));
                    writer.WriteElementString("Enabled", XmlConvert.ToString(element.Enabled));
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteStartElement("ModuleThresholds");
                foreach (ModuleThresholdElement element2 in this._moduleThresholdElements)
                {
                    writer.WriteStartElement("ModuleThreshold");
                    writer.WriteAttributeString("ModuleName", element2.ModuleName);
                    writer.WriteAttributeString("SatisfactoryCoverage", XmlConvert.ToString(element2.SatisfactoryCoverage));
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
                stream.Close();
            }
        }

        private void _BuildTempConfigXmlFile(string settingsFileName)
        {
            using (Stream stream = File.Create(settingsFileName))
            {
                XmlTextWriter writer = new XmlTextWriter(stream, Encoding.UTF8);
                writer.Indentation = 2;
                writer.Formatting = Formatting.Indented;
                writer.WriteStartDocument();
                writer.WriteStartElement("ConsoleSetting");
                writer.WriteElementString("ProjectName", this._projectName);
                writer.WriteElementString("SatisfactoryCoverageThreshold", XmlConvert.ToString(this._satisfactoryCoverage));
                writer.WriteElementString("TreeSortStyle", this._treeSortStyle.ToString());
                writer.WriteElementString("TreeFilterStyle", this._treeFilterStyle.ToString());
                writer.WriteStartElement("CoverageExclusions");
                foreach (CoverageExclusionElement element in this._coverageExclusionElements)
                {
                    writer.WriteStartElement("CoverageExclusion");
                    writer.WriteElementString("ExclusionType", element.ExclusionType);
                    writer.WriteElementString("Pattern", element.Pattern);
                    writer.WriteElementString("IsRegex", XmlConvert.ToString(element.IsRegex));
                    writer.WriteElementString("Enabled", XmlConvert.ToString(element.Enabled));
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteStartElement("ModuleThresholds");
                foreach (ModuleThresholdElement element2 in this._moduleThresholdElements)
                {
                    writer.WriteStartElement("ModuleThreshold");
                    writer.WriteAttributeString("ModuleName", element2.ModuleName);
                    writer.WriteAttributeString("SatisfactoryCoverage", XmlConvert.ToString(element2.SatisfactoryCoverage));
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteStartElement("CoverageFileNames");
                foreach (string str in this._coverageFiles.FileNames)
                {
                    writer.WriteElementString("CoverageFileName", str);
                }
                writer.WriteEndElement();
                writer.WriteElementString("ReportType", this._reportType.ToString());
                if (!string.IsNullOrEmpty(this._htmlReportName))
                {
                    if (!Path.IsPathRooted(this._htmlReportName) && !string.IsNullOrEmpty(this._outputDir))
                    {
                        this._htmlReportName = Path.Combine(this._outputDir, this._htmlReportName);
                    }
                    writer.WriteElementString("HtmlReportFileName", this._htmlReportName);
                }
                if (!string.IsNullOrEmpty(this._xmlReportName))
                {
                    if (!Path.IsPathRooted(this._xmlReportName) && !string.IsNullOrEmpty(this._outputDir))
                    {
                        this._xmlReportName = Path.Combine(this._outputDir, this._xmlReportName);
                    }
                    writer.WriteElementString("XmlReportFileName", this._xmlReportName);
                }
                if (!string.IsNullOrEmpty(this._mergeFileName))
                {
                    if (!Path.IsPathRooted(this._mergeFileName) && !string.IsNullOrEmpty(this._outputDir))
                    {
                        this._mergeFileName = Path.Combine(this._outputDir, this._mergeFileName);
                    }
                    writer.WriteElementString("MergeFileName", this._mergeFileName);
                }
                writer.WriteElementString("ShowExcludedFooter", XmlConvert.ToString(this._showExcluded));
                writer.WriteElementString("FailIfBelowMinimum", XmlConvert.ToString(this._failMinimum));
                writer.WriteElementString("FailIfBelowCombinedMinimum", XmlConvert.ToString(this._failCombinedMinimum));
                writer.WriteElementString("MinimumCoverage", this._minimumCoverage.ToString());
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
                stream.Close();
            }
        }

        private string _GetConfigFilename()
        {
            if ((this._configName == null) || (this._configName.Length == 0))
            {
                this._configName = Path.GetTempFileName();
            }
            return this._configName;
        }

        private bool _IsLegacyNCoverExplorerConsole(string exeName)
        {
            if (!File.Exists(exeName))
            {
                return false;
            }
            string versionForExecutable = PathSearch.GetVersionForExecutable(exeName, false);
            return ((versionForExecutable.Length > 0) && (int.Parse(versionForExecutable.Replace(".", "")) < 140));
        }

        private void _OnProcessExited(object sender, EventArgs e)
        {
            string path = this._GetConfigFilename();
            if (File.Exists(path))
            {
                this.Log(Level.Verbose, "Deleting config file: " + path);
                File.Delete(path);
            }
        }

        protected override void PrepareProcess(Process process)
        {
            if (this._coverageFiles.FileNames.Count == 0)
            {
                throw new BuildException("No matching coverage files found in the <fileset> specified in your <ncoverexplorer> NAnt task.");
            }
            string settingsFileName = this._GetConfigFilename();
            if (this._IsLegacyNCoverExplorerConsole(this.ExeName))
            {
                this._BuildLegacyTempConfigXmlFile(settingsFileName);
                this._BuildLegacyArguments();
            }
            else
            {
                this._BuildTempConfigXmlFile(settingsFileName);
            }
            this._programArguments.AppendFormat("/c:\"{0}\" ", settingsFileName);
            if (base.IsLogEnabledFor(Level.Verbose))
            {
                string message = File.ReadAllText(settingsFileName);
                this.Log(Level.Verbose, "Contents of NCoverExplorer settings file:");
                this.Log(Level.Verbose, message);
            }
            this.Log(Level.Verbose, "Working directory: {0}", new object[] { this.BaseDirectory });
            this.Log(Level.Verbose, "Arguments: {0}", new object[] { this.ProgramArguments });
            base.PrepareProcess(process);
        }

        protected override Process StartProcess()
        {
            if (!Path.IsPathRooted(this.ExeName))
            {
                this.ExeName = this.Project.GetFullPath(this.ExeName);
            }
            Process process = base.StartProcess();
            process.Exited += new EventHandler(this._OnProcessExited);
            return process;
        }

        // Properties
        [BuildElementCollection("exclusions", "exclusion")]
        public CoverageExclusionElementCollection CoverageExclusions
        {
            get
            {
                return this._coverageExclusionElements;
            }
        }

        [BuildElement("fileset")]
        public FileSet CoverageFiles
        {
            get
            {
                return this._coverageFiles;
            }
            set
            {
                this._coverageFiles = value;
            }
        }

        [StringValidator(AllowEmpty=false), TaskAttribute("program", Required=false)]
        public override string ExeName
        {
            get
            {
                return base.ExeName;
            }
            set
            {
                base.ExeName = value;
            }
        }

        [TaskAttribute("failCombinedMinimum")]
        public bool FailCombinedMinimum
        {
            get
            {
                return this._failCombinedMinimum;
            }
            set
            {
                this._failCombinedMinimum = value;
            }
        }

        [TaskAttribute("failMinimum")]
        public bool FailMinimum
        {
            get
            {
                return this._failMinimum;
            }
            set
            {
                this._failMinimum = value;
            }
        }

        [TaskAttribute("filter")]
        public string Filter
        {
            get
            {
                return this._treeFilterStyle.ToString();
            }
            set
            {
                this._treeFilterStyle = (TreeFilterStyle) Enum.Parse(typeof(TreeFilterStyle), value);
                if (!Enum.IsDefined(typeof(TreeFilterStyle), this._treeFilterStyle))
                {
                    throw new ArgumentOutOfRangeException("value", value, "Not a valid filter style.");
                }
            }
        }

        [TaskAttribute("htmlReportName")]
        public string HtmlReportName
        {
            get
            {
                return this._htmlReportName;
            }
            set
            {
                this._htmlReportName = value;
            }
        }

        [TaskAttribute("mergeFileName")]
        public string MergeFileName
        {
            get
            {
                return this._mergeFileName;
            }
            set
            {
                this._mergeFileName = value;
            }
        }

        [TaskAttribute("minimumCoverage")]
        public float MinimumCoverage
        {
            get
            {
                return this._minimumCoverage;
            }
            set
            {
                this._minimumCoverage = value;
            }
        }

        [BuildElementCollection("moduleThresholds", "moduleThreshold")]
        public ModuleThresholdElementCollection ModuleThresholds
        {
            get
            {
                return this._moduleThresholdElements;
            }
        }

        [StringValidator(AllowEmpty=false), TaskAttribute("outputDir", Required=false)]
        public string OutputDir
        {
            get
            {
                return this._outputDir;
            }
            set
            {
                this._outputDir = value;
            }
        }

        public override string ProgramArguments
        {
            get
            {
                return this._programArguments.ToString();
            }
        }

        [TaskAttribute("projectName")]
        public string ProjectName
        {
            get
            {
                return this._projectName;
            }
            set
            {
                this._projectName = value;
            }
        }

        [TaskAttribute("reportType")]
        public string ReportType
        {
            get
            {
                return this._reportType.ToString();
            }
            set
            {
                this._reportType = (CoverageReportType) Enum.Parse(typeof(CoverageReportType), value);
                if (!Enum.IsDefined(typeof(CoverageReportType), this._reportType))
                {
                    throw new ArgumentOutOfRangeException("value", value, "Not a valid report type.");
                }
            }
        }

        [TaskAttribute("satisfactoryCoverage")]
        public float SatisfactoryCoverage
        {
            get
            {
                return this._satisfactoryCoverage;
            }
            set
            {
                this._satisfactoryCoverage = value;
            }
        }

        [TaskAttribute("showExcluded"), BooleanValidator]
        public bool ShowExcluded
        {
            get
            {
                return this._showExcluded;
            }
            set
            {
                this._showExcluded = value;
            }
        }

        [TaskAttribute("sort")]
        public string Sort
        {
            get
            {
                return this._treeSortStyle.ToString();
            }
            set
            {
                this._treeSortStyle = (TreeSortStyle) Enum.Parse(typeof(TreeSortStyle), value);
                if (!Enum.IsDefined(typeof(TreeSortStyle), this._treeSortStyle))
                {
                    throw new ArgumentOutOfRangeException("value", value, "Not a valid sort style.");
                }
            }
        }

        [TaskAttribute("xmlReportName")]
        public string XmlReportName
        {
            get
            {
                return this._xmlReportName;
            }
            set
            {
                this._xmlReportName = value;
            }
        }
    }

}
