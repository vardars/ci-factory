
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

namespace InstallShield.Tasks
{
    public abstract class BuildInstallShieldBase : ExternalProgramBase
	{
		private const string UNINSTALL_KEY = "Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\";

        protected string m_sIsmFile;
		private string m_sRelease;
		private string m_sStandAloneBuildExe;
		private string m_sBuildLocation;
		private string m_sUpdateProductVersion;
		private bool m_bBuildSilently = false;
		private bool m_bStopOnError = false;
		private bool m_bWarningAsError = false;
		private bool m_bNoCompile = false;

		[TaskAttribute("p", Required=true)]
		public string p
		{
			get { return m_sIsmFile; }
			set { m_sIsmFile = value; }
		}

		[TaskAttribute("r", Required=false)]
		public string r
		{
			get { return m_sRelease; }
			set { m_sRelease = value; }
		}

		[TaskAttribute("standalonebuildexe", Required=true)]
		public string standalonebuildexe
		{
			get { return m_sStandAloneBuildExe; }
			set { m_sStandAloneBuildExe = value; }
		}

		[TaskAttribute("b", Required=false)]
		public string b
		{
			get { return m_sBuildLocation; }
			set { m_sBuildLocation = value; }
		}

		[TaskAttribute("updateproductversion", Required=false)]
		public string updateproductversion
		{
			get { return m_sUpdateProductVersion; }
			set { m_sUpdateProductVersion = value; }
		}

		[TaskAttribute("s", Required=false)]
		[BooleanValidator()]
		public bool s
		{
			get { return m_bBuildSilently; }
			set { m_bBuildSilently = value; }
		}

		[TaskAttribute("x", Required=false)]
		[BooleanValidator()]
		public bool x
		{
			get { return m_bStopOnError; }
			set { m_bStopOnError = value; }
		}

		[TaskAttribute("w", Required=false)]
		[BooleanValidator()]
		public bool w
		{
			get { return m_bWarningAsError; }
			set { m_bWarningAsError = value; }
		}

		[TaskAttribute("n", Required=false)]
		[BooleanValidator()]
		public bool n
		{
			get { return m_bNoCompile; }
			set { m_bNoCompile = value; }
		}

		public override string ProgramFileName 
		{
			// The path and file name to the InstallShield stand alone build.
			get { return GetISSAExe(); }
		}

		// The base class calls this to build the command-line string.
		public abstract override string ProgramArguments 
		{
			get;
		}

		protected override void ExecuteTask() 
		{
			// we’ll let the base task do all the work.
			base.ExecuteTask();
		}

		protected string GetBaseArguments()
		{
			if (updateproductversion != null)
			{
				ChangeProductVersion();
			}

			string sCmdLine;
			sCmdLine = "-p " + "\"" + p + "\"";

			if (r != null)
			{
				sCmdLine += " -r " + "\"" + r + "\"";
			}

			if (b != null)
			{
				sCmdLine += " -b " + "\"" + b + "\"";
			}

			if (s)
			{
				sCmdLine += " -s";
			}

			if (w)
			{
				sCmdLine += " -w";
			}

			if (x)
			{
				sCmdLine += " -x";
			}

			if (n)
			{
				sCmdLine += " -n";
			}

			return sCmdLine;
		}

		private string GetISSAExe()
		{
			string sExe = m_sStandAloneBuildExe;
			if ('{' == sExe[0])
			{
				string sKey = Path.Combine(UNINSTALL_KEY, sExe);

				RegistryKey reg = Registry.LocalMachine.OpenSubKey(sKey);
				string sValue = (string)reg.GetValue("InstallLocation");

				sExe = Path.Combine(sValue, "IsSABld.exe");
				reg.Close();

				// format it
				sExe = " " + sExe + " ";
			}
			return sExe;
		}

		protected void ChangeProductVersion()
		{
			if (!File.Exists(m_sIsmFile))
			{
				return;
			}

			System.IO.FileAttributes fileAttribs = File.GetAttributes(m_sIsmFile);

			const string TEMP_FILE_EXT = ".tmp";
			string sTempFile = m_sIsmFile + TEMP_FILE_EXT;

			if (File.Exists(sTempFile))
			{
				File.SetAttributes(sTempFile, System.IO.FileAttributes.Normal);
				File.Delete(sTempFile);
			}

			bool bReplaced = false;

			try 
			{
				using (StreamWriter sw = new StreamWriter(sTempFile))
				{
					using (StreamReader sr = new StreamReader(m_sIsmFile)) 
					{
						const string BEGIN_PROPERTY_TABLE = "<table name=\"Property\">";
						const string END_TABLE = "</table>";
						const string END_OF_ELEMENT_START_OF_NEW = "</td><td>";
						const string ELEM_VALUE_TO_REPLACE = "ProductVersion";

						bool bFoundPropertySection = false;
						bool bFoundEndOfPropertySection = false;

						int nPos = -1;
						string sLine;
						while ((sLine = sr.ReadLine()) != null)
						{
							if (!bReplaced && !bFoundEndOfPropertySection)
							{
								if (bFoundPropertySection)
								{
									bFoundEndOfPropertySection = (-1 < sLine.LastIndexOf(END_TABLE));
								}
								else
								{
									bFoundPropertySection = (-1 < sLine.LastIndexOf(BEGIN_PROPERTY_TABLE));
								}

								if (!bFoundEndOfPropertySection && bFoundPropertySection && -1 < (nPos = sLine.LastIndexOf(ELEM_VALUE_TO_REPLACE)))
								{
									// Found "ProductVersion"

									nPos += ELEM_VALUE_TO_REPLACE.Length + END_OF_ELEMENT_START_OF_NEW.Length;
									string sBegin = sLine.Substring(0, nPos);

									// Find the end of the ProductVersion value
									// (if the version is "1.0.0.0", find the
									// position of the last '0')
									while (sLine[nPos++] != '<');

									--nPos;
									string sEnd = sLine.Substring(nPos, sLine.Length - nPos);

									
									sLine = sBegin + m_sUpdateProductVersion + sEnd;
									bReplaced = true;
								}
							}
							sw.WriteLine(sLine);
						}
					}
				}
				if (bReplaced)
				{
					File.SetAttributes(m_sIsmFile, System.IO.FileAttributes.Normal);
					File.Delete(m_sIsmFile);
					File.Move(sTempFile, m_sIsmFile);
					File.SetAttributes(m_sIsmFile, fileAttribs);
				}
			}
			catch (Exception ex) 
			{
				Debug.Assert(false, ex.ToString(), ex.Message);
				return;
			}

			Debug.Assert(bReplaced);
		}
	}

