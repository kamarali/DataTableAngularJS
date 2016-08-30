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
            Type:</label>
          <%:Html.SupportingDocTypeDropdownList(ControlIdConstants.SupportingDocumentType, (int)Model.Type)%>
        </div>
        <div>
          <label>
            Invoice Number:</label>
          <%:Html.TextBoxFor(m => m.InvoiceNumber, new { maxLength = 10 })%>
        </div>
      </div>
      <div>
        <div>
        <label for="SourceCodeId">
           Source Code:</label>
        <%:Html.TextBoxFor(m => m.SourceCodeId, new { @class = "autocComplete populated" })%>
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
          <%: Html.TextBox(ControlIdConstants.RMBMCMNumber, Model.RMBMCMNumber)%>
      </div>
      <div>
        <label for="TicketDocNumber">
          Ticket/Doc Number:</label>
        <%:Html.TextBoxFor(m => m.TicketDocNumber, new { @class = "integer", @maxLength = 11 })%>
      </div>
      </div>
      <div>
        <div>
          <label for="CouponNumber">
             Coupon Number:</label>
          <%:Html.TextBoxFor(m => m.CouponNumber, new { @class = "integer", @maxLength = 1 })%>
        </div>
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
