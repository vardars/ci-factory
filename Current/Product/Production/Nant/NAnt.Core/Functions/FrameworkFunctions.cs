// NAnt - A .NET build tool
// Copyright (C) 2001-2003 Gerry Shaw
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
// Ian Maclean (imaclean@gmail.com)
// Jaroslaw Kowalski (jkowalski@users.sourceforge.net)
// Gert Driesen (driesen@users.sourceforge.net)

using System;
using System.Globalization;
using System.IO;
using NAnt.Core.Attributes;
using NAnt.Core.Util;

namespace NAnt.Core.Functions {
	[FunctionSet("framework", "NAnt")]
	public class FrameworkFunctions : FunctionSetBase {
		#region Public Instance Constructors

		public FrameworkFunctions(Project project, Location location, PropertyDictionary properties)
			: base(project, location, properties)
		{
		}

		#endregion Public Instance Constructors

		#region Public Instance Methods

		/// <summary>
		/// Checks whether the specified framework exists.
		/// </summary>
		/// <param name="name">The framework to test.</param>
		/// <returns>
		/// <see langword="true" /> if the specified framework exists; otherwise,
		/// <see langword="false" />.
		/// </returns>
		[Function("exists")]
		public bool Exists(string name) {
			return this.Project.Frameworks.ContainsKey(name);
		}

		/// <summary>
		/// Checks whether the SDK for the specified framework is installed.
		/// </summary>
		/// <param name="name">The framework to test.</param>
		/// <returns>
		/// <see langword="true" /> if the SDK for specified framework is installed; 
		/// otherwise, <see langword="false" />.
		/// </returns>
		/// <seealso cref="FrameworkFunctions.GetRuntimeFramework()" />
		/// <seealso cref="FrameworkFunctions.GetTargetFramework()" />
		[Function("sdk-exists")]
		public bool SdkExists(string name) {
			if (this.Project.Frameworks.ContainsKey(name)) {
				return (this.Project.Frameworks[name].SdkDirectory != null);
			} else {
				return false;
			}
		}

		/// <summary>
		/// Gets the identifier of the current target framework.
		/// </summary>
		/// <returns>
		/// The identifier of the current target framework.
		/// </returns>
		[Function("get-target-framework")]
		public string GetTargetFramework() {
			return this.Project.TargetFramework.Name;
		}

		/// <summary>
		/// Gets the identifier of the runtime framework.
		/// </summary>
		/// <returns>
		/// The identifier of the runtime framework.
		/// </returns>
		[Function("get-runtime-framework")]
		public string GetRuntimeFramework() {
			return this.Project.RuntimeFramework.Name;
		}

		/// <summary>
		/// Gets the family of the specified framework.
		/// </summary>
		/// <param name="framework">The framework of which the family should be returned.</param>
		/// <returns>
		/// The family of the specified framework.
		/// </returns>
		/// <exception cref="ArgumentException"><paramref name="framework" /> is not a valid framework identifier.</exception>
		/// <seealso cref="FrameworkFunctions.GetRuntimeFramework()" />
		/// <seealso cref="FrameworkFunctions.GetTargetFramework()" />
		[Function("get-family")]
		public string GetFamily(string framework) {
			// ensure the framework is valid
			this.CheckFramework(framework);
			// return the family of the specified framework
			return this.Project.Frameworks[framework].Family;
		}

		/// <summary>
		/// Gets the version of the specified framework.
		/// </summary>
		/// <param name="framework">The framework of which the version should be returned.</param>
		/// <returns>
		/// The version of the specified framework.
		/// </returns>
		/// <exception cref="ArgumentException"><paramref name="framework" /> is not a valid framework identifier.</exception>
		/// <seealso cref="FrameworkFunctions.GetRuntimeFramework()" />
		/// <seealso cref="FrameworkFunctions.GetTargetFramework()" />
		[Function("get-version")]
		public Version GetVersion(string framework) {
			// ensure the framework is valid
			this.CheckFramework(framework);
			// return the family of the specified framework
			return this.Project.Frameworks[framework].Version;
		}

		/// <summary>
		/// Gets the description of the specified framework.
		/// </summary>
		/// <param name="framework">The framework of which the description should be returned.</param>
		/// <returns>
		/// The description of the specified framework.
		/// </returns>
		/// <exception cref="ArgumentException"><paramref name="framework" /> is not a valid framework identifier.</exception>
		/// <seealso cref="FrameworkFunctions.GetRuntimeFramework()" />
		/// <seealso cref="FrameworkFunctions.GetTargetFramework()" />
		[Function("get-description")]
		public string GetDescription(string framework) {
			// ensure the framework is valid
			this.CheckFramework(framework);
			// return the description of the specified framework
			return this.Project.Frameworks[framework].Description;
		}

