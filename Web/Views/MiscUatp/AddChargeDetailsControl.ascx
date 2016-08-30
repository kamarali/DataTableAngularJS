<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MiscUatp.InvoiceAddOnCharge>" %>
<h2>
  Add/Deduct Charge Capture</h2>
<div class="solidBox dataEntry">
  <div class="fieldContainer horizontalFlowFor4FieldsPerLine" id="AddChargeDetails">
    <div>
      <div>
        <label for="Name">
          <span>*</span> Add/Deduct Charge Name:</label>
        <%: Html.TextBoxFor(addOnCharge=>addOnCharge.Name, new { maxLength=30 })%>
        <%: Html.TextBoxFor(addOnCharge => addOnCharge.Id, new { @class = "hidden", id = "AddChargeId" })%>
      </div>
      <div>
        <label for="ChargeableAmount">
          Base Amount:</label>
        <%: Html.TextBox(ControlIdConstants.ChargeableAmount, "", new { @class = "num_18_3 amount" })%>
      </div>
      <div>
        <label for="Percentage">
          Percent:</label>
        <%: Html.TextBox(ControlIdConstants.AddChargePercentage, "",new { @class = "num_5_2 percent" })%>
      </div>
      <div>
        <label for="Amount">
          <span>*</span> Calculated Amount:</label>
        <%: Html.TextBox(ControlIdConstants.AddChargeAmount, "", new { @class = "num_18_3 amount" })%>
      </div>
    </div>
    <div>
      <div>
        <label for="ChargeForLineItemNumber">
          Line Item Numbers:</label>
        <%: Html.TextBoxFor(addOnCharge=>addOnCharge.ChargeForLineItemNumber)%>
      </div>
    </div>
</div>
<div class="clear">
</div>
</div> 