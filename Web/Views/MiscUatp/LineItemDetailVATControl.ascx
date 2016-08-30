<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<Iata.IS.Model.MiscUatp.LineItemDetailTax>>" %>
<%@ Import Namespace="Iata.IS.Model.MiscUatp" %>

<form id="formVatDetails" method="get" action=""> 
<div>
  <% Html.RenderPartial("~/Views/MiscUatp/LineItemDetailVATDetailsControl.ascx", new LineItemDetailTax());%>
</div>
<div class="buttonContainer">
  <input class="primaryButton ignoredirty" type="submit" value="Add" />
</div>
</form>
<h2>VAT Breakdown List</h2>
<div>
  <table id="vatGrid">
  </table>
</div>
<div class="clear">
</div>
<div class="buttonContainer">
  <input class="secondaryButton" type="button" value="Close" onclick="closeVatDetail();" />
</div>