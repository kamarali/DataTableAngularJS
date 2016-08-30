<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MiscUatp.ContactInformation>" %>
<h2>
  Contact Details</h2>
<div class="solidBox dataEntry">
  <div class="fieldContainer horizontalFlowFor4FieldsPerLine" id="AdditionalContactDetails">
    <div>
      <div>
       <label for="BillingContactType">
         <span>*</span> Contact Type:</label>
        <%:Html.ContactTypeDropdownList(ControlIdConstants.BillingContactType, null)%>
        <%: Html.TextBox(ControlIdConstants.BillingContactId, Model.Id, new { @class = "hidden" })%>
      </div>
      <div style="width:auto">
       <label for="BillingContactValue">
        <span>*</span> Contact Value:</label>
        <%: Html.TextBox(ControlIdConstants.BillingContactValue, "", new { maxLength = 255, @class = "xlargeTextField" })%>
      </div>
    </div>
    <div>     
      <label for="BillingContactDescription">
        Description
      </label>
      <%:Html.TextBox(ControlIdConstants.BillingContactDescription, "", new { @class = "descriptionTextField" })%>      
    </div>
  </div>
  <div class="clear">
  </div>
</div>
