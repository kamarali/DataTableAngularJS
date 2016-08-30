<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<Iata.IS.Model.Pax.RMCouponTax>>" %>
<%@ Import Namespace="Iata.IS.Model.Pax" %>
<form id="formTaxDetails" method="get" action=""> 
<div>
  <% Html.RenderPartial(Url.Content("~/Areas/Pax/Views/BaseFXF/RMCouponTaxDetailsControl.ascx"), new RMCouponTax());%>
</div>
<div class="buttonContainer">
  <input class="primaryButton ignoredirty" type="submit" value="Add" />
</div>
</form>
<h2>Tax Breakdown List</h2>
<div>
  <table id="taxGrid">
  </table>
</div>
<div class="clear">
</div>
<div class="buttonContainer">
  <input class="secondaryButton" type="button" value="Close" onclick="closeTaxDetail();setFocusAndBlur('#TaxAmountDifference')" />
</div>
