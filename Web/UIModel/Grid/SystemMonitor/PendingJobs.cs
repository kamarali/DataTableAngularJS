using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Iata.IS.Web.Util;
using Trirand.Web.Mvc;



namespace Iata.IS.Web.UIModel.Grid.SystemMonitor
{
  public class PendingJobs : GridBase
  {

    public PendingJobs(string gridId, string dataUrl)
      : base(gridId, dataUrl)
    {
    }

    protected override void InitializeColumns()
    {
      if (_grid != null)
      {
        _grid.Columns = new List<JQGridColumn>
                                    {
                                        GridColumnHelper.HiddenColumn("FileId",isPrimaryKey:true),
                                        GridColumnHelper.TextColumn("FileName", "File Name", 400,true),
                                        GridColumnHelper.DateTimeColumn("FileTime", "File Time", 200,true),
                                        GridColumnHelper.TextColumn("Status", "Status", 300,true),
                                        /* CMP #675: Progress Status Bar for Processing of Billing Data Files. Desc: New column Added. */
	                                    GridColumnHelper.ActionColumn("FileProgressStatus", 80, heading: "File Progress Status", textAlignment: TextAlign.Center)
                                    };


        var formatter = new CustomFormatter { FormatFunction = "formatFileName" };
        _grid.Columns[1].Formatter = formatter;

        _grid.SortSettings.InitialSortColumn = "FileTime";
        _grid.SortSettings.InitialSortDirection = SortDirection.Desc;
        _grid.Width = 980;

        /* CMP #675: Progress Status Bar for Processing of Billing Data Files. 
        * Desc: Formatter Added for New Column. */
        _grid.Columns.Find(column => column.DataField == "FileProgressStatus").Formatter =
                            new CustomFormatter
                            {
                                FormatFunction =
                                    string.Format(
                                        "{0}_GenerateProgressBarAction",
                                        ControlIdConstants.SMCurrentStatsGrid)
                            };
      }
    }


  }
}