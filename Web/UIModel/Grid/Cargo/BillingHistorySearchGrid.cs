using System.Collections.Generic;
using Iata.IS.Web.UIModel.Grid.Base;
using Iata.IS.Web.Util;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Cargo
{
    public class BillingHistorySearchGrid : GridBase
    {
        public BillingHistorySearchGrid(string gridId, string dataUrl)
            : base(gridId, dataUrl)
        {
        }

        /// <summary>
        /// Initializes columns for grid
        /// </summary>
        protected override void InitializeColumns()
        {
            if (_grid == null)
            {
                return;
            }
            /*PLEASE UPDATE IN SCRIPTHELPER JAVASCRIPT IF SEQUENCE OF COLUMNS IS CHANGED.*/
            _grid.Columns = new List<JQGridColumn> 
                        { 
                          GridColumnHelper.ActionColumn("TransactionId", 90),
                          GridColumnHelper.HiddenColumn("InvoiceId"),
                          GridColumnHelper.HiddenColumn("BillingMemberId"),
                          GridColumnHelper.HiddenColumn("CorrInitiatingMember"),
                          GridColumnHelper.HiddenColumn("TransactionType"),
                          GridColumnHelper.TextColumn("TransactionType", "Transaction Type", 100, true),
                          GridColumnHelper.TextColumn("TransactionDate", "Transaction Date", 90, true),
                          GridColumnHelper.TextColumn("TransactionNumber", "Transaction No.", 100, true),
                          GridColumnHelper.TextColumn("InvoiceNumber", "Invoice No.", 80, true),
                          GridColumnHelper.TextColumn("MemberCode", "Member Code", 80, true),
                          GridColumnHelper.TextColumn("RejectionStage","Rejection Stage", 60, true),
                          GridColumnHelper.TextColumn("ReasonCode","Reason Code", 50, true),
                          GridColumnHelper.TextColumn("DisplayCorrespondenceStatus", "Correspondence Status",100, true),
                          GridColumnHelper.TextColumn("DisplayCorrespondenceSubStatus","Correspondence Sub Status",100, true),
                          GridColumnHelper.NumberColumn("NoOfDaysToExpire","Number Of Days To Expire", isSortable: true),
                          GridColumnHelper.TextColumn("AuthorityToBill","Authority To Bill",60, true),
                          GridColumnHelper.AmountColumn("TotalNetAmountValue", "Transaction Amount",width:140, isSortable:true),
                          GridColumnHelper.HiddenColumn("CorrespondenceStatusId"), 
                          GridColumnHelper.HiddenColumn("BillingYear"), 
                          GridColumnHelper.HiddenColumn("BillingMonth"), 
                          GridColumnHelper.HiddenColumn("SettlementMethodId"),
                          GridColumnHelper.HiddenColumn("BillingCodeId"),
                          GridColumnHelper.HiddenColumn("BillingPeriod"),
                          //SCP244122 - CMP 572 - Aligning the sort logic between CGO/UATP and PAX / MISC
                          GridColumnHelper.HiddenColumn("TotalNetAmountCurrency") //23
                        };

            _grid.Columns.Find(column => column.DataField == "TransactionId").Formatter = new CustomFormatter { FormatFunction = string.Format("{0}_GenerateBillingHistoryActions", ControlIdConstants.BHSearchResultsGrid) };
            _grid.Columns.Find(column => column.DataField == "TransactionNumber").Formatter = new CustomFormatter { FormatFunction = "SetAwbSerialNumber" };
            _grid.Columns.Find(column => column.DataField == "AuthorityToBill").Formatter = new CustomFormatter { FormatFunction = "SetAuthorityToBill" };

            //SCP244122 - CMP 572 - Aligning the sort logic between CGO/UATP and PAX/MISC
            _grid.Columns.Find(column => column.DataField == "TotalNetAmountValue").Formatter = new CustomFormatter { FormatFunction = "ConcateTotalNetAmountCurrency" };

            // Grid Checkbox column
            _grid.MultiSelect = true;

            _grid.MultiSelectMode = MultiSelectMode.SelectOnCheckBoxClickOnly;

            _grid.ClientSideEvents = new ClientSideEvents
            {
                RowSelect = "GetSelectedRecordId"
            };
        }
    }
}
