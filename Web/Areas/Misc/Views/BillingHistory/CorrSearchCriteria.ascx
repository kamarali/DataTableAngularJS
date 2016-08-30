<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Common.CorrespondenceSearchCriteria>" %>
<h2>
  Correspondence Search Criteria</h2>
<div class="solidBox dataEntry">
  <div class="fieldContainer horizontalFlow">
    <div>
      <div>
        <label for="FromDate">
          <span class="required">*</span> From Date:</label>
          
          <%:Html.TextBox(ControlIdConstants.CorrespondenceFromDate, Model.FromDate != null ? Model.FromDate.Value.ToString(FormatConstants.DateFormat):null, new { @class = "datePicker"})%>          
      </div>
      <div>
        <label for="ToDate">
          <span class="required">*</span> To Date:</label>
          <%:Html.TextBox(ControlIdConstants.CorrespondenceToDate, Model.ToDate != null ? Model.ToDate.Value.ToString(FormatConstants.DateFormat):null, new { @class = "datePicker"})%>          
      </div>
      <div>
        <label for="MemberCode">
          Member Code:</label>
          <!-- CMP #596: Length of Member Accounting Code to be Increased to 12 
           Desc: Increasing field size by specifying in-line width
           Ref: 3.5 Table 19 Row 6 -->          
          <%:Html.TextBoxFor(model => model.CorrBilledMemberText, new { @class = "autocComplete textboxWidth" })%>
          <%:Html.TextBoxFor(model => model.CorrBilledMemberId, new {@class="hidden"}) %>
      </div>
      <div>
        <label for="CorrespondenceOwner">
          Correspondence Owner:</label>          
          <%: Html.CorrespondenceOwnerDropdownList(ControlIdConstants.CorrespondenceOwnerId,  Model.CorrespondenceOwnerId, SessionUtil.MemberId) %>          
      </div>      
      <div>
        <label for="CorrespondenceInitiator">
          Correspondence Initiating Member:</label>          
          <%: Html.CorrespondenceInitiatingMemberDropdownList(ControlIdConstants.InitiatingMember, Model.InitiatingMember?? 0) %>          
      </div>
    </div>
    <div>      
      <div>
        <label for="CorrespondenceNumber">
           Correspondence Ref. No.:</label>
        <%:Html.TextBoxFor(model => model.CorrespondenceNumber, new { @class="numeric", maxLength = 11 })%>
      </div>
      <div>
       <label for="CorrespondenceStatus"><span>*</span> Correspondence Status:</label>
        <%: Html.CorrespondenceStatusropdownList(ControlIdConstants.CorrespondenceStatusId, Model.CorrespondenceStatusId) %>
      </div>
       <div>
       <label for="CorrespondenceSubStatus">Correspondence Sub Status:</label>
        <%: Html.CorrespondenceSubStatusDropdownList(ControlIdConstants.CorrespondenceSubStatusId, Model.CorrespondenceSubStatusId)%>
      </div>
      <div>
        <label for="AuthorityToBill">Authority To Bill:</label>
        <%:Html.CheckBoxFor(model => model.AuthorityToBill)%>        
      </div>
      <div>
        <label for="NumberOfDaysToExpiry">Number Of Days To Expiry:</label>
        <%:Html.TextBoxFor(model => model.NoOfDaysToExpiry, new { @class = "numeric", maxLength = 3 })%>
      </div>
    </div>
  </div>
  <div class="clear">
  </div>
</div>

