<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<Iata.IS.Model.Cargo.CMAwbOtherCharge>>" %>
<%@ Import Namespace="Iata.IS.Model.Cargo" %>
<form id="formOtherChargDetails" method="get" action="">
<div>
  <% Html.RenderPartial("CMAwbOtherChargeDetailsControl", new CMAwbOtherCharge());%>
</div>
<div class="buttonContainer">
  <input class="primaryButton ignoredirty" type="submit" value="Add" />
</div>
</form>
<h2>
  Other Charge List</h2>
<div>
  <table id="OtherChargeGrid">
  </table>
</div>
<div class="clear">
</div>
<div class="buttonContainer">
  <input class="secondaryButton" type="button" value="Close" onclick="closeOtherChargeDetail(); setFocusAndBlur('#VatAmount')" />
</div>
