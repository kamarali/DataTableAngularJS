<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Common.ValidationErrorCorrection>" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<%@ Import Namespace="Iata.IS.Web.Util" %>
<div class="searchCriteria">
    <div class="solidBox dataEntry">
        <div class="fieldContainer horizontalFlow">
            <div class="">
                <div>
                    <label>
                        <span>*</span>Provisional Billing Year/Month:</label>
                     <%:Html.PaxSamplingBillingYearMonthDropdown(ControlIdConstants.BillingYearMonthDropDown, Model.BillingYear, Model.BillingMonth)%>
                </div>
                 <%= Html.HiddenFor(m => m.BillingYear)%>
                <%= Html.HiddenFor(m => m.BillingMonth)%>

                <div>
                    <label>
                        Provisional Billing Member:</label>
                    <%:Html.TextBoxFor(validationErrorCorrection => validationErrorCorrection.BilledMember, new { @class = "autocComplete" })%>
                    <%= Html.HiddenFor(m => m.BilledMemberId)%>
                    <%= Html.HiddenFor(m => m.BillingMemberId)%>
                </div>
                <div>
                    <label>
                        Exception Code:</label>
                    <%:Html.TextBoxFor(validationErrorCorrection => validationErrorCorrection.ExceptionCode, new { @class = "autocComplete" })%>
                     <%= Html.HiddenFor(m => m.ExceptionCodeId)%>
                </div>
            </div>
            <div>
                <div>
                    <label>
                        File Submission Date:</label>
                         <%:Html.TextBox(ControlIdConstants.FileSubmissionDate, Model.FileSubmissionDate != null ? Model.FileSubmissionDate.Value.ToString(FormatConstants.DateFormat) : null, new { @class = "datePicker" })%>
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
        <input class="primaryButton" type="submit" value="Search" />
        <%--  <input class="primaryButton" type="button" value="Add New Template" onclick="javascript:location.href = '<%:Url.Action("NewPermissionTemplate", "Permission")%>';" />
        --%>
        <input class="secondaryButton" type="button" value="Clear" onclick="SamplingresetForm();" />
    </div>
</div>
