<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.ValidationErrorCorrection>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: UATP :: Receivables :: Validation Error Correction
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<h1>Validation Error Correction</h1>
 <h2>
    Search Criteria
 </h2>

  <div>
    <% using (Html.BeginForm("Index", "UatpValidationErrorCorrection", FormMethod.Post, new { id = "frmValidationError" }))
       { %>
    <% Html.RenderPartial("UatpValidationErrorCorrectionSeach", Model); %>
    <% } %>
  </div>  
   <div id="ExceptionSummaryGrid"style="width: 1028px;">
        <h2>Exception Summary</h2>
       <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.UatpExceptionSummaryGrid]);%>
  </div>
  <div id="ExceptionDetailsGrid"style="width: 502px;">
        <h2>Exception Details</h2>
       <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.UatpExceptionDetailsGrid]);%>
  </div>

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
    Html.RenderPartial("~/Views/MiscUatp/CorrectLinkingErrorPopUp.ascx", Model);%>
</div>

   <br />
<div >
  
  <input class="primaryButton" type="submit" value="Update" onclick="javascript:UpdatePopUpClick()" id="UpdateButton1"  />
   <!--SCP456030: PMI Problem -->
  <input class="primaryButton" type="submit" value="Correct Linking Error" onclick="javascript:CorrectLinkingErrorClick()" id="CorrectLinkingErrorButton1" />   

  <input class="secondaryButton" type="button" value="Back" style="visibility :hidden"/>
</div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">

  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Uatp/ValidationErrorCorrection.js")%>"></script>  

    <script type="text/javascript">
      var clearSearchUrl = '<%: Url.Action("ClearSearch", "UatpValidationErrorCorrection") %>';
      $(document).ready(function () {
        $('#BilledMember').focus();
        UpdateValidationErrorUrl = '<%:Url.Action("UpdateValidationErrors","UatpValidationErrorCorrection",new {area = "Uatp"}) %>';
        ExceptionDetailsGridDataUrl = '<%:Url.Action("ExceptionDetailsGridData","UatpValidationErrorCorrection",new {area = "Uatp"}) %>';
        IsDisplayLinkingButtonUrl = '<%:Url.Action("IsDisplayLinkingButton","UatpValidationErrorCorrection",new {area = "Uatp"}) %>';
        ValidateErrorUrl = '<%:Url.Action("ValidateError","UatpValidationErrorCorrection",new {area = "Uatp"}) %>';
        BatchUpdatedCountUrl = '<%:Url.Action("BatchUpdatedCount","UatpValidationErrorCorrection",new {area = "Uatp"}) %>';
        UpdateCorrectLinkingErrorUrl = '<%:Url.Action("UpdateCorrectLinkingError","UatpValidationErrorCorrection",new {area = "Uatp"}) %>';

        registerAutocomplete('BilledMember', 'BilledMemberId', '<%:Url.Action("GetMemberList", "Data",  new  {  area = "" })%>', 0, true, null);

        registerAutocomplete('ExceptionCode', 'ExceptionCodeId', '<%:Url.Action("GetUatpExceptionCodeList", "Data", new { area = "" })%>', 0, true, null);

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

        // When user enters corrected value change it to uppercase and set value of text box, so we can get it in uppercase in action method
        $("#NewValue").keyup(function () {
          var newValue = $("#NewValue").val();
          $("#NewValue").val(newValue.toUpperCase());
        });

        // Set focus on update button when user enters New value
        $("#NewValue").blur(function () { $('#UpdateButton1').focus() });
      });
     
    </script>    

</asp:Content>
