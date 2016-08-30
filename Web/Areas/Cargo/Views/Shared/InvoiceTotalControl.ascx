<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Cargo.CargoInvoiceTotal>" %>
<h2>
 <%-- <%:(Model.Invoice.InvoiceType == InvoiceType.CreditNote ? "Credit Note" : "Invoice")%>--%>Invoice Total</h2>
<div class="solidBox dataEntry">
   <div class="fieldContainer horizontalFlow">
    <div>
    <div>
        <label for="TotalWeightCharge">
          Total Weight Charge:</label>          
        <%: Html.TextBoxFor(m => m.TotalWeightCharge, new { @readonly = true, @class = "amount" })%>
      </div>
      <div>
        <label for="TotalValuationCharge">
          Total Valuation Charge:</label>
          <%: Html.TextBoxFor(m => m.TotalValuationCharge, new { @readonly = true, @class = "amount" })%>
      </div>
      <div>
        <label for="TotalOtherCharge">
          Total Other Charge:</label>
        <%: Html.TextBoxFor(m => m.TotalOtherCharge, new { @readonly = true, @class = "amount" })%>
      </div>
      <div>
        <label for="TotalIscAmount">
          Total ISC Amount:</label>
        <%: Html.TextBoxFor(m => m.TotalIscAmount, new { @readonly = true, @class = "amount" })%>
      </div>
      </div>
      <div>
      
      <div>
        <label for="NetInvoiceTotal">
          Net Invoice Total:</label>
          <%: Html.TextBoxFor(m => m.NetTotal, new { @readonly = true, @class = "amount" })%>
      </div>
      <div>
        <label for="TotalVatAmount">
         Total VAT Amount:</label>
         <%: Html.TextBoxFor(m => m.TotalVatAmount, new { @readonly = true, @class = "amount" })%>
      </div>
      <%--<div>
        <label for="TotalNetAmountWithoutVat">
          Total Net Amount Without VAT:</label>
        <%: Html.TextBoxFor(m => m.TotalNetAmountWithoutVat, new { @readonly = true, @class = "amount" })%>
      </div>--%>
      <div>
        <label for="NetInvoiceBillingTotal">
          Net Invoice Billing Total:</label>
          <%: Html.TextBoxFor(m => m.NetBillingAmount, new { @readonly = true, @class = "amount" })%>
      </div>
      <%--<div>
        <label for="NetBillingAmount">
          Total No. Of Records:</label>
          <%: Html.TextBoxFor(m => m.TotalNoOfRecords, new { @readonly = true, @class = "amount" })%>        
      </div>--%>
      <div>
        <label for="NoOfBillingRecords">
          Total Number of Records:</label>
        <%: Html.TextBoxFor(model => model.NoOfBillingRecords, new { @readonly = true })%>
      </div>
    </div>
  </div>
  <div class="clear">
  </div>
</div>
