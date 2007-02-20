using System;
using System.IO;
using System.Collections;
using NAnt.Core;
using NAnt.Core.Attributes;
using NAnt.Core.Tasks;
using MattGriffith.UpdateVersion;

namespace UpdateVersion.Tasks.Tasks
{
	[TaskName("IncrementAssemblyVersion")]
	public class IncrementAssemblyVersion : Task
	{
		
		private string _File;

		[TaskAttribute("File", Required=true)]
		public string File
		{
			set
			{
				this._File = value;
			}
			get
			{
				return this._File;
			}
		}

		protected override void ExecuteTask()
		{
			Options options = new Options();
			options.BuildNumberType = BuildNumberType.Fixed;
			options.InputFilename = this.File;
			options.OutputFilename = this.File;
			options.RevisionNumberType = RevisionNumberType.Increment;
			options.AttributeName = "AssemblyVersion";
			RunUpdateVersion version = new RunUpdateVersion();
			version.execute(options);
		}

		public void test()
		{
			this.File = "C:\\Projects\\Test CI\\Current\\Product\\Production\\MyApplication\\AssemblyInfo.vb";
			this.ExecuteTask();
		}

	}
}
