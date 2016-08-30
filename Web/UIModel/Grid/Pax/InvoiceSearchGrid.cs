using System.Collections.Generic;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Pax
{
  /// <summary>
  /// Invoice Search Grid
  /// </summary>
  public class InvoiceSearchGrid : GridBase
  {
    public InvoiceSearchGrid(string gridId, string dataUrl)
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
                            //If columns are re-ordered, please update the index of InvoiceStatusId in ScriptHelper::GenerateGridEditViewValidateDeleteScript
                            GridColumnHelper.ActionColumn("Id", 70),
                            GridColumnHelper.HiddenColumn("InvoiceStatusId"),
                            GridColumnHelper.HiddenColumn("SubmissionMethodId"),
                            GridColumnHelper.HiddenColumn("LastUpdatedOn"),
                            GridColumnHelper.HiddenColumn("IsLegalPdfGenerated"),
                            GridColumnHelper.HiddenColumn("DigitalSignatureStatusId"),
                            GridColumnHelper.HiddenColumn("InputFileStatusId"),
                            GridColumnHelper.HiddenColumn("InvoiceTypeId"), //SCP419601: PAX permissions issue //Column Row[7]
                            GridColumnHelper.HiddenColumn("BillingCodeId"), //SCP419601: PAX permissions issue //Column Row[8]
                            GridColumnHelper.TextColumn("DisplayBillingMonthYear", "Billing Period", 70, true),
                            GridColumnHelper.TextColumn("BilledMemberText", "Billed Member", 150, true),
                            GridColumnHelper.TextColumn("InvoiceOwnerDisplayText", "Invoice/Credit Note Owner", 100, true),
                            GridColumnHelper.TextColumn("DisplayBillingCode", "Billing Code", 46, true),
                            GridColumnHelper.TextColumn("InvoiceNumber", "Invoice/Credit Note No", 120, true),
                            GridColumnHelper.TextColumn("InvoiceStatusDisplayText", "Invoice/Credit Note  Status", 120, true),
                            GridColumnHelper.TextColumn("SettlementMethodDisplayText", "SMI", 35, true),
                            GridColumnHelper.TextColumn("ListingCurrencyDisplayText", "Listing Currency", 70, true),
                            GridColumnHelper.AmountColumn("ListingAmount", "Listing Amount", isSortable:true),
                            GridColumnHelper.CustomColumn("ExchangeRate", "Exchange Rate", 70, TextAlign.Right, new NumberFormatter { DecimalPlaces = 5 }, isSortable:true ),
                            GridColumnHelper.TextColumn("BillingCurrencyDisplayText", "Billing Currency", 70, true),
                            GridColumnHelper.AmountColumn("BillingAmount", "Billing Amount", isSortable:true),
                            GridColumnHelper.TextColumn("SubmissionMethodDisplayText", "Submission Method", 70, true),
                            GridColumnHelper.TextColumn("InputFileNameDisplayText", "FileName", 217, true),
                            
                          };

        // to show the invoice in descending order of the date.
        _grid.SortSettings.InitialSortColumn = "LastUpdatedOn";
        _grid.SortSettings.InitialSortDirection = SortDirection.Desc;

        var formatter = new CustomFormatter { FormatFunction = string.Format("{0}_EditViewDeleteRecord", _grid.ID) };

        // Grid Checkbox column
        _grid.MultiSelect = true;
        _grid.MultiSelectMode = MultiSelectMode.SelectOnCheckBoxClickOnly;
        _grid.Columns[0].Formatter = formatter;
      }
    }
  }
}