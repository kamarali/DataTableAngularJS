<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Cargo.RMAwbOtherCharge>" %>
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
        <label for="OtherChargeCodeValue">
          <span>*</span> Other Charge Value:</label>
        <%: Html.TextBox("OtherChargeCodeValue", Model.OtherChargeCodeValue, new { @class = "popupTextField amt_11_3 amount" })%>
      </div>
      <div>
        <label for="VatLabel">
          VAT Label:</label>
        <%: Html.TextBox("OtherChargeVatLabel", Model.OtherChargeVatLabel, new { maxLength = 5, @class = "xlargeTextField popupTextField alphaNumericWithSpace" })%>
      </div>
      <div>
        <label for="OtherChargeVatText">
          VAT Text:</label>
        <%: Html.TextBox("OtherChargeVatText", Model.OtherChargeVatText, new { maxLength = 50, @class = "xlargeTextField popupTextField alphaNumericWithSpace" })%>
      </div>
      </div>
      <div>
      <div>
        <label for="OtherChargeVatBaseAmount">
          VAT Base Amount:</label>
        <%: Html.TextBox("OtherChargeVatBaseAmount", Model.OtherChargeVatBaseAmount, new { id = "OtherChargeVatBaseAmount", @class = "amount amt_11_3" })%>
      </div>
      <div>
        <label for="OtherChargeVatPercentage">
          VAT Percentage:</label>
        <%: Html.TextBox("OtherChargeVatPercentage", Model.OtherChargeVatPercentage, new { id = "OtherChargeVatPercentage", @class = "percent" })%>
      </div>
      <div>
        <label for="OtherChargeVatCalculatedAmount">
          VAT Calculated Amount:</label>
        <%: Html.TextBox("OtherChargeVatCalculatedAmount", "", new { @readonly = "true", id = "OtherChargeVatCalculatedAmount" })%>
        <%: Html.TextBox("OtherChargeId", Model.Id, new { @class = "hidden" })%>
      </div>
    </div>
  </div>
  <div class="clear">
  </div>
</div>