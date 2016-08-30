using System.Collections.Generic;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;
using Iata.IS.Web.Util;

namespace Iata.IS.Web.UIModel.Grid.MUPayables
{
  /// <summary>
  /// Misc Invoice Search Grid
  /// </summary>
    public class MiscDailyPayableInvoiceSearchGrid : GridBase
  {
    public MiscDailyPayableInvoiceSearchGrid(string gridId, string dataUrl, bool ispageRedirect)
          : base(gridId, dataUrl)
    {
        // CMP #665: User Related Enhancements-FRS-v1.2.doc [Sec 2.8 Conditional Redirection of users upon login in is-web]
        if (ispageRedirect)
        {
            PageSizeOptions = "[5,10,20,50, 100,150,200,500]";
            DefaultPageSize = 500;
        }
    }

    /// <summary>
    /// Initializes columns for grid
    /// </summary>
    protected override void InitializeColumns()
    {
      if (_grid != null)
      {
          JQGridColumn dateCol = GridColumnHelper.TextColumn("TargetDate", "Delivery Date", 100, true);
          dateCol.DataFormatString = FormatConstants.GridColumnDateFormat;

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
                            dateCol,
                            GridColumnHelper.TextColumn("BillingMemberText", "Billing Member", MemberNumericCodeColoumnWidth, true), // CMP#596 Update grid column as per Table 3, Table 9,Table 13 and Table 21  
                            GridColumnHelper.TextColumn("BilledMemberLocation", "Billed Member’s Location ID", 100, true), // CMP #655: IS-WEB Display per Location ID
                            GridColumnHelper.TextColumn("InvoiceTypeDisplayText", "Transaction Type", 100, true),
                            GridColumnHelper.TextColumn("InvoiceNumber", "Invoice/Credit Note Number", 120, true),
                            GridColumnHelper.TextColumn("ChargeCategoryDisplayName", "Charge Category", 120, true),
                            GridColumnHelper.TextColumn("SettlementMethodDisplayText", "SMI", 100, true),
                            GridColumnHelper.TextColumn("ListingCurrencyDisplayText", "Billing Currency", 70, true),
                            GridColumnHelper.AmountColumn("BillingAmount", "Billing Amount", 3, isSortable: true)
                          };
        // to show the invoice in descending order of the date.
        _grid.SortSettings.InitialSortColumn = "TargetDate";
        _grid.SortSettings.InitialSortDirection = SortDirection.Desc;
       
        var formatter = new CustomFormatter
                          {
                            FormatFunction = string.Format("{0}_ActionItems", _grid.ID)
                          };
        _grid.Columns.Find(col => col.DataField == "Id").Formatter = formatter;
      }

    }
  }
}