using System.Collections.Generic;
using Iata.IS.Model.MemberProfile.Enums;
using Iata.IS.Web.UIModel.Grid.Base;
using Iata.IS.Web.Util;
using Trirand.Web.Mvc;

namespace Iata.IS.Web.UIModel.Grid.Reports
{
  public class InvoiceStatusSearchResults : GridBase
  {
    public InvoiceStatusSearchResults(string gridId, string dataUrl, bool isdisplayDailyDeliveryStatusCol= false)
      : base(gridId, dataUrl, isdisplayDailyDeliveryStatusCol: isdisplayDailyDeliveryStatusCol)
    {
    }

    /// <summary>
    /// Following method is used to initialize Columns InvoiceSearchResults grid on InvoiceStatusSearchResultControl.ascx
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
                              GridColumnHelper.HiddenColumn("InvoiceId",isPrimaryKey:true),
                              // SCP255637: Sorting Billing Period in the Processing Dashboard does not work properly
                              GridColumnHelper.SortableTextColumn("BillingPeriodNumber", "Billing Period", 66),
                              GridColumnHelper.SortableTextColumn("SettleMethodIndicator", "SMI",35),
                              GridColumnHelper.SortableTextColumn("BillingMemberCode","Billing Member",MemberNumericCodeColoumnWidth), // CMP#596 Update grid column as per Table 3, Table 9,Table 13 and Table 21  
                              GridColumnHelper.SortableTextColumn("BillingMemberName", "Billing Member Name",120),
                              GridColumnHelper.SortableTextColumn("BilledMemberCode","Billed Member",MemberNumericCodeColoumnWidth), // CMP#596 Update grid column as per Table 3, Table 9,Table 13 and Table 21  
                              GridColumnHelper.SortableTextColumn("BilledMemberName","Billed Member Name",120),
                              GridColumnHelper.SortableTextColumn("InvoiceStatusDescription","Invoice Status",120),
                              //GridColumnHelper.SortableTextColumn("UInvoiceId","Unique Invoice No.",135),
                              GridColumnHelper.SortableTextColumn("InvoiceNo","Invoice No.",100),
                              GridColumnHelper.SortableTextColumn("FormatedInvoiceDate","Invoice Date",66),
                              GridColumnHelper.SortableTextColumn("BillingCategory", "Billing Category",76),
                              GridColumnHelper.SortableTextColumn("InvoiceCurrency","Invoice Curr.",42),
                              GridColumnHelper.AmountColumn("InvoiceAmount","Invoice Amount", 3, 110, true),

                              //CMP#415- Clearance Currency and Amount field in Dashboard
                              GridColumnHelper.SortableTextColumn("CurrancyOfBilling","Clearance Curr.",60),
                              GridColumnHelper.AmountColumn("CurrencyAmount","Clearance Amount", 3, 100, true),
                              
