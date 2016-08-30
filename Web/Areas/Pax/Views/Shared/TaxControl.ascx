<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.PrimeCouponTax>" %>
<div>
  <% Html.RenderPartial(Url.Content("TaxCaptureControl"), Model);%>
</div>
<div class="buttonContainer">
  <input class="primaryButton" type="button" value="Add" />
</div>
<div>
Tax List Control...
</div>
<div class="buttonContainer">
  <input class="primaryButton" type="button" value="Save" />
  <input class="secondaryButton" type="button" value="Close" onclick="$('#divTax').dialog('close');" />
</div>
