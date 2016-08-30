<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.SupportingDocuments.UnlinkedSupportingDocumentEx>" %>
<h2>
  Modify Batch Header Information</h2>
<div class="solidBox dataEntry">
  <div class="fieldContainer horizontalFlow">
    <div>
      <div>
        <label>
          <span>*</span> Billing Year/Month:
        </label>
        <%:Html.BillingYearMonthDropdown(ControlIdConstants.SupportingDocumentBillingYearMonthDetailView, Model.BillingYear, Model.BillingMonth)%>
      </div>
      <div>
        <label>
          <span>*</span> Billing Period:</label>
        <%:Html.StaticBillingPeriodDropdownList("PeriodNumber", Model.PeriodNumber,false, new { id = ControlIdConstants.SupportingDocumentBillingPeriodDetailView })%>
      </div>
      <div>
        <label>
          <span>*</span>Billed Member:</label>
        <%:Html.HiddenFor(m => m.BilledMemberId, new { id = ControlIdConstants.SupportingDocumentBilledMemeberIdDetailView })%>
        <!-- CMP #596: Length of Member Accounting Code to be Increased to 12 
            Desc: Increasing field size by specifying in-line width
            Ref: 3.5 Table 19 Row 2,3.5 Table 20 Row 5 -->
        <%:Html.TextBoxFor(m => m.BilledMemberText, new { @class = "autocComplete textboxWidth", id = ControlIdConstants.SupportingDocumentBilledMemberDetailView })%>
      </div>
      <div>
        <label>
          <span>*</span>Invoice Number:</label>
        <%:Html.TextBoxFor(m => m.InvoiceNumber, new { maxLength = 11, id = ControlIdConstants.SupportingDocumentInvoiceNoDetailView })%>
      </div>
    </div>
    <div>
      <div>
        <label>
          New File Name:</label>
        <%: Html.TextBoxFor(m => m.OriginalFileName, new { @class = "xlargeTextField", maxLength = 100, id = ControlIdConstants.SupportingDocumentFileNameDetailView })%>
      </div>
    </div>
  </div>
  <div class="clear">
  </div>
</div>
<div class="buttonContainer">
  <input class="primaryButton" type="submit" value="Link" id="LinkButton" />
  <input class="secondaryButton" type="button" value="Show Mismatched Transactions From BRD" id="ShowMismatchedTransactionFromBRDButton" />
</div>
<div class="clear">
</div>
