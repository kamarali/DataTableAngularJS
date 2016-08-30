<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MiscUatp.LineItemDetail>" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<%@ Import Namespace="Iata.IS.Web.Util" %>
<div>
  Line Item Detail #:<b>
    <% =Html.TextBox("DetailNumber", Model.DetailNumber, new { @readOnly = true, @class = "populated" })%>
  </b>
</div>
<div>
  <div>
    <label>
      Service Start Date:</label>
    <%: Html.TextBox(ControlIdConstants.ServiceStartDay, Model.StartDate.HasValue ? Model.StartDate.Value.Day.ToString() : string.Empty, new { @class = "vSmallTextField digits", maxLength = "2", min = 1, max = 31 })%>
    <%: Html.ServiceDateMonthYearDropdownList(ControlIdConstants.ServiceStartDateDropdown, Model.StartDate.HasValue ? Model.StartDate.Value.Month : 0, Model.StartDate.HasValue ? Model.StartDate.Value.Year : 0)%>
    <%: Html.Hidden("LineItemServiceStartDate", Model.LineItem != null && Model.LineItem.StartDate.HasValue ? Model.LineItem.StartDate.Value.ToString(FormatConstants.HiddenDateFormat) : string.Empty)%>
  </div>
  <div>
    <label>
      <span>*</span> Service End Date:</label>
    <%: Html.TextBox(ControlIdConstants.ServiceEndDay, Model.EndDate.Day, new { @class = "vSmallTextField digits populated", maxLength = "2", min = 1, max = 31 })%>
    <%: Html.ServiceDateMonthYearDropdownList(ControlIdConstants.ServiceEndDateDropdown, Model.EndDate.Month, Model.EndDate.Year)%>
    <%: Html.Hidden("LineItemServiceEndDate", Model.LineItem != null ? Model.LineItem.EndDate.ToString(FormatConstants.HiddenDateFormat) : string.Empty)%>
  </div>
  <div>
    <label>
      Product ID:</label>
    <%-- CMP # 533: RAM A13 New Validations and New Charge Code [Start]--%>
    <%-- Display Product Id control as Dropdown for Billing Category = Misc and Charge Category = Service Provider and Charge Code = GDS--%>
		<%if (Model.LineItem.Invoice.BillingCategory == BillingCategoryType.Misc && ViewData[ViewDataConstants.IsProductIdDropDown] == "True")
      {%>
        <%: Html.ProductIdDropdownListFor(lineItemDetail => lineItemDetail.ProductId)%>
		<%}else{%>
          <%: Html.TextBoxFor(lineItemDetail => lineItemDetail.ProductId, new { @class = "populated", maxLength = "25" })%>
		<%}%> 
    <%-- CMP # 533: RAM A13 New Validations and New Charge Code [End]--%>
    <%: Html.Hidden("LineItemProductId", Model.LineItem != null ? Model.LineItem.ProductId : string.Empty)%>
  </div>
</div>
<div>
  <div>
    <label>
      <span>*</span> Description:</label>
      <%-- SCP82887: Line Items Details Discrepancy%>
   <%-- <%if (Model.LineItem != null && !string.IsNullOrEmpty(Model.LineItem.Description))
      { %>
    <%:Html.TextArea(ControlIdConstants.Description, Model.LineItem.Description, 4, 80, new { })%>
    <% }
      else
      { %>--%>
    <%:Html.TextAreaFor(lineItemDetail => lineItemDetail.Description, 4, 80, new {})%>
   <%-- <% } %>--%>
  </div>
</div>
<div>
  <div>
    <label for="MinimumQuantityFlag">
      Minimum Quantity Flag:</label>
    <%:Html.CheckBoxFor(lineItemDetail => lineItemDetail.MinimumQuantityFlag)%>
  </div>
  <div>
    <label>
      <span>*</span> Quantity:</label>
    <%: Html.TextBoxFor(lineItemDetail => lineItemDetail.Quantity, new { @class = "pos_num_18_4", watermark = ControlIdConstants.FourDecimalPlaces, roundTo = 4 })%>
  </div>
  <div>
    <label>
      <span>*</span> UOM Code:</label>
    <%: Html.UomCodeDropdown(ControlIdConstants.UomCodeDropdown, Model.UomCodeId)%>
  </div>
  <div>
    <label>
      <span>*</span> Unit Price:</label>
    <%:Html.TextBox("UnitPrice", Model.UnitPrice, new { @class = "amountTextfield num_18_4", watermark = ControlIdConstants.FourDecimalPlaces, roundTo = 4 })%>
  </div>
  <div>
    <label>
      Scaling Factor:</label>
      <%if (Model.LineItem.Invoice.BillingCategory == BillingCategoryType.Misc)
        {%>
    <%:Html.TextBoxFor(lineItemDetail => lineItemDetail.ScalingFactor, new {@class = "numeric populated", maxLength = 5, min = 1})%>
    <%
        }else
        {%>
        <%:Html.TextBoxFor(lineItemDetail => lineItemDetail.ScalingFactor, new {@class = "numeric populated", maxLength = 5, min = 1, @readOnly = true})%>
        <%
        }%>
  </div>
</div>
<div>
  <div>
    <label for="Total">
      Line Detail Total:</label>
    <%:Html.TextBoxFor(lineItemDetail => lineItemDetail.ChargeAmount, new { @readonly = true, watermark = ControlIdConstants.PositiveAmount, @class = "num_18_3 amount"})%>
  </div>
  <div>
    <%:ScriptHelper.GenerateDialogueHtml("Tax Amount:", "Tax capture", "TaxBreakdown", 500, 900)%><br />
    <%:Html.TextBox(ControlIdConstants.TotalTaxAmount, Model.TotalTaxAmount, new { @class = "amountTextfield num_18_3 amount", @readOnly = true, @id = ControlIdConstants.TaxAmount })%>
  </div>
  <div>
    <%:ScriptHelper.GenerateDialogueHtml("VAT Amount:", "VAT Capture", "VATBreakdown", 500, 900)%><br />
    <%:Html.TextBox(ControlIdConstants.TotalVatAmount, Model.TotalVatAmount, new { @class = "amountTextfield num_18_3 amount", @readOnly = true, @id = ControlIdConstants.VatAmount })%>
  </div>
  <div>
    <%:ScriptHelper.GenerateDialogueHtml("Add/Deduct Charge:", "Add/Deduct Charge Capture", "AddChargeBreakdown", 500, 900)%><br />
    <%:Html.TextBox(ControlIdConstants.LineDetailTotalAddOnChargeAmount, Model.TotalAddOnChargeAmount, new { @class = "amountTextfield num_18_3 amount", @readOnly = true, @id = "TotalAddChargeAmount" })%>
  </div>
  <div>
    <label for="LineDetailNetTotal">
      Line Detail Net Total:</label>
    <%:Html.TextBox(ControlIdConstants.LineDetailNetTotal, Model.TotalNetAmount, new { @readOnly = true, @class = "num_18_3 amount" })%>
  </div>

</div>
