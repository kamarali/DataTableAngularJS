<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.SupportingDocuments.SupportingDocSearchCriteria>" %>
<div class="searchCriteria">
  <div class="solidBox">
    <div class="fieldContainer horizontalFlow">
      <div>
        <div>
          <label>
            <span>*</span> Billing Year/Month:</label>
          <%:Html.PayableSupportingDocBillingYearMonthDropdown(ControlIdConstants.BillingYearMonthDropDown, Model.BillingYear, Model.BillingMonth, Model.SupportingDocumentTypeId)%>
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
          <label>
            Billing Code:</label> 
            <%:Html.StaticCgoBillingCodeDropdownListFor(m => m.BillingCode, true)%>
        </div>
        <div>
        <label for="BatchSequenceNumber">
           Batch Number:</label>
        <%:Html.TextBoxFor(couponRecord => couponRecord.BatchSequenceNumber, new { @class = "integer", @maxLength = 5 })%>
      </div>
      <div>
        <label for="RecordSequenceWithinBatch">
           Sequence Number:</label>
        <%:Html.TextBoxFor(couponRecord => couponRecord.RecordSequenceWithinBatch, new { @class = "integer", @maxLength = 5 })%>
      </div>
        <div>
          <label>
            RM/BM/CM No:</label>
          <%: Html.TextBox(ControlIdConstants.RMBMCMNumber, Model.RMBMCMNumber, new { @maxLength = 11 })%>
      </div>
      <div>
        <label for="AWBNumber">
          AWB Number:</label>
        <%:Html.TextBoxFor(m => m.AWBSerialNumber, new { @class = "integer", @maxLength = 7 })%>
      </div>
      </div>
      <div>
        <div>
          <label for="attachmentIndicatorOriginal">
            Attachment Indicator - Validated:
          </label>
          <%:Html.SupportingDocAttachmentIndicatorDropdownList(m => m.AttachmentIndicatorValidated,"Misc")%>
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
