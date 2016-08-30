using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Masters
{
  public class TimeLimitSearch : GridBase
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="TimeLimitSearch"/> class.
    /// </summary>
    /// <param name="gridId">The grid id.</param>
    /// <param name="dataUrl">The data URL.</param>
    public TimeLimitSearch(string gridId, string dataUrl)
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
                                        GridColumnHelper.SortableTextColumn("Limit", "Time Limit", 115),
                                        GridColumnHelper.DateFullYearTimeColumn("EffectiveFromPeriod", "Effective From Date/Period", 150),
                                        GridColumnHelper.DateFullYearTimeColumn("EffectiveToPeriod", "Effective To Date/Period", 150),
                                        GridColumnHelper.SortableTextColumn("ClearingHouse", "Settlement Method", 80),
                                        GridColumnHelper.SortableTextColumn("TransactionTypeName", "Transaction Type", 150),
                                        GridColumnHelper.SortableTextColumn("CalculationMethod", "Calculation Method", 115),
                                        GridColumnHelper.SortableTextColumn("IsActive", "Active", 80),
                                        GridColumnHelper.DateFullYearTimeColumn("LastUpdatedOn", "Last Updated On", 200,true),
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