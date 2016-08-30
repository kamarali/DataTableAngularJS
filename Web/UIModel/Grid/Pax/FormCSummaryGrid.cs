using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Pax
{
  public class FormCSummaryGrid: GridBase
  {
    public FormCSummaryGrid(string gridId, string dataUrl)
      : base(gridId, dataUrl)
    {
    }

    /// <summary>
    /// Initializes columns for grid
    /// </summary>
    protected override void InitializeColumns()
    {
      if (_grid != null)
      {
        _grid.Columns = new List<JQGridColumn> 
        {
          GridColumnHelper.NumberColumn("TotalRecords", "Total No. of Items"),           
          GridColumnHelper.CustomColumn("ListingCurrencyCode","Curr. of Listing / Evaluation", 180, TextAlign.Left),
          GridColumnHelper.CustomColumn("TotalGrossAmountAlf", "Total Gross Amount/ALF", 180, TextAlign.Right, new NumberFormatter() { DecimalPlaces = 2, DecimalSeparator = ".", ThousandsSeparator = "," })
        };
      }
    }
  }
}