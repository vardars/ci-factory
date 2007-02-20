/*

  Copyright (c) 2002 Matt Griffith

  Permission is hereby granted, free of charge, to any person obtaining 
  a copy of this software and associated documentation files (the "Software"), 
  to deal in the Software without restriction, including without limitation 
  the rights to use, copy, modify, merge, publish, distribute, sublicense, 
  and/or sell copies of the Software, and to permit persons to whom the 
  Software is furnished to do so, subject to the following conditions:

  The above copyright notice and this permission notice shall be included in 
  all copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
  THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
  
*/
using System;
using System.Text.RegularExpressions;

namespace MattGriffith.UpdateVersion
{
	/// <summary>
	/// Searches for an AssemblyVersion attribute in an input string and updates the version
	/// number using the provided options.
	/// </summary>
	public class VersionUpdater
	{
		/// <summary>
		/// A regex that matches strings like 'AssemblyVersion("1.0.0.1")'
		/// </summary>
		private static readonly Regex AssemblyVersionRegex = 
			new Regex("AssemblyVersion(?:Attribute)?\\(\\s*?\"(?<version>(?<major>[0-9]+)\\.(?<minor>[0-9]+)\\.(?<build>[0-9]+)\\.(?<revision>[0-9]+))\"\\s*?\\)");

		private static readonly Regex FileVersionRegex = 
			new Regex("AssemblyFileVersion(?:Attribute)?\\(\\s*?\"(?<version>(?<major>[0-9]+)\\.(?<minor>[0-9]+)\\.(?<build>[0-9]+)\\.(?<revision>[0-9]+))\"\\s*?\\)");


		/// <summary>
		/// The input containing the version number to update.
		/// </summary>
		private string _Input;

		/// <summary>
		/// Stores the output with the updated version number.
		/// </summary>
		private string _Output;

		/// <summary>
		/// Stores the active regex in use based on the type of version we're looking for.
		/// </summary>
		private Regex _ActiveRegex;

		/// <summary>
		/// Stores the string format used to generate the replacement string.
		/// </summary>
		private string _ReplaceFormat;

		/// <summary>
		/// Initializes a new VersionUpdater instance.
		/// </summary>
		/// <param name="input">
		/// The input string containing the AssemblyVersion attribute
		/// to update.
		/// </param>
		/// <param name="options">The options to use for updating the version number.</param>
		public VersionUpdater(string input, Options options)
		{
			// Save the input
			this._Input = input;
			this._Output = input;
			
			this._ActiveRegex = new Regex(options.AttributeName + "(?:Attribute)?\\(\\s*?\"(?<version>(?<major>[0-9]+)\\.(?<minor>[0-9]+)\\.(?<build>[0-9]+)\\.(?<revision>[0-9]+))\"\\s*?\\)");
			this._ReplaceFormat = options.AttributeName + "(\"{0}\")";

			try
			{
				Match match = this._ActiveRegex.Match(input);

				if(null != match)
				{
					string inputVersion = match.Groups["version"].Value;

					this.doUpdate(inputVersion, options);
				}

			}
			catch(Exception)
			{
				// There was a problem finding or updating the version number.
				this._Output = this._Input;
				throw;
			}
		}


		private void doUpdate(string inputVersion, Options options)
		{
			string replacement = "";
			if (options.ReplacementVersion != null)
			{
				replacement = string.Format(this._ReplaceFormat, 
					options.ReplacementVersion);
			}
			else if (null == inputVersion || string.Empty == inputVersion) 
			{
				VersionCalculator calculator = new VersionCalculator("1.0.0.0");
	        
				calculator.StartDate = options.StartDate;
				calculator.BuildNumberType = options.BuildNumberType;
				calculator.RevisionNumberType = options.RevisionNumberType;

				string replacment = options.AttributeName + "(\"" + calculator.NewVersion.ToString() + "\")";

				this._Output = this._Input.Replace(options.AttributeName + "(\"1.0.*\")", replacment);
				return;
			}
			else 
			{
				VersionCalculator calculator = new VersionCalculator(inputVersion);
				        
				calculator.StartDate = options.StartDate;
				calculator.BuildNumberType = options.BuildNumberType;
				calculator.RevisionNumberType = options.RevisionNumberType;

				replacement = string.Format(this._ReplaceFormat, 
					calculator.NewVersion.ToString());
			}
			
			string outputVersion = this._ActiveRegex.Replace(this._Input, replacement, 1);
			
			this._Output = outputVersion;
		}

		/// <summary>
		/// Gets the input string that this VersionUpdater was based on.
		/// </summary>
		public string Input
		{
			get { return this._Input; }
		}

		/// <summary>
		/// Gets the output string that contains the updated version number.
		/// </summary>
		public string Output
		{
			get { return this._Output; }
		}
		
	}
}
