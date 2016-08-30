<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.PrimeCouponTax>" %>
<h2>
  Tax Details</h2>
<div class="solidBox dataEntry">
  <div class="fieldContainer horizontalFlow">
    <div>
      <div>
        <label>
          Tax Code</label>
        <%: Html.TextBoxFor(tax => tax.TaxCode, "AutopopulatedTaxCode upperCase")%>
      </div>
      <div>
        <label>
          Amount</label>
        <%: Html.TextBoxFor(tax => tax.Amount, new {@class = "pos_amt_12_3"})%>
      </div>
    </div>
  </div>
  <div class="clear">
  </div>
</div>
