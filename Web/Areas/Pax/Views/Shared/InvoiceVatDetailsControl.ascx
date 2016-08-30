<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.InvoiceVat>" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>

<form id="formVatDetails" method="get" action="">
<h2>
  VAT Breakdown Capture</h2>
<div class="solidBox">
  <div class="fieldContainer horizontalFlowFor4FieldsPerLine" id="divVatDetails">
    <div id="">
      <div>
        <label for="VATIdentifier">
          <span>*</span> VAT Identifier:</label>
        <%: Html.VatIdentifierListDropdownListFor(vat => vat.VatIdentifierId, BillingCategoryType.Pax)%>
      </div>
      <div>
        <label for="VATLabel">
          VAT Label:</label>
        <%: Html.TextBox("VatLabel", Model.VatLabel, new { maxLength = 5 })%>
      </div>
      <div>
        <label for="VATText">
          <span>*</span> VAT Text:</label>
        <%: Html.TextBox("VatText", Model.VatText, new { @class = "xlargeTextField alphaNumericWithSpace" })%>
      </div>
    </div>
    <div>
      <div>
        <label for="VATBaseAmount">
          <span>*</span> VAT Base Amount:</label>
        <%: Html.TextBox("VatBaseAmount", Model.VatBaseAmount, new { @class = "amount" })%>
      </div>
      <div>
        <label for="VATPercentage">
          VAT Percentage:</label>
        <%: Html.TextBox("VatPercentage", Model.VatPercentage, new { @class = "percent amt_5_3" })%>
      </div>
      <div>
        <label for="VATCalculatedAmount">
          VAT Calculated Amount:</label>
        <%: Html.TextBox("VatCalculatedAmount", "", new { @readonly = "true"})%>
        <%: Html.TextBox("InvoiceId", Model.ParentId, new { @class = "hidden populated" })%>
      </div>
    </div>
  </div>
  <div class="clear">
  </div>
</div>
<div class="buttonContainer">
  <input class="primaryButton ignoredirty" type="submit" value="Add" />
</div>
</form>