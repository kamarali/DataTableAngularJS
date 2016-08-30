<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.SupportingDocuments.UnlinkedSupportingDocumentEx>" %>
<h2>
  Modify Batch Header Information</h2>
<div class="solidBox dataEntry">
  <div class="fieldContainer horizontalFlow">
    <div>
      <div>
        <label>
          <span>*</span> Form C:
        </label>
        <%:Html.UnlinkedSupportingDocumentFormCYNDropdownList(ControlIdConstants.SupportingDocumentFormCDetailView, new { id = ControlIdConstants.SupportingDocumentFormCDetailView })%>
      </div>
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
        <%:Html.HiddenFor(m => m.BilledMemberId, new { ID = ControlIdConstants.SupportingDocumentBilledMemeberIdDetailView })%>
        <%:Html.TextBoxFor(m => m.BilledMemberText, new { @class = "autocComplete", id = ControlIdConstants.SupportingDocumentBilledMemberDetailView })%>
      </div>
    </div>
    <div>
      <div>
        <label>
          <span>*</span>Invoice Number:</label>
        <%:Html.TextBoxFor(m => m.InvoiceNumber, new { maxLength = 11, id = ControlIdConstants.SupportingDocumentInvoiceNoDetailView })%>
      </div>
      <div>
        <label>
          Batch Number:</label>
        <%:Html.TextBoxFor(m => m.BatchNumber, new { maxLength = 5, id = ControlIdConstants.SupportingDocumentBatchNumberDetailView })%>
      </div>
      <div>
        <label>
          Sequence Number:</label>
        <%:Html.TextBoxFor(m => m.SequenceNumber, new { maxLength = 5, id = ControlIdConstants.SupportingDocumentSequenceNumberDetailView })%>
      </div>
      <div>
        <label>
          Breakdown Serial Number:</label>
        <%:Html.TextBoxFor(m => m.CouponBreakdownSerialNumber, new { maxLength = 5, id = ControlIdConstants.SupportingDocumentCouponBreakdownSerialNumberDetailView })%>
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
