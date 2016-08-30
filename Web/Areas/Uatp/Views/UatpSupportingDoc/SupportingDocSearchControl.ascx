<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.SupportingDocuments.SupportingDocSearchCriteria>" %>
<div class="searchCriteria">
  <div class="solidBox">
    <div class="fieldContainer horizontalFlow">
      <div>
        <div>
          <label>
            <span>*</span> Billing Year/Month:</label>
          <%:Html.SupportingDocBillingYearMonthDropdown(ControlIdConstants.BillingYearMonthDropDown, Model.BillingYear, Model.BillingMonth, Model.SupportingDocumentTypeId)%>
        </div>
        <div>
          <label>
            <span>*</span> Billing Period:</label>
          <%:Html.StaticBillingPeriodDropdownListFor(m => m.BillingPeriod, true)%>
        </div>
        <div>
          <label>
            <span>*</span> Billed Member:</label>
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
          <label for="attachmentIndicatorOriginal">
            Attachment Indicator Original:
          </label>
          <%:Html.SupportingDocAttachmentIndicatorDropdownListFor(m => m.AttachmentIndicatorOriginal)%>
        </div>
        <div>
          <label for="IsMismatchCases">
            Mismatch Cases:
          </label>
          <%:Html.CheckBoxFor(m => m.IsMismatchCases)%>
        </div>
        <%:Html.HiddenFor(model => model.IsUatp) %>
      </div>
    </div>
    <div class="clear">
    </div>
  </div>
</div>
<div class="buttonContainer">
  <input class="primaryButton" type="submit" value="Search" />
  <input class="secondaryButton" type="button" onclick="resetSearchCriteria();" value="Clear" />
</div>
<div class="clear">
</div>
