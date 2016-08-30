<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Cargo.AwbRecord>" %>
<table class="formattedTable">
  <thead>
    <tr>
      <td>
        Billing Period
      </td>
      <td>
        Billing Member
      </td>
      <td>
        Billed Member
      </td>
      <td>
        Invoice Number
      </td>    
    </tr>
  </thead>
  <tbody>
    <tr>
      <td>
        <%: Model.Invoice.DisplayBillingPeriod%>
      </td>
      <td>
        <%: Model.Invoice.BillingMemberText%>
      </td>
      <td>
        <%:Model.Invoice.BilledMemberText%>
      </td>
      <td>
        <%: Model.Invoice.InvoiceNumber%>
      </td>
    </tr>
  </tbody>
</table>
<table class="formattedTable">
    <thead>
      <tr>
        <td rowspan="2">
          Issuing Airln.
        </td>
        <%--SCP220888: Audit trail on IS-WEB [Added column]--%>
        <td rowspan="2">
          Awb Breakdown Serial No.
        </td>
        <td rowspan="2">
          Batch Seq. No.
        </td>
        <td rowspan="2">
          Record Seq. No. <br /> Within Batch
        </td>
        <td rowspan="2">
          From-To
        </td>
        <td rowspan="2">
          Listing Currency
        </td>        
        <td colspan="2">
          ISC
        </td>
        <td rowspan="2">
          Other Charges
        </td>
        <td rowspan="2">
          VAT Amt.
        </td>
        <td rowspan="2">
          Total Amt.
        </td>
        <td rowspan="2">
          Weight Charges
        </td>
        <td rowspan="2">
          Valuation Charges
        </td>
        <td rowspan="2">
          IS - Validation Flag
        </td>
      </tr>
      <tr>
        <td style="width: 50px;">
          %
        </td>
        <td style="width: 50px;">
          Amt.
        </td>
      </tr>
    </thead>
    <tbody>      
      <tr>
        <td>
          <%: Model.AwbIssueingAirline%>
        </td>
        <%--SCP220888: Audit trail on IS-WEB [Added column]--%>
        <td>
          <%: Model.AwbSerialNumber%>
        </td>
        <td class="numeric">
          <%: Model.BatchSequenceNumber%>
        </td>
        <td class="numeric">
          <%: Model.RecordSequenceWithinBatch%>
        </td>
        <td class="numeric">
          <%: Model.CarriageFromId%> - <%: Model.CarriageToId%>
        </td>
        <td>
          <%: Model.Invoice.ListingCurrencyDisplayText %>
        </td>
        <td class="numeric">
          <%: Convert.ToDouble(Model.IscPer).ToString(FormatConstants.ThreeDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: Convert.ToDouble(Model.IscAmount).ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: Convert.ToDouble(Model.OtherCharges).ToString(FormatConstants.ThreeDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: Convert.ToDouble(Model.VatAmount).ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: Convert.ToDouble(Model.AwbTotalAmount).ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
         <td class="numeric">
          <%: Convert.ToDouble(Model.WeightCharges).ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: Convert.ToDouble(Model.ValuationCharges).ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td>
          <%: Model.ISValidationFlag%>
        </td>
      </tr>
      
    </tbody>


</table>
<%
               if (Model.Attachments.Count > 0)
               {
%>
<br />
<div>
  <b>Supporting Document(s)</b><br />
  <%
                 foreach (var attachment in Model.Attachments)
                 {%>
  <a href="<%:Url.Action("BillingHistoryAttachmentDownload", new { invoiceId = attachment.Id })%>">
    <%:attachment.OriginalFileName%></a><br />
  <%
                 }
%>
</div>
<%
               }
%>