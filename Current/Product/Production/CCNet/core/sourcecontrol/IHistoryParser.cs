using System;
using System.IO;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol
{	
	public interface IHistoryParser
	{
		Modification[] Parse(TextReader history, DateTime from, DateTime to);
	}
}
