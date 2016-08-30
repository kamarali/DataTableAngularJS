<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.RMCouponVat>" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>

<h2>
  VAT Breakdown Capture</h2>

<div class="solidBox">
  <div class="fieldContainer horizontalFlowFor4FieldsPerLine" id="divVatDetails">
    <div id="">
      <div>
        <label for="VatIdentifierId">
          <span>*</span> VAT Identifier:</label>
        <%: Html.VatIdentifierListDropdownListFor(vat => vat.VatIdentifierId, BillingCategoryType.Pax) %>
      </div>
      <div>
        <label for="VatLabel">
          <span>*</span> VAT Label:</label>
        <%: Html.TextBox("VatLabel", Model.VatLabel, new { maxLength = 5, @class = "popupTextField" })%>
      </div>
      <div>
        <label for="VatText">
          <span>*</span> VAT Text:</label>
        <%: Html.TextBox("VatText", Model.VatText, new { @class = "xlargeTextField popupTextField alphaNumericWithSpace" })%>
      </div>
      </div>
      <div>
      <div>
        <label for="VatBaseAmount">
          <span>*</span> VAT Base Amount:</label>
        <%: Html.TextBox("VatBaseAmount", Model.VatBaseAmount, new { @class = "amount" }) %>
      </div>
      <div>
        <label for="VatPercentage">
          <span>*</span> VAT Percentage:</label>
        <%: Html.TextBox("VatPercentage", Model.VatPercentage, new { @class = "percent amt_5_3" })%>
      </div>
      <div>
        <label for="VatCalculatedAmount">
          VAT Calculated Amount:</label>
        <%: Html.TextBox("VatCalculatedAmount", "", new { @readonly = "true"})%>
        <%: Html.TextBox("VatId", Model.Id, new { @class = "hidden"})%>
      </div>
    </div>
  </div>
  <div class="clear">
  </div>
</div>
