<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Cargo.CargoCreditMemo>" %>
<%@ Import Namespace="Iata.IS.Web.Util" %>
<h2>
  Credit Memo Data Capture</h2>
<div class="solidBox dataEntry">
  <div class="fieldContainer horizontalFlow">
    <div class="bottomLine">
      <div>
        <label>
          <span class="required">*</span> Batch No.:</label>
        <%:Html.TextBoxFor(cm => cm.BatchSequenceNumber, new { @class = "integer", @maxLength = 5 })%>
      </div>
      <div>
        <label>
          <span class="required">*</span> Sequence Number:</label>
        <%:Html.TextBoxFor(cm => cm.RecordSequenceWithinBatch, new { @class = "integer", @maxLength = 5 })%>
      </div>
    </div>
    <div class="bottomLine">
      <div>
        <label>
          <span class="required">*</span> Credit Memo Number:</label>
        <%:Html.TextBoxFor(cm => cm.CreditMemoNumber, new { @maxLength = 11, @class = "alphaNumeric" })%>
      </div>
      <div>
        <label>
          <span class="required">*</span> Reason Code:</label>
        <%:Html.TextBoxFor(cm => cm.ReasonCode, new { @class = "autocComplete upperCase", @maxLength = 2  })%>
        <%:Html.HiddenFor(cm => cm.CouponAwbBreakdownMandatory)%>
      </div>
      <div>
        <label>
          Our Reference:</label>
        <%:Html.TextBoxFor(cm => cm.OurRef, new { @maxLength = 20, @class = "alphaNumeric" })%>
      </div>
    </div>
    <div class="bottomLine">
      <div>
        <label>
          Your Invoice Number:</label>
        <%:Html.TextBoxFor(cm => cm.YourInvoiceNumber, new { @maxLength = 10 })%>
        <%:Html.HiddenFor(cm => cm.InvoiceId)%>
        <%:Html.HiddenFor(cm => cm.BillingCode)%>
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
    </div>
    <div class="bottomLine">
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
        <%:Html.TextBoxFor(cm => cm.AirlineOwnUse, new { @maxLength = 20 })%>
      </div>
    </div>
    <%if (ViewData[ViewDataConstants.BillingType].ToString() == BillingType.Payables) {%>
      <div id="divPayables">
        <div>
          <label for="attachmentIndicatorValidated">
            Attachment Indicator - Validated:</label>
          <%:Html.TextBox(ControlIdConstants.AttachmentIndicatorValidated, Model.AttachmentIndicatorValidated == true ? "Yes" : "No")%>
        </div>
        <div>
          <label for="numberOfAttachments">
            Number Of Attachments:</label>
          <%:Html.TextBoxFor(cm => cm.NumberOfAttachments)%>
        </div>
        <div>
          <label for="isValidationFlag">
            IS Validation Flag:</label>
          <%:Html.TextBoxFor(cm => cm.ISValidationFlag)%>
        </div>
      </div>
          <div class="fieldSeparator">
      </div>
    <% }%>
    <div>
      <div>
        <label>
          Reason Remarks:</label>
        <%:Html.TextAreaFor(cm => cm.ReasonRemarks, 10, 80, new { maxlength = "4000", @class = "notValidCharsTextarea" })%>
      </div>
    </div>
    <div class="fieldSeparator">
    </div>
    <div class="amountFieldsDiv1">
      <div>
        <label>
          Total Weight Charges:</label>
        <%:Html.TextBoxFor(cm => cm.TotalWeightCharges, new { @class = "breakDownDerived amount amt_15_3", watermark = ControlIdConstants.PositiveAmount })%>
      </div>
      <div>
        <label>
          Total Valuation Charges:</label>
        <%:Html.TextBoxFor(cm => cm.TotalValuationAmt, new { @class = "breakDownDerived amount amt_15_3", watermark = ControlIdConstants.PositiveAmount })%>
      </div>
      <div>
        <label>
          Total Other Charges:</label>
        <%:Html.TextBoxFor(cm => cm.TotalOtherChargeAmt, new { @class = "breakDownDerived amount amt_15_3" })%>
        <%--<%:ScriptHelper.GenerateDialogueHtml("Other Charges", "Billing Memo Other Charge Capture", "divOtherCharge", 500, 900, "otherChargeBreakDown")%>--%>
      </div>
      <div>
        <label>
          Total ISC Amount:</label>
        <%:Html.TextBoxFor(cm => cm.TotalIscAmountCredited, new { @class = "breakDownDerived amount amt_15_3", watermark = ControlIdConstants.NegativeAmount })%>
      </div>
      
    </div>
    <div class="amountFieldsDiv2">
    <div>
        <label>
          Total VAT Amount:</label>
        <%:Html.TextBoxFor(cm => cm.TotalVatAmountCredited, new { @class = "amount amountTextfield amt_15_3", id = "VatAmount", @readonly = true })%>
        <%:ScriptHelper.GenerateDialogueHtml("VAT Breakdown", "Credit Memo VAT Capture", "divVatBreakdown", 500, 900, "vatBreakdown")%>
      </div>
      <div>
        <label>
           Net Credited Amount:</label>
        <%:Html.TextBoxFor(cm => cm.NetAmountCredited, new { @class = "amount neg_amt_15_3", @readonly = "true", watermark = ControlIdConstants.NegativeAmount })%>
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