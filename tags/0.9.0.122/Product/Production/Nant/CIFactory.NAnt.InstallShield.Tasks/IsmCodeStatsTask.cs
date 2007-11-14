using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;
using Microsoft.Win32;
using NAnt;
using NAnt.Core;
using NAnt.Core.Attributes;
using NAnt.Core.Tasks;
using NAnt.Core.Types;
using NAnt.Contrib.Types;

namespace CIFactory.NAnt.InstallShield.Tasks
{
    [TaskName("ismcodestats")]
    public class IsmCodeStatsTask : ExternalProgramBase
    {
        // Methods
        //private FileCodeCountInfo CountFile(string fileName);

        // Fields
        private bool _appendFile;
        private string _buildName;
        private CodeStatsCountCollection _codeStats;
        private int _commentLineCount;
        private const string _commentBeginComment = "<!--";
        private const string _commentEndComment = "-->";
        private int _emptyLinesCount;
        private ArrayList _fileNames;
        private int _lineCount;
        private FileInfo _outputFile;
        private bool _summarize;

        public IsmCodeStatsTask()
        {
            this._fileNames = new ArrayList();
            this._codeStats = new CodeStatsCountCollection();
        }

        // Nested Types
        [StructLayout(LayoutKind.Sequential)]
        private struct FileCodeCountInfo
        {
            public int LineCount;
            public int CommentLineCount;
            public int EmptyLineCount;
            public string FileName;
            public FileCodeCountInfo(string fileName, int lineCount, int commentLineCount, int emptyLineCount)
            {
                FileName = fileName;
                LineCount = lineCount;
                CommentLineCount = commentLineCount;
                EmptyLineCount = emptyLineCount;
            }
        }

        [TaskAttribute("output")]
        public FileInfo OutputFile
        {
            get
            {
                return this._outputFile;
            }
            set
            {
                this._outputFile = value;
            }
        }

        [TaskAttribute("append")]
        public bool AppendFile
        {
            get
            {
                return this._appendFile;
            }
            set
            {
                this._appendFile = value;
            }
        }

        [TaskAttribute("buildname")]
        public string BuildName
        {
            get
            {
                return this._buildName;
            }
            set
            {
                this._buildName = value;
            }
        }

        [TaskAttribute("summarize")]
        public bool Summarize
        {
            get
            {
                return this._summarize;
            }
            set
            {
                this._summarize = value;
            }
        }

        [BuildElementCollection("counts", "count")]
        public CodeStatsCountCollection CodeStats
        {
            get
            {
                return this._codeStats;
            }
        }

        // The base class calls this to build the command-line string.
        public override string ProgramArguments
        {
            get
            {
                return "";
            }
        }

        private IsmCodeStatsTask.FileCodeCountInfo CountFile(string fileName)
        {
            int num1 = 0;
            int num2 = 0;
            int num3 = 0;
            bool flag1 = false;
            using (StreamReader reader1 = File.OpenText(fileName))
            {
                while (reader1.Peek() != -1)
                {
                    string text1 = reader1.ReadLine();
                    if (text1 == null)
                    {
                        continue;
                    }
                    text1 = text1.Trim();
                    if (text1 == "")
                    {
                        num3++;
                    }
                    else if (text1.StartsWith(_commentBeginComment))
                    {
                        num2++;
                        if (text1.IndexOf(_commentEndComment) == 0)
                        {
                            flag1 = true;
                        }
                    }
                    else if (flag1)
                    {
                        num2++;
                        if (text1.IndexOf(_commentEndComment) > 0)
                        {
                            flag1 = false;
                        }
                    }
                    num1++;
                }
                reader1.Close();
            }
            if (!this.Summarize)
            {
                this.Log(Level.Info, "{0} Totals:\t[T] {1}\t[C] {2}\t[E] {3}", new object[] { fileName, num1, num2, num3 });
            }
            return new IsmCodeStatsTask.FileCodeCountInfo(fileName, num1, num2, num3);
        }

