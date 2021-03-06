﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Common.ValidationErrorCorrection>" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<%@ Import Namespace="Iata.IS.Web.Util" %>
<div class="searchCriteria">
<!-- CMP #596: Length of Member Accounting Code to be Increased to 12 
  Desc: Increasing field size from 85% to 120% to keep layout intact -->
    <div class="solidBox dataEntry" style="width: 120%" >
        <div class="fieldContainer horizontalFlow">
            <div class="">
                <div>
                    <label>
                        <span>*</span> Billing Year/Month/Period:</label>
                     <%:Html.BillingYearMonthPeriodDropdownValidationErrorCorrection(ControlIdConstants.BillingYearMonthDropDown, Model.BillingYear, Model.BillingMonth, Model.BillingPeriod)%>
                </div>
                 <%= Html.HiddenFor(m => m.BillingYear)%>
                <%= Html.HiddenFor(m => m.BillingMonth)%>
                <%= Html.HiddenFor(m => m.BillingPeriod)%>
               <%-- <div>
                    <label>
                      <span>*</span> Billing Period:</label>
                    <%:Html.StaticBillingPeriodDropdownList(ControlIdConstants.BillingPeriod, Model.BillingPeriod.ToString(), TransactionMode.InvoiceSearch)%>
                </div>--%>

                <div>
                    <label>
                        Billed Member:</label>
                     <!-- CMP #596: Length of Member Accounting Code to be Increased to 12 
                     Desc: Increasing field size by specifying in-line width
                     Ref: 3.5 Table 19 Row 4 -->
                    <%:Html.TextBoxFor(validationErrorCorrection => validationErrorCorrection.BilledMember, new { @class = "autocComplete textboxWidth" })%>
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
                    <%:Html.TextBoxFor(validationErrorCorrection => validationErrorCorrection.FileName, new { maxLength = 255 })%>
                </div>
                <div >
                  <label>
                    Charge Category:</label>
                    <%--CMP609: MISC Changes Required as per ISW2. Added new parameter 'isIncludeInactive'. If it is true then method will return the all charge category for misc category including in-active.--%>
                  <%: Html.ChargeCategoryDropdownList(ControlIdConstants.ChargeCategory, Model.ChargeCategoryId, BillingCategoryType.Misc, true, isIncludeInactive: true)%>
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
        <input class="secondaryButton" type="button" value="Clear" onclick="resetForm();" />
    </div>
</div>
