using System.Collections.Generic;
using Iata.IS.AdminSystem;
using Iata.IS.Model.Enums;
using Iata.IS.Web.UIModel.Grid.Base;
using Iata.IS.Web.Util;
using Trirand.Web.Mvc;
using Iata.IS.AdminSystem;
using Iata.IS.Model.Enums;

namespace Iata.IS.Web.UIModel.Grid.MU
{
  public class BillingHistorySearchGrid : GridBase
  {
    public BillingHistorySearchGrid(string gridId, string dataUrl, int? billingCategoryId)
      : base(gridId, dataUrl)
    {
      //249863 - Request to extend the search results for PAX/MISC payables and Billing History screen to 500
      if(billingCategoryId!=null && billingCategoryId == (int)BillingCategoryType.Misc)
      {
        var pageSize = string.IsNullOrEmpty(GlobalVariables.PageSizeOptions)
                      ? SystemParameters.Instance.UIParameters.PageSizeOptions
                      : GlobalVariables.PageSizeOptions;

        this.PageSizeOptions = (pageSize.Contains("200") && !pageSize.Contains("500"))
                                 ? pageSize.Replace("200", "500")
                                 : GlobalVariables.PageSizeOptions;
        //CMP #655: IS-WEB Display per Location ID
        // Since this class is used for both Misc and UATP billing history, so as per usage below column has been shown in the grid.
        _grid.Columns.Find(column => column.DataField == "DisplayMemberLocation").Visible = true;
      }
      else { _grid.Columns.Find(column => column.DataField == "DisplayMemberLocation").Visible = false; }
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
                          GridColumnHelper.ActionColumn("InvoiceId", 90),//0
                          GridColumnHelper.HiddenColumn("BillingMemberId"),//1
                          GridColumnHelper.HiddenColumn("ClearingHouse"),//2
                          GridColumnHelper.TextColumn("TransactionDate", "Transaction Date", 120, true),//3
                          GridColumnHelper.TextColumn("TransactionNumber", "Transaction No.", 120, true),//4
                          GridColumnHelper.TextColumn("MemberCode", "Member Code", MemberNumericCodeColoumnWidth, true),//5 // CMP#596 Update grid column as per Table 3, Table 9,Table 13 and Table 21  
                          GridColumnHelper.TextColumn("DisplayMemberLocation", "Billing/Billed Member’s Location ID", 130, true),//6 //CMP #655: IS-WEB Display per Location ID
                          GridColumnHelper.TextColumn("ChargeCategory", "Charge Category", 140, true),//7
                          GridColumnHelper.TextColumn("RejectionStage","Rejection Stage", 60, true),//8
                          GridColumnHelper.TextColumn("DisplayCorrespondenceStatus", "Correspondence Status",100, true),//9
                          GridColumnHelper.TextColumn("DisplayCorrespondenceSubStatus","Correspondence Sub Status",100, true),//10
                          GridColumnHelper.NumberColumn("NoOfDaysToExpire","Number Of Days To Expire", isSortable: true),//11
                          GridColumnHelper.NumberColumn("AuthorityToBill","Authority To Bill", isSortable: true),//12
                          GridColumnHelper.AmountColumn("TotalNetAmt", "Transaction Amount",width:140, isSortable:true),//13
                          GridColumnHelper.HiddenColumn("CorrInitiatingMember"),//14
                          GridColumnHelper.HiddenColumn("CorrespondenceId"),//15
                          GridColumnHelper.HiddenColumn("CorrespondenceStatusId"), //16
                          GridColumnHelper.HiddenColumn("InvoiceTypeId"), //17
                          //SCP244122 - CMP 572 - Aligning the sort logic between CGO/UATP and PAX/MISC
                          GridColumnHelper.HiddenColumn("NetAmtCurrency")//18
                        };

      _grid.Columns.Find(column => column.DataField == "InvoiceId").Formatter = new CustomFormatter { FormatFunction = string.Format("{0}_GenerateBillingHistoryActions", ControlIdConstants.BHSearchResultsGrid) };
      
      _grid.Columns.Find(column => column.DataField == "AuthorityToBill").Formatter = new CustomFormatter { FormatFunction = "SetAuthorityToBill" };

      _grid.Columns.Find(column => column.DataField == "NoOfDaysToExpire").Formatter = new CustomFormatter { FormatFunction = "SetNoOfDaysToExpireAsBlank" };
     
      //SCP244122 - CMP 572 - Aligning the sort logic between CGO/UATP and PAX/MISC
      _grid.Columns.Find(column => column.DataField == "TotalNetAmt").Formatter = new CustomFormatter { FormatFunction = "ConcateCurreny" };
      
      _grid.ClientSideEvents = new ClientSideEvents
      {
        RowSelect = "GetSelectedRecordId"
      };
    }
  }
}
