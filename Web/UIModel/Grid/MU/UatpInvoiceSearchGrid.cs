using System.Collections.Generic;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;


namespace Iata.IS.Web.UIModel.Grid.MU
{
    public class UatpInvoiceSearchGrid : GridBase
    {
        public UatpInvoiceSearchGrid(string gridId, string dataUrl)
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
                            GridColumnHelper.ActionColumn("Id", 70),
                            GridColumnHelper.HiddenColumn("InvoiceStatusId"),
                            GridColumnHelper.HiddenColumn("SubmissionMethodId"),
                            GridColumnHelper.HiddenColumn("LastUpdatedOn"),
                            GridColumnHelper.HiddenColumn("IsLegalPdfGenerated"),
                            GridColumnHelper.HiddenColumn("DigitalSignatureStatusId"),
                            GridColumnHelper.HiddenColumn("InputFileStatusId"),
                            GridColumnHelper.TextColumn("DisplayBillingPeriod", "Billing Period", 80, true),
                            GridColumnHelper.TextColumn("BilledMemberText", "Billed Member", MemberNumericCodeColoumnWidth, true), // CMP#596 Update grid column as per Table 3, Table 9,Table 13 and Table 21  
                            GridColumnHelper.TextColumn("InvoiceOwnerDisplayText", "Invoice/Credit Note Owner", 100, true),
                            GridColumnHelper.TextColumn("InvoiceTypeDisplayText", "Transaction Type", 100, true),
                            GridColumnHelper.TextColumn("InvoiceNumber", "Invoice/Credit Note Number", 120, true),
                            GridColumnHelper.TextColumn("InvoiceStatusDisplayText", "Invoice/Credit Note Status", 100, true),
                            GridColumnHelper.TextColumn("ChargeCategoryDisplayName", "Charge Category", 120, true),
                            GridColumnHelper.TextColumn("SettlementMethodDisplayText", "SMI", 46, true),
                            GridColumnHelper.TextColumn("ListingCurrencyDisplayText", "Billing Currency", 70, true),
                            GridColumnHelper.AmountColumn("BillingAmount", "Billing Amount", 3, 90, true),
                            GridColumnHelper.CustomColumn("ExchangeRate", "Exchange Rate", 100, TextAlign.Right, new NumberFormatter {  DecimalPlaces = 5 }, true, null, true),
                            GridColumnHelper.TextColumn("BillingCurrencyDisplayText", "Clearance Currency", 70, true),
                            GridColumnHelper.AmountColumn("ClearanceAmount", "Clearance Amount", 3, 90, true),
                            GridColumnHelper.TextColumn("SubmissionMethodDisplayText", "Submission Method", 70, true),
                            GridColumnHelper.TextColumn("InputFileNameDisplayText", "File Name", 217, true),
                            GridColumnHelper.HiddenColumn("InvoiceTypeId")
                          };
                // to show the invoice in descending order of the date.
                _grid.SortSettings.InitialSortColumn = "LastUpdatedOn";
                _grid.SortSettings.InitialSortDirection = SortDirection.Desc;
                var formatter = new CustomFormatter { FormatFunction = string.Format("{0}_EditViewDeleteRecord", _grid.ID) };

                // Grid Checkbox column
                _grid.MultiSelect = true;
                _grid.MultiSelectMode = MultiSelectMode.SelectOnCheckBoxClickOnly;
                _grid.Columns[0].Formatter = formatter;

                _grid.ClientSideEvents = new ClientSideEvents
                {
                    RowSelect = "GetSelectedRecordId"
                };

                //280744 - MISC UATP Exchange Rate population/validation during error 
                // Desc: Function to display currency amount, Exchange Rate as NULL/blank instead of 0.
                var exchangeRateNullFormatter = new CustomFormatter { FormatFunction = "exchangeRateNullFormatter" };
                _grid.Columns[17].Formatter = exchangeRateNullFormatter;
                var clearanceAmountNullFormatter = new CustomFormatter { FormatFunction = "clearanceAmountNullFormatter" };
                _grid.Columns[19].Formatter = clearanceAmountNullFormatter;

            }
        }


    }
}