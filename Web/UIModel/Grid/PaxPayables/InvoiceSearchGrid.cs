using System.Collections.Generic;
using Iata.IS.AdminSystem;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.PaxPayables
{
  /// <summary>
  /// Invoice Search Grid
  /// </summary>
  public class InvoiceSearchGrid : GridBase
  {
    public InvoiceSearchGrid(string gridId, string dataUrl)
      : base(gridId, dataUrl)
    {
      //249863 - Request to extend the search results for PAX/MISC payables and Billing History screen to 500
      var pageSize = string.IsNullOrEmpty(GlobalVariables.PageSizeOptions)
                      ? SystemParameters.Instance.UIParameters.PageSizeOptions
                      : GlobalVariables.PageSizeOptions;

      this.PageSizeOptions = (pageSize.Contains("200") && !pageSize.Contains("500"))
                               ? pageSize.Replace("200", "500")
                               : GlobalVariables.PageSizeOptions;
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
                            GridColumnHelper.ActionColumn("Id", 90),
                            GridColumnHelper.HiddenColumn("IsLegalPdfGenerated"),
                            GridColumnHelper.HiddenColumn("DigitalSignatureStatusId"),
                            GridColumnHelper.HiddenColumn("InvoiceTypeId"), //SCP419601: PAX permissions issue //Column Row[3]
                            GridColumnHelper.HiddenColumn("BillingCodeId"), //SCP419601: PAX permissions issue //Column Row[4]
                            GridColumnHelper.TextColumn("DisplayBillingMonthYear", "Billing Period", 70, true),
                            GridColumnHelper.TextColumn("BillingMemberText", "Billing Member", 150, true),
                            GridColumnHelper.TextColumn("DisplayBillingCode", "Billing Code", 46, true),
                            GridColumnHelper.TextColumn("InvoiceNumber", "Invoice/Credit Note No", 120, true),
                            GridColumnHelper.TextColumn("SettlementMethodDisplayText", "SMI", 35, true),
                            GridColumnHelper.TextColumn("ListingCurrencyDisplayText", "Listing Currency", 70, true),
                            GridColumnHelper.AmountColumn("ListingAmount", "Listing Amount", isSortable:true),
                            GridColumnHelper.CustomColumn("ExchangeRate", "Exchange Rate", 70, TextAlign.Right, new NumberFormatter { DecimalPlaces = 5 }, isSortable:true),
                            GridColumnHelper.TextColumn("BillingCurrencyDisplayText", "Billing Currency", 70, true),
                            GridColumnHelper.AmountColumn("BillingAmount", "Billing Amount",isSortable:true)
                         //   GridColumnHelper.TextColumn("InputFileNameDisplayText", "FileName", 220, true),
                          };

        var formatter = new CustomFormatter { FormatFunction = string.Format("{0}_ViewRejectRecord", _grid.ID) };
        _grid.Columns[0].Formatter = formatter;
      }
    }
  }
}