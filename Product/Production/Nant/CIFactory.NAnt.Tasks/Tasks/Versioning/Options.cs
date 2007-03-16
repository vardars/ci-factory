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

*/
using System;

namespace MattGriffith.UpdateVersion
{

	/// <summary>
	/// Represents the command line options for this application.
	/// </summary>
	public class Options
	{

		public string PinOption;

		public Options()
		{
		}

		/// <summary>
		/// Gets the StartDate specified on the command line.
		/// </summary>
		/// <value>Returns DateTime.MinValue if this option was not provided.</value>
		public DateTime StartDate;

		/// <summary>
		/// Gets the BuildNumberType specified on the command line.
		/// </summary>
		/// <value>
		/// Returns the default BuildNumberType if the build option 
		/// was not specified on the command line.
		/// </value>
		public BuildNumberType BuildNumberType;
		/// <summary>
		/// Private helper that converts a string to the equivilent BuildNumberType.
		/// </summary>
		/// <param name="buildNumberDescription">
		/// The string representing a BuildNumberType.
		/// </param>
		/// <returns>
		/// Returns the default BuildNumberType if the string is not a recognized BuildNumberType.
		/// </returns>
		private static BuildNumberType ToBuildNumberType(string buildNumberDescription)
		{
			BuildNumberType result = BuildNumberType.Fixed;
			string lower = buildNumberDescription.ToLower();

			if(lower.StartsWith("i"))
			{
				result = BuildNumberType.Increment;
			}
			else if(lower.StartsWith("m"))
			{
				result = BuildNumberType.MonthDay;
			}
			else if(lower.StartsWith("b"))
			{
				result = BuildNumberType.BuildDay;
			}
			else
			{
				result = BuildNumberType.Fixed;
			}

			return result;
		}

		/// <summary>
 		/// Gets the PinVersion specified on the command line.
 		/// </summary>
 		/// <value>
 		/// Returns null if the pin option 
 		/// was not specified on the command line.
 		/// </value>
		public Version PinVersion
		{
			get
			{
				Version pinVersion = null;
 
				if(this.PinOption == "")
				{
					pinVersion = null;
				}
				else
				{
					pinVersion = new Version(this.PinOption);
				}
 
				return pinVersion;
			}
		}
		/// <summary>
		/// Indicates whether the version number is pinned.
		/// </summary>
		/// <value>
		/// Returns true if the pin option was specified on the command line. Otherwise
		/// returns false.
		/// </value>
		public bool VersionIsPinned;
		/// <summary>
		/// Gets the RevisionNumberType specified on the command line.
		/// </summary>
		/// <value>
		/// Returns the default RevisionNumberType if the revision option 
		/// was not specified on the command line.
		/// </value>
		public RevisionNumberType RevisionNumberType;
		/// <summary>
		/// Private helper that converts a string to the equivilent RevisionNumberType.
		/// </summary>
		/// <param name="revisionNumberDescription">
		/// The string representing a RevisionNumberType.
		/// </param>
		/// <returns>
		/// Returns the default RevisionNumberType if the string is not a recognized RevisionNumberType.
		/// </returns>
		private static RevisionNumberType ToRevisionNumberType(string revisionNumberDescription)
		{
			RevisionNumberType result = RevisionNumberType.Automatic;
			string lower = revisionNumberDescription.ToLower();

			if(lower.StartsWith("i"))
			{
				result = RevisionNumberType.Increment;
			}
			else if(lower.StartsWith("f"))
			{
				result = RevisionNumberType.Fixed;
			}
			else
			{
				result = RevisionNumberType.Automatic;
			}

			return result;
		}

		/// <summary>
		/// Gets the inputfile specified on the command line.
		/// </summary>
		/// <value>
		/// Returns null if the inputfile was not specified on the command line.
		/// </value>
		public string InputFilename;
		/// <summary>
		/// Gets the outputfile specified on the command line.
		/// </summary>
		/// <value>
		/// Returns null if the outputfile was not specified on the command line.
		/// </value>
		public string OutputFilename;
		/// <summary>
		/// Gets the VersionType specified on the command line.
		/// </summary>
		/// <value>
		/// Returns the default VersionType if the version option 
		/// was not specified on the command line.
		/// </value>
		public string AttributeName;
		
		public string ReplacementVersion;

		/// <summary>
		/// A private helper method that verifies the pin option specified on the commandlind.
		/// </summary>
		/// <exception cref="System.ApplicationException">
		/// The pin option is not a valid version number. See 
		/// <seealso cref="System.Version">Version</see> for more information.
		/// </exception>
		private void ValidatePinOption()
		{
			if(this.PinOption != "")
			{
				try
				{
					Version version = new Version(this.PinOption);
				}
				catch(Exception e)
				{
					throw new ApplicationException("The version number specified for the pin " +
						"option is not valid. Please provide a version number in the correct format.",
						e);
				}
			}
		}
	}
}
