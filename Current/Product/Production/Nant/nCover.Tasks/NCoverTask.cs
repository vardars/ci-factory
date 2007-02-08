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

using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using System.Collections.Specialized;

using NAnt.Core;
using NAnt.Core.Attributes;
using NAnt.Core.Types;
using NAnt.Core.Util;

namespace NAnt.NCoverExplorer.Tasks
{
	/// <summary>
	/// NAnt task for automating NCover.Console.exe, with both NCover 1.3.3 and 1.5.x support. Note that this task
	/// relies on CoverLib.dll being COM registered at the relevant NCover folder path location prior to being called.
	/// </summary>
	/// <example>
	/// This example shows the standard profiling using NCover 1.3.3 for standard nunit tests with minimum arguments.
	/// Defaults are with no logging, profiling all assemblies, output filename of coverage.xml and this assumes that
	/// NCover and NUnit are in the path.
	///        <code>
	///            <![CDATA[
	///            <ncover  commandLineExe="${nunit.path}\nunit-console.exe" 
	///						commandLineArgs="${build.path}\myapp.tests.dll"
	///						version="1.3.3" >
	///            </ncover>
	///            ]]>
	///        </code>
	/// </example>
	/// <example>
	/// This example shows the standard profiling using NCover 1.5.4 for standard nunit tests with minimal arguments.
	/// Defaults are with logging to coverage.log, profiling all assemblies, output filename of coverage.xml and this 
	/// example specifies a full path to NCover.Console.exe.
	///        <code>
	///            <![CDATA[
	///            <ncover  program="C:\Program Files\NCover\ncover.console.exe" 
	///						commandLineExe="${nunit.path}\nunit-console.exe" 
	///						commandLineArgs="${build.path}\myapp.tests.dll" >
	///            </ncover>
	///            ]]>
	///        </code>
	/// </example>
	/// <example>
	/// This example shows the standard profiling using NCover 1.5.4 for a Windows application, specifying a coverage
	/// exclusion, verbose logging to a named file, specifically named log, output xml and html files. It also shows
	/// coverage exclusion attributes, overriding the NCover location to run from and a way of listing assemblies
	/// to be included in the profiled NCover results.
	///        <code>
	///            <![CDATA[
	///            <ncover  program="tools\ncover\ncover.console.exe" 
	///						commandLineExe="${nunit.path}\nunit-console.exe" 
	///						commandLineArgs="myapp.tests.dll"
	///						coverageFile="myapp.coverage.xml"
	///						logLevel="Verbose"
	///						logFile="myapp.coverage.log"
	///						workingDirectory="${build.path}"
	///						excludeAttributes="CoverageExcludeAttribute" >
	///                <assemblies basedir="${build.path}">
	///                    <include name="myapp.*.dll" />
	///                </assemblies>
	///            </ncover>
	///            ]]>
	///        </code>
	/// </example>
	[TaskName("ncover")]
	public class NCoverTask : NAnt.Core.Tasks.ExternalProgramBase
	{
		#region Private Variables / Constants

		private const string DefaultApplicationName = "NCover.Console.exe";

		private string _version;
		private string _commandLineExe;
		private string _commandLineArgs;
		private string _coverageFile;
		private NCoverLogLevel _logLevel;
		private string _logFile;
		private string _workingDirectory;
		private FileSet _assemblyFiles;	
		private bool _copyXsl;
		
		private string _excludeAttributes;
		private bool _profileIIS;
		private string _profileService;

		private string _settingsFile;
		private StringBuilder _programArguments;

		#endregion Private Variables / Constants

		#region NCoverLogLevel Enum

		/// <summary>
		/// Logging levels to use within NCover.
		/// </summary>
		public enum NCoverLogLevel
		{
			/// <summary>Default level - no logging.</summary>
			Quiet = 0,
			/// <summary>Writes standard log output.</summary>
			Normal,
			/// <summary>Writes verbose log output.</summary>
			Verbose
		}

		#endregion NCoverLogLevel Enum

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="NCoverTask"/> class.
		/// </summary>
		public NCoverTask()
		{
			_version = "1.5";
			_commandLineExe = string.Empty;
			_commandLineArgs = string.Empty;
			_workingDirectory = string.Empty;
			_coverageFile = "coverage.xml";
			_logLevel = NCoverLogLevel.Normal;
			_logFile = "coverage.log";
			_copyXsl = true;

			_assemblyFiles = new FileSet();
			_settingsFile = string.Empty;

			_excludeAttributes = string.Empty;
			_profileIIS = false;
			_profileService = string.Empty;

			this.ExeName = DefaultApplicationName;
			_programArguments = new StringBuilder();
		}

