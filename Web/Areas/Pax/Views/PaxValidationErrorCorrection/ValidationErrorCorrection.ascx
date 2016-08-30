<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Common.ValidationErrorCorrection>" %>
<%@ Import Namespace="System.Security.Policy" %>

<%-- <% using (Html.BeginForm("ValidationErrorCorrection", "PaxValidationErrorCorrection", FormMethod.Post, new { id = "ValidationErrorCorrection" }))
       { %>
 --%>

 <h2>
    Search Criteria
 </h2>
 
 
  <div>
  
 <div class="searchCriteria">
    <div class="solidBox dataEntry">
        <div class="fieldContainer horizontalFlow">
            <div class="">
                <div>
                    <label>
                        <span>*</span> Billing Year/Month/Period:</label>
                     <%:Html.PaxBillingYearMonthDropdown(ControlIdConstants.BillingYearMonthDropDown, Model.BillingYear, Model.BillingMonth, Model.BillingPeriod, htmlAttributes: new { id="NonSamplingBillingYearMonth" })%>
                </div>
                 <%= Html.HiddenFor(validationErrorCorrection => validationErrorCorrection.BillingYear)%>
                <%= Html.HiddenFor(m => m.BillingMonth)%>
                <%= Html.HiddenFor(m => m.BillingPeriod)%>

                <div>
                    <label>
                        Billed Member:</label>
                    <%:Html.TextBoxFor(validationErrorCorrection => validationErrorCorrection.BilledMember, new { @class = "autocComplete" })%>
                    <%= Html.HiddenFor(m => m.BilledMemberId)%>
                </div>
                <div>
                    <label>
                        Invoice Number:</label>
                    <%:Html.TextBoxFor(validationErrorCorrection => validationErrorCorrection.InvoiceNumber, new { maxLength = 10 })%>
                </div>
            </div>
            <div>
                <div>
                    <label>
                        Exception Code:</label>
                    <%:Html.TextBoxFor(validationErrorCorrection => validationErrorCorrection.ExceptionCode, new { @class = "autocComplete" })%>
                     <%= Html.HiddenFor(m => m.ExceptionCodeId)%>
                </div>
                <div>
                    <label>
                        File Submission Date:</label>
                         <%:Html.TextBox(ControlIdConstants.FileSubmissionDate, Model.FileSubmissionDate != null ? Model.FileSubmissionDate.Value.ToString(FormatConstants.DateFormat) : null, new { @class = "datePicker" })%>
                </div>
                <div>
                    <label>
                        File Name:</label>
                    <%:Html.TextBoxFor(validationErrorCorrection => validationErrorCorrection.FileName, new { @id = "NSFileName", maxLength = 255 })%>
                </div>
            </div>
        </div>
        <div class="clear">
        </div>
    </div>
    <div class="buttonContainer">
        <input class="primaryButton" type="button" value="Search" id="btnSearch" onclick="ShowSearchResult()"/>
        <%--  <input class="primaryButton" type="button" value="Add New Template" onclick="javascript:location.href = '<%:Url.Action("NewPermissionTemplate", "Permission")%>';" />
        --%>
        <input class="secondaryButton" type="button" value="Clear" onclick="resetForm();"  id="btnClear"/>
    </div>
</div>

   
  </div>  
   <div id="ExceptionSummaryGrid" style="width: 1102px;">
        <h2>Exception Summary</h2>
       <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.UatpExceptionSummaryGrid]);%>
  </div>
  <div id="ExceptionDetailsGrid"style="width: 1252px;">
        <h2>Exception Details</h2>
       <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.UatpExceptionDetailsGrid]);%>
  </div>

  <div id="InvoiceUpdatePopUp" class="hidden">
	<%
    Html.RenderPartial("~/Views/Pax/ValidationErrorUpdatePopup.ascx", Model);%>
</div>

 <div id="BatchUpdatePopup" class="hidden">
	<%
    Html.RenderPartial("~/Views/Pax/BatchUpdatePopup.ascx", Model);%>
</div>

<div id="CorrectLinkingErrorPopUp" class="hidden">
<%
    Html.RenderPartial("~/Views/Pax/CorrectLinkingErrorPopUp.ascx", Model);%>
</div>

   <br />
<div >
  
  <input class="primaryButton" type="submit" value="Update" onclick="javascript:UpdatePopUpClick()" id="UpdateButton1"  />
  <input class="primaryButton" type="submit" value="Correct Linking Error" onclick="javascript:CorrectLinkingErrorClick()" id="CorrectLinkingErrorButton1" />
  <input class="secondaryButton" type="button" value="Back" style="visibility :hidden"/>
</div>
<%--<% } %>--%>

  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Pax/ValidationErrorCorrection.js")%>"></script>  

    <script type="text/javascript">

        $(function () {
            
         $("#tabs").tabs({
   
      cache: false
      });
      });
        var clearSearchUrl = '<%: Url.Action("ClearSearch", "PaxValidationErrorCorrection") %>';
        $(document).ready(function () {
          $('#SamplingUpdatePopUp').remove();
          $('#BillingYearMonth').focus();
          ExceptionSummaryGridDatasUrl = '<%:Url.Action("ExceptionSummaryGridData","PaxValidationErrorCorrection",new {area = "Pax"}) %>';
          ExceptionDetailsGridDataUrl = '<%:Url.Action("ExceptionDetailsGridData","PaxValidationErrorCorrection",new {area = "Pax"}) %>';
          IsDisplayLinkingButtonUrl = '<%:Url.Action("IsDisplayLinkingButton","PaxValidationErrorCorrection",new {area = "Pax"}) %>';
          UpdateUrl = '<%:Url.Action("UpdateValidationErrors","PaxValidationErrorCorrection",new {area = "Pax"}) %>';
          ValidateErrorUrl = '<%:Url.Action("ValidateError","PaxValidationErrorCorrection",new {area = "Pax"}) %>';
          BatchUpdatedCountUrl = '<%:Url.Action("BatchUpdatedCount","PaxValidationErrorCorrection",new {area = "Pax"}) %>';
          UpdateCorrectLinkingErrorUrl = '<%:Url.Action("UpdateCorrectLinkingError","PaxValidationErrorCorrection",new {area = "Pax"}) %>';
          /*CMP #596: Length of Member Accounting Code to be Increased to 12 
          Desc: The list of Members shown in the auto-complete should exclude Type B Members, Applying New auto-complete logic #MW1.
          Ref: FRS Section 3.4 Table 15 Row 11 */
          registerAutocomplete('BilledMember', 'BilledMemberId', '<%:Url.Action("GetMemberListForPaxCgo", "Data",  new  {  area = "" })%>', 0, true, null);

          registerAutocomplete('ExceptionCode', 'ExceptionCodeId', '<%:Url.Action("GetPaxExceptionCodeList", "Data", new { area = "" })%>', 0, true, null);

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
        });

        
    </script>    

     