	[TaskName("buildinstallscript")]
	public class BuildInstallScript : BuildInstallShieldBase
	{
		// The base class calls this to build the command-line string.
		public override string ProgramArguments 
		{
			get
			{
				return base.GetBaseArguments();
			}
		}
	}

	[TaskName("buildinstallshieldmsi")]
	public class BuildInstallShieldMsi : BuildInstallShieldBase
	{
		private string m_sProductConfiguration;
		private string m_sReleaseConfiguration;
		private string m_sMergeModuleSearchPath;
		private bool m_bSkipUpgrade = false;
		private string m_sDotNetFrameworkPath;
		private string m_sMinimumTargetMsiVersion;
		private string m_sMinimumTargetDotNetFrameworkVersion;
		private bool m_bCreateSetupExe = false;
		private string m_sReleaseFlags;
		private bool m_bQ1 = false;
		private bool m_bQ2 = false;
		private bool m_bQ3 = false;

		[TaskAttribute("a", Required=false)]
		public string a
		{
			get { return m_sProductConfiguration; }
			set { m_sProductConfiguration = value; }
		}

		[TaskAttribute("c", Required=false)]
		public string c
		{
			get { return m_sReleaseConfiguration; }
			set { m_sReleaseConfiguration = value; }
		}

		[TaskAttribute("o", Required=false)]
		public string o
		{
			get { return m_sMergeModuleSearchPath; }
			set { m_sMergeModuleSearchPath = value; }
		}

		[TaskAttribute("h", Required=false)]
		[BooleanValidator()]
		public bool h
		{
			get { return m_bSkipUpgrade; }
			set { m_bSkipUpgrade = value; }
		}

		[TaskAttribute("t", Required=false)]
		public string t
		{
			get { return m_sDotNetFrameworkPath; }
			set { m_sDotNetFrameworkPath = value; }
		}

		[TaskAttribute("g", Required=false)]
		public string g
		{
			get { return m_sMinimumTargetMsiVersion; }
			set { m_sMinimumTargetMsiVersion = value; }
		}

		[TaskAttribute("j", Required=false)]
		public string j
		{
			get { return m_sMinimumTargetDotNetFrameworkVersion; }
			set { m_sMinimumTargetDotNetFrameworkVersion = value; }
		}

		[TaskAttribute("e", Required=false)]
		[BooleanValidator()]
		public bool e
		{
			get { return m_bCreateSetupExe; }
			set { m_bCreateSetupExe = value; }
		}

		[TaskAttribute("f", Required=false)]
		public string f
		{
			get { return m_sReleaseFlags; }
			set { m_sReleaseFlags = value; }
		}

		[TaskAttribute("q1", Required=false)]
		[BooleanValidator()]
		public bool q1
		{
			get { return m_bQ1; }
			set { m_bQ1 = value; }
		}

		[TaskAttribute("q2", Required=false)]
		[BooleanValidator()]
		public bool q2
		{
			get { return m_bQ2; }
			set { m_bQ2 = value; }
		}

		[TaskAttribute("q3", Required=false)]
		[BooleanValidator()]
		public bool q3
		{
			get { return m_bQ3; }
			set { m_bQ3 = value; }
		}

		// The base class calls this to build the command-line string.
		public override string ProgramArguments 
		{
			get 
			{
				string sCmdLine = base.GetBaseArguments();

				if (null != a)
				{
					sCmdLine += " -a " + "\"" + a + "\"";
				}

				if (null != o)
				{
					sCmdLine += " -o " + "\"" + o + "\"";
				}

                if (null != c)
				{
					sCmdLine += " -c " + "\"" + c + "\"";
				}

				if (h)
				{
					sCmdLine += " -h";
				}

                if (null != t)
				{
					sCmdLine += " -t " + "\"" + t + "\"";
				}

                if (null != g)
				{
					sCmdLine += " -g " + "\"" + g + "\"";
				}

                if (null != j)
				{
					sCmdLine += " -j " + "\"" + j + "\"";
				}

				if (e)
				{
					sCmdLine += " -e y";
				}
				else
				{
					sCmdLine += " -e n";
				}

                if (null != f)
				{
					sCmdLine += " -f " + "\"" + f + "\"";
				}

				if (q1)
				{
					sCmdLine += " -q1";
				}

				if (q2) 
				{
					sCmdLine += " -q2";
				}

				if (q3) 
				{
					sCmdLine += " -q3";
				}

				return sCmdLine;
			}
		}
	}

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
