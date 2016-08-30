﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<Iata.IS.Model.Pax.BillingMemoVat>>" %>
<%@ Import Namespace="Iata.IS.Model.Pax" %>
<form id="formVatDetails" method="get" action=""> 
<div>
  <% Html.RenderPartial("BMVatDetailsControl", new BillingMemoVat());%>
</div>
<div class="buttonContainer">
  <input class="primaryButton ignoredirty" type="submit" value="Add" />
</div>
</form>
<h2>VAT List</h2>
<div>
  <table id="vatGrid">
  </table>
</div>
<div class="clear">
</div>
<div class="buttonContainer">
  <input class="secondaryButton" type="button" value="Close" onclick="closeVatDetail(); setFocusAndBlur('#VatAmount')" />
</div>
