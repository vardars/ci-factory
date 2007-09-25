
namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IAbsoluteLink
	{
		string Text { get; }
		string Url { get; }
        string Img { get; }
		string LinkClass { set; }
	}
}