		#endregion Constructors

		#region Properties

		/// <summary>
		/// The name of the executable that should be used to launch the external program.
		/// </summary>
		/// <value>
		/// The name of the executable that should be used to launch the external
		/// program, or <see langword="null"/> if no name is specified.
		/// </value>
		/// <remarks>
		/// If available, the configured value in the NAnt configuration
		/// file will be used if no name is specified.
		/// </remarks>
		[TaskAttribute("program", Required=false)]
		[StringValidator(AllowEmpty=false)]
		public override string ExeName
		{
			get { return base.ExeName; }
			set { base.ExeName = value; }
		}

		/// <summary>
		/// Gets or sets the NCover version this task runs against. Different NCover versions have different command
		/// line arguments. The default is support for NCover 1.5.x by using "1.5", other supported versions at 
		/// this time are "1.3.3".
		/// </summary>
		/// <value>The NCover compatibility version, "1.3.3" or "1.5".</value>
		[TaskAttribute("version", Required=false)]
		public string Version
		{
			get { return _version; }
			set { _version = value; }
		}

		/// <summary>
		/// The command line executable to be launched by NCover (such as nunit-console.exe).
		/// </summary>
		[TaskAttribute("commandLineExe")]
		[StringValidator(AllowEmpty=false)]
		public string CommandLineExe
		{
			get { return _commandLineExe; }
			set { _commandLineExe = value; }
		}

		/// <summary>
		/// The arguments to pass to the command line executable to be launched by NCover (such as nunit-console.exe).
		/// </summary>
		[TaskAttribute("commandLineArgs", Required=false)]
		public string CommandLineArgs
		{
			get { return _commandLineArgs; }
			set { _commandLineArgs = value; }
		}

		/// <summary>
		/// The filename for the output coverage.xml file (default).
		/// </summary>
		[TaskAttribute("coverageFile", Required=false)]
		public string CoverageFile
		{
			get { return _coverageFile; }
			set { _coverageFile = value; }
		}

		/// <summary>
		/// What level of NCover logging to provide. Values are "Quiet", "Normal" (default) and "Verbose".
		/// Note that due to a current bug in NCover 1.5.4 there is no different between Quiet and Normal
		/// since the //q argument (NoLog=true) will lock NCover. So logging is always on for NCover 1.5.4
		/// </summary>
		[TaskAttribute("logLevel")]
		public NCoverLogLevel LogLevel
		{
			get { return _logLevel; }
			set { _logLevel = value; }
		}

		/// <summary>
		/// Gets or sets the logfile name to write to if logLevel is set to anything other than "Quiet". The default
		/// is "coverage.log".
		/// </summary>
		[TaskAttribute("logFile", Required=false)]
		public string LogFile
		{
			get { return _logFile; }
			set { _logFile = value; }
		}

		/// <summary>
		/// Gets or sets the working directory for the command line executable.
		/// </summary>
		[TaskAttribute("workingDirectory", Required=false)]
		public string WorkingDirectory
		{
			get { return _workingDirectory; }
			set { _workingDirectory = value; }
		}

		/// <summary>
		/// Determines whether to copy the xsl file form the NCover folder to the output folder. Applies
		/// to NCover 1.3.3 only (NCover 1.5 onwards never copies the xsl). 
		/// Default value is <see langword="true" /> (for NCover 1.3.3).
		/// </summary>
		[TaskAttribute("copyXsl")]
		[BooleanValidator()]
		public bool CopyXsl
		{
			get { return _copyXsl; }
			set { _copyXsl = value; }
		}

		/// <summary>
		/// If coverage exclusion attributes have been applied (NCover 1.5.4 onwards) specify the full namespace
		/// to the attribute including the "Attribute" suffix - e.g. "CoverageExcludeAttribute" if defined in no
		/// namespace.
		/// </summary>
		[TaskAttribute("excludeAttributes")]
		public string ExcludeAttributes
		{
			get { return _excludeAttributes; }
			set { _excludeAttributes = value; }
		}

		/// <summary>
		/// Determines whether to profile under IIS (//iis). Default value is <see langword="false" />.
		/// </summary>
		[TaskAttribute("profileIIS")]
		[BooleanValidator()]
		public bool ProfileIIS
		{
			get { return _profileIIS; }
			set { _profileIIS = value; }
		}

