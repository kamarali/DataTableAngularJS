<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.RMCouponTax>" %>
<h2>
  Tax Details</h2>
<div class="solidBox dataEntry">
  <div class="fieldContainer horizontalFlowFor4FieldsPerLine">
    <div id="divTaxDetails">
      <div>
        <label for="taxCode">
          <span>*</span> Tax Code:</label>
          <%: Html.TextBoxFor(tax => tax.TaxCode, new { @class = "autocComplete upperCase" })%>
          <%: Html.TextBox("TaxId", Model.Id, new { @class = "hidden"})%>
      </div>
      <div>
        <label for="amount">
          <span>*</span> Tax Amount Billed:</label>
        <%: Html.TextBox("CouponTaxAmountBilled", null, new { @class = "pos_amt_12_3 amount" })%>
      </div>
      <div>
        <label for="amountAccepted">
          <span>*</span> Tax Amount Accepted:</label>
        <%: Html.TextBox("CouponTaxAmountAccepted", null, new { @class = "pos_amt_12_3 amount" })%>
      </div>
      <div>
        <label for="amountDifference">
          Tax Amount Difference:</label>
        <%: Html.TextBox("CouponTaxAmountDifference", "", new { @readOnly = true })%>
      </div>
    </div>
  </div>
  <div class="clear">
  </div>
</div>
