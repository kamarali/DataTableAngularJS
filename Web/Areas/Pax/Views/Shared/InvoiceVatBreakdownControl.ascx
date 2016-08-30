<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.InvoiceVat>" %>
<div class="solidBox dataEntry">
  <div class="fieldContainer horizontalFlow">
    <div>
      <div>
        <label for="VATIdentifier">
          <span>*</span>VAT Identifier:</label>
       <%: Html.VatIdentifierDropdownListFor(vat => vat.VatIdentifierId) %>
      </div>
      <div>
        <label for="VATLabel">
          <span>*</span>VAT Label:</label>
        <%: Html.TextBoxFor(vat => vat.VatLabel) %>
      </div>
      <div>
        <label for="VATText">
          <span>*</span>VAT Text:</label>
        <%: Html.TextBoxFor(vat => vat.VatText, new { @class = "largeTextField alphaNumericWithSpace" })%>
      </div>
      <div>
        <label for="VATBaseAmount">
          <span>*</span>VAT Base Amount:</label>
        <%: Html.TextBoxFor(vat => vat.VatBaseAmount) %>
      </div>
      <div>
        <label for="VATPercentage">
          <span>*</span>VAT %:</label>
        <%: Html.TextBoxFor(vat => vat.VatPercentage) %>
      </div>
      <div>
        <label for="VATCalculatedAmount">
          VAT Calculated Amount:</label>
        <%: Html.TextBoxFor(vat => vat.VatCalculatedAmount, new { @readonly = "true"})%>
      </div>
    </div>
  </div>
  <div class="clear">
  </div>
</div>
<div class="buttonContainer">
  <input class="primaryButton" type="button" value="Add" />
  <input class="secondaryButton" type="button" value="Back" onclick="history.back();" />
</div>
<div class="clear">
</div>
