// NAnt - A .NET build tool
// Copyright (C) 2003 Gerry Shaw
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// Gerry Shaw (gerry_shaw@yahoo.com)

using System;
using System.Globalization;
using System.IO;
using System.Xml;

using NAnt.Core.Attributes;
using NAnt.Core.Types;
using NAnt.Core.Util;

namespace NAnt.Core.Tasks {
    /// <summary>
    /// Executes a system command.
    /// </summary>
    /// <example>
    ///   <para>Ping "nant.sourceforge.net".</para>
    ///   <code>
    ///     <![CDATA[
    /// <exec program="ping">
    ///     <arg value="nant.sourceforge.net" />
    /// </exec>
    ///     ]]>
    ///   </code>
    /// </example>
    /// <example>
    ///   <para>
    ///   Execute a java application using <c>IKVM.NET</c> that requires the 
    ///   Apache FOP jars, and a set of custom jars.
    ///   </para>
    ///   <code>
    ///     <![CDATA[
    ///         <path id="fop-classpath">
    ///             <pathelement file="${fop.dist.dir}/build/fop.jar" />
    ///             <pathelement file="${fop.dist.dir}/lib/xercesImpl-2.2.1.jar" />
    ///             <pathelement file="${fop.dist.dir}/lib/avalon-framework-cvs-20020806.jar" />
    ///             <pathelement file="${fop.dist.dir}/lib/batik.jar" />
    ///         </path>
    ///         <exec program="ikvm.exe" useruntimeengine="true">
    ///             <arg value="-cp" />
    ///             <arg>
    ///                 <path>
    ///                     <pathelement dir="conf" />
    ///                     <path refid="fop-classpath" />
    ///                     <pathelement file="lib/mylib.jar" />
    ///                     <pathelement file="lib/otherlib.zip" />
    ///                 </path>
    ///             </arg>
    ///             <arg value="org.me.MyProg" />
    ///         </exec>
    ///     ]]>
    ///   </code>
    ///   <para>
    ///   Assuming the base directory of the build file is "c:\ikvm-test" and
    ///   the value of the "fop.dist.dir" property is "c:\fop", then the value
    ///   of the <c>-cp</c> argument that is passed to<c>ikvm.exe</c> is
    ///   "c:\ikvm-test\conf;c:\fop\build\fop.jar;conf;c:\fop\lib\xercesImpl-2.2.1.jar;c:\fop\lib\avalon-framework-cvs-20020806.jar;c:\fop\lib\batik.jar;c:\ikvm-test\lib\mylib.jar;c:\ikvm-test\lib\otherlib.zip"
    ///   on a DOS-based system.
    ///   </para>
    /// </example>
    [TaskName("exec")]
    public class ExecTask : ExternalProgramBase {
        #region Private Instance Fields

        private string _program;
        private string _commandline;
        private string _baseDirectory;
        private string _workingDirectory;
        private FileInfo _output;
        private bool _outputAppend;
        private EnvironmentSet _environmentSet = new EnvironmentSet();
        private bool _useRuntimeEngine;
        private string _resultProperty;

        #endregion Private Instance Fields

        #region Public Instance Properties

        /// <summary>
        /// The program to execute without command arguments.
        /// </summary>
        /// <remarks>
        /// The path will not be evaluated to a full path using the project
        /// base directory.
        /// </remarks>
        [TaskAttribute("program", Required=true)]
        [StringValidator(AllowEmpty=false)]
        public string FileName {
            get { return _program; }
            set { _program = StringUtils.ConvertEmptyToNull(value); }
        }

        /// <summary>
        /// The command-line arguments for the program.
        /// </summary>
        [TaskAttribute("commandline")]
        public string CommandLineArguments {
            get { return _commandline; }
            set { _commandline = StringUtils.ConvertEmptyToNull(value); }
        }

        /// <summary>
        /// Environment variables to pass to the program.
        /// </summary>
        [BuildElement("environment")]
        public EnvironmentSet EnvironmentSet {
            get { return _environmentSet; }
			set
			{
				_environmentSet = value;
			}
        }

        /// <summary>
        /// The directory in which the command will be executed.
        /// </summary>
        /// <value>
        /// The directory in which the command will be executed. The default 
        /// is the project's base directory.
        /// </value>
        /// <remarks>
        /// <para>
        /// The working directory will be evaluated relative to the project's
        /// base directory if it is relative.
        /// </para>
        /// </remarks>
        [TaskAttribute("workingdir")]
        public string WorkingDirectory {
            get { 
                if (_workingDirectory == null) {
                    return base.BaseDirectory;
                } 
                return _workingDirectory;
            }
            set { _workingDirectory = value; }
        }

        /// <summary>
        /// <para>
        /// The name of a property in which the exit code of the program should 
        /// be stored. Only of interest if <see cref="Task.FailOnError" /> is 
        /// <see langword="false" />.
        /// </para>
        /// <para>
        /// If the exit code of the program is "-1000" then the program could 
        /// not be started, or did not exit (in time).
        /// </para>
        /// </summary>
        [TaskAttribute("resultproperty")]
        [StringValidator(AllowEmpty=false)]
        public string ResultProperty {
            get { return _resultProperty; }
            set { _resultProperty = value; }
        }

