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
using System.Threading;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace MattGriffith.UpdateVersion
{
	/// <summary>
	/// 
	/// </summary>
	public class RunUpdateVersion
	{

		public RunUpdateVersion() {}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		public void execute(Options opts)
		{
			string input = null;
			VersionUpdater updater = null;

			///////////////////////////////////////////////////////////////////
			// Get the input
			input = GetInput(opts);

			///////////////////////////////////////////////////////////////////
			// Update the version number in the input
			updater = new VersionUpdater(input, opts);

			///////////////////////////////////////////////////////////////////
			// Write the output
			WriteOutput(updater.Output, opts);
			
		}

		/// <summary>
		/// Private helper method that gets the input string from the appropriate source.
		/// </summary>
		/// <param name="options">The command line options.</param>
		/// <returns>
		/// Returns the input string.
		/// </returns>
		public static string GetInput(Options opts)
		{
			string input = null;

			if(null == opts.InputFilename)
			{
				// The input file name was not specified on the command line wo we will
				// get the input from the standard input stream.
				input = Console.In.ReadToEnd();
			}
			else
			{
				// An input file was specified on the command line. 
				input = ReadFile(opts.InputFilename);
			}

			return input;
		}

		/// <summary>
		/// Private helper that reads the input string from a file.
		/// </summary>
		/// <param name="filename">The name of the file to read.</param>
		/// <returns>The string representing the data stored in the input file.</returns>
		public static string ReadFile(string filename)
		{
			string result = null;
			
			if(!File.Exists(filename))
				throw new ArgumentException("File does not exist.", "filename");

			using(FileStream stream = File.OpenRead(filename))
			{
				StreamReader reader = new StreamReader(stream, Encoding.Default, true);
				result = reader.ReadToEnd();
			}

//			using(StreamReader reader = File.OpenText(filename))
//			{
//				result = reader.ReadToEnd();
//			}
			
			return result;
		}

		/// <summary>
		/// Writes the output string to the appropriate target.
		/// </summary>
		/// <param name="output">
		/// The output string.
		/// </param>
		/// <param name="options">
		/// The command line options.
		/// </param>
		public static void WriteOutput(string output, Options opts)
		{
			if(null == output)
				throw new ArgumentNullException("output", "Output is null.");

			if(null == opts.OutputFilename)
			{
				// The output file name was not specified on the command line wo we will
				// write the output to the standard output stream.
				Console.Out.Write(output);
			}
			else
			{
				// An output file was specified on the command line. So we will write the
				// output to the specified file.
//				using(StreamWriter writer = 
//						  File.CreateText(options.OutputFilename))
//				{
//					writer.Write(output);
//				}
				using(StreamWriter writer = 
						  new StreamWriter(opts.OutputFilename, false, Encoding.Default))
				{
					
					writer.Write(output);
				}
			}
		}
	}
}
