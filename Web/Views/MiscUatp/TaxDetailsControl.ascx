<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MiscUatp.MiscUatpInvoiceTax>" %>

<h2>
  Tax Breakdown Capture</h2>
<div class="solidBox dataEntry">
  <div class="fieldContainer horizontalFlow" id="TaxDetails">
    <div>
      <div>
        <label for="SubTypeId"><span>*</span> Tax Name:</label>
        <%--CMP #534: Tax Issues in MISC and UATP Invoices. [Start]--%>
        <%--To display TaxSubType drop down when Billing category is Misc i.e if ViewData["isTaxNameDropdown"] contains True value which is filled from Controller--%>
        <%if (ViewData["isTaxNameDropdown"] == "True")
		      {%>
			    <%:Html.TaxSubTypeDropdownList(ControlIdConstants.TAXSubType, null)%>
		    <%}
		      else
		      {%>
			    <%:Html.TextBox(ControlIdConstants.TAXSubType, "", new { @class = "alphaNumeric", @maxLength = 25 })%>
		    <%}%>
        <%--CMP #534: Tax Issues in MISC and UATP Invoices. [End]--%>
      </div>
      <div>
        <label for="TaxAmount">
          Tax Base Amount:</label>
        <%: Html.TextBox(ControlIdConstants.TaxBaseAmount, "", new  { @class = "num_18_3 amount" })%>
        <%: Html.TextBox(ControlIdConstants.TaxId, Model.Id, new { @class = "hidden" })%>
      </div>
      <div>
        <label for="TaxPercentage">
          Tax Percent:</label>
        <%: Html.TextBox(ControlIdConstants.TaxPercentage, "", new { @class = "percent amt_5_3", min = -99.999, max = 99.999 })%>
      </div>
      <div>
        <label for="CalculatedAmount">
          Tax Calculated Amount:</label>
        <%: Html.TextBox(ControlIdConstants.CalculatedAmount, "", new { @class = "num_18_3 amount" })%>
      </div>
      <div>
        <label for="CategoryCode">
          Tax Category:</label>
        <%:Html.TaxCategoryDropdownList(ControlIdConstants.CategoryCode, null)%>
      </div>
    </div>
    <div>
      <div>
        <label for="Description">
          Tax Text:</label>
          <%--CMP464: Increase TaxDescription Length--%>
        <%: Html.TextArea(ControlIdConstants.TaxDescription, "", new { @rows = 3, @cols = 90})%>
      </div>
    </div>
  </div>
  <div class="clear">
  </div>
</div>
