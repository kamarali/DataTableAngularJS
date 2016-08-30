<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MiscUatp.MiscUatpInvoice>" %>
<%@ Import Namespace="Iata.IS.Model.MiscUatp" %>
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
        PO Number
      </td>
      <td>
        Net Amount
      </td>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td>
        <%:Html.DisplayFor(m => m.DisplayBillingPeriod)%>
      </td>
      <td>
        <%:Html.DisplayFor(m => m.BillingMemberText)%>
      </td>
      <td>
        <%:Html.DisplayFor(m => m.BilledMemberText)%>
      </td>
      <td>
        <%:Html.DisplayFor(m => m.InvoiceNumber)%>
      </td>
      <td>
        <%:Model.InvoiceDate.ToString(FormatConstants.DateFormat)%>
      </td>
      <td>
        <%:Html.DisplayFor(m => m.ChargeCategory.Name)%>
      </td>
      <td>
        <%:Html.DisplayFor(m => m.PONumber)%>
      </td>
      <td>
        <%:Model.ListingCurrencyDisplayText ?? "USD"%> <%:Model.BillingAmount.ToString(FormatConstants.ThreeDecimalsFormat)%>
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
    <tbody>  <%
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
                 foreach (LineItem lineItem in Model.LineItems.OrderBy(lineItem => lineItem.LineItemNumber))
                 {
%>
      <tr>
        <td>
          <%:lineItem.LineItemNumber%>
        </td>
        <td>
          <%:lineItem.ChargeCode.Name%>
        </td>
        <td>
          <%:lineItem.Description%>
        </td>
        <td>
          <%:lineItem.Quantity%>
        </td>
        <td>
          <%:lineItem.UomCodeId%>
        </td>
        <td>
          <%:lineItem.UnitPrice.ToString(FormatConstants.FourDecimalsFormat)%>
        </td>
        <td>
          <%:lineItem.Invoice.ListingCurrencyDisplayText ?? "USD"%>
        </td>
        <td>
          <%:lineItem.ChargeAmount.ToString(FormatConstants.ThreeDecimalsFormat)%>
        </td>
        <td>
          <%:lineItem.TotalTaxAmount.HasValue ? lineItem.TotalTaxAmount.Value.ToString(FormatConstants.ThreeDecimalsFormat) : String.Empty%>
        </td>
        <td>
          <%:lineItem.TotalVatAmount.HasValue ? lineItem.TotalVatAmount.Value.ToString(FormatConstants.ThreeDecimalsFormat) : String.Empty%>
        </td>
        <td>
          <%:lineItem.TotalAddOnChargeAmount.HasValue ? lineItem.TotalAddOnChargeAmount.Value.ToString(FormatConstants.ThreeDecimalsFormat) : String.Empty%>
        </td>
        <td>
          <%:lineItem.TotalNetAmount.ToString(FormatConstants.ThreeDecimalsFormat)%>
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
