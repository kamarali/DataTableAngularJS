using System.Collections.Generic;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Common
{
  public class UpcomingEventsGrid : GridBase
  {
    private readonly int _gridType;
    public UpcomingEventsGrid(string gridId, string dataUrl)
      : base(gridId, dataUrl)
    {

    }
    protected override void InitializeColumns()
    {
      if (_grid != null)
      {
        _grid.Columns = new List<JQGridColumn>
                                  {
                                    GridColumnHelper.HiddenColumn("Id",isPrimaryKey:true),
                                    GridColumnHelper.TextColumn("Period","Period",80),
                                    GridColumnHelper.TextColumn("EventDescription","Milestone",180),
                                    GridColumnHelper.DateTimeColumn("YmqDateTime","Date (EST)",80),
                                    GridColumnHelper.DateTimeColumn("LocalDateTime","Local Date",80)
                                   
                                  };
        _grid.PagerSettings.ScrollBarPaging=true;
        _grid.AppearanceSettings.ShowFooter = false;

      }
    }
  }
}