        private void CountLines(FileSet TargetFileSet, string label)
        {
            this._lineCount = 0;
            this._commentLineCount = 0;
            this._emptyLinesCount = 0;
            this._fileNames = new ArrayList();
            this._fileNames.Capacity = TargetFileSet.FileNames.Count;
            StringEnumerator enumerator1 = TargetFileSet.FileNames.GetEnumerator();
            while (enumerator1.MoveNext())
            {
                string text1 = enumerator1.Current;
                IsmCodeStatsTask.FileCodeCountInfo info1 = this.CountFile(text1);
                this._lineCount += info1.LineCount;
                this._emptyLinesCount += info1.EmptyLineCount;
                this._commentLineCount += info1.CommentLineCount;
                this._fileNames.Add(info1);
            }
            this._fileNames.TrimToSize();
            this.Log(Level.Info, "Totals:\t[{0}] \t[T] {1}\t[C] {2}\t[E] {3}", new object[] { label, this._lineCount, this._commentLineCount, this._emptyLinesCount });
        }

        protected override void ExecuteTask()
        {
            XmlDocument document1 = new XmlDocument();
            XmlNode node1 = null;
            if (this.OutputFile != null)
            {
                if (this.AppendFile && this.OutputFile.Exists)
                {
                    document1.Load(this.OutputFile.FullName);
                    node1 = document1.SelectSingleNode("//code-summaries");
                }
                else
                {
                    XmlNode node2 = document1.CreateXmlDeclaration("1.0", "UTF-8", "yes");
                    document1.AppendChild(node2);
                    node1 = document1.CreateElement("code-summaries");
                    document1.AppendChild(node1);
                }
                XmlNode node3 = document1.CreateElement("code-summary");
                XmlAttribute attribute1 = document1.CreateAttribute("date");
                attribute1.Value = XmlConvert.ToString(DateTime.Now);
                node3.Attributes.Append(attribute1);
                XmlAttribute attribute2 = document1.CreateAttribute("buildname");
                attribute2.Value = this.BuildName;
                node3.Attributes.Append(attribute2);
                CodeStatsCountEnumerator enumerator1 = this.CodeStats.GetEnumerator();
                while (enumerator1.MoveNext())
                {
                    CodeStatsCount count1 = enumerator1.Current;
                    this.CountLines(count1.FileSet, count1.Label);
                    XmlNode node4 = document1.CreateElement("linecount");
                    XmlAttribute attribute3 = document1.CreateAttribute("label");
                    attribute3.Value = count1.Label;
                    node4.Attributes.Append(attribute3);
                    XmlAttribute attribute4 = document1.CreateAttribute("totalLineCount");
                    attribute4.Value = this._lineCount.ToString();
                    node4.Attributes.Append(attribute4);
                    XmlAttribute attribute5 = document1.CreateAttribute("emptyLineCount");
                    attribute5.Value = this._emptyLinesCount.ToString();
                    node4.Attributes.Append(attribute5);
                    XmlAttribute attribute6 = document1.CreateAttribute("commentLineCount");
                    attribute6.Value = this._commentLineCount.ToString();
                    node4.Attributes.Append(attribute6);
                    if (!this.Summarize)
                    {
                        XmlNode node5 = document1.CreateElement("file-summaries");
                        ICollection collection1 = this._fileNames;
                        int num1 = collection1.Count;
                        foreach (IsmCodeStatsTask.FileCodeCountInfo info1 in collection1)
                        {
                            XmlElement element1 = document1.CreateElement("file-summary");
                            element1.SetAttribute("name", info1.FileName);
                            element1.SetAttribute("totalLineCount", info1.LineCount.ToString());
                            element1.SetAttribute("emptyLineCount", info1.EmptyLineCount.ToString());
                            element1.SetAttribute("commentLineCount", info1.CommentLineCount.ToString());
                            node5.AppendChild(element1);
                        }
                        node4.AppendChild(node5);
                    }
                    node3.AppendChild(node4);
                }
                node1.AppendChild(node3);
                document1.Save(this.OutputFile.FullName);
            }
        }
    }
}
