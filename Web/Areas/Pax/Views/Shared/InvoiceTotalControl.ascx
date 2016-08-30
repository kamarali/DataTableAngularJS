<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.InvoiceTotal>" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<h2>
  <%:(Model.Invoice.InvoiceType == InvoiceType.CreditNote ? "Credit Note" : "Invoice")%> Total</h2>
<div class="solidBox dataEntry">
   <div class="fieldContainer horizontalFlow">
    <div>
    <div>
        <label for="TotalGrossValue">
          Total Gross Amount:</label>          
        <%: Html.TextBoxFor(m => m.TotalGrossValue, new { @readonly = true, @class = "amount" })%>
      </div>
      <div>
        <label for="TotalISCAmount">
          Total ISC Amount:</label>
          <%: Html.TextBoxFor(m => m.TotalIscAmount, new { @readonly = true, @class = "amount" })%>
      </div>
      <div>
        <label for="TotalOtherCommission">
          Total Other Commission Amount:</label>
        <%: Html.TextBoxFor(m => m.TotalOtherCommission, new { @readonly = true, @class = "amount" })%>
      </div>
      <div>
        <label for="TotalUATPAmount">
          Total UATP Amount:</label>
        <%: Html.TextBoxFor(m => m.TotalUatpAmount, new { @readonly = true, @class = "amount" })%>
      </div>
      <div>
        <label for="TotalHandlingFee">
         Total Handling Fee Amount:</label>
         <%: Html.TextBoxFor(m => m.TotalHandlingFee, new { @readonly = true, @class = "amount" })%>
      </div>
      <div>
        <label for="TotalTaxAmount">
          Total Tax Amount:</label>
          <%: Html.TextBoxFor(m => m.TotalTaxAmount, new { @readonly = true, @class = "amount" })%>
      </div>
      <div>
        <label for="VATAmount">
          Total VAT Amount:</label>
        <%: Html.TextBoxFor(m => m.TotalVatAmount, new { @readonly = true, @class = "amount" })%>
      </div>
      <div>
        <label for="NetTotal">
          Net Total Amount:</label>
          <%: Html.TextBoxFor(m => m.NetTotal, new { @readonly = true, @class = "amount" })%>
      </div>
      <div>
        <label for="NetBillingAmount">
          Net Billing Amount:</label>
          <%: Html.TextBoxFor(m => m.NetBillingAmount, new { @readonly = true, @class = "amount" })%>        
      </div>
      <div>
        <label for="NoOfBillingRecords">
          No. of Billing Records:</label>
        <%: Html.TextBoxFor(model => model.NoOfBillingRecords, new { @readonly = true })%>
      </div>
    </div>
  </div>
  <div class="clear">
  </div>
</div>
