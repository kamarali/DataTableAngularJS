<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.Sampling.SamplingFormDRecord>" %>
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
      <td>
        Billing Code
      </td>
      <td>
        Source Code
      </td>
      <td>
        Provisional Billing Month
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
      <td>
        <%: Model.Invoice.DisplayBillingCode%>
      </td>
      <td>
        <%: Model.SourceCodeId%>
      </td>
      <td>
        <%: Model.Invoice.ProvisionalBillingMonth%>
      </td>
    </tr>
  </tbody>
</table>
<table class="formattedTable">
  <thead>
    <tr>
      <td rowspan="2">
        Doc. / Cpn. No.
      </td>
      <td rowspan="2">
        Issuing Airln.
      </td>
      <td rowspan="2">
        Prov. Invoice No.
      </td>
      <td rowspan="2">
        Prov. Batch Seq. No.
      </td>
      <td rowspan="2">
        Prov. Record Seq. No. Within Batch
      </td>
      <td rowspan="2">
        From - To
      </td>
      <td rowspan="2">
        Gross Amt.
      </td>
      <td colspan="2">
        ISC
      </td>
      <td colspan="2">
        Other Commission
      </td>
      <td colspan="2">
        UATP
      </td>
      <td rowspan="2">
        Handling Fee Amount
      </td>
      <td rowspan="2">
        Tax Amount
      </td>
      <td rowspan="2">
        VAT Amount
      </td>
      <td rowspan="2">
        Total Amount
      </td>
      <td rowspan="2">
        Original PMI
      </td>
      <td rowspan="2">
        Validated PMI
      </td>
      <td rowspan="2">
        Agreement Indicator - Supplied
      </td>
      <td rowspan="2">
        Agreement Indicator - Validated
      </td>
      <td rowspan="2">
        IS - Validation Flag
      </td>
    </tr>
    <tr>
      <td style="width: 30px;">
        %
      </td>
      <td>
        Amt.
      </td>
      <td style="width: 30px;">
        %
      </td>
      <td>
        Amt.
      </td>
      <td style="width: 30px;">
        %
      </td>
      <td>
        Amt.
      </td>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td>
        <%: Model.TicketDocNumber%>
        <%: Model.CouponNumber%>
      </td>
      <td>
        <%: Model.TicketIssuingAirline%>
      </td>
      <td>
        <%: Model.ProvisionalInvoiceNumber%>
      </td>
      <td class="numeric">
        <%: Model.BatchNumberOfProvisionalInvoice%>
      </td>
       <td class="numeric">
        <%: Model.RecordSeqNumberOfProvisionalInvoice%>
      </td>
      <td>
      
      </td>
     
      <td class="numeric">
        <%: Model.EvaluatedGrossAmount%>
      </td>
      <td class="numeric">
        <%: Model.IscPercent.ToString(FormatConstants.ThreeDecimalsFormat) %>
      </td>
      <td class="numeric">
        <%: Model.IscAmount%>
      </td>
      <td class="numeric">
        <%: Model.OtherCommissionPercent.ToString(FormatConstants.ThreeDecimalsFormat) %>
      </td>
      <td class="numeric">
        <%: Model.OtherCommissionAmount%>
      </td>
      <td class="numeric">
        <%: Model.UatpPercent.ToString(FormatConstants.ThreeDecimalsFormat) %>
      </td>
      <td class="numeric">
        <%: Model.UatpAmount%>
      </td>
      <td class="numeric">
        <%: Model.HandlingFeeAmount%>
      </td>
      <td class="numeric">
        <%: Model.TaxAmount%>
      </td>
      <td class="numeric">
        <%: Model.VatAmount%>
      </td>
      <td class="numeric">
        <%: Model.EvaluatedNetAmount%>
      </td>
      <td>
        <%: Model.OriginalPmi%>
      </td>
      <td>
        <%: Model.ValidatedPmi%>
      </td>
      <td>
        <%: Model.AgreementIndicatorSupplied%>
      </td>
      <td>
        <%: Model.AgreementIndicatorValidated%>
      </td>
      <td>
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
  <a href="<%:Url.Action("BillingHistoryFormDEAttachmentDownload", new { invoiceId = attachment.Id })%>">
    <%:attachment.OriginalFileName%></a><br />
  <%
                 }
  %>
</div>
<br />
<%
               }
%>