<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Cargo.CargoRejectionMemo>" %>
<%@ Import Namespace="Iata.IS.Model.Cargo" %>
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
          Memo No.
        </td>
        <td>
          Batch Seq. No.
        </td>
        <td>
          Rec. Seq. Batch
        </td>
        <td>
          Reason Code
        </td>
        <td>
          IS-Validation Flag
        </td>
        <td>
          Net Reject Amount
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
        <td>
          <%: Model.RejectionMemoNumber%>
        </td>
        <td class="numeric">
          <%: Model.BatchSequenceNumber%>
        </td>
        <td class="numeric">
          <%: Model.RecordSequenceWithinBatch%>
        </td>
        <td>
          <%: Model.ReasonCode%>
        </td>
        <td>
          <%: Model.ISValidationFlag%>
        </td>
        <td class="numeric">
          <%: Convert.ToDecimal(Model.TotalNetRejectAmount).ToString(FormatConstants.TwoDecimalsFormat)%>
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
          BM/CM No.
        </td>
        <td>
          BM/CM Indc.
        </td>
      </tr>
    </thead>
    <tbody>
      <tr>
        <td>
          <%:string.Format("{0} {1} P{2}", System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(Model.YourInvoiceBillingMonth), Model.YourInvoiceBillingYear, Model.YourInvoiceBillingPeriod)%>
        </td>
        <td>
          <%: Model.YourInvoiceNumber%>
        </td>
        <td>
          <%: Model.YourRejectionNumber%>
        </td>
        <td>
          <%: Model.YourBillingMemoNumber%>
        </td>
        <td>
          <%: Model.BMCMIndicatorId == 2 ? "BM" : Model.BMCMIndicatorId == 3 ? "CM" : ""%>
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
  foreach (RMAwb coupon in Model.CouponBreakdownRecord)
  {
%>
<div>
  <%--SCP220888: Audit trail on IS-WEB [Replace Coupon to AWB]--%>
  <b>AWB Breakdown</b>
  <table class="formattedTable">
    <thead>
      <tr>
        <td rowspan="2">
          Awb Breakdown Serial No. and Check Digit
        </td>
        <td rowspan="2">
          Awb Issuing Airln.
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
        <td colspan="2">
          ISC
        </td>
        <td rowspan="2">
          VAT Amt.
        </td>
        <td rowspan="2">
          Weight Charge Amt.
        </td>
        <td rowspan="2">
          Valuation Charge Amt.
        </td>
        <td rowspan="2">
          Other Charge Amt.
        </td>
        <td rowspan="2">
          Net Reject Amt.
        </td>
        <td rowspan="2">
          Part Shipment Indicator
        </td>
        <td rowspan="2">
          KgLb Indicator
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
        <td rowspan="3">
          <%: coupon.AwbSerialNumberCheckDigit%>
        </td>
        <td rowspan="3">
          <%: coupon.AwbIssueingAirline%>
        </td>
        <td rowspan="3">
          <%: coupon.CarriageFromId%> - <%: coupon.CarriageToId %>
        </td>
        <td rowspan="3">
          <%: Model.Invoice.ListingCurrencyDisplayText%>
        </td>
        <td>
          Billed
        </td>
        <td class="numeric">
          <%: coupon.AllowedIscPercentage.ToString(FormatConstants.ThreeDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: coupon.AllowedIscAmount.ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: Convert.ToDouble(coupon.BilledVatAmount).ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: Convert.ToDouble(coupon.BilledWeightCharge).ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: Convert.ToDouble(coupon.BilledValuationCharge).ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: coupon.BilledOtherCharge.ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td rowspan="2">
          &nbsp;
        </td>
        <td rowspan="3">
          <%: coupon.PartShipmentIndicator%>
        </td>
        <td rowspan="3">
          <%: coupon.KgLbIndicator%>
        </td>
      </tr>
      <tr>
        <td>
          Accepted
        </td>
        <td class="numeric">
          <%: coupon.AcceptedIscPercentage.ToString(FormatConstants.ThreeDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: coupon.AcceptedIscAmount.ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: Convert.ToDouble(coupon.AcceptedVatAmount).ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: Convert.ToDouble(coupon.AcceptedWeightCharge).ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: Convert.ToDouble(coupon.AcceptedValuationCharge).ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: coupon.AcceptedOtherCharge.ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
      </tr>
      <tr>
        <td>
          Difference
        </td>
        <td>
          &nbsp;
        </td>
        <td class="numeric">
          <%: coupon.IscAmountDifference.ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: Convert.ToDouble(coupon.VatAmountDifference).ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: Convert.ToDouble(coupon.WeightChargeDiff).ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: Convert.ToDouble(coupon.ValuationChargeDiff).ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: coupon.OtherChargeDiff.ToString(FormatConstants.TwoDecimalsFormat)%>
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
