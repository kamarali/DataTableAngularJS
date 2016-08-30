<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.SupportingDocuments.UnlinkedSupportingDocumentEx>" %>
<div class="searchCriteria">
  <!-- CMP #596: Length of Member Accounting Code to be Increased to 12 
  Desc: Increasing field size from 85% to 120% to keep layout intact -->
  <div class="solidBox" style="width: 120%" >
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
           <!-- CMP #596: Length of Member Accounting Code to be Increased to 12 
            Desc: Increasing field size by specifying in-line width
            Ref: 3.5 Table 19 Row 2,3.5 Table 20 Row 5 -->
          <%:Html.TextBoxFor(m => m.BilledMemberText, new { @class = "autocComplete textboxWidth" })%>
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
