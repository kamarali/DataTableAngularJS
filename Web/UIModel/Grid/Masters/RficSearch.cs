using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Masters
{
    public class RficSearch : GridBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RficSearch"/> class.
        /// </summary>
        /// <param name="gridId">The grid id.</param>
        /// <param name="dataUrl">The data URL.</param>
        public RficSearch(string gridId, string dataUrl)
      : base(gridId, dataUrl)
    {
    }

        /// <summary>
        /// Initializes the columns.
        /// </summary>
    protected override void InitializeColumns()
    {
      if (_grid != null)
      {
        _grid.Columns = new List<JQGridColumn>
                                    {
                                        GridColumnHelper.ActionColumn("Id",80),
                                        GridColumnHelper.HiddenColumn("IsActive"),
                                        GridColumnHelper.SortableTextColumn("Id", "RFIC Code", 80),
                                        GridColumnHelper.SortableTextColumn("Description", "Description", 150),
                                        GridColumnHelper.SortableTextColumn("IsActive", "Active", 115),
                                         GridColumnHelper.DateTimeColumn("LastUpdatedOn", "Last Updated On", 280,true),
                                       
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