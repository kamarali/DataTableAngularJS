<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.BMCouponVat>" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<h2>
  VAT Breakdown Capture</h2>
<div class="solidBox">
  <div class="fieldContainer horizontalFlowFor4FieldsPerLine" id="divVatDetails">
    <div id="">
      <div>
        <label for="VATIdentifier">
          <span>*</span>VAT Identifier:</label>
        <%: Html.VatIdentifierListDropdownListFor(vat => vat.VatIdentifierId, BillingCategoryType.Pax)%>
      </div>
      <div>
        <label for="VATLabel">
          <span>*</span>VAT Label:</label>
        <%: Html.TextBox("VatLabel", Model.VatLabel, new { maxLength = 5, @class = "popupTextField" })%>
      </div>
      <div>
        <label for="VATText">
          <span>*</span>VAT Text:</label>
        <%: Html.TextBox("VatText", Model.VatText, new { @class = "xlargeTextField popupTextField alphaNumericWithSpace" })%>
      </div>
      </div>
      <div>
      <div>
        <label for="VATBaseAmount">
          <span>*</span>VAT Base Amount:</label>
        <%: Html.TextBox("VatBaseAmount", Model.VatBaseAmount, new { id = "VatBaseAmount", @class = "amount" })%>
      </div>
      <div>
        <label for="VATPercentage">
          <span>*</span>VAT Percentage:</label>
        <%: Html.TextBox("VatPercentage", Model.VatPercentage, new { id = "VatPercentage", @class = "percent amt_5_3" })%>
      </div>
      <div>
        <label for="VATCalculatedAmount">
          VAT Calculated Amount:</label>
        <%: Html.TextBox("VatCalculatedAmount", "", new { @readonly = "true", id = "VatCalculatedAmount"})%>
        <%: Html.TextBox("VatId", Model.Id, new { @class = "hidden"})%>
      </div>
    </div>
  </div>
  <div class="clear">
  </div>
</div>