                              GridColumnHelper.ColorCodedStatusColumn("IsSuspendedLateSubmitted","Suspended / LateSubmitted", 90),
                              GridColumnHelper.ColorCodedStatusColumn("ValidationStatus","Validation", 56),
                              GridColumnHelper.ColorCodedStatusColumn("ValueConfirmationStatus","Value Confirmation", 74),
                              GridColumnHelper.ColorCodedStatusColumn("DigitalSignatureStatus","Digital Signature", 62),
                              GridColumnHelper.ColorCodedStatusColumn("SettlementFileStatus","Settlement File Sent", 80),
                              //CMP529 : Daily Output Generation for MISC Bilateral Invoices
                              _isdisplayDailyDeliveryStatusCol ? GridColumnHelper.ColorCodedStatusColumn("DailyDeliveryStatusId","Daily Delivery", 80):GridColumnHelper.HiddenColumn("DailyDeliveryStatusId"),
                              GridColumnHelper.ColorCodedStatusColumn("PresentedStatus","Presented", 71),
                              //CMP559 : Add Submission Method Column to Processing Dashboard
                              GridColumnHelper.SortableTextColumn("FileName","Submission Method",250),
                              GridColumnHelper.SortableTextColumn("UInvoiceId","Unique Invoice No.",135),
                              GridColumnHelper.HiddenColumn("ValidationStatusId"),
                              GridColumnHelper.HiddenColumn("SettleMethodIndicatorId"),
                              GridColumnHelper.HiddenColumn("BillingMonth"),
                              //CMP559 : Add Submission Method Column to Processing Dashboard
                              GridColumnHelper.HiddenColumn("SubmissionMethodId"),//27
                              GridColumnHelper.HiddenColumn("isPurged"),//28
                              GridColumnHelper.HiddenColumn("FileLogId"),//29
                              // SCP255637: Sorting Billing Period in the Processing Dashboard does not work properly
                              GridColumnHelper.HiddenColumn("BillingPeriod") //30
                            };
      }// end if()

      // Following Code is used to format Color coded columns on InvoiceSearchResults grid.
       
      _grid.Columns.Find(c => c.DataField == "ValidationStatus").Formatter = new CustomFormatter
      {
          FormatFunction = "formatStatusColumn",

          UnFormatFunction = "unformatStatusColumn"
      };
      _grid.Columns.Find(c => c.DataField == "ValueConfirmationStatus").Formatter = new CustomFormatter
      {
          FormatFunction = "formatStatusColumn",
          UnFormatFunction = "unformatStatusColumn"
      };
      _grid.Columns.Find(c => c.DataField == "DigitalSignatureStatus").Formatter = new CustomFormatter
      {
          FormatFunction = "formatStatusColumn",
          UnFormatFunction = "unformatStatusColumn"
      };
      _grid.Columns.Find(c => c.DataField == "SettlementFileStatus").Formatter = new CustomFormatter
      {
          FormatFunction = "formatStatusColumn",
          UnFormatFunction = "unformatStatusColumn"
      };
      _grid.Columns.Find(c => c.DataField == "PresentedStatus").Formatter = new CustomFormatter
      {
          FormatFunction = "formatStatusColumn",
          UnFormatFunction = "unformatStatusColumn"
      };
      _grid.Columns.Find(c => c.DataField == "IsSuspendedLateSubmitted").Formatter = new CustomFormatter
      {
          FormatFunction = "formatSuspendedLateSubmitted",
          UnFormatFunction = "unformatSuspendedLateSubmitted"
      };

      if (SessionUtil.UserCategory == UserCategory.Member)
      {
        _grid.Columns.Find(c => c.DataField == "InvoiceNo").Formatter = new CustomFormatter
                                                                          {
                                                                            FormatFunction = "formatInvoice",
                                                                            UnFormatFunction = "unformatInvoice"
                                                                          };
      }

      _grid.PagerSettings.PageSizeOptions = "[15,30,45,60]";
      _grid.PagerSettings.PageSize = 15;
      _grid.Height = 350;

      _grid.ClientSideEvents = new ClientSideEvents
      {
        RowSelect = "GetSelectedRecordId"
      };

      //CMP559 : Add Submission Method Column to Processing Dashboard
      _grid.Columns.Find(c => c.DataField == "FileName").Formatter = new CustomFormatter
                                                                                     {
                                                                                         FormatFunction = "formatSubmissionMethod",
                                                                                         UnFormatFunction = "UnformatSubmissionMethod"
                                                                                     };

      //CMP529 : Daily Output Generation for MISC Bilateral Invoices
      _grid.Columns.Find(c => c.DataField == "DailyDeliveryStatusId").Formatter = new CustomFormatter
      {
          FormatFunction = "formatDailyDeliveryColumn",//"formatStatusColumn",
          UnFormatFunction = "unformatDailyDeliveryColumn"//"unformatStatusColumn"
      };

      // SCP255637: Sorting Billing Period in the Processing Dashboard does not work properly
      _grid.Columns.Find(column => column.DataField == "BillingPeriodNumber").Formatter = new CustomFormatter { FormatFunction = "showInDateFormat" };

      //280744 - MISC UATP Exchange Rate population/validation during error 
      // Desc: Function to display currency amount as NULL/blank instead of 0.
      var clearanceAmountNullFormatter = new CustomFormatter { FormatFunction = "clearanceAmountNullFormatter" };
      _grid.Columns[14].Formatter = clearanceAmountNullFormatter;

    }// end InitializeColumns() 
  }// end InvoiceStatusSearchResults class
}// end namespace
