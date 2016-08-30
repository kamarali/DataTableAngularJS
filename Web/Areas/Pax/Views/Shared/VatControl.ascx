<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.PrimeCouponVat>" %>
<div>
  <% Html.RenderPartial(Url.Content("VatCaptureControl"), Model); %>
</div>
<div>
  <h2>
    VAT List</h2>    

</div>
<div class="buttonContainer">
  <input class="primaryButton" type="button" value="Save" />
  <input class="secondaryButton" type="button" value="Close" onclick="$('#divVat').dialog('close');" />
</div>
