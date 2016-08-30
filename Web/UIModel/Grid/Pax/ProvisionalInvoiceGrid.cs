using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Iata.IS.Model.Pax.Sampling;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Pax
{
  public class ProvisionalInvoiceGrid : GridBase
  {
    public ProvisionalInvoiceGrid(string gridId, string dataUrl, bool isGridViewOnly = false)
      : base(gridId, dataUrl, isGridViewOnly)
    {
    }

    /// <summary>
    /// Initializes columns for grid
    /// </summary>
    protected override void InitializeColumns()
    {
      if (_grid == null) return;
      _grid.Columns = new List<JQGridColumn>
                        {
                          GridColumnHelper.ActionColumn("Id",90,_isGridViewOnly),
                          GridColumnHelper.TextColumn("InvoiceNumber","Provisional Invoice No.",70),
                          GridColumnHelper.DateColumn("InvoiceDate","Provisional Invoice Date"),
                          GridColumnHelper.NumberColumn("BillingPeriodNo","Provisional Billing Period No."),
                          GridColumnHelper.TextColumn("ListingCurrencyDisplayText","Listing Currency",70),
                          GridColumnHelper.AmountColumn("InvoiceListingAmount","Listing Amount"),
                          GridColumnHelper.ExchangeRateColumn("ListingToBillingRate","Exchange Rate"),
                          GridColumnHelper.AmountColumn("InvoiceBillingAmount","Billing Amount"),
                        };

      var formatter = new CustomFormatter
                        {
                          FormatFunction = string.Format("{0}_DeleteRecord", _grid.ID)
                        };

      _grid.Columns[0].Formatter = formatter;
    }
  }
}