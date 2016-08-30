using System.Collections.Generic;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Common
{
  public class AlertsGrid : GridBase
  {
    public AlertsGrid(string gridId, string dataUrl)
      : base(gridId, dataUrl)
    {
      
    }
    protected override void InitializeColumns()
    {
      if(_grid != null)
      {
         _grid.Columns = new List<JQGridColumn>
                                  {
                                    GridColumnHelper.HiddenColumn("MessageId",isPrimaryKey:true),
                                    GridColumnHelper.ColorCodedStatusColumn("RAGIndicator"," ",20),
                                    GridColumnHelper.TextColumn("Detail","Detail",400,true),
                                    GridColumnHelper.ColorCodedStatusColumn("Recipients"," ",40),
                                    GridColumnHelper.DateTimeColumn("RaisedDate","Date (YMQ)", 80,true),
                                    GridColumnHelper.ColorCodedStatusColumn("Clear","Clear",40)
                                  };

         _grid.SortSettings.InitialSortColumn = "RAGIndicator";
         _grid.SortSettings.InitialSortDirection = SortDirection.Asc;
          _grid.Columns.Find(c => c.DataField == "RAGIndicator").Formatter = new CustomFormatter
          {
            FormatFunction = "formatRAGIndicatorColumn",
            UnFormatFunction = "unformatRAGIndicatorColumn"
          };

          _grid.Columns.Find(c => c.DataField == "Clear").Formatter = new CustomFormatter
          {
            FormatFunction = "formatClearColumn",
            UnFormatFunction = "unformatClearColumn"
          };

          _grid.Columns.Find(c => c.DataField == "Recipients").Formatter = new CustomFormatter
          {
            FormatFunction = "formatRecipientsColumn",
            UnFormatFunction = "unformatRecipientsColumn"
          };
      }
    }
  }
}