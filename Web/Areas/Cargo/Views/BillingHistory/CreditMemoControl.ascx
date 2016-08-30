<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Cargo.CargoCreditMemo>" %>
<%@ Import Namespace="Iata.IS.Model.Pax" %>
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
          Invoice Number
        </td>
        <td>
          Memo No.
        </td>
        <td>
          Batch Seq. No.
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
          <%: Model.CreditMemoNumber%>
        </td>
        <td>
          <%: Model.BatchSequenceNumber%>
        </td>
        <td>
          <%: Model.ReasonCode%>
        </td>
        <td>
          <%: Model.ISValidationFlag%>
        </td>
        <td>
          <%: Model.Invoice.ListingCurrencyDisplayText ?? "USD"%>
          <%: Convert.ToDecimal(Model.NetAmountCredited).ToString(FormatConstants.TwoDecimalsFormat)%>
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
    foreach (CMAirWayBill awbRecord in Model.AWBBreakdownRecord)
    {
      %>

<div>
  <table class="formattedTable">
    <thead>
      <tr>
        <td rowspan="2">
          Awb Serial No.
        </td>
        <td rowspan="2">
          Issuing Airln.
        </td>
        <td rowspan="2">
          Listing Curr.
        </td>
        <td colspan="2">
          ISC
        </td>
        <td rowspan="2">
          VAT Amt.
        </td>
        <td rowspan="2">
          Total Amt.
        </td>
      </tr>
      <tr>
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
          <%:awbRecord.AwbSerialNumber%>
        </td>
        <td>
          <%:awbRecord.AwbIssueingAirline%>
        </td>
        <td> 
        <%: Model.Invoice.ListingCurrencyDisplayText %>
        </td>
        <td class="numeric">
          <%:awbRecord.CreditedIscPercentage.ToString(FormatConstants.ThreeDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%:awbRecord.CreditedIscAmount.ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: Convert.ToDecimal(awbRecord.CreditedVatAmount).ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%:awbRecord.TotalAmountCredited.ToString(FormatConstants.TwoDecimalsFormat)%>
        </td>
      </tr>
       </tbody>
  </table>
</div>

<%
      if (awbRecord.Attachments.Count > 0)
               {
%>
<br />
<div>
  <b>Supporting Document(s)</b><br />
  <%
                 foreach (var attachment in awbRecord.Attachments)
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
   

