<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Reports.CorrespondenceStatus.CorrespondenceStatusUIModel>" %>
<h2>
  Search Criteria</h2>
  <script type="text/javascript">
      $(document).ready(function () {
          
          if ('<%:ViewData["Category"]%>' != 3 ) {
              $('#chargeCategorydiv').hide();
          }

          $('#Fromdate').val('<%=ViewData["CurrentDate"] %>');
          $('#ToDate').val('<%=ViewData["CurrentDate"] %>');
          $('#CorrespondenceSubStatusId').focus();          
      });
  </script>
<div class="solidBox">
  <div class="fieldContainer horizontalFlow">
    <div>
      <div style="width:180px;">
        <label for="CorrespondenceNumber">
           Correspondence Ref. No.:</label>
        <%:Html.TextBoxFor(model => model.CorrespondenceNumber, new { @class="numeric", maxLength = 11 })%>
      </div>
      <div style="width:180px;">
        <label>
          <span style="color:Red">*</span>From Date:</label>
         <%: Html.TextBoxFor(model => model.Fromdate, new {  @class = "datePicker", @readOnly = true })%>
      </div>
      <div style="width:180px;">
        <label>
          <span style="color:Red">*</span>To Date:</label>
         <%: Html.TextBoxFor(model => model.ToDate, new {  @class = "datePicker", @readOnly = true })%>
      </div>
      <div style="width:180px;">
        <label>
          Corr. Initiating Member:</label>
         <%: Html.CorrespondenceInitiatingMemberDropdownList(ControlIdConstants.InitiatingMember,1)%>
      </div>
      <div style="width:180px;">
        <label> Member Code:</label>
             <%:Html.TextBoxFor(model => model.MemberCode, new { @class = "autocComplete textboxWidth" })%>
        <%:Html.TextBoxFor(model => model.MemberId, new { @class = "hidden" })%>
      </div>
      
   </div>
   
   <div>
     <div id="chargeCategorydiv" style="width:180px;">
        <label for="ChargeCategory"> Charge Category:</label>
        <%--CMP609: MISC Changes Required as per ISW2. Added new parameter 'isIncludeInactive'. If it is true then method will return the all charge category for misc category including in-active.--%>
        <%:Html.ChargeCategoryDropdownListFor(model => model.ChargeCategory, isIncludeInactive: true)%>
    </div>
    <div style="width:180px;">
       <label for="CorrespondenceStatus"><span></span> Correspondence Status:</label>
        <%: Html.CorrespondenceStatusropdownList(ControlIdConstants.CorrespondenceStatusId,-1)%>
      </div>
       <div style="width:220px;">
       <label for="CorrespondenceSubStatus">Correspondence Sub Status:</label>
        <%: Html.CorrespondenceSubStatusDropdownList(ControlIdConstants.CorrespondenceSubStatusId,-1,"200")%>
      </div>
      <div style="width:180px;">
        <label>Correspondence Stage > =</label>
            <%:Html.TextBoxFor(model => model.Corrstage, new { @class = "numeric", maxLength = 3 })%>
      </div>
      <div style="width:180px;">
        <label for="NumberOfDaysToExpiry">Expiring In (no of days): </label>
        <%:Html.TextBoxFor(model => model.Expiryindays, new { @class = "numeric", maxLength = 3 })%>
      </div>
    <div style="width:160px;">
        <label>Show only Authority to bill cases: </label>
        <%:Html.CheckBoxFor(model => model.IsAuthorityToBillCase) %>
    </div>   
   
  </div>
  <div>
    <%--CMP526 - Passenger Correspondence Identifiable by Source Code--%>
    <%if((int)ViewData["Category"] == (int)Iata.IS.Model.Enums.BillingCategoryType.Pax)
       {
       %>
       <div>
           <label>Source Code: </label>
           <%:Html.TextBoxFor(model => model.SourceCode, new { @class = "autocComplete" })%>
       </div>
    <%
       }%>
  </div>
   </div>
  <div class="clear">
  </div>
</div>
