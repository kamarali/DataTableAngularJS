<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MiscUatp.ContactInformation>" %>
<h2>
  Contact Details</h2>
<div class="solidBox dataEntry">
  <div class="fieldContainer horizontalFlowFor4FieldsPerLine" id="BilledContactDetails">
    <div>
      <div>
       <label for="BilledContactType">
         <span>*</span> Contact Type:</label>
        <%:Html.ContactTypeDropdownList(ControlIdConstants.BilledContactType, null)%>
        <%: Html.TextBox(ControlIdConstants.BilledContactId, Model.Id, new { @class = "hidden" })%>
      </div>
      <div style="width:auto">
       <label for="BilledContactValue">
        <span>*</span> Contact Value:</label>
        <%: Html.TextBox(ControlIdConstants.BilledContactValue, "", new { @class = "xlargeTextField", maxLength = 255 })%>
      </div>
    </div>
    <div>
      
        <label for="BilledContactDescription">
          Description
        </label>
        <%:Html.TextBox(ControlIdConstants.BilledContactDescription, "", new { @class = "descriptionTextField" })%>
     
    </div>
  </div>
  <div class="clear">
  </div>
</div>
