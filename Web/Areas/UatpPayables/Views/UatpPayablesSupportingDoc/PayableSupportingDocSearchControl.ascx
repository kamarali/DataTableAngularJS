<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.SupportingDocuments.SupportingDocSearchCriteria>" %>
<div class="searchCriteria">
  <div class="solidBox">
    <div class="fieldContainer horizontalFlow">
      <div>
        <div>
          <label>
            <span>*</span> Billing Year/Month:</label>
          <%:Html.PayableBillingYearMonthDropdown(ControlIdConstants.BillingYearMonthDropDown, Model.BillingYear, Model.BillingMonth, TransactionMode.Payables)%>
        </div>
        <div>
          <label>
            <span>*</span> Billing Period:</label>
          <%:Html.StaticBillingPeriodDropdownListFor(m => m.BillingPeriod, true)%>
        </div>
        <div>
          <label>
            <span>*</span> Billing Member:</label>
          <%:Html.HiddenFor(m => m.BillingMemberId)%>
          <%:Html.TextBoxFor(m => m.BillingMemberText, new { @class = "autocComplete" })%>
        </div>
        <div>
          <label>
            Invoice Number:</label>
          <%:Html.TextBoxFor(m => m.InvoiceNumber, new { maxLength = 10 })%>
        </div>
        
      </div>
      <div>
        <div>
          <label for="attachmentIndicatorOriginal">
            Attachment Indicator Validated:
          </label>
          <%:Html.SupportingDocAttachmentIndicatorDropdownListFor(m => m.AttachmentIndicatorValidated)%>
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
