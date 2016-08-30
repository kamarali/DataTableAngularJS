using System.Collections.Generic;
using Iata.IS.Web.UIModel.Grid.Base;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Reports
{
  public class ManageSuspendedInvoicesSearchResult : GridBase
  {
    public ManageSuspendedInvoicesSearchResult(string gridId, string dataUrl)
      : base(gridId, dataUrl)
    {
    }

    /// <summary>
    /// Following method is used to initialize Columns of FileSearchResults grid on FileStatusSearchResultControl.ascx
    /// </summary>
    protected override void InitializeColumns()
    {
      if (_grid != null)
      {
        // Following code is used to add multiselect checkbox in Jqgrid header row.
        _grid.MultiSelectMode = MultiSelectMode.SelectOnCheckBoxClickOnly;
        _grid.MultiSelect = true;

        _grid.Columns = new List<JQGridColumn>
                          {
                            //TFS#9952 :IE:Version 11- Error message is not displaying in "Manage Suspended Invoices".
                            GridColumnHelper.HiddenColumn("Id",isPrimaryKey:true),
                            GridColumnHelper.HiddenColumn("ResubmissionStatusId"),
                            GridColumnHelper.HiddenColumn("BillingCategoryId"),
                            GridColumnHelper.TextColumn("InvoiceNumber","Invoice Number",100),
                            GridColumnHelper.TextColumn("FormatedInvoiceDate","Invoice Date",90),
                            GridColumnHelper.TextColumn("BillingCategory","Billing Category",100),
                            GridColumnHelper.TextColumn("OriginalBillingMonth","Original Billing Period",100),
                            GridColumnHelper.TextColumn("BillingCurrencyDisplayText","Invoice Currency",100),
                            GridColumnHelper.AmountColumn("InvoiceAmount","Invoice Total Amt",2),
                            GridColumnHelper.TextColumn("DisplayBilledMemberCode","Billed member",MemberNumericCodeColoumnWidth), // CMP#596 Update grid column as per Table 3, Table 9,Table 13 and Table 21  
                            GridColumnHelper.TextColumn("SettlementMethodDisplayText","SMI",80),
                            GridColumnHelper.TextColumn("SuspensionMonth","Suspension Period",130),
                            GridColumnHelper.TextColumn("ReinstatementMonth","Reinstatement Period",100),
                            GridColumnHelper.TextColumn("ResubmissionBillingMonth","Resubmission Billing Period",130),
                            GridColumnHelper.SortableTextColumn("ResubmissionStatusDisplayText","Resubmission Status",150),
                            GridColumnHelper.TextColumn("ResubmissionRemarks","Remarks",150),
                            };



      }
      _grid.Columns.Find(c => c.DataField == "ResubmissionRemarks").Formatter = new CustomFormatter
      {
        FormatFunction = "formatRemarkLink",
        UnFormatFunction = "unformatRemarkMember"
      };
      _grid.Columns.Find(c => c.DataField == "InvoiceNumber").Formatter = new CustomFormatter
      {
        FormatFunction = "formatInvoice",
        UnFormatFunction = "unformatInvoice"
      };

      var clientSideEvents = new ClientSideEvents { RowSelect = "RowSelectEvent" };
      _grid.ClientSideEvents = clientSideEvents;
    }// end InitializeColumns()  
  }// end MaangeSuspendedInvoicesSearchResult class
}// end namespace