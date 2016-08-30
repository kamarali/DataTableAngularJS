using System.Collections.Generic;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Pax
{
  public class FormCPayablesSearchGrid : GridBase
  {
    public FormCPayablesSearchGrid(string gridId, string dataUrl)
      : base(gridId, dataUrl)
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
                          /* IMPORTANT: DO NOT CHANGE THE SEQUENCE OF COLUMNS AS IT IS USED IN JAVASCRIPT */
                          GridColumnHelper.ActionColumn("UniqueKey",90),
                          GridColumnHelper.TextColumn("ProvisionalInvoiceBillingYearMonth","Prov. Billing Month",100),
                          // For payables, this is the From member Id.
                          GridColumnHelper.CustomColumn("ProvisionalBillingMember","Prov. Billed Member",120, TextAlign.Left),
                          
                          GridColumnHelper.TextColumn("NilFormCIndicator","Nil Form C Indc.",70),
                          GridColumnHelper.NumberColumn("TotalRecords","Total No. of Items"),
                          GridColumnHelper.CustomColumn("ListingCurrencyCode","Curr. of Listing / Evaluation",100,TextAlign.Left),
                          GridColumnHelper.CustomColumn("TotalGrossAmountAlf","Total Gross Amount/ALF",120,TextAlign.Right, new NumberFormatter() { DecimalPlaces = 2, DecimalSeparator = ".", ThousandsSeparator = "," }),
                          
                          GridColumnHelper.HiddenColumn("InvoiceStatus"),
                          GridColumnHelper.HiddenColumn("ProvisionalBillingYear"),
                          //hidden
                          GridColumnHelper.HiddenColumn("ProvisionalBillingMonth"),
                          //hidden
                          GridColumnHelper.HiddenColumn("ProvisionalBillingMemberId"),
                          //hidden
                          GridColumnHelper.HiddenColumn("FromMemberId"),
                          //hidden
                          GridColumnHelper.HiddenColumn("ListingCurrencyId"),
                          //hidden
                          GridColumnHelper.HiddenColumn("InvoiceStatusId")
                        };
      
      var formatter = new CustomFormatter() { FormatFunction = string.Format("{0}_ViewDownloadZipRecord", _grid.ID) };

      _grid.Columns[0].Formatter = formatter;
    }
  }
}