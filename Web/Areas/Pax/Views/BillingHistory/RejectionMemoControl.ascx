<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.RejectionMemo>" %>
<%@ Import Namespace="Iata.IS.Model.Pax" %>
<div>
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
          Invoice No.
        </td>
        <td>
          Listing Currency
        </td>
        <td>
          Billing Code
        </td>
        <td>
          Memo No.
        </td>
        <td>
          Batch Seq. No.
        </td>
        <td>
          Rec. Seq. Batch
        </td>
        <td>
          Source Code
        </td>
        <td>
          Reason Code
        </td>
        <td>
          IS-Rejection Flag
        </td>
        <td>
          IS-Validation Flag
        </td>
        <td>
          Net Reject Amount
        </td>
        <td>
          Sampling Constant
        </td>
        <td>
          Net Reject Amt After Sampling Constant
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
          <%: Model.Invoice.BilledMemberText%>
        </td>
        <td>
          <%: Model.Invoice.InvoiceNumber%>
        </td>
        <td>
          <%: Model.Invoice.ListingCurrencyDisplayText%>
        </td>
        <td class="numeric">
          <%: Model.Invoice.DisplayBillingCode%>
        </td>
        <td>
          <%: Model.RejectionMemoNumber%>
        </td>
        <td class="numeric">
          <%: Model.BatchSequenceNumber%>
        </td>
        <td class="numeric">
          <%: Model.RecordSequenceWithinBatch%>
        </td>
        <td class="numeric">
          <%: Model.SourceCodeId%>
        </td>
        <td>
          <%: Model.ReasonCode%>
        </td>
        <td>
          <%: Model.IsRejectionFlag%>
        </td>
        <td>
          <%: Model.ISValidationFlag%>
        </td>
        <td class="numeric">
          <%: Model.TotalNetRejectAmount.ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: Model.SamplingConstant%>
        </td>
        <td class="numeric">
          <%: Model.TotalNetRejectAmountAfterSamplingConstant%>
        </td>
      </tr>
    </tbody>
  </table>
</div>
<div>
  <table class="formattedTable">
    <thead>
      <tr>
        <td>
          Your Invoice Billing Period
        </td>
        <td>
          Your Invoice No.
        </td>
        <td>
          Your Rejection Memo No.
        </td>
        <td>
          FIM/BM/CM No.
        </td>
        <td>
          FIM Coupon No.
        </td>
        <td>
          FIM/BM/CM Indc.
        </td>
      </tr>
    </thead>
    <tbody>
      <tr>
        <td>         
      <%: string.Format("{0} {1} P{2}",((Model.YourInvoiceBillingMonth < 1 || Model.YourInvoiceBillingMonth > 13) ?   Model.YourInvoiceBillingMonth.ToString() : System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(Model.YourInvoiceBillingMonth)),Model.YourInvoiceBillingYear,
                Model.YourInvoiceBillingPeriod)%>
        </td>
        <td>
          <%: Model.YourInvoiceNumber%>
        </td>
        <td>
          <%: Model.YourRejectionNumber%>
        </td>
        <td>
          <%: Model.FimBMCMNumber%>
        </td>
        <td>
          <%: Model.FimCouponNumber%>
        </td>
        <td>
          <%: Model.FIMBMCMIndicatorId == 2 ? "FIM" : Model.FIMBMCMIndicatorId == 3 ? "BM" : Model.FIMBMCMIndicatorId == 4 ? "CM" :""%>
        </td>
      </tr>
    </tbody>
  </table>
</div>

<%
  if(!string.IsNullOrEmpty(Model.ReasonRemarks))
  {%>
    <div>
      <b>Remarks</b>: 
    </div>
    <br />    
  <%} %>

<%
  if(!string.IsNullOrEmpty(Model.ReasonRemarks))
  {
    char[] array = Model.ReasonRemarks.ToArray();
    int cnt = 0;
    while (true)
    {
      var str = string.Join("", array.Skip(cnt).Take(80).ToArray());
      if (string.IsNullOrEmpty(str))
      {
        break;
      }
      else
      {
        cnt = cnt + 80; %>
        <% if (cnt <= array.Length){ %>
          <%: str%> <br />
          <%} else{%>
  <%: str %> <br />
<%} %>
      <% 
      }
    }
    %>
  <%}
