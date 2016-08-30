<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.SupportingDocuments.UnlinkedSupportingDocumentEx>" %>
<div class="searchCriteria">
  <div class="solidBox">
    <div class="fieldContainer horizontalFlow">
      <div>
        <div>
          <label>
            Billing Year/Month:</label>
          <%:Html.UnlinkedSupportingDocumentBillingYearMonthDropdown(ControlIdConstants.SupportingDocumentBillingYearMonth, Model.BillingYear, Model.BillingMonth)%>
        </div>
        <div>
          <label>
            Billing Period:</label>
          <%:Html.StaticBillingPeriodDropdownListFor(m => m.PeriodNumber, true)%>
        </div>
        <div>
          <label>
            Billed Member:</label>
          <%:Html.HiddenFor(m => m.BilledMemberId)%>
          <%:Html.TextBoxFor(m => m.BilledMemberText, new { @class = "autocComplete" })%>
        </div>
        <div>
          <label>
            Invoice Number:</label>
          <%:Html.TextBoxFor(m => m.InvoiceNumber, new { maxLength = 10 })%>
        </div>
      </div>
      <div>
        <div>
          <label>
            Supporting Document File Name:</label>
          <%: Html.TextBoxFor(m=>m.OriginalFileName, new { @class = "xlargeTextField", maxLength = 30 })%>
        </div>
        <div>
          <label>
            Submission Date:</label>
          <%:Html.TextBox(ControlIdConstants.SupportingDocumentSubmissionDate, Model.SubmissionDate != null ? Model.SubmissionDate.Value.ToString(FormatConstants.DateFormat) : null, new { @class = "datePicker" })%>
        </div>
      </div>
    </div>
    <div class="clear">
    </div>
  </div>
</div>
<div class="buttonContainer">
  <input class="primaryButton" type="submit" value="Search" />
  <input class="secondaryButton" type="button" onclick="resetForm();" value="Clear" />
</div>
<div class="clear">
</div>