		/// <summary>
		/// Gets the Common Language Runtime version of the specified framework.
		/// </summary>
		/// <param name="framework">The framework of which the Common Language Runtime version should be returned.</param>
		/// <returns>
		/// The Common Language Runtime version of the specified framework.
		/// </returns>
		/// <exception cref="ArgumentException"><paramref name="framework" /> is not a valid framework identifier.</exception>
		/// <seealso cref="FrameworkFunctions.GetRuntimeFramework()" />
		/// <seealso cref="FrameworkFunctions.GetTargetFramework()" />
		[Function("get-clr-version")]
		public Version GetClrVersion(string framework) {
			// ensure the framework is valid
			this.CheckFramework(framework);
			// return the family of the specified framework
			return this.Project.Frameworks[framework].ClrVersion;
		}

		/// <summary>
		/// Gets the framework directory of the specified framework.
		/// </summary>
		/// <param name="framework">The framework of which the framework directory should be returned.</param>
		/// <returns>
		/// The framework directory of the specified framework.
		/// </returns>
		/// <exception cref="ArgumentException"><paramref name="framework" /> is not a valid framework identifier.</exception>
		/// <seealso cref="FrameworkFunctions.GetRuntimeFramework()" />
		/// <seealso cref="FrameworkFunctions.GetTargetFramework()" />
		[Function("get-framework-directory")]
		public string GetFrameworkDirectory(string framework) {
			// ensure the framework is valid
			this.CheckFramework(framework);
			// return full path to the framework directory of the specified framework
			return this.Project.Frameworks[framework].FrameworkDirectory.FullName;
		}

		/// <summary>
		/// Gets the assembly directory of the specified framework.
		/// </summary>
		/// <param name="framework">The framework of which the assembly directory should be returned.</param>
		/// <returns>
		/// The assembly directory of the specified framework.
		/// </returns>
		/// <exception cref="ArgumentException"><paramref name="framework" /> is not a valid framework identifier.</exception>
		/// <seealso cref="FrameworkFunctions.GetRuntimeFramework()" />
		/// <seealso cref="FrameworkFunctions.GetTargetFramework()" />
		[Function("get-assembly-directory")]
		public string GetAssemblyDirectory(string framework) {
			// ensure the framework is valid
			this.CheckFramework(framework);
			// return full path to the assembly directory of the specified framework
			return this.Project.Frameworks[framework].FrameworkAssemblyDirectory.FullName;
		}

		/// <summary>
		/// Gets the SDK directory of the specified framework.
		/// </summary>
		/// <param name="framework">The framework of which the SDK directory should be returned.</param>
		/// <returns>
		/// The SDK directory of the specified framework, or an empty 
		/// <see cref="string" /> if the SDK of the specified framework is not 
		/// installed.
		/// </returns>
		/// <exception cref="ArgumentException"><paramref name="framework" /> is not a valid framework identifier.</exception>
		/// <seealso cref="FrameworkFunctions.GetRuntimeFramework()" />
		/// <seealso cref="FrameworkFunctions.GetTargetFramework()" />
		[Function("get-sdk-directory")]
		public string GetSdkDirectory(string framework) {
			// ensure the framework is valid
			this.CheckFramework(framework);
			// get the SDK directory of the specified framework
			DirectoryInfo sdkDirectory = this.Project.Frameworks[framework].SdkDirectory;
			// return directory or empty string if SDK is not installed
			return (sdkDirectory != null) ? sdkDirectory.FullName : string.Empty;
		}

		/// <summary>
		/// Gets the runtime engine of the specified framework.
		/// </summary>
		/// <param name="framework">The framework of which the runtime engine should be returned.</param>
		/// <returns>
		/// The full path to the runtime engine of the specified framework, or
		/// an empty <see cref="string" /> if no runtime engine is defined
		/// for the specified framework.
		/// </returns>
		/// <exception cref="ArgumentException"><paramref name="framework" /> is not a valid framework identifier.</exception>
		/// <seealso cref="FrameworkFunctions.GetRuntimeFramework()" />
		/// <seealso cref="FrameworkFunctions.GetTargetFramework()" />
		[Function("get-runtime-engine")]
		public string GetRuntimeEngine(string framework) {
			// ensure the framework is valid
			this.CheckFramework(framework);
			// getthe runtime engine of the specified framework
			FileInfo runtimeEngine = this.Project.Frameworks[framework].RuntimeEngine;
			// return runtime engine or empty string if not defined
			return (runtimeEngine != null) ? runtimeEngine.FullName : string.Empty;
		}

		#endregion Public Instance Methods

		#region Private Instance Methods

		/// <summary>
		/// Checks whether the specified framework is valid.
		/// </summary>
		/// <param name="framework">The framework to check.</param>
		/// <exception cref="ArgumentException"><paramref name="framework" /> is not a valid framework identifier.</exception>
		private void CheckFramework(string framework) {
			if (!this.Project.Frameworks.ContainsKey(framework)) {
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture,
				                                          ResourceUtils.GetString("NA1096"), framework));
			}
		}

		#endregion Private Instance Methods
	}
}