		/// <summary>
		/// The service name to profile if any (//svc). Default is none.
		/// </summary>
		[TaskAttribute("profileService")]
		public string ProfileService
		{
			get { return _profileService; }
			set { _profileService = value; }
		}

		/// <summary>
		/// Used to specify the assemblies to be profiled. To use see <see cref="FileSet" />.
		/// </summary>
		[BuildElement("assemblies")]
		public FileSet Assemblies
		{
			get { return _assemblyFiles; }
			set { _assemblyFiles = value; }
		}

		#endregion Properties

		#region Override implementation of ExternalProgramBase

		/// <summary>
		/// The command-line arguments for the external program.
		/// </summary>
		public override string ProgramArguments
		{
			get	{ return _programArguments.ToString(); }
		}

		/// <summary>
		/// Performs logic before the external process is started.
		/// </summary>
		/// <param name="process">Process.</param>
		protected override void PrepareProcess(Process process) 
		{
			_settingsFile = Path.GetTempFileName() + ".ncoversettings";
			Log( Level.Verbose, "Creating settings file: " + _settingsFile );

			if (_version == "1.3.3")
			{
				_BuildTempSettingsXmlFileForNCover133();
				_programArguments.AppendFormat("/r \"{0}\" ", _settingsFile);
			}
			else
			{
				_BuildTempSettingsXmlFileForNCover15();
				_programArguments.AppendFormat("//r \"{0}\" ", _settingsFile);
			}

			Log(Level.Verbose, "Working directory: {0}", BaseDirectory);
			Log(Level.Verbose, "Arguments: {0}", ProgramArguments);
			if (IsLogEnabledFor(Level.Verbose))
			{
				// Dump out the contents of the settings file.
				string fileContents = _GetFileContents(_settingsFile);
				Log(Level.Verbose, fileContents);
			}

			base.PrepareProcess(process);
		}

		/// <summary>
		/// Starts the process and handles errors.
		/// </summary>
		/// <returns>
		/// The <see cref="T:System.Diagnostics.Process"/> that was started.
		/// </returns>
		protected override Process StartProcess()
		{
			Process process = base.StartProcess();
			process.Exited += new System.EventHandler(_OnProcessExited);
			return process;
		}

		#endregion Override implementation of ExternalProgramBase

		#region Private Methods

		/// <summary>
		/// Removes generated settings file after process has run.
		/// </summary>
		private void _OnProcessExited(object sender, System.EventArgs e)
		{
			if ( File.Exists( _settingsFile ) )
			{
				Log( Level.Verbose, "Deleting settings file: " + _settingsFile );
				File.Delete( _settingsFile );
			}
		}

		/// <summary>
		/// Build the Xml .ncoversettings file to pass to the NCover.Console executable using NCover 1.3.3 syntax.
		/// </summary>
		private void _BuildTempSettingsXmlFileForNCover133()
		{
			// Build the list of assembly names that will be included in the NCover results.
			string assemblyNames = _BuildAssemblyNameList();

			using (Stream fileStream = File.Create(_settingsFile))
			{
				XmlTextWriter xmlTextWriter = new XmlTextWriter(fileStream, Encoding.UTF8);
				xmlTextWriter.Indentation = 2;
				xmlTextWriter.Formatting = Formatting.Indented;

				xmlTextWriter.WriteStartDocument();
				xmlTextWriter.WriteStartElement("ProfilerSettings");
				xmlTextWriter.WriteElementString("CommandLineExe", _commandLineExe);
				xmlTextWriter.WriteElementString("CommandLineArgs", _commandLineArgs);
				xmlTextWriter.WriteElementString("WorkingDirectory", _workingDirectory);
				xmlTextWriter.WriteElementString("Assemblies", assemblyNames);
				xmlTextWriter.WriteElementString("CoverageFile", _coverageFile);
				if (_logLevel == NCoverLogLevel.Quiet)
				{
					xmlTextWriter.WriteElementString("LogFile", string.Empty);
					xmlTextWriter.WriteElementString("VerboseLog", "false");
					xmlTextWriter.WriteElementString("NoLog", "true");
				}
				else
				{
					xmlTextWriter.WriteElementString("LogFile", _logFile);
					xmlTextWriter.WriteElementString("VerboseLog", (_logLevel == NCoverLogLevel.Verbose).ToString().ToLower());
					xmlTextWriter.WriteElementString("NoLog", "false");
				}
				xmlTextWriter.WriteElementString("NoXslCopy", (!_copyXsl).ToString().ToLower());

				xmlTextWriter.WriteEndElement(); // ProfilerSettings
				xmlTextWriter.WriteEndDocument();
				xmlTextWriter.Flush();

				fileStream.Close();
			}
		}

