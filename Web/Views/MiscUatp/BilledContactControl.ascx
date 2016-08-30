<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<Iata.IS.Model.MiscUatp.ContactInformation>>" %>
<%@ Import Namespace="Iata.IS.Model.MiscUatp" %>
<form id="formBilledContactDetails" method="get" action=""> 
<div>
  <% Html.RenderPartial("~/Views/MiscUatp/BilledContactDetailsControl.ascx", new ContactInformation());%>
</div>
<div class="buttonContainer">
  <input class="primaryButton ignoredirty" type="submit" value="Add" id="AddBilledContactButton" />
</div>
</form>
<h2>Contact Details List</h2>
<div>
  <table id="billedContactGrid">
  </table>
</div>
<div class="clear">
</div>
<div class="buttonContainer">
  <input class="secondaryButton" type="button" value="Close" onclick="closeBilledContactDetail()" />
</div>