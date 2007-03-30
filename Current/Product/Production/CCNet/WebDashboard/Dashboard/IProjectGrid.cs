using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IProjectGrid
	{
		ProjectGridRow[] GenerateProjectGridRows(IFarmService farmService, ProjectStatusOnServer[] statusList, string forceBuildActionName, ProjectGridSortColumn sortColumn, bool sortIsAscending);
	}
}
