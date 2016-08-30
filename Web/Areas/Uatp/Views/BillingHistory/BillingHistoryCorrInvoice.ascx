<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MiscUatp.MiscUatpInvoice>" %>
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
        Invoice Date
      </td>
      <td>
        Charge Category
      </td>
      <td>
        Correspondence<br /> Ref. No.
      </td>
      <td>
        Net Amount
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
        <%: Model.CorrespondenceRefNo.HasValue? Model.CorrespondenceRefNo.Value.ToString(FormatConstants.CorrespondenceNumberFormat): string.Empty %>
      </td>
      <td>
        <%: Model.ListingCurrencyDisplayText ?? "USD"%>
        <%: Html.DisplayFor(m => m.BillingAmount)%>
      </td>
    </tr>
  </tbody>
</table>
<div>
  <table class="formattedTable">
    <thead>
      <tr>
        <td>
          Line Item #
        </td>
        <td>
          Charge Code
        </td>
        <td>
          Description
        </td>
        <td>
          Quantity
        </td>
        <td>
          UOM Code
        </td>
        <td>
          Unit Price
        </td>
        <td>
          Currency<br /> Code
        </td>
        <td>
          Gross Amount
        </td>
        <td>
          Tax
        </td>
        <td>
          VAT
        </td>
        <td>
          Add/Deduct<br /> Charge
        </td>
        <td>
          Net Amount
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
        foreach (var lineItem in Model.LineItems.OrderBy(lineitem => lineitem.LineItemNumber))
         {
      %>
      <tr>
        <td>
          <%: lineItem.LineItemNumber %>
        </td>
        <td>
          <%: lineItem.ChargeCode.Name %>
        </td>
        <td>
          <%: lineItem.Description %>
        </td>
        <td>
          <%: lineItem.Quantity %>
        </td>
        <td>
          <%: lineItem.UomCodeNameDisplayText %>
        </td>
        <td>
          <%: lineItem.UnitPrice.ToString(FormatConstants.FourDecimalsFormat) %>
        </td>
        <td>
          <%: Model.ListingCurrencyDisplayText ?? "USD" %>
        </td>
        <td>
          <%: lineItem.ChargeAmount.ToString(FormatConstants.ThreeDecimalsFormat) %>
        </td>
        <td>
          <%: Convert.ToDecimal(lineItem.TotalTaxAmount).ToString(FormatConstants.ThreeDecimalsFormat) %>
        </td>
        <td>
          <%: Convert.ToDecimal(lineItem.TotalVatAmount).ToString(FormatConstants.ThreeDecimalsFormat) %>
        </td>
        <td>
          <%: Convert.ToDecimal(lineItem.TotalAddOnChargeAmount).ToString(FormatConstants.ThreeDecimalsFormat) %>
        </td>
        <td>
          <%: lineItem.TotalNetAmount.ToString(FormatConstants.ThreeDecimalsFormat) %>
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
