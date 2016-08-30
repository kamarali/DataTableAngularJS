<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MiscUatp.ValidationErrorCorrection>" %>
<div class="searchCriteria">
    <div class="solidBox dataEntry">
        <div class="fieldContainer horizontalFlow">
            <div class="">
                <div>
                    <label>
                        <span>*</span> Billing Year/Month:</label>
                    <%:Html.BillingYearMonthDropdownValidationErrorCorrection(ControlIdConstants.BillingYearMonthDropDown,Model.BillingYear, Model.BillingPeriod)%>
                </div>

                <div>
                    <label>
                      <span>*</span> Billing Period:</label>
                    <%:Html.StaticBillingPeriodDropdownList(ControlIdConstants.BillingPeriod, Model.BillingPeriod.ToString(), TransactionMode.InvoiceSearch)%>
                </div>

                <div>
                    <label>
                        Billed Member:</label>
                    <%:Html.TextBoxFor(validationErrorCorrection => validationErrorCorrection.BilledMember, new { @class = "autocComplete" })%>
                    <%= Html.HiddenFor(m => m.BilledMemberId)%>
                </div>
                <div>
                    <label>
                        Invoice Number:</label>
                    <%:Html.TextBoxFor(validationErrorCorrection => validationErrorCorrection.InvoiceNumber)%>
                </div>
            </div>
            <div>
                <div>
                    <label>
                        Exception Code:</label>
                    <%:Html.TextBoxFor(validationErrorCorrection => validationErrorCorrection.ExceptionCode)%>
                </div>
                <div>
                    <label>
                        File Submission Date:</label>
                    <%:Html.TextBox(ControlIdConstants.SupportingDocumentSubmissionDate, Model.BillingMonth != 0 ? Model.BillingMonth.ToString() : null, new { @class = "datePicker" })%>
                </div>
                <div>
                    <label>
                        File Name:</label>
                    <%:Html.TextBoxFor(validationErrorCorrection => validationErrorCorrection.FileName)%>
                </div>
            </div>
        </div>
        <div class="clear">
        </div>
    </div>
    <div class="buttonContainer" align="left">
        <input class="primaryButton" type="submit" value="Search" />
        <%--  <input class="primaryButton" type="button" value="Add New Template" onclick="javascript:location.href = '<%:Url.Action("NewPermissionTemplate", "Permission")%>';" />
        --%>
        <input class="secondaryButton" type="button" value="Clear" />
    </div>
</div>
