using System;
using System.IO;
using System.Linq;
using NAnt.Core.Attributes;
using NAnt.Core.Util;

namespace NAnt.Core.Functions
{
	[FunctionSet("scriptfile", "NAnt")]
	public class ScriptFileFunctions : FunctionSetBase
	{
		#region Public Instance Constructors

		public ScriptFileFunctions(Project project, Location location, PropertyDictionary properties)
			: base(project, location, properties)
		{
		}

		#endregion Public Instance Constructors

		#region Public Instance Methods

		[Function("exists")]
		public bool Exists(string scriptName)
		{
			return this.Project.ScriptFileInfoList.Contains(scriptName);
		}

		[Function("loaded")]
		public bool Loaded(string scriptFilePath)
		{
			return this.Project.ScriptFileInfoList.Where(scriptFileInfo => scriptFileInfo.FilePath == scriptFilePath).ToList().Count == 1;
		}

		[Function("get-file-Path")]
		public string GetFilePath(string scriptName)
		{
			return this.Project.ScriptFileInfoList[scriptName].FilePath;
		}

		[Function("get-directory-Path")]
		public string GetDirectoryPath(string scriptName)
		{
			return Path.GetDirectoryName(this.Project.ScriptFileInfoList[scriptName].FilePath);
		}

		[Function("get-name")]
		public string GetName(string scriptFilePath)
		{
			ScriptFileInfo ScriptInfo = null;
			try
			{
				ScriptInfo = this.Project.ScriptFileInfoList.Where(script => script.FilePath == scriptFilePath).Single<ScriptFileInfo>();
			}
			catch (Exception ex)
			{
				throw new BuildException(String.Format("Could not find script {0}.", scriptFilePath), ex);
			}
			return StringUtils.ConvertNullToEmpty(ScriptInfo.ProjectName);
		}

		[Function("get-current-name")]
		public string GetCurrentName()
		{
			ScriptFileInfo ScriptInfo = null;
			try
			{
				ScriptInfo = this.Project.ScriptFileInfoList.Where(script => script.FilePath == this.Location.FileName).Single<ScriptFileInfo>();
			}
			catch (Exception ex)
			{
				throw new BuildException("Could not find current script name.", this.Location, ex);
			}
			return StringUtils.ConvertNullToEmpty(ScriptInfo.ProjectName);
		}

		[Function("get-current-file-path")]
		public string GetCurrentFilePath()
		{
			ScriptFileInfo ScriptInfo = null;
			try
			{
				ScriptInfo = this.Project.ScriptFileInfoList.Where(script => script.FilePath == this.Location.FileName).Single<ScriptFileInfo>();
			}
			catch (Exception ex)
			{
				throw new BuildException("Could not find current script name.", this.Location, ex);
			}
			return ScriptInfo.FilePath;
		}

		[Function("get-current-directory")]
		public string GetCurrentDirectory()
		{
			ScriptFileInfo ScriptInfo = null;
			try
			{
				ScriptInfo = this.Project.ScriptFileInfoList.Where(script => script.FilePath == this.Location.FileName).Single<ScriptFileInfo>();
			}
			catch (Exception ex)
			{
				throw new BuildException("Could not find current script name.", this.Location, ex);
			}
			return Path.GetDirectoryName(ScriptInfo.FilePath);
		}

		#endregion Public Instance Methods
	}
}