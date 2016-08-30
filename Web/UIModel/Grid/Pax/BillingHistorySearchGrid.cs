using System.Collections.Generic;
using Iata.IS.AdminSystem;
using Iata.IS.Web.UIModel.Grid.Base;
using Iata.IS.Web.Util;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Pax
{
  public class BillingHistorySearchGrid : GridBase
  {
    public BillingHistorySearchGrid(string gridId, string dataUrl)
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
                          GridColumnHelper.HiddenColumn("BillingCodeId"),
                          GridColumnHelper.HiddenColumn("TransactionType"),
                          GridColumnHelper.TextColumn("DisplayTransactionType", "Transaction Type", 100, true),
                          GridColumnHelper.TextColumn("TransactionDate", "Transaction Date", 90, true),
                          GridColumnHelper.TextColumn("TransactionNumber", "Transaction No.", 100, true),
                          GridColumnHelper.TextColumn("DisplayBillingCode", "Billing Code", 46, true),
                          GridColumnHelper.TextColumn("InvoiceNumber", "Invoice No.", 80, true),
                          GridColumnHelper.TextColumn("MemberCode", "Member Code", 80, true),
                          GridColumnHelper.TextColumn("SourceCodeId", "Source Code", 50, true),
                          GridColumnHelper.TextColumn("RejectionStage","Rejection Stage", 60, true),
                          GridColumnHelper.TextColumn("ReasonCode","Reason Code", 50, true),
                          GridColumnHelper.TextColumn("DisplayCorrespondenceStatus", "Correspondence Status",100, true),
                          GridColumnHelper.TextColumn("DisplayCorrespondenceSubStatus","Correspondence Sub Status",100, true),
                          GridColumnHelper.NumberColumn("NoOfDaysToExpire","Number Of Days To Expire", isSortable: true),
                          GridColumnHelper.TextColumn("AuthorityToBill","Authority To Bill",60, true),
                          GridColumnHelper.TextColumn("TotalNetAmount", "Transaction Amount",120, true),
                          GridColumnHelper.HiddenColumn("CorrespondenceStatusId"), 
                          GridColumnHelper.HiddenColumn("BillingYear"), 
                          GridColumnHelper.HiddenColumn("BillingMonth"), 
                          GridColumnHelper.HiddenColumn("SettlementMethodId"), 
                          GridColumnHelper.HiddenColumn("BillingPeriod"), 
                        };

      _grid.Columns.Find(column => column.DataField == "TransactionId").Formatter = new CustomFormatter { FormatFunction = string.Format("{0}_GenerateBillingHistoryActions", ControlIdConstants.BHSearchResultsGrid) };

      _grid.Columns.Find(column => column.DataField == "AuthorityToBill").Formatter = new CustomFormatter { FormatFunction = "SetAuthorityToBill" };
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
