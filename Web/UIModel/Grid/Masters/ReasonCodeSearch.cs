using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Masters
{
    public class ReasonCodeSearch: GridBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReasonCodeSearch"/> class.
        /// </summary>
        /// <param name="gridId">The grid id.</param>
        /// <param name="dataUrl">The data URL.</param>
        public ReasonCodeSearch(string gridId, string dataUrl)
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
                                        GridColumnHelper.SortableTextColumn("Code", "Code", 80),
                                        GridColumnHelper.SortableTextColumn("TransactionTypeName", "Transaction Type", 110),
                                        GridColumnHelper.SortableTextColumn("Description", "Description", 150),
                                         GridColumnHelper.SortableTextColumn("CouponAwbBreakdownMandatory", "Coupon AWB Breakdown Mandatory", 130),
                                          GridColumnHelper.SortableTextColumn("BilateralCode", "Bilateral Code", 110),
                                        GridColumnHelper.SortableTextColumn("IsActive", "Active", 115),
                                         GridColumnHelper.DateTimeColumn("LastUpdatedOn", "Last Updated On", 200,true),
                                       
                                    };
        _grid.SortSettings.InitialSortColumn = "LastUpdatedOn";
        _grid.SortSettings.InitialSortDirection = SortDirection.Desc;
        _grid.Width = 890;
        var formatter = new CustomFormatter
        {
          FormatFunction = string.Format("{0}_EditViewDeleteRecord", _grid.ID)
        };

        _grid.Columns[0].Formatter = formatter;

      }
    }
    }
}
