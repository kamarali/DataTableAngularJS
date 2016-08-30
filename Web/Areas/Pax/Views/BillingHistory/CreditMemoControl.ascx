<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.CreditMemo>" %>
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
          Invoice Number
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
          Source Code
        </td>
        <td>
          Reason Code
        </td>
        <td>
          IS - Validation Flag
        </td>
        <td>
          Net Billed Amount
        </td>
      </tr>
    </thead>
    <tbody>
      <tr>
        <td>
          <%: Html.DisplayFor(m => m.Invoice.DisplayBillingPeriod)%>
        </td>
        <td>
          <%: Html.DisplayFor(m => m.Invoice.BillingMemberText)%>
        </td>
        <td>
          <%: Html.DisplayFor(m => m.Invoice.BilledMemberText)%>
        </td>
        <td>
          <%: Html.DisplayFor(m => m.Invoice.InvoiceNumber)%>
        </td>
        <td>
          <%: Model.Invoice.DisplayBillingCode %>
        </td>
        <td>
          <%: Model.CreditMemoNumber%>
        </td>
        <td>
          <%: Model.BatchSequenceNumber%>
        </td>
        <td>
          <%: Model.SourceCodeId%>
        </td>
        <td>
          <%: Model.ReasonCode%>
        </td>
        <td>
          <%: Model.ISValidationFlag%>
        </td>
        <td>
          <%: Model.Invoice.ListingCurrencyDisplayText ?? "USD"%>
          <%: Model.NetAmountCredited.ToString(FormatConstants.TwoDecimalsFormat)%>
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
          Correspondence Ref. No.
        </td>
        <td>
          FIM No./Billing Memo No.
        </td>
        <td>
          FIM Coupon No.
        </td>
      </tr>
    </thead>
    <tbody>
      <tr>
        <td>
          <%  
            if (Model.YourInvoiceBillingMonth != 0)
            {
          %>
          <%:string.Format("{0} {1} P{2}",
                                            System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(Model.YourInvoiceBillingMonth),
                                            Model.YourInvoiceBillingYear,
                                            Model.YourInvoiceBillingPeriod)%>
          <%
          }
          %>
        </td>
        <td>
          <%: Model.YourInvoiceNumber%>
        </td>
        <td>
          <% if (Model.CorrespondenceRefNumber != 0) %>  <%: Model.CorrespondenceRefNumber %>

          
        </td>
        <td>
          <%: Model.CreditMemoNumber%>
        </td>
        <td>
          <%: Model.FimCouponNumber%>
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
<br />
<div>
  <b>Supporting Document(s)</b><br />
  <%
                 foreach (var attachment in Model.Attachments)
                 {%>
  <a href="<%:Url.Action("BillingHistoryCMAttachmentDownload", new { invoiceId = attachment.Id })%>">
    <%:attachment.OriginalFileName%></a><br />
  <%
                 }
%>
</div>
<%
               }
%>


      <%
    foreach (CMCoupon couponRecord in Model.CouponBreakdownRecord)
    {
      %>

<div>
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
          From - To
        </td>
        <td rowspan="2">
          Listing Curr.
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
          Net Amt. Billed
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
        <td style="width: 80px;">
          <%:couponRecord.TicketDocOrFimNumber%>
          Coupon
          <%=Html.Encode(couponRecord.TicketOrFimCouponNumber)%>
        </td>
        <td>
          <%:couponRecord.TicketOrFimIssuingAirline%>
        </td>
        <td>
          <%:couponRecord.FromToAirport%>
        </td>
        <td> 
        <%: Model.Invoice.ListingCurrencyDisplayText %>
        </td>
        <td class="numeric">
          <%:couponRecord.GrossAmountCredited.ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%:couponRecord.IscPercent.ToString(FormatConstants.ThreeDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%:couponRecord.IscAmountBilled.ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%:couponRecord.OtherCommissionPercent.ToString(FormatConstants.ThreeDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%:couponRecord.OtherCommissionBilled.ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%:couponRecord.UatpPercent.ToString(FormatConstants.ThreeDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%:couponRecord.UatpAmountBilled.ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%:couponRecord.HandlingFeeAmount.ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%:couponRecord.TaxAmount.ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%:couponRecord.VatAmount.ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%:couponRecord.NetAmountCredited.ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td>
          <%:couponRecord.OriginalPmi%>
        </td>
        <td>
          <%:couponRecord.ValidatedPmi%>
        </td>
        <td>
          <%:couponRecord.AgreementIndicatorSupplied%>
        </td>
        <td>
          <%:couponRecord.ISValidationFlag%>
        </td>
      </tr>
       </tbody>
  </table>
</div>

<%
               if (couponRecord.Attachments.Count > 0)
               {
%>
<br />
<div>
  <b>Supporting Document(s)</b><br />
  <%
                 foreach (var attachment in couponRecord.Attachments)
                 {%>
  <a href="<%:Url.Action("BillingHistoryCMCouponAttachmentDownload", new { invoiceId = attachment.Id })%>">
    <%:attachment.OriginalFileName%></a><br />
  <%
                 }
%>
</div>
<%
               }
%>

      <%
    }
      %>
   

