using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Masters
{
    public class InvPaymentStatusSearch : GridBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvPaymentStatusSearch"/> class.
        /// </summary>
        /// <param name="gridId">The grid id.</param>
        /// <param name="dataUrl">The data URL.</param>
        public InvPaymentStatusSearch(string gridId, string dataUrl)
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
                                        GridColumnHelper.SortableTextColumn("Description", "Payment Status Description", 250),
                                        GridColumnHelper.SortableTextColumn("Applicable", "Applicalbe For", 120),
                                        GridColumnHelper.SortableTextColumn("IsActive", "Active", 80),
                                        GridColumnHelper.DateTimeColumn("LastUpdatedOn", "Last Updated On", 200,true),
                                       
                                    };
        _grid.SortSettings.InitialSortColumn = "Description";
        _grid.SortSettings.InitialSortDirection = SortDirection.Asc;
        var formatter = new CustomFormatter
        {
          FormatFunction = string.Format("{0}_DeleteRecord", _grid.ID)
        };

        _grid.Columns[0].Formatter = formatter;

      }
    }
    }
}