using System;
using System.IO;
using System.Collections;
using NAnt.Core;
using NAnt.Core.Attributes;
using NAnt.Core.Tasks;

namespace UpdateVersion.Tasks
{
	[TaskName("GenerateAssemblyInfoList")]
	public class GenerateAssemblyInfoList : Task
	{

		private string _SearchPattern;
		private DirectoryInfo _SearchDirectory;
		private bool _Append = false;
		private DateTime _DateTime;
		private FileInfo _OutputFile;

		[TaskAttribute("SearchDirectory", Required=true)]
		public DirectoryInfo SearchDirectory
		{
			set
			{
				this._SearchDirectory = value;
			}
			get
			{
				return this._SearchDirectory;
			}
		}

		[TaskAttribute("SearchPattern", Required=true)]
		public string SearchPattern
		{
			set
			{
				this._SearchPattern = value;
			}
			get
			{
				return this._SearchPattern;
			}
		}

		[TaskAttribute("Append", Required=false)]
		public bool Append
		{
			set
			{
				this._Append = value;
			}
			get
			{
				return this._Append;
			}
		}

		[TaskAttribute("Output", Required=true)]
		public FileInfo OutputFile
		{
			set
			{
				this._OutputFile = value;
			}
			get
			{
				return this._OutputFile;
			}
		}

		[TaskAttribute("DateTime", Required=true)]
		public string SetNewerThanDateTime
		{
			set
			{
				this._DateTime = DateTime.Parse(value);
			}
		}

		public DateTime NewerThanDateTime
		{
			get
			{
				return this._DateTime;
			}
		}

		protected override void ExecuteTask()
		{
			FileInfo[] List;
			List =	this.GetAssemblyInfoList(this.SearchDirectory);
			this.WriteToFile(List);
		}

		public FileInfo[] GetAssemblyInfoList(DirectoryInfo directory)
		{
			ArrayList AssemblyInfoList = new ArrayList();
			foreach  (FileInfo FileInforamation in directory.GetFiles(this.SearchPattern)) 
			{
				if ((FileInforamation.LastWriteTime.CompareTo(this.NewerThanDateTime) > 0) & !FileInforamation.Name.StartsWith("AssemblyInfo"))
				{
					foreach (FileInfo AsmFileInforamation in directory.GetFiles("AssemblyInfo.*")) 
					{
						AssemblyInfoList.Add(AsmFileInforamation);
					}
					break;
				}
			}
			
			
			foreach (DirectoryInfo SubDirectoryInfo in directory.GetDirectories())
			{
				AssemblyInfoList.AddRange(this.GetAssemblyInfoList(SubDirectoryInfo));
			}
			return (FileInfo[]) AssemblyInfoList.ToArray(typeof(FileInfo));
		}

		public void WriteToFile(FileInfo[] fileList)
		{
			using(StreamWriter Writer = this.GetStream())
			{
				foreach (FileInfo FileInformation in fileList)
				{
					Writer.WriteLine(FileInformation.FullName);
				}
			}
		}

		public StreamWriter GetStream()
		{
			if (this.Append)
			{
				return this.OutputFile.AppendText();
			} 
			else 
			{
				return this.OutputFile.CreateText();
			}
		}

		public void test()
		{
			this.Append = true;
			this.SearchPattern = "*.vb";
			this.SearchDirectory = new DirectoryInfo("C:\\Projects\\Test CI\\Current\\Product\\Production");
			this._OutputFile = new FileInfo("C:\\temp\\AsmList.txt");
			this._DateTime = DateTime.Now;
			this._DateTime = this._DateTime.Subtract(new TimeSpan(100,0,0,0));
			this.ExecuteTask();
		}

	}
}
