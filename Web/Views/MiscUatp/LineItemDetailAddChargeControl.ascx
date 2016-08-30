<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<Iata.IS.Model.MiscUatp.LineItemDetailAddOnCharge>>" %>
<%@ Import Namespace="Iata.IS.Model.MiscUatp" %>
<form id="formAddChargeDetails" method="get" action=""> 
<div>
  <% Html.RenderPartial("~/Views/MiscUatp/LineItemDetailAddChargeDetailsControl.ascx", new LineItemDetailAddOnCharge());%>
</div>
<div class="buttonContainer">
  <input class="primaryButton ignoredirty" type="submit" value="Add" />
</div>
</form>
<h2>Add/Deduct Charge List</h2>
<div>
  <table id="AddChargeGrid">
  </table>
</div>
<div class="clear">
</div>
<div class="buttonContainer">
  <input class="secondaryButton" type="button" value="Close" onclick="closeAddChargeDetail();" />
</div>
