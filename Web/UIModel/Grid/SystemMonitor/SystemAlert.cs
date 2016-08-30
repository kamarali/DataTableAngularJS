using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;



namespace Iata.IS.Web.UIModel.Grid.SystemMonitor
{
    public class SystemAlert :  GridBase 
    {

      public SystemAlert(string gridId, string dataUrl)
            : base(gridId, dataUrl)
        { 
        }

        protected override void InitializeColumns()
        {
            if (_grid != null)
            {
                _grid.Columns = new List<JQGridColumn>
                                    {
                                    GridColumnHelper.HiddenColumn("MessageId",isPrimaryKey:true),
                                    //GridColumnHelper.ColorCodedStatusColumn("RAGIndicator"," ",20),
                                    GridColumnHelper.TextColumn("Detail","Detail",200,true),
                                    GridColumnHelper.DateTimeColumn("RaisedDate","Date (YMQ)",120,true),
                                    GridColumnHelper.ColorCodedStatusColumn("Clear","Clear",40)
                                   };

                _grid.SortSettings.InitialSortColumn = "RaisedDate";
                _grid.SortSettings.InitialSortDirection = SortDirection.Desc;
                _grid.Columns.Find(c => c.DataField == "Clear").Formatter = new CustomFormatter
                {
                    FormatFunction = "formatClearColumn",
                    UnFormatFunction = "unformatClearColumn"
                };
                _grid.Width = 400;
                
            }
        }


    }
}