%>
<br />
<%
  if (Model.Attachments.Count > 0)
  {
%>
<div>
  <b>Supporting Document(s)</b><br />
  <%
                 foreach (var attachment in Model.Attachments)
                 {%>
  <a href="<%:Url.Action("BillingHistoryRMAttachmentDownload", new { invoiceId = attachment.Id })%>">
    <%:attachment.OriginalFileName%></a><br />
  <%
                 }
  %>
</div>
<br />
<%
               }
%>

<%
  foreach (RMCoupon coupon in Model.CouponBreakdownRecord)
  {
%>
<div>
  <b>Coupon Breakdown</b>
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
          From-To
        </td>
        <td rowspan="2">
          Listing Curr.
        </td>
        <td rowspan="2">
          &nbsp;
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
          Net Reject Amt.
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
        <td rowspan="3" style="width: 80px;">
          <%: coupon.TicketDocOrFimNumber%>
          Coupon
          <%: coupon.TicketOrFimCouponNumber%>
        </td>
        <td rowspan="3">
          <%: coupon.TicketOrFimIssuingAirline%>
        </td>
        <td rowspan="3">
          <%: coupon.FromToAirport%>
        </td>
        <td rowspan="3">
          <%: Model.Invoice.ListingCurrencyDisplayText%>
        </td>
        <td>
          Billed
        </td>
        <td class="numeric">
          <%: coupon.GrossAmountBilled.ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: coupon.AllowedIscPercentage.ToString(FormatConstants.ThreeDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: coupon.AllowedIscAmount.ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: coupon.AllowedOtherCommissionPercentage.ToString(FormatConstants.ThreeDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: coupon.AllowedOtherCommission.ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: coupon.AllowedUatpPercentage.ToString(FormatConstants.ThreeDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: coupon.AllowedUatpAmount.ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: coupon.AllowedHandlingFee.ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: coupon.TaxAmountBilled.ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: coupon.VatAmountBilled.ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td rowspan="2">
          &nbsp;
        </td>
        <td rowspan="3">
          <%: coupon.OriginalPmi%>
        </td>
        <td rowspan="3">
          <%: coupon.ValidatedPmi%>
        </td>
        <td rowspan="3">
          <%: coupon.AgreementIndicatorSupplied%>
        </td>
        <td rowspan="3">
          <%: coupon.AgreementIndicatorValidated%>
        </td>
      </tr>
      <tr>
        <td>
          Accepted
        </td>
        <td class="numeric">
          <%: coupon.GrossAmountAccepted.ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: coupon.AcceptedIscPercentage.ToString(FormatConstants.ThreeDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: coupon.AcceptedIscAmount.ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: coupon.AcceptedOtherCommissionPercentage.ToString(FormatConstants.ThreeDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: coupon.AcceptedOtherCommission.ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: coupon.AcceptedUatpPercentage.ToString(FormatConstants.ThreeDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: coupon.AcceptedUatpAmount.ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: coupon.AcceptedHandlingFee.ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: coupon.TaxAmountAccepted.ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: coupon.VatAmountAccepted.ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
      </tr>
      <tr>
        <td>
          Difference
        </td>
        <td class="numeric">
          <%: coupon.GrossAmountDifference.ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td>
          &nbsp;
        </td>
        <td class="numeric">
          <%: coupon.IscDifference.ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td>
          &nbsp;
        </td>
        <td class="numeric">
          <%: coupon.OtherCommissionDifference.ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td>
          &nbsp;
        </td>
        <td class="numeric">
          <%: coupon.UatpDifference.ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: coupon.HandlingDifference.ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: coupon.TaxAmountDifference.ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: coupon.VatAmountDifference.ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: coupon.NetRejectAmount.ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
      </tr>
    </tbody>
  </table>
</div>
<%
          if (coupon.Attachments.Count > 0)
          {
%>
<div>
  <b>Supporting Document(s)</b><br />
  <%
                 foreach (var attachment in coupon.Attachments)
                 {%>
  <a href="<%:Url.Action("BillingHistoryRMCouponAttachmentDownload", new { invoiceId = attachment.Id })%>">
    <%:attachment.OriginalFileName%></a><br />
  <%
                 }
  %>
</div>
<br />
<%
               }
        }
%>
