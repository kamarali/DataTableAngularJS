using System.Collections.Generic;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Reports
{
  /// <summary>
  /// Offline report Search Grid.
  /// </summary>
	public class OfflineReportSearchGrid : GridBase
	{
		public OfflineReportSearchGrid(string gridId,string dataUrl)
      : base(gridId, dataUrl)
    {
    }

    /// <summary>
    /// Initializes columns for grid
    /// </summary>
    protected override void InitializeColumns()
    {
      if (_grid != null)
      {
      	_grid.Columns = new List<JQGridColumn>
      	                	{
      	                		GridColumnHelper.HiddenColumn("Id",isPrimaryKey:true),
      	                		GridColumnHelper.TextColumn("ReportName", "Report", 250, true),
      	                		GridColumnHelper.TextColumn("RequestDatetime", "UTC Date/Time of Report Request", 150, true),
      	                		GridColumnHelper.TextColumn("SearchCriteria", "Search Criteria", 650),
      	                		GridColumnHelper.TextColumn("Status", "Status", 50),
      	                		GridColumnHelper.ActionColumn("DownloadLinkId", 46),
      	                		GridColumnHelper.HiddenColumn("LastUpdatedOn")
      	                	};

      	_grid.SortSettings.InitialSortColumn = "LastUpdatedOn";
      	_grid.SortSettings.InitialSortDirection = SortDirection.Desc;
      	var formatter = new CustomFormatter {FormatFunction = string.Format("{0}_downloadZip", _grid.ID)};
      	_grid.Columns[5].Formatter = formatter;
      }
    }
	}
}