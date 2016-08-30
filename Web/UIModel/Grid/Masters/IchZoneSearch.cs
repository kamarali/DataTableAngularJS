using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;
namespace Iata.IS.Web.UIModel.Grid.Masters
{
    public class IchZoneSearch : GridBase
    {
        public IchZoneSearch(string gridId, string dataUrl)
      : base(gridId, dataUrl)
    {
    }

    protected override void InitializeColumns()
    {
      if (_grid != null)
      {
        _grid.Columns = new List<JQGridColumn>
                                    {
                                        GridColumnHelper.ActionColumn("Id",80),
                                        GridColumnHelper.HiddenColumn("IsActive"),
                                        GridColumnHelper.SortableTextColumn("Zone", "Zone", 80),
                                        GridColumnHelper.SortableTextColumn("ClearanceCurrency", "Clearance Currency", 150),
                                        GridColumnHelper.SortableTextColumn("Description", "Description", 280),
                                        GridColumnHelper.SortableTextColumn("IsActive", "Active", 115),
                                        GridColumnHelper.SortableTextColumn("LastUpdatedOn", "Last Updated On", 280),
                                       
                                    };
        _grid.SortSettings.InitialSortColumn = "LastUpdatedOn";
        _grid.SortSettings.InitialSortDirection = SortDirection.Desc;
        var formatter = new CustomFormatter
        {
          FormatFunction = string.Format("{0}_EditViewDeleteRecord", _grid.ID)
        };

        _grid.Columns[0].Formatter = formatter;

      }
    }
    }
}