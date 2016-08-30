<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.PrimeCoupon>" %>
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
    </tr>
  </tbody>
</table>
<table class="formattedTable">
    <thead>
      <tr>
        <td rowspan="2">
          Doc. / Cpn No.
        </td>
        <td rowspan="2">
          Issuing Airln.
        </td>
        <td rowspan="2">
          Batch Seq. No.
        </td>
        <td rowspan="2">
          Record Seq. No. Within Batch
        </td>
        <td rowspan="2">
          From-To
        </td>
        <td rowspan="2">
          Listing Currency
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
          Handling Fee Amt.
        </td>
        <td rowspan="2">
          Tax Amt.
        </td>
        <td rowspan="2">
          VAT Amt.
        </td>
        <td rowspan="2">
          Total Amt.
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
        <td style="width: 50px;">
          %
        </td>
        <td style="width: 50px;">
          Amt.
        </td>
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
        <td style="width: 80px;">
         <%: Model.TicketDocOrFimNumber%> Coupon <%: Model.TicketOrFimCouponNumber%>
        </td>
        <td>
          <%: Model.TicketOrFimIssuingAirline%>
        </td>
        <td class="numeric">
          <%: Model.BatchSequenceNumber%>
        </td>
        <td class="numeric">
          <%: Model.RecordSequenceWithinBatch%>
        </td>
        <td>
          <%: Model.FromToAirport%>
        </td>
        <td>
          <%: Model.Invoice.ListingCurrencyDisplayText %>
        </td>
        <td>
          <%: Model.CouponGrossValueOrApplicableLocalFare.ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: Model.IscPercent.ToString(FormatConstants.ThreeDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: Model.IscAmount.ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: Model.OtherCommissionPercent.ToString(FormatConstants.ThreeDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: Model.OtherCommissionAmount.ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: Model.UatpPercent.ToString(FormatConstants.ThreeDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: Model.UatpAmount.ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: Model.HandlingFeeAmount.ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: Model.TaxAmount.ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: Model.VatAmount.ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: Model.CouponTotalAmount.ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: Model.OriginalPmi%>
        </td>
        <td>
          <%: Model.ValidatedPmi%>
        </td>
        <td>
          <%: Model.AgreementIndicatorSupplied%>
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