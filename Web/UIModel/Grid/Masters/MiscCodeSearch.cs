using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Masters
{
    public class MiscCodeSearch : GridBase
    {
        public MiscCodeSearch(string gridId, string dataUrl)
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
                                        GridColumnHelper.SortableTextColumn("MiscCodeGroupName", "Misc Code Group", 110),
                                        GridColumnHelper.SortableTextColumn("Name", "Misc Code", 110),
                                        GridColumnHelper.SortableTextColumn("Description", "Description", 130),                                        
                                        GridColumnHelper.SortableTextColumn("IsActive", "Active", 110),
                                        GridColumnHelper.DateTimeColumn("LastUpdatedOn", "Last Updated On", 200,true),
                                       
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