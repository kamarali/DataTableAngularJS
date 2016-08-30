using System.Collections.Generic;
using Iata.IS.AdminSystem;
using Iata.IS.Model.Enums;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.MUPayables
{
  /// <summary>
  /// Misc Invoice Search Grid
  /// </summary>
  public class MiscInvoiceSearchGrid : GridBase
  {
    public MiscInvoiceSearchGrid(string gridId, string dataUrl, int? billingCategoryId)
      : base(gridId, dataUrl)
    {
      //249863 - Request to extend the search results for PAX/MISC payables and Billing History screen to 500
      if (billingCategoryId != null && billingCategoryId == (int)BillingCategoryType.Misc)
      {
        var pageSize = string.IsNullOrEmpty(GlobalVariables.PageSizeOptions)
                      ? SystemParameters.Instance.UIParameters.PageSizeOptions
                      : GlobalVariables.PageSizeOptions;

        this.PageSizeOptions = (pageSize.Contains("200") && !pageSize.Contains("500"))
                                 ? pageSize.Replace("200", "500")
                                 : GlobalVariables.PageSizeOptions;
        //CMP #655: IS-WEB Display per Location ID
        // Since this class is used for both Misc and UATP billing history, so as per usage below column has been shown in the grid.
        _grid.Columns.Find(column => column.DataField == "BilledMemberLocation").Visible = true;
      }
      else { _grid.Columns.Find(column => column.DataField == "BilledMemberLocation").Visible = false; }
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
                            // PLEASE UPDATE IN JS for InvoiceTypeId, RejectionStage if columns are reordered.
                            // CMP #665: User Related Enhancements-FRS-v1.2.doc[Sec 2.9: IS-WEB MISC Payables Invoice Search Screen]
                            //Changes in the width 90 to 120 because of this cmp, we have included 2 more icon in the action tab of grid.
                            GridColumnHelper.ActionColumn("Id", 120),
                            GridColumnHelper.HiddenColumn("InvoiceTypeId"),
                            GridColumnHelper.HiddenColumn("RejectionStage"),
                            GridColumnHelper.HiddenColumn("SettlementMethodId"),
                            GridColumnHelper.HiddenColumn("LastUpdatedOn"),
                            GridColumnHelper.HiddenColumn("IsLegalPdfGenerated"),
                            GridColumnHelper.HiddenColumn("DigitalSignatureStatusId"),
                            GridColumnHelper.TextColumn("DisplayBillingPeriod", "Billing Period", 70, true),
                            GridColumnHelper.TextColumn("BillingMemberText", "Billing Member", MemberNumericCodeColoumnWidth, true), // CMP#596 Update grid column as per Table 3, Table 9,Table 13 and Table 21  
                            GridColumnHelper.TextColumn("BilledMemberLocation", "Billed Member’s Location ID", 100, true), // CMP #655: IS-WEB Display per Location ID
                            GridColumnHelper.TextColumn("InvoiceTypeDisplayText", "Transaction Type", 100, true),
                            GridColumnHelper.TextColumn("InvoiceNumber", "Invoice/Credit Note Number", 120, true),
                            GridColumnHelper.TextColumn("ChargeCategoryDisplayName", "Charge Category", 120, true),
                            GridColumnHelper.TextColumn("SettlementMethodDisplayText", "SMI", 100, true),
                            GridColumnHelper.TextColumn("ListingCurrencyDisplayText", "Billing Currency", 70, true),
                            GridColumnHelper.AmountColumn("BillingAmount", "Billing Amount", 3, isSortable: true),
                            GridColumnHelper.CustomColumn("ExchangeRate",
                                                          "Exchange Rate",
                                                          100,
                                                          TextAlign.Right,
                                                          new NumberFormatter
                                                            {
                                                              DecimalPlaces = 5
                                                            },
                                                          isSortable: true),
                            GridColumnHelper.TextColumn("BillingCurrencyDisplayText", "Clearance Currency", 70, true),
                            GridColumnHelper.AmountColumn("ClearanceAmount", "Clearance Amount", 3, isSortable: true)

                          };
        // to show the invoice in descending order of the date.
        _grid.SortSettings.InitialSortColumn = "LastUpdatedOn";
        _grid.SortSettings.InitialSortDirection = SortDirection.Desc;
        var formatter = new CustomFormatter
                          {
                            FormatFunction = string.Format("{0}_ViewRejectRecord", _grid.ID)
                          };
        _grid.Columns.Find(col => col.DataField == "Id").Formatter = formatter;
        /* SCP#390702 - KAL: Issue with Clearance Amount. Desc: Clearance Amount is made nullable. */
        var clearanceAmountNullFormatter = new CustomFormatter { FormatFunction = "clearanceAmountNullFormatter" };
        _grid.Columns[18].Formatter = clearanceAmountNullFormatter;
      }

    }
  }
}
