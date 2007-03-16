using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core
{
	public class LogFile
	{
		public const string FilenamePrefix = "log";
		public const string FilenameDateFormat = "yyyyMMddHHmmss";
		public static readonly Regex BuildNumber = new Regex(@"Lbuild\.(.+)\.xml");

		private DateTime _date;
		private string _label;
		private bool _succeeded;
		private IFormatProvider _formatter = CultureInfo.CurrentCulture;

		public LogFile(string filename)
		{
			ValidateFilename(filename);
			_date = ParseDate(filename);
			_label = ParseLabel(filename);
			_succeeded = IsSuccessful(filename);
		}

		public LogFile(string filename, IFormatProvider formatter) : this(filename)
		{
			_formatter = formatter;
		}

		public LogFile(IIntegrationResult result)
		{
			_date = result.StartTime;
			_label = result.Label;
			_succeeded = result.Succeeded;
		}

		public DateTime Date
		{
			get { return _date; }
		}

		public string FormattedDateString
		{
			get { return DateUtil.FormatDate(_date, _formatter); }
		}

		public string Label
		{
			get { return _label; }
		}

		public bool Succeeded
		{
			get { return _succeeded; }
		}

		public string Filename
		{
			get { return (_succeeded) ? CreateSuccessfulBuildLogFileName() : CreateFailedBuildLogFileName(); }
		}

		private string CreateFailedBuildLogFileName()
		{
			return string.Format("{0}{1}.xml", FilenamePrefix, FilenameFormattedDateString);
		}

		private string CreateSuccessfulBuildLogFileName()
		{
			return string.Format("{0}{1}Lbuild.{2}.xml", FilenamePrefix, FilenameFormattedDateString, _label);
		}

		public string FilenameFormattedDateString
		{
			get { return _date.ToString(FilenameDateFormat); }
		}

		/// <summary>
		/// Validates filename structure, throwing exceptions if badly formed.
		/// </summary>
		/// <param name="filename">The filename to validate.</param>
		/// <exception cref="ArgumentNullException">If <see cref="filename"/> is null</exception>
		/// <exception cref="ArgumentException">If <see cref="filename"/> is badly formed</exception>
		private void ValidateFilename(string filename)
		{
			if (filename == null)
				throw new ArgumentNullException("filename");

			if (!filename.StartsWith(FilenamePrefix))
				throw new ArgumentException(string.Format(
					"{0} does not start with {1}.", filename, FilenamePrefix));

			if (filename.Length < FilenamePrefix.Length + FilenameDateFormat.Length)
				throw new ArgumentException(string.Format(
					"{0} does not start with {1} followed by a date in {2} format",
					filename, FilenamePrefix, FilenameDateFormat));
		}

		private DateTime ParseDate(string filename)
		{
			string dateString = filename.Substring(FilenamePrefix.Length, FilenameDateFormat.Length);
			return DateTime.ParseExact(dateString, FilenameDateFormat, _formatter);
		}

		private string ParseLabel(string filename)
		{
			string value = BuildNumber.Match(filename).Groups[1].Value;
			if (value == null || value.Length == 0)
				return "0";

			return value;
		}

		private bool IsSuccessful(string filename)
		{
			int characterIndex = FilenamePrefix.Length + FilenameDateFormat.Length;
			return filename[characterIndex] == 'L';
		}
	}

	/// <summary>
	/// Provides utility methods for dealing with log files.
	/// </summary>
	public class LogFileUtil
	{
		public const string LogQueryString = "log";
		public const string ProjectQueryString = "project";

		/// <summary>
		/// Utility class, not intended for instantiation.
		/// </summary>
		private LogFileUtil()
		{
		}

		public static string[] GetLogFileNames(string path)
		{
			string[] filenames = GetFileNames(path, "log*.xml");
			return filenames;
		}

		[DllImport("kernel32", EntryPoint="FindFirstFileA", CharSet=CharSet.Ansi, SetLastError=true, ExactSpelling=true)]
		private static extern IntPtr FindFirstFile([MarshalAs(UnmanagedType.VBByRefStr)] ref string lpFileName, ref WIN32_FIND_DATA lpFindFileData);

		[DllImport("kernel32", EntryPoint="FindNextFileA", CharSet=CharSet.Ansi, SetLastError=true, ExactSpelling=true)]
		private static extern bool FindNextFile(IntPtr hFindFile, ref WIN32_FIND_DATA lpFindFileData);

		[DllImport("kernel32.dll")]
		private static extern bool FindClose(IntPtr hFindFile);
 
		private static string[] GetFileNames(string path, string filter /* = "*" */)
		{
			string fullPath = Path.Combine(path, filter);
			string[] textArray1 = new string[1];
			WIN32_FIND_DATA win_find_data1 = new WIN32_FIND_DATA();
			IntPtr ptr1 = FindFirstFile(ref fullPath, ref win_find_data1);
			if (ptr1.ToString() == "-1")
			{
				int num2 = Marshal.GetLastWin32Error();
				if (num2 == 2)
				{
					return new string[0];
				}
				WinIOError(num2, path);
			}
			int num1 = 0;
			bool flag1 = true;
			do
			{
				if (((FileAttributes) 0) == (win_find_data1.dwFileAttributes & FileAttributes.Directory))
				{
					string FileName = Encoding.ASCII.GetString(Encoding.Unicode.GetBytes(win_find_data1.cFileName)).Split(new char[] { '\0' })[0];
					
					textArray1[num1] = FileName;
					num1++;
					if ((num1 >= textArray1.Length) && flag1)
					{
						string[] textArray3 = new string[((num1 * 2) - 1) + 1];
						textArray1.CopyTo(textArray3, 0);
						textArray1 = textArray3;
					}
				}
			}
			while (flag1 && FindNextFile(ptr1, ref win_find_data1));
			FindClose(ptr1);
			if (num1 < textArray1.Length)
			{
				string[] textArray4 = new string[(num1 - 1) + 1];
				Array.Copy(textArray1, 0, textArray4, 0, num1);
				textArray1 = textArray4;
			}
			return textArray1;
		}

		private static void WinIOError(int errorCode, string str)
		{
			switch (errorCode)
			{
				case 2:
					if (str.Length == 0)
					{
						throw new FileNotFoundException("The file path provided does not exist");
					}
					throw new FileNotFoundException("The file Path " + str + " does not exist");

				case 3:
					if (str.Length == 0)
					{
						throw new DirectoryNotFoundException("The directory path provided does not exist");
					}
					throw new DirectoryNotFoundException("The file Path " + str + " does not exist");

				case 4:
					throw new IOException("Some unknown IO Error occured");

				case 5:
					if (str.Length == 0)
					{
						throw new UnauthorizedAccessException("Unauthorized Access to NULL Path");
					}
					throw new UnauthorizedAccessException("Unauthorized access to " + str);

				case 0x20:
					if (str.Length == 0)
					{
						throw new IOException("Sharing Violation on NULL Path");
					}
					throw new IOException("Sharing violation on Path: " + str);

				case 0x57:
					throw new IOException("Some unknown IO Error occured");

				case 0xce:
					throw new PathTooLongException("Path " + str + " is too long");

				case 80:
					if (str.Length != 0)
					{
						throw new IOException("File already exists");
					}
					break;
			}
		}

		[StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
			protected struct WIN32_FIND_DATA
		{
			public FileAttributes dwFileAttributes;
			public FILETIME ftCreationTime;
			public FILETIME ftLastAccessTime;
			public FILETIME ftLastWriteTime;
			public int nFileSizeHigh;
			public int nFileSizeLow;
			public int dwReserved0;
			public int dwReserved1;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst=260)]
			public string cFileName;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst=14)]
			public string cAlternate;
		}
 


		public static int GetLatestBuildNumber(string path)
		{
			if (Directory.Exists(path))
				return GetLatestBuildNumber(GetLogFileNames(path));
			else
				return 0;
		}

		public static int GetLatestBuildNumber(string[] filenames)
		{
			int result = 0;
			foreach (string filename in filenames)
			{
				result = Math.Max(result, GetNumericBuildNumber(new LogFile(filename).Label));
			}
			return result;
		}

		private static int GetNumericBuildNumber(string buildlabel)
		{
			return Int32.Parse(Regex.Replace(buildlabel, @"\D", ""));
		}

		public static DateTime GetLastBuildDate(string[] filenames, DateTime defaultValue)
		{
			if (filenames.Length == 0)
				return defaultValue;

			ArrayList.Adapter(filenames).Sort();
			string filename = filenames[filenames.Length - 1];
			return new LogFile(filename).Date;
		}

		public static DateTime GetLastBuildDate(string path, DateTime defaultValue)
		{
			if (Directory.Exists(path))
				return GetLastBuildDate(GetLogFileNames(path), defaultValue);
			else
				return defaultValue;
		}

		// TODO refactor other GetLatest methods to use this one
		public static string GetLatestLogFileName(string path)
		{
			if (!Directory.Exists(path))
				return null;

			string[] filenames = GetLogFileNames(path);
			return GetLatestLogFileName(filenames);
		}

		// TODO refactor other GetLatest methods to use this one
		public static string GetLatestLogFileName(string[] filenames)
		{
			if (filenames.Length == 0)
				return null;

			ArrayList.Adapter(filenames).Sort();
			return filenames[filenames.Length - 1];
		}

		public static string CreateUrl(string filename)
		{
			return string.Format("?{0}={1}", LogQueryString, filename);
		}

		public static string CreateUrl(string filename, string projectname)
		{
			return string.Format("{0}&{1}={2}", CreateUrl(filename), ProjectQueryString, projectname);
		}

		public static string CreateUrl(IIntegrationResult result)
		{
			return CreateUrl(new LogFile(result).Filename);
		}

		public static string CreateUrl(string urlRoot, IIntegrationResult result)
		{
			return String.Concat(urlRoot, CreateUrl(result));
		}
	}
}