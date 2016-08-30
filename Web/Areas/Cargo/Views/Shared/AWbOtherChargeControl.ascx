<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<Iata.IS.Model.Cargo.AwbOtherCharge>>" %>
<%@ Import Namespace="Iata.IS.Model.Cargo" %>
<form id="formOtherChargDetails" method="get" action="">
<div>
  <% Html.RenderPartial("AwbOtherChargeDetailsControl", new AwbOtherCharge());%>
</div>
<div class="buttonContainer">
  <input class="primaryButton ignoredirty" type="submit" value="Add"/>
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
