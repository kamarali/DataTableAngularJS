<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<List<Iata.IS.Model.Cargo.BMAwbVat>>" %>
<%@ Import Namespace="Iata.IS.Model.Cargo" %>
<form id="formVatDetails" method="get" action=""> 
<div>
  <% Html.RenderPartial("BMAwbVatDetailsControl", new BMAwbVat());%>
</div>
<div class="buttonContainer">
  <input class="primaryButton ignoredirty" type="submit" value="Add" />
</div>
</form>
<div class="solidBox hidden" id="otherChargeVatDiv">
  <div class="fieldContainer horizontalFlowFor4FieldsPerLine">
    <div>
      <div>
        <div>
          VAT Amount From Other Charge:
          <%:Html.TextBox("otherchargevatHidden","", new { @readOnly = true })%>
        </div>
      </div>
    </div>
  </div>
  <div class="clear"></div>
</div>
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
