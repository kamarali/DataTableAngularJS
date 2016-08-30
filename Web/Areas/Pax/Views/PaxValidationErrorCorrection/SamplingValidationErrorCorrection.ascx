<%--<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.ValidationErrorCorrection>" %>--%>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Common.ValidationErrorCorrection>" %>
<%@ Import Namespace="System.Security.Policy" %>
 <% using (Html.BeginForm("SamplingValidationErrorCorrection", "PaxValidationErrorCorrection", FormMethod.Post, new { id = "SamplingValidationErrorCorrection" }))
       { %>
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
                        <span>*</span> Provisional Billing Year/Month:</label>
                     <%:Html.PaxSamplingBillingYearMonthDropdown(ControlIdConstants.BillingYearMonthDropDown, Model.BillingYear, Model.BillingMonth)%>
                </div>
                 <%= Html.HiddenFor(m => m.BillingYear)%>
                <%= Html.HiddenFor(m => m.BillingMonth)%>

                <div>
                    <label>
                        Provisional Billing Member:</label>
                    <%:Html.TextBoxFor(validationErrorCorrection => validationErrorCorrection.BilledMember, new { @class = "autocComplete", id="provBillingMember" })%>
                    <%= Html.HiddenFor(m => m.BilledMemberId)%>
                    <%= Html.HiddenFor(m => m.BillingMemberId)%>
                </div>
                <div>
                    <label>
                        Exception Code:</label>
                    <%:Html.TextBoxFor(validationErrorCorrection => validationErrorCorrection.ExceptionCode, new { @class = "autocComplete", id="paxExceptionCode"   })%>
                     <%= Html.HiddenFor(m => m.ExceptionCodeId)%>
                </div>
            </div>
            <div>
                <div>
                    <label>
                        File Submission Date:</label>
                         <%:Html.TextBox(ControlIdConstants.FileSubmissionDate, Model.FileSubmissionDate != null ? Model.FileSubmissionDate.Value.ToString(FormatConstants.DateFormat) : null, new { @class = "datePicker" ,id="paxFileSubmissionDate" })%>
                </div>
                <div>
                    <label>
                        File Name:</label>
                    <%:Html.TextBoxFor(validationErrorCorrection => validationErrorCorrection.FileName, new { maxLength = 255 })%>
                </div>               
            </div>
        </div>
        <div class="clear">
        </div>
    </div>
    <div class="buttonContainer">
     <input class="primaryButton" type="button" value="Search" id="btnSearch" onclick="javascript:ShowSearchResultPax()"/>
        <%--  <input class="primaryButton" type="button" value="Add New Template" onclick="javascript:location.href = '<%:Url.Action("NewPermissionTemplate", "Permission")%>';" />
        --%>
        <input class="secondaryButton" type="button" value="Clear" onclick="SamplingresetForm();" />
    </div>
</div>
   
  </div>  
   <div id="ExceptionSummaryGrid" style="width: 1102px;">
        <h2>Exception Summary</h2>
       <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.SamplingExceptionSummaryGrid]);%>
  </div>
  <div id="ExceptionDetailsGrid"style="width: 1252px;">
        <h2>Exception Details</h2>
       <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.SamplingExceptionDetailsGrid]);%>
  </div>

  <div id="SamplingUpdatePopUp" class="hidden">
	<%
           Html.RenderPartial("~/Views/Pax/SamplingValidationErrorUpdatePopup.ascx", Model);%>
</div>

 <div id="BatchUpdatePopup" class="hidden">
	<%
    Html.RenderPartial("~/Views/Pax/SamplingBatchUpdatePopup.ascx", Model);%>
</div>

<div id="SamplingCorrectLinkingErrorPopUp" class="hidden">
<%
    Html.RenderPartial("~/Views/Pax/SamplingCorrectLinkingErrorPopUp.ascx", Model);%>
</div>

   <br />
<div >
  
  <input class="primaryButton" type="submit" value="Update" onclick="javascript:SamplingUpdatePopUpClick()" id="SamplingUpdateButton1"  />
  <input class="primaryButton" type="submit" value="Correct Linking Error" onclick="javascript:SamplingCorrectLinkingErrorClick()" id="SamplingCorrectLinkingErrorButton1"/>
  <input class="secondaryButton" type="button" value="Back" style="visibility :hidden"/>
</div>
<% } %>

  <script type="text/javascript" src="<%:Url.Content("~/Scripts/Pax/SamplingValidationErrorCorrection.js")%>"></script>  
  <!--Make changes in javascript -->

    <script type="text/javascript">
        $(function () {
            
            $("#tabs").tabs({

                cache: false
            });
        });
        var clearSearchUrl = '<%: Url.Action("ClearSearch", "PaxValidationErrorCorrection") %>';
        $(document).ready(function () {


          $('#InvoiceUpdatePopUp').remove();
          ExceptionSummaryGridDatasUrl = '<%:Url.Action("SamplingExceptionSummaryGridData","PaxValidationErrorCorrection",new {area = "Pax"}) %>';
          ExceptionDetailsGridDataUrl = '<%:Url.Action("SamplingExceptionDetailsGridData","PaxValidationErrorCorrection",new {area = "Pax"}) %>';
          IsDisplayLinkingButtonUrl = '<%:Url.Action("IsDisplayLinkingButton","PaxValidationErrorCorrection",new {area = "Pax"}) %>';
          UpdateUrl = '<%:Url.Action("UpdateValidationErrors","PaxValidationErrorCorrection",new {area = "Pax"}) %>';
          ValidateErrorUrl = '<%:Url.Action("ValidateError","PaxValidationErrorCorrection",new {area = "Pax"}) %>';
          BatchUpdatedCountUrl = '<%:Url.Action("BatchUpdatedCount","PaxValidationErrorCorrection",new {area = "Pax"}) %>';
          UpdateCorrectLinkingErrorUrl = '<%:Url.Action("SamplingUpdateCorrectLinkingError","PaxValidationErrorCorrection",new {area = "Pax"}) %>';
          /*CMP #596: Length of Member Accounting Code to be Increased to 12 
          Desc: The list of Members shown in the auto-complete should exclude Type B Members, Applying New auto-complete logic #MW1.
          Ref: FRS Section 3.4 Table 15 Row 12 */
          registerAutocomplete('provBillingMember', 'BilledMemberId', '<%:Url.Action("GetMemberListForPaxCgo", "Data",  new  {  area = "" })%>', 0, true, null);

          registerAutocomplete('paxExceptionCode', 'ExceptionCodeId', '<%:Url.Action("GetPaxExceptionCodeList", "Data", new { area = "" })%>', 0, true, null);


          $('#SamplingCorrectLinkingErrorButton1').attr('disabled', 'disabled');
          $('#SamplingUpdateButton1').attr('disabled', 'disabled');
          if ($.browser.mozilla) {
            $("#SamplingCorrectLinkingErrorButton1").removeClass('primaryButton');
            $("#SamplingCorrectLinkingErrorButton1").addClass('disabledButtonClassForMozilla');
            $("#SamplingUpdateButton1").removeClass('primaryButton');
            $("#SamplingUpdateButton1").addClass('disabledButtonClassForMozilla');
          }       
          $('#YourBmCmNo').val("");
          $('#YourBmCmNo').attr('disabled', 'disabled');
          $('#SamplingUpdateButton').attr('disabled', 'disabled');
          $("#BmCmIndicator option[value='']").remove();
        });
     
    </script>    

     