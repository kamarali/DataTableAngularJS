<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Cargo.AwbOtherCharge>" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<h2>
  Other Charge Details</h2>
<div class="solidBox">
  <div class="fieldContainer horizontalFlowFor4FieldsPerLine" id="divOtherChargeDetails">
    <div id="">
      <div>
        <label for="OtherChargeCode">
          <span>*</span> Other Charge Code:</label>
        <%: Html.TextBoxFor(oc => oc.OtherChargeCode, new { @class = "autocComplete populated" })%>
      </div>
      <div>
        <label for="OtherChargeValue">
          <span>*</span>Other Charge Value:</label>
        <%: Html.TextBox("OtherChargeCodeValue", Model.OtherChargeCodeValue, new { id = "OtherChargeCodeValue", @class = "amount amt_11_3" })%>
      </div>
      <div>
        <label for="VatLabel">
          VAT Label:</label>
        <%: Html.TextBox("OtherChargeVatLabel", Model.OtherChargeVatLabel, new { @class = "popupTextField alphaNumericWithSpace",maxlength=5 })%>
      </div>
      <div>
        <label for="VATText">
          VAT Text:</label>
        <%: Html.TextBox("OtherChargeVatText", Model.OtherChargeVatText, new { @class = "xlargeTextField popupTextField alphaNumericWithSpace",maxlength=50 })%>
      </div>
    </div>
    <div>
      <div>
        <label for="VATBaseAmount">
          VAT Base Amount:</label>
        <%: Html.TextBox("OtherChargeVatBaseAmount", Model.OtherChargeVatBaseAmount, new { id = "OtherChargeVatBaseAmount", @class = "amount amt_11_3" })%>
      </div>
      <div>
        <label for="VATPercentage">
          VAT Percentage:</label>
        <%: Html.TextBox("OtherChargeVatPercentage", Model.OtherChargeVatPercentage, new { id = "OtherChargeVatPercentage", @class = "percent amt_5_3" })%>
      </div>
      <div>
        <label for="VATCalculatedAmount">
          VAT Calculated Amount:</label>
        <%: Html.TextBox("OtherChargeVatCalculatedAmount", "", new { @readonly = "true", id = "OtherChargeVatCalculatedAmount", @class = "amt_11_3" })%>
        <%: Html.TextBox("OtherChargeId", Model.Id, new { @class = "hidden" })%>
      </div>
    </div>
  </div>
  <div class="clear">
  </div>
</div>
