using System.Collections.Generic;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Common
{
  public class MessagesGrid : GridBase
  {
    public MessagesGrid(string gridId, string dataUrl)
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
                                      GridColumnHelper.TextColumn("Detail","Detail",400,true),
                                      GridColumnHelper.DateTimeColumn("FromDate","Date (YMQ)", 100,true),
                                      GridColumnHelper.ColorCodedStatusColumn("Recipients"," ",30),
                                      GridColumnHelper.ColorCodedStatusColumn("Clear","Clear",30)
                                    };
        _grid.SortSettings.InitialSortColumn = "FromDate";
        _grid.SortSettings.InitialSortDirection = SortDirection.Desc;

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