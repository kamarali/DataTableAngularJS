<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.BMCouponTax>" %>
<h2>
  Tax Details</h2>
<div class="solidBox dataEntry">
  <div class="fieldContainer verticalFlow">
    <div id="divTaxDetails" class="oneColumn">
      <div class="twoColumn">
        <label for="taxCode">
          Tax Code:</label>
          <%: Html.TextBoxFor(tax => tax.TaxCode, new { @class = "autocComplete upperCase" })%>
          <%: Html.TextBox("TaxId", Model.Id, new { @class = "hidden"})%>
      </div>
      <div class="twoColumn">
        <label for="amount">
          Amount:</label>
        <%: Html.TextBox("Amount", "", new { id = "Amount", @class = "amt_12_3 amount" })%>
      </div>
    </div>
  </div>
  <div class="clear">
  </div>
</div>
