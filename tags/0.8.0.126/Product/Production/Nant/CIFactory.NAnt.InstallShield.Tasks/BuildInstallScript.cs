
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;
using Microsoft.Win32;
using NAnt;
using NAnt.Core;
using NAnt.Core.Attributes;
using NAnt.Core.Tasks;
using NAnt.Core.Types;
using NAnt.Contrib.Types;

namespace CIFactory.NAnt.InstallShield.Tasks
{
	[TaskName("buildinstallscript")]
	public class BuildInstallScript : BuildInstallShieldBase
	{
		// The base class calls this to build the command-line string.
		public override string ProgramArguments 
		{
			get
			{
				return base.GetBaseArguments();
			}
		}
	}
}
