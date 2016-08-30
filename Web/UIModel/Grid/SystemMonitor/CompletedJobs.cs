using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;



namespace Iata.IS.Web.UIModel.Grid.SystemMonitor
{
    public class CompletedJobs :  GridBase 
    {

      public CompletedJobs(string gridId, string dataUrl)
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
                                        GridColumnHelper.DateTimeColumn("FormatedStartTime", "Start Time", 250,true),
                                        GridColumnHelper.DateTimeColumn("FormatedEndtime", "End Time", 250,true),
                                        GridColumnHelper.TextColumn("Status", "Status", 300,true),
                                        GridColumnHelper.TextColumn("IsPurged","Purged ?",100,true),
                                    };

                _grid.SortSettings.InitialSortColumn = "FormatedEndtime";
                _grid.SortSettings.InitialSortDirection = SortDirection.Desc;

                var formatterFileName = new CustomFormatter { FormatFunction = "formatJobCompleteFileName" };
                _grid.Columns[1].Formatter = formatterFileName;

                var formatterIsPurged = new CustomFormatter { FormatFunction = "formatJobCompleteIspurged" };
                _grid.Columns[5].Formatter = formatterIsPurged;

                _grid.Width = 900;
                
            }
        }


    }
}