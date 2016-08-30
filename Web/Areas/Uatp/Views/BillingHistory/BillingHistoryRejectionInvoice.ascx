<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MiscUatp.MiscUatpInvoice>" %>
<%@ Import Namespace="Iata.IS.Model.MiscUatp" %>
<table class="formattedTable">
  <thead>
    <tr>
      <td>
        Billing period
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
        Invoice Date
      </td>
      <td>
        Charge Category
      </td>
      <td>
        Net Reject<br /> Amount
      </td>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td>
        <%: Html.DisplayFor(m => m.DisplayBillingPeriod)%>
      </td>
      <td>
        <%: Html.DisplayFor(m => m.BillingMemberText)%>
      </td>
      <td>
        <%: Html.DisplayFor(m => m.BilledMemberText)%>
      </td>
      <td>
        <%: Html.DisplayFor(m => m.InvoiceNumber)%>
      </td>
      <td>
        <%: Model.InvoiceDate.ToString(FormatConstants.DateFormat) %>
      </td>
      <td>
        <%: Html.DisplayFor(m => m.ChargeCategory.Name)%>
      </td>
      <td>
        <%: Model.ListingCurrencyDisplayText ?? "USD" %> <%: Model.BillingAmount.ToString(FormatConstants.ThreeDecimalsFormat) %>
      </td>
    </tr>
  </tbody>
</table>
<div>
  <table class="formattedTable">
    <thead>
      <tr>
        <td>
          Rejection<br /> Line Item #
        </td>
        <td>
          Original<br /> Line Item #
        </td>
        <td>
          Currency<br /> Code
        </td>
        <td>
          Rejection Line Item<br />Net Amount
        </td>
        <td>
          Original Line Item<br /> Net Amount
        </td>
        <td>
          Charge Code
        </td>
        <td>
          Reason Description
        </td>
      </tr>
    </thead>
    <tbody>
       <%
         if (Model.LineItems.Count == 0)
         {
          %>
          <tr>
            <td colspan="10">
              No line items present.
            </td>
          </tr>
         <%   
         }
         else
         {
           foreach (LineItem lineItem in Model.LineItems.OrderBy(lineitem => lineitem.LineItemNumber))
           {
%>
      <tr>
        <td>
          <%: lineItem.LineItemNumber %>
        </td>
        <td>
          <%: lineItem.OriginalLineItemNumber %>
        </td>
        <td>
          <%: Model.ListingCurrencyDisplayText ?? "USD" %>
        </td>
        <td>
          <%: lineItem.TotalNetAmount.ToString(FormatConstants.ThreeDecimalsFormat) %>
        </td>
        <td>
          <%: lineItem.ChargeAmount.ToString(FormatConstants.ThreeDecimalsFormat) %>
        </td>
        <td>
          <%: lineItem.ChargeCode.Name %>
        </td>
        <td>
          <%: lineItem.Description %>
        </td>
      </tr>
      <%
           }
         }
%>
    </tbody>
  </table>
</div>
<%
  if (Model.Attachments.Count > 0)
  {
%>
<br />
<div>
  <b>Supporting Document(s)</b><br />
  <%
    foreach (var attachment in Model.Attachments)
    {  
  %>
  <a href="<%:Url.Action("BillingHistoryAttachmentDownload", new {invoiceId = attachment.Id}) %>">
    <%:attachment.OriginalFileName%></a><br />
  <%
    }
  %>
</div>
<%
  }
%>
