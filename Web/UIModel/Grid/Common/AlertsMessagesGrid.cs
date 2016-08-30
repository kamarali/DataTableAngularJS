using System.Collections.Generic;
using Iata.IS.Model.Enums;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Common
{
  public class AlertsMessagesGrid : GridBase
  {
    private readonly int _gridType;
    
    public AlertsMessagesGrid(string gridId, string dataUrl, int gridType):base(gridId, dataUrl)
    {
      _gridType = gridType;
      InitializeColumns();
    }

    protected override sealed void InitializeColumns()
    {
      if (_grid != null)
      {

        switch (_gridType)
        {
          case (int)MessageType.Alert: _grid.Columns = new List<JQGridColumn>
                                  {
                                    GridColumnHelper.HiddenColumn("MessageId",isPrimaryKey:true),
                                    GridColumnHelper.ColorCodedStatusColumn("RAGIndicator"," ",20),
                                    GridColumnHelper.TextColumn("Detail","Detail",320,true),
                                    GridColumnHelper.DateTimeColumn("RaisedDate","Date (YMQ)",100,true),
                                    GridColumnHelper.ColorCodedStatusColumn("Clear","Clear",30)
                                  };
                                  _grid.SortSettings.InitialSortColumn = "RAGIndicator";
                                  _grid.SortSettings.InitialSortDirection = SortDirection.Asc;
                                  _grid.Width = 460;
                                  break;
          case (int)MessageType.Message: _grid.Columns = new List<JQGridColumn>
                                    {
                                      GridColumnHelper.HiddenColumn("MessageId",isPrimaryKey:true),
                                      GridColumnHelper.TextColumn("Detail","Detail",320,true),
                                      GridColumnHelper.DateTimeColumn("FromDate","Date (YMQ)", 100,true),
                                      GridColumnHelper.ColorCodedStatusColumn("Clear","Clear",30)
                                    };
                                  _grid.SortSettings.InitialSortColumn = "FromDate";
                                  _grid.SortSettings.InitialSortDirection = SortDirection.Desc;
                                  _grid.Width = 440;
                                    break;
          case (int)MessageType.Announcement: _grid.Columns = new List<JQGridColumn>
                                        {
                                           GridColumnHelper.DateColumn("FromDate","Date (YMQ)", 80),
                                           GridColumnHelper.TextColumn("Detail","Detail",400)
                                        };
                                        _grid.Width = 500;
                                        break;
        }
        
      }

      if (_gridType == (int)MessageType.Alert)
        _grid.Columns.Find(c => c.DataField == "RAGIndicator").Formatter = new CustomFormatter
        {
          FormatFunction = "formatRAGIndicatorColumn",
          UnFormatFunction = "unformatRAGIndicatorColumn"
        };

      if (_gridType == (int)MessageType.Message || _gridType == (int)MessageType.Alert)
      _grid.Columns.Find(c => c.DataField == "Clear").Formatter = new CustomFormatter
      {
        FormatFunction = "formatClearColumn",
        UnFormatFunction = "unformatClearColumn"
      };

    }
  }
}