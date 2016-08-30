<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.ValidationErrorCorrection>" %>
<%@ Import Namespace="System.Security.Policy" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Cargo :: Receivables :: Validation Error Correction
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<h1>Validation Error Correction</h1>
  <h2>
    Search Criteria</h2>
  <div>

    <% using (Html.BeginForm("Index", "ValidationErrorCorrection", FormMethod.Post, new { id = "frmValidationError" }))
       { %>
    <% Html.RenderPartial("ValidationErrorCorrectionSeach", Model); %>
    <% } %>
  </div>
  <div id="ValidationErrorCorrectionGrid" style="width: 1118px;">
        <h2>Exception Summary</h2>
    <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.ValidationErrorCorrectionGrid]);%>
  </div>
    <div id="ExceptionSummaryGrid" style="width: 1108px;">
        <h2>Exception Details</h2>
    <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.ExceptionSummaryGrid]);%>
  </div>
      
            
       
  <%--  </div>

 </div>--%>

 <div id="UpdatePopUp" class="hidden">
	<%
    Html.RenderPartial("~/Views/MiscUatp/ValidationErrorUpdatePopup.ascx", Model);%>
</div>

 <div id="BatchUpdatePopup" class="hidden">
	<%
    Html.RenderPartial("~/Views/MiscUatp/BatchUpdatePopup.ascx", Model);%>
</div>
<div id="CorrectLinkingErrorPopUp" class="hidden">
<%
  Html.RenderPartial("~/Views/Shared/CorrectLinkingErrorPopUp.ascx", Model);%>
</div>

 <br />
<div >
  
  <input class="primaryButton" type="submit" value="Update" onclick="javascript:UpdatePopUpClick()" id="UpdateButton1"  />
   <!--SCP456030: PMI Problem -->
  <input class="primaryButton" type="submit" value="Correct Linking Error" onclick="javascript:CorrectLinkingErrorClick()" id="CorrectLinkingErrorButton1" />  

  <input class="secondaryButton" type="button" value="Back" style="visibility :hidden"/>
</div>

<%--<a class="ignoredirty" href="#" onclick="return searchContactTypes('<%:Url.Action("GetMyGridDataJson", "Member", new { area = "Profile"}) %>','#divContactAssignmentSearchResult','ACH','<%:Url.Action("SaveAllContactAssignment", "Member", new { area = "Profile"}) %>');">
          View/Edit</a>--%>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">

  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Cargo/UpdateValidationErrorCorrection.js")%>"></script>

  <script type="text/javascript">
    $(document).ready(function () {
      $('#BilledMember').focus();
      ExceptionSummaryGridDatasUrl = '<%:Url.Action("ExceptionSummaryGridDatas","ValidationErrorCorrection",new {area = "Cargo"}) %>';
      IsDisplayLinkingButtonUrl = '<%:Url.Action("IsDisplayLinkingButton","ValidationErrorCorrection",new {area = "Cargo"}) %>';
      UpdateUrl = '<%:Url.Action("UpdateValidationErrors","ValidationErrorCorrection",new {area = "Cargo"}) %>';
      ValidateErrorUrl = '<%:Url.Action("ValidateError","ValidationErrorCorrection",new {area = "Cargo"}) %>';
      BatchUpdatedCountUrl = '<%:Url.Action("BatchUpdatedCount","ValidationErrorCorrection",new {area = "Cargo"}) %>';
      UpdateCorrectLinkingErrorUrl = '<%:Url.Action("UpdateCorrectLinkingError","ValidationErrorCorrection",new {area = "Cargo"}) %>';
      var updateform;
      registerAutocomplete('ExceptionCode', 'ExceptionCodeId', '<%:Url.Action("GetCgoExceptionCodeList", "Data", new { area = "" })%>', 0, true, null);
      /*CMP #596: Length of Member Accounting Code to be Increased to 12 
      Desc: The list of Members shown in the auto-complete should exclude Type B Members, Applying New auto-complete logic #MW1.
      Ref: FRS Section 3.4 Table 15 Row 25 */
      registerAutocomplete('BilledMember', 'BilledMemberId', '<%:Url.Action("GetMemberListForPaxCgo", "Data",  new  {  area = "" })%>', 0, true, null);
      $('#CorrectLinkingErrorButton1').attr('disabled', 'disabled');
      $('#UpdateButton1').attr('disabled', 'disabled');
      // Buttons which have disabled attribute are not grayed out in Firefox, so if browser is Firefox remove "primaryButton" class and add "disabledButtonClassForMozilla" class 
      if ($.browser.mozilla) {
        $("#CorrectLinkingErrorButton1").removeClass('primaryButton');
        $("#CorrectLinkingErrorButton1").addClass('disabledButtonClassForMozilla');
        $("#UpdateButton1").removeClass('primaryButton');
        $("#UpdateButton1").addClass('disabledButtonClassForMozilla');
      }
      $('#YourBmCmNo').val("");
      $('#YourBmCmNo').attr('disabled', 'disabled');
      $("#BmCmIndicator option[value='']").remove();
      ValidateUpdatePopup();

      // When user enters corrected value change it to uppercase and set value of text box, so we can get it in uppercase in action method
      $("#NewValue").keyup(function () {
        var newValue = $("#NewValue").val();
        $("#NewValue").val(newValue.toUpperCase());
      });

      // Set focus on update button when user enters New value
      $("#NewValue").blur(function () { $('#UpdateButton1').focus() });

    });
        var clearSearchUrl = '<%: Url.Action("ClearSearch", "ValidationErrorCorrection") %>';
  </script>
</asp:Content>

