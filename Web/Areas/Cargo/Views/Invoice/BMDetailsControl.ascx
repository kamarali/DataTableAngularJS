<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Cargo.CargoBillingMemo>" %>
<h2>
  Billing Memo Data Capture</h2>
<div class="solidBox dataEntry">
  <div class="fieldContainer horizontalFlow">
    <div class="bottomLine">
      <div>
        <label>
          <span class="required">*</span> Batch No.:</label>
        <%:Html.TextBoxFor(bm => bm.BatchSequenceNumber, new { @class = "integer", @maxLength = 5 })%>
      </div>
      <div>
        <label>
          <span class="required">*</span> Sequence Number:</label>
        <%:Html.TextBoxFor(bm => bm.RecordSequenceWithinBatch, new { @class = "integer", @maxLength = 5 })%>
      </div>
    </div>
    <div class="bottomLine">
      <div>
        <label>
          <span class="required">*</span> Billing Memo Number:</label>
        <%:Html.TextBoxFor(bm => bm.BillingMemoNumber, new { @maxLength = 11, @class = "alphaNumeric" })%>
      </div>
      <div>
        <label>
          <span class="required">*</span> Reason Code:</label>
        <%:Html.TextBoxFor(bm => bm.ReasonCode, new { @class = "autocComplete upperCase", @maxLength = 2 })%>
        <%:Html.HiddenFor(bm => bm.AwbBreakdownMandatory)%> 
      </div>
      <div>
        <label>
          Our Reference:</label>
        <%:Html.TextBoxFor(bm => bm.OurRef, new { @maxLength = 20, @class = "alphaNumeric" })%>
      </div>
    </div>
    <div class="bottomLine">
      <div>
        <label>
          Your Invoice Number:</label>
        <%:Html.TextBoxFor(bm => bm.YourInvoiceNumber, new { @maxLength = 10 })%>
        <%:Html.HiddenFor(bm => bm.InvoiceId)%>
      </div>
      <div>
        <label >
          Your Billing Year:</label>
        <%:Html.BillingYearDropdownList("YourInvoiceBillingYear", Model.YourInvoiceBillingYear)%>
      </div>
      <div>
        <label>
          Your Billing Month:</label>
        <%:Html.BillingMonthDropdownList("YourInvoiceBillingMonth", Model.YourInvoiceBillingMonth)%>
      </div>
      <div>
        <label>
          Your Billing Period:</label>
        <%:Html.StaticBillingPeriodDropdownList("YourInvoiceBillingPeriod", Model.YourInvoiceBillingPeriod)%>
      </div>
      <div>
        <label>
          Correspondence Ref Number:</label>
       <%-- <%:Html.TextBoxFor(bm => bm.CorrespondenceReferenceNumber, new { @maxLength = 11, @class = "numeric" })%>--%>
       <%--SCP#391029: FW: CORRESPONDENCE_REF_NO equals "0" in PAX and Cargo--%>
        <%: Html.TextBox("CorrespondenceReferenceNumber", Model.CorrespondenceReferenceNumber == -1 ? string.Empty : Model.CorrespondenceReferenceNumber.ToString(FormatConstants.CorrespondenceNumberFormat), new { @class = "numeric", maxLength = 11 })%>
        <%: Html.Hidden("UserCorrRefNo") %>
      </div>
    </div>
    <div>
      <div>
        <label >
          Attachment Indicator - Original:
        </label>
        <%:Html.AttachmentIndicatorTextBox(ControlIdConstants.AttachmentIndicatorOriginal, Model.AttachmentIndicatorOriginal)%>
        <a class="ignoredirty" id="attachmentBreakdown" href="#" onclick="return openAttachment();">Attachment</a>
      </div>
      <div>
        <label>
          Airline Own Use:</label>
        <%:Html.TextBoxFor(bm => bm.AirlineOwnUse, new { @maxLength = 20 })%>
      </div>
    </div>
    <%if (ViewData[ViewDataConstants.BillingType].ToString() == BillingType.Payables) {%>
    <div class="fieldSeparator" id="payablesSeparatorDiv">
    </div>
    <div id="divPayables">
      <div>
        <label for="attachmentIndicatorValidated">
          Attachment Indicator - Validated:</label>
        <%:Html.TextBox(ControlIdConstants.AttachmentIndicatorValidated, Model.AttachmentIndicatorValidated == true ? "Yes" : "No")%>
      </div>
      <div>
        <label for="numberOfAttachments">
          Number Of Attachments:</label>
        <%:Html.TextBoxFor(bm => bm.NumberOfAttachments)%>
      </div>
      <div>
        <label for="isValidationFlag">
          IS Validation Flag:</label>
        <%:Html.TextBoxFor(bm => bm.ISValidationFlag)%>
      </div>
    </div>
  <% }%>
    <div class="fieldSeparator" id="Div1">
    </div>
    <div>
      <div>
        <label>
          Reason Remarks:</label>
        <%:Html.TextAreaFor(bm => bm.ReasonRemarks, 10, 80, new { maxlength = "4000", @class = "notValidCharsTextarea" })%>
      </div>
    </div>
    <div class="fieldSeparator">
    </div>
    <div class="amountFieldsDiv1">
      <div>
        <label>
          Total Weight Charges:</label>
        <%:Html.TextBoxFor(bm => bm.BilledTotalWeightCharge, new { @class = "breakDownDerived amount amt_15_3", watermark = ControlIdConstants.PositiveAmount })%>
      </div>
      <div>
        <label>
          Total Valuation Charges:</label>
        <%:Html.TextBoxFor(bm => bm.BilledTotalValuationAmount, new { @class = "breakDownDerived amount amt_15_3", watermark = ControlIdConstants.PositiveAmount })%>
      </div>
      <div>
        <label>
          Total Other Charges:</label>
        <%:Html.TextBoxFor(bm => bm.BilledTotalOtherChargeAmount, new { @class = "breakDownDerived amount amt_15_3", watermark = ControlIdConstants.PositiveAmount })%>
        <%--<%:ScriptHelper.GenerateDialogueHtml("Other Charges", "Billing Memo Other Charge Capture", "divOtherCharge", 500, 900, "otherChargeBreakDown")%>--%>
      </div>
      <div>
        <label>
          Total ISC Amount:</label>
        <%:Html.TextBoxFor(bm => bm.BilledTotalIscAmount, new { @class = "breakDownDerived amount amt_15_3", watermark = ControlIdConstants.NegativeAmount })%>
      </div>
      
    </div>
    <div class="amountFieldsDiv2">
    <div>
        <label>
          Total VAT Amount:</label>
        <%:Html.TextBoxFor(bm => bm.BilledTotalVatAmount, new { @class = "amount amountTextfield amt_15_3", id = "VatAmount", @readonly = true })%>
        <%:ScriptHelper.GenerateDialogueHtml("VAT Breakdown", "Billing Memo VAT Capture", "divVatBreakdown", 500, 900, "vatBreakdown")%>
      </div>
      <div>
        <label>
           Net Billed Amount:</label>
        <%:Html.TextBoxFor(bm => bm.NetBilledAmount, new { @class = "breakDownDerived amount pos_amt_15_3", @readonly = "true", watermark = ControlIdConstants.PositiveAmount })%>
      </div>
    </div>
    <div class="clear">
    </div>
  </div>
  <div class="clear">
  </div>
</div>
<div id="childVatList" class="hidden">
</div>
<div id="childAttachmentList" class="">
</div>
<%--<div id="childOtherChargeList" class="hidden">
</div>
--%>