        #endregion Public Instance Properties

        #region Override implementation of ExternalProgramBase

        /// <summary>
        /// Specifies whether the external program should be executed using a 
        /// runtime engine, if configured. The default is <see langword="false" />.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if the external program should be executed 
        /// using a runtime engine; otherwise, <see langword="false" />.
        /// </value>
        [TaskAttribute("useruntimeengine")]
        [FrameworkConfigurable("useruntimeengine")]
        public override bool UseRuntimeEngine {
            get { return _useRuntimeEngine; }
            set { _useRuntimeEngine = value; }
        }

        /// <summary>
        /// Gets the filename of the external program to start.
        /// </summary>
        /// <value>
        /// The filename of the external program.
        /// </value>
        public override string ProgramFileName {
            get {
                if (Path.IsPathRooted(FileName)) {
                    return FileName;
                } else if (_baseDirectory == null) {
                    // resolve program to full path relative to project directory
                    string fullPath = Project.GetFullPath(FileName);
                    // check if the program exists in that location
                    if (File.Exists(fullPath)) {
                        // return full path to program (which we know exists)
                        return fullPath;
                    }
                    return FileName;
                } else {
                    return Path.GetFullPath(Path.Combine(BaseDirectory, 
                        FileName));
                }
            }
        }

        /// <summary>
        /// Performs additional checks after the task has been initialized.
        /// </summary>
        /// <param name="taskNode">The <see cref="XmlNode" /> used to initialize the task.</param>
        /// <exception cref="BuildException"><see cref="FileName" /> does not hold a valid file name.</exception>
        protected override void InitializeTask(XmlNode taskNode) {
            try {
                // just check if program file to execute is a valid file name
                if (Path.IsPathRooted(FileName)) {
                    // do nothing
                }
            } catch (Exception ex) {
                throw new BuildException(string.Format(CultureInfo.InvariantCulture,
                    ResourceUtils.GetString("NA1117"), 
                    FileName, Name), Location, ex);
            }

            base.InitializeTask(taskNode);
        }

        /// <summary>
        /// Gets the command-line arguments for the external program.
        /// </summary>
        /// <value>
        /// The command-line arguments for the external program.
        /// </value>
        public override string ProgramArguments {
            get { return _commandline; }
        }
        
        /// <summary>
        /// The directory the program is in.
        /// </summary>
        /// <remarks>
        /// <value>
        /// The directory the program is in. The default is the project's base 
        /// directory.
        /// </value>
        /// <para>
        /// The basedir will be evaluated relative to the project's base 
        /// directory if it is relative.
        /// </para>
        /// </remarks>
        [TaskAttribute("basedir")]
        public override string BaseDirectory {
            get {
                if (_baseDirectory == null) {
                    return base.BaseDirectory;
                }
                return _baseDirectory;
            }
            set { _baseDirectory = value; }
        }

        /// <summary>
        /// The file to which the standard output will be redirected.
        /// </summary>
        /// <remarks>
        /// By default, the standard output is redirected to the console.
        /// </remarks>
        [TaskAttribute("output", Required=false)]
        public override FileInfo Output {
            get { return _output; }
            set { _output = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether output should be appended 
        /// to the output file. The default is <see langword="false" />.
        /// </summary>
        /// <value>
        /// <see langword="true" /> if output should be appended to the <see cref="Output" />; 
        /// otherwise, <see langword="false" />.
        /// </value>
        [TaskAttribute("append", Required=false)]
        public override bool OutputAppend {
            get { return _outputAppend; }
            set { _outputAppend = value; }
        }

        /// <summary>
        /// Executes the external program.
        /// </summary>
        protected override void ExecuteTask() {
            base.ExecuteTask();
            if (ResultProperty != null) {
                Properties[ResultProperty] = base.ExitCode.ToString(
                    CultureInfo.InvariantCulture);
            }
        }

        protected override void PrepareProcess(System.Diagnostics.Process process) {
            base.PrepareProcess(process);

            // set working directory specified by user
            process.StartInfo.WorkingDirectory = WorkingDirectory;

            // set environment variables
            foreach (Option option in EnvironmentSet.Options) {
                if (option.IfDefined && !option.UnlessDefined) {
                    if (option.Value == null) {
                        process.StartInfo.EnvironmentVariables[option.OptionName] = "";
                    } else {
                        process.StartInfo.EnvironmentVariables[option.OptionName] = option.Value;
                    }
                }
            }
            foreach (EnvironmentVariable variable in EnvironmentSet.EnvironmentVariables) {
                if (variable.IfDefined && !variable.UnlessDefined) {
                    if (variable.Value == null) {
                        process.StartInfo.EnvironmentVariables[variable.VariableName] = "";
                    } else {
                        process.StartInfo.EnvironmentVariables[variable.VariableName] = variable.Value;
                    }
                }
            }
        }

        #endregion Override implementation of ExternalProgramBase
    }
}
