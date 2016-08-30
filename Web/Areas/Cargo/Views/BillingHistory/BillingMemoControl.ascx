<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Cargo.CargoBillingMemo>" %>
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
          <%: Model.BillingMemoNumber%>
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
          <%: Convert.ToDecimal(Model.NetBilledAmount).ToString(FormatConstants.TwoDecimalsFormat)%>
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
          <% if(Model.CorrespondenceReferenceNumber !=0 )%>
               <%: Model.CorrespondenceReferenceNumber%>
               
                
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
  <a href="<%:Url.Action("BillingHistoryBMAttachmentDownload", new { invoiceId = attachment.Id })%>">
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
        foreach (CargoBillingMemoAwb awbRecord in Model.AwbBreakdownRecord)
    {
      %>
<div>
  <table class="formattedTable">
    <thead>
      <tr>
        <td rowspan="2">
          Awb Serial Number
        </td>
        <td rowspan="2">
          Issuing Airln.
        </td>
        <td rowspan="2">
          Billing Code
        </td>
        <td rowspan="2">
          From - To
        </td>
        <td rowspan="2">
          Weight Charges
        </td>
        <td rowspan="2">
          Valuation Charges
        </td>
        <td rowspan="2">
          Amt. Subj. to ISC
        </td>
         <td rowspan="2">
          Other Charges
        </td>
        <td colspan="2">
          ISC
        </td>
        <td rowspan="2">
          VAT Amt.
        </td>
        <td rowspan="2">
          Net Amt. Billed
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
        <%:awbRecord.AwbBillingCodeDisplay%>
        </td>
         <td>
        <%:awbRecord.CarriageFromId%> - <%:awbRecord.CarriageToId%> 
        </td>
        <td class="numeric">
        <%: Convert.ToDouble(awbRecord.BilledWeightCharge).ToString(FormatConstants.ThreeDecimalsFormat)%>
        </td>
        <td class="numeric">
        <%: Convert.ToDouble(awbRecord.BilledValuationCharge).ToString(FormatConstants.ThreeDecimalsFormat)%>
        </td>
        <td class="numeric">
        <%:awbRecord.BilledAmtSubToIsc.ToString(FormatConstants.ThreeDecimalsFormat)%>
        </td>
       <td class="numeric">
        <%:awbRecord.BilledOtherCharge.ToString(FormatConstants.ThreeDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: awbRecord.BilledIscPercentage.ToString(FormatConstants.ThreeDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%:awbRecord.BilledIscAmount.ToString(FormatConstants.ThreeDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%: Convert.ToDouble(awbRecord.BilledVatAmount).ToString(FormatConstants.ThreeDecimalsFormat)%>
        </td>
        <td class="numeric">
          <%:awbRecord.TotalAmount.ToString(FormatConstants.ThreeDecimalsFormat)%>
        </td>
        <td>
          <%:awbRecord.ISValidationFlag%>
        </td>
      </tr>
     
    </tbody>
  </table>
</div>

<%
      if (awbRecord.Attachments.Count > 0)
  {
%>
<div>
  <b>Supporting Document(s)</b><br />
  <%
    foreach (var attachment in awbRecord.Attachments)
    {  
  %>
  <a href="<%:Url.Action("BillingHistoryBMAwbAttachmentDownload", new {invoiceId = attachment.Id}) %>">
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
  }
%>

