<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MiscUatp.LineItemTax>" %>

<h2>
  VAT Breakdown Capture</h2>
<div class="solidBox dataEntry">
  <div class="fieldContainer horizontalFlow" id="VatDetails">
    <div>
      <div>
        <label for="SubTypeId">
          <span>*</span> VAT SubType:</label>
          <%:Html.VatSubTypeDropdownList(ControlIdConstants.VATSubType, null) %>
      </div>
      <div>
        <label for="Amount">
          <span>*</span> VAT Base Amount:</label>
        <%: Html.TextBox(ControlIdConstants.VATBaseAmount, "", new { @class = "num_18_3 amount" })%>
      </div>
      <div>
        <label for="Percentage">
          VAT Percent:</label>
        <%: Html.TextBox(ControlIdConstants.VATPercent, "", new { @class = "percent amt_5_3" })%>
      </div>
      <div>
        <label for="CalculatedAmount">
          VAT Calculated Amount:</label>
        <%: Html.TextBox(ControlIdConstants.VATCalculatedAmount, "", new { @class = "num_18_3 amount", @readonly = "true" }) %>
      </div>
      <div>
        <label for="CategoryId" id ="categoryCodeLabel">
          VAT Category:</label>
        <%:Html.TaxCategoryDropdownList(ControlIdConstants.VATCategoryCode, null)%>
      </div>
    </div>
    <div>
      <div>
        <label for="Description">
          Description
        </label>
        <%--CMP:464--%>
        <%:Html.TextArea(ControlIdConstants.VATDescription, Model.Description, new { @rows = 3, @cols = 90 })%>
        <%: Html.TextBox(ControlIdConstants.VATId, Model.Id, new { @class = "hidden"})%>
      </div>
    </div>
  </div>
  <div class="clear">
  </div>
</div>

