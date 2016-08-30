using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;



namespace Iata.IS.Web.UIModel.Grid.SystemMonitor
{
    public class OutstandingItems :  GridBase 
    {

      public OutstandingItems(string gridId, string dataUrl)
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
                                        GridColumnHelper.HiddenColumn("FileName"),
                                        GridColumnHelper.TextColumn("Request", "Request", 250,true),
                                        GridColumnHelper.TextColumn("SendTo", "Sent To", 100,true),
                                        GridColumnHelper.TextColumn("FileName", "File Name", 600,true),
                                        GridColumnHelper.DateTimeColumn("SentOnDate", "Sent On Date", 300,true),
                                        GridColumnHelper.TextColumn("CurrentStatus", "Current Status", 300,true),
                                   };

                _grid.SortSettings.InitialSortColumn = "SentOnDate";
                _grid.SortSettings.InitialSortDirection = SortDirection.Desc;

                var formatter = new CustomFormatter { FormatFunction = "formatFileName" };
                _grid.Columns[4].Formatter = formatter;

                 _grid.Width = 830;
                
            }
        }


    }
}