		/// <summary>
		/// Build the Xml .ncoversettings file to pass to the NCover.Console executable using NCover 1.5 syntax.
		/// </summary>
		private void _BuildTempSettingsXmlFileForNCover15()
		{
			// Build the list of assembly names that will be included in the NCover results.
			string assemblyNames = _BuildAssemblyNameList();

			using (Stream fileStream = File.Create(_settingsFile))
			{
				XmlTextWriter xmlTextWriter = new XmlTextWriter(fileStream, Encoding.UTF8);
				xmlTextWriter.Indentation = 2;
				xmlTextWriter.Formatting = Formatting.Indented;

				xmlTextWriter.WriteStartDocument();
				xmlTextWriter.WriteStartElement("ProfilerSettings");
				xmlTextWriter.WriteElementString("CommandLineExe", _commandLineExe);
				xmlTextWriter.WriteElementString("CommandLineArgs", _commandLineArgs);
				xmlTextWriter.WriteElementString("WorkingDirectory", _workingDirectory);
				xmlTextWriter.WriteElementString("Assemblies", assemblyNames);
				xmlTextWriter.WriteElementString("CoverageXml", _coverageFile);
				if (_logLevel == NCoverLogLevel.Quiet)
				{
					// HACK: Setting NoLog to "true" results in NCover hanging in the NCover 1.5.4 release
					// For now we will just leave at false and always write a log file until Peter fixes it.
					//xmlTextWriter.WriteElementString("LogFile", string.Empty);
					//xmlTextWriter.WriteElementString("VerboseLog", "false");
					//xmlTextWriter.WriteElementString("NoLog", "true");
					xmlTextWriter.WriteElementString("LogFile", _logFile);
					xmlTextWriter.WriteElementString("VerboseLog", "false");
					xmlTextWriter.WriteElementString("NoLog", "false");
				}
				else
				{
					xmlTextWriter.WriteElementString("LogFile", _logFile);
					xmlTextWriter.WriteElementString("VerboseLog", (_logLevel == NCoverLogLevel.Verbose).ToString().ToLower());
					xmlTextWriter.WriteElementString("NoLog", "false");
				}
				if (!StringUtils.IsNullOrEmpty(_excludeAttributes))
				{
					xmlTextWriter.WriteElementString("ExclusionAttributes", _excludeAttributes);
				}
				if (_profileIIS)
				{
					xmlTextWriter.WriteElementString("ProfileIIS", "true");
				}
				if (!StringUtils.IsNullOrEmpty(_profileService))
				{
					xmlTextWriter.WriteElementString("ProfileService", _profileService);
				}
				xmlTextWriter.WriteElementString("DumpOnErrorNormal", "false");
				xmlTextWriter.WriteElementString("DumpOnErrorFull", "false");

				xmlTextWriter.WriteEndElement(); // ProfilerSettings
				xmlTextWriter.WriteEndDocument();
				xmlTextWriter.Flush();

				fileStream.Close();
			}
		}

		/// <summary>
		/// Builds a semi-colon delimited assembly name list.
		/// </summary>
        private string _BuildAssemblyNameList()
        {
            string assemblyNames = string.Empty;
            StringCollection assemblyList = new StringCollection();
            foreach (string fileName in _assemblyFiles.FileNames)
            {
                string assemblyName = Path.GetFileNameWithoutExtension(fileName);
                if (!assemblyList.Contains(assemblyName))
                {
                    assemblyNames += ";" + assemblyName;
                }
                assemblyList.Add(assemblyName);
            }
            if (assemblyNames.Length > 0)
            {
                assemblyNames = assemblyNames.Substring(1);
            }
            return assemblyNames;
        }

		/// <summary>
		/// Reads the file contents and returns as a string.
		/// </summary>
		private string _GetFileContents(string fileName)
		{
			using (StreamReader streamReader = File.OpenText(fileName))
			{
				return streamReader.ReadToEnd();
			}
		}

		#endregion Private Methods
	}
}

