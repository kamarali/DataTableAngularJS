<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.CreditMemo>" %>
<h2>
  Credit Memo Data Capture</h2>
<div class="solidBox dataEntry">
  <div class="fieldContainer horizontalFlow">
    <div>
      <div>
        <label for="BatchSequenceNumber">
          <span>*</span> Batch Number:</label>
        <%: Html.TextBoxFor(creditMemo => creditMemo.BatchSequenceNumber, new { max = 99999, min = 0, @class = "digits integer", maxLength = 5 })%>
      </div>
      <div>
        <label for="RecordSequenceWithinBatch">
          <span>*</span> Sequence Number:</label>
        <%: Html.TextBoxFor(creditMemo => creditMemo.RecordSequenceWithinBatch, new { max = 99999, min = 0, @class = "digits integer", maxLength = 5 })%>
      </div>
    </div>
    <div class="fieldSeparator">
    </div>
    <div>
      <div>
        <label for="ChargeCode">
          <span>*</span> Source Code:</label>
        <%: Html.TextBoxFor(creditMemo => creditMemo.SourceCodeId, new { @readOnly = true, @class = "autocComplete populated" })%>
      </div>
      <div>
        <label for="CreditMemoNumber">
          <span>*</span> Credit Memo Number:</label>
        <%: Html.TextBoxFor(creditMemo => creditMemo.CreditMemoNumber, new { maxLength = 11, @class = "alphaNumeric" })%>
      </div>
      <div>
        <label for="CreditReasonCode">
          <span>*</span> Reason Code:</label>
        <%:Html.TextBoxFor(m => m.ReasonCode, new { @class = "autocComplete upperCase" })%>
        <%: Html.HiddenFor(m => m.CouponAwbBreakdownMandatory)%>
      </div>
      <div>
        <label for="OurRef">
          Our Reference:</label>
        <%: Html.TextBoxFor(creditMemo => creditMemo.OurRef, new { maxLength = 20 })%>
      </div>
    </div>
    <div class="fieldSeparator">
    </div>
    <div>
      <div>
        <label for="YourInvoiceNumber">
          Your Invoice Number:</label>
        <%: Html.TextBoxFor(creditMemo => creditMemo.YourInvoiceNumber, new { maxLength = 10 })%>
      </div>
      <div>
        <label for="BillingYear">
          Your Billing Year:
        </label>
        <%:Html.BillingYearDropdownList("YourInvoiceBillingYear", Model.YourInvoiceBillingYear)%>
      </div>
      <div>
        <label for="BillingMonth">
          Your Billing Month:
        </label>
        <%:Html.BillingMonthDropdownList("YourInvoiceBillingMonth", Model.YourInvoiceBillingMonth)%>
      </div>
      <div>
        <label for="BillingMonthPeriod">
          Your Billing Period:
        </label>
        <%:Html.StaticBillingPeriodDropdownList("YourInvoiceBillingPeriod", Model.YourInvoiceBillingPeriod)%>
      </div>
    </div>
    <div class="fieldSeparator">
    </div>
    <div>
      <div>
        <label for="CorrespondenceRefNumber">
          Correspondence Ref. Number:</label>
        <%: Html.TextBoxFor(creditMemo => creditMemo.CorrespondenceRefNumber, new { @class = "integer", maxLength = 11 })%>
      </div>
      <div>
        <label for="FIMNumber">
          FIM Number:</label>
        <%: Html.TextBoxFor(creditMemo => creditMemo.FimNumber, new { @class = "integer", maxLength = 11 })%>
      </div>
      <div>
        <label for="FIMCouponNumber">
          FIM Coupon Number:</label>
        <%: Html.TextBoxFor(creditMemo => creditMemo.FimCouponNumber, new { @class = "integer", maxLength = 2 })%>
      </div>
    </div>
    <div class="fieldSeparator">
    </div>
    <div>
      <div>
        <%if (ViewData[ViewDataConstants.BillingType].ToString() == BillingType.Payables)
          {%>
          <label for="attachmentIndicatorOriginal">
            Attachment Indicator - Original:
          </label>
        <%
          }
          else
          {%>
          <label for="attachmentIndicatorOriginal">
            <span>*</span> Attachment Indicator:
          </label>
          <%} %>
        <%:Html.AttachmentIndicatorTextBox(ControlIdConstants.AttachmentIndicatorOriginal, Model.AttachmentIndicatorOriginal)%>
        <a class="ignoredirty" href="#" onclick="return openAttachment();">Attachment</a>
      </div>
      <div>
        <label for="AirlineOwnUse">
          Airline Own Use:</label>
        <%: Html.TextBoxFor(creditMemo => creditMemo.AirlineOwnUse, new { maxLength = 20 })%>
      </div>
    </div>
    <%if (ViewData[ViewDataConstants.BillingType].ToString() == BillingType.Payables)
    {%>
      <div class="fieldSeparator">
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
          <%:Html.TextBoxFor(creditMemo => creditMemo.NumberOfAttachments)%>
        </div>
        <div>
          <label for="isValidationFlag">
            IS Validation Flag:</label>
          <%:Html.TextBoxFor(creditMemo => creditMemo.ISValidationFlag)%>
        </div>
      </div>
    <% }%>
    <div class="fieldSeparator">
    </div>
    <div>
      <div>
        <label for="ReasonRemarks">
          Remarks:</label>
        <%: Html.TextAreaFor(creditMemo => creditMemo.ReasonRemarks, 10, 80, new { maxlength = "4000", @class="notValidCharsTextarea" })%>
      </div>
    </div>
    <div class="fieldSeparator">
    </div>
    <div class="AmountFieldsDiv1">
      <div>
        <label for="TotalGrossAmountBilled">
          Total Gross Amount:</label>
        <%: Html.TextBoxFor(creditMemo => creditMemo.TotalGrossAmountCredited, new { min = -9999999999999.90, max = 9999999999999.90, @class = "amount", watermark = ControlIdConstants.PositiveAmount })%>
      </div>
      <div>
        <label for="TotalISCAmountCredited">
          Total ISC Amount:</label>
        <%: Html.TextBoxFor(creditMemo => creditMemo.TotalIscAmountCredited, new { min = -999999999999999.90, max = 999999999999999.90, @class = "amount", watermark = ControlIdConstants.NegativeAmount })%>
      </div>
      <div>
        <label for="TotalOtherCommissionAmountCredited">
          Total Other Commission Amount:</label>
        <%: Html.TextBoxFor(creditMemo => creditMemo.TotalOtherCommissionAmountCredited, new { min = -999999999999999.90, max = 999999999999999.9, @class = "amount", watermark = ControlIdConstants.NegativeAmount })%>
      </div>
      <div>
        <label for="TotalUATPsAmountCredited">
          Total UATP Amount:</label>
        <%: Html.TextBoxFor(creditMemo => creditMemo.TotalUatpAmountCredited, new { min = -999999999999999.90, max = 999999999999999.9, @class = "amount", watermark = ControlIdConstants.NegativeAmount })%>
      </div>
    </div>
    <div class="AmountFieldsDiv2">
      <div>
        <label for="TotalHandlingFeeCredited">
          Total Handling Fee Amount:</label>
        <%: Html.TextBoxFor(creditMemo => creditMemo.TotalHandlingFeeCredited, new { min = -999999999.9, max = 999999999.9, @class = "amount", watermark = ControlIdConstants.PositiveAmount })%>
      </div>
      <div>
        <label for="TaxAmount">
          Total Tax Amount:</label>
        <%: Html.TextBoxFor(creditMemo => creditMemo.TaxAmount, new { min = -999999999999999.9, max = 999999999999999.9, @class = "amount", watermark = ControlIdConstants.NegativeAmount })%>
      </div>
      <div>
        <label for="VatAmount">
          Total VAT Amount:</label>
        <%: Html.TextBoxFor(creditMemo => creditMemo.VatAmount, new { @class = "amountTextfield amount", min = -999999999999999.9, max = 999999999999999.9, @readonly = true })%>
        <%: ScriptHelper.GenerateDialogueHtml("VAT Breakdown", "Credit Memo VAT Capture", "divVatBreakdown", 500, 900, "vatBreakdown")%>
      </div>
      <div>
        <label for="NetAmountCredited">
          <span>*</span> Net Credited Amount:</label>
        <%: Html.TextBoxFor(creditMemo => creditMemo.NetAmountCredited, new { @readOnly = true, min = -999999999999999.9, max = 0, @class = "amount", watermark = ControlIdConstants.NegativeAmount })%>
      </div>
    </div>
  </div>
  <div class="clear">
  </div>
</div>
<div class="hidden" id="childVatList">
</div>
<div id="childAttachmentList" class="">
</div>
