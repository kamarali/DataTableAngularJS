<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.BillingMemo>" %>
<h2>
  Billing Memo Details</h2>
<div class="solidBox dataEntry">
  <div class="fieldContainer horizontalFlow">
    <div>
      <div>
        <label for="BatchSequenceNumber">
          <span class="required">*</span> Batch No.:</label>
        <%: Html.TextBoxFor(bmRecord => bmRecord.BatchSequenceNumber, new { max = 99999, min = 0, @class = "digits integer", @maxLength = 5 })%>
      </div>
      <div>
        <label for="RecordSequenceWithinBatch">
          <span class="required">*</span> Sequence No.:
        </label>
        <%: Html.TextBoxFor(bmRecord => bmRecord.RecordSequenceWithinBatch, new { max = 99999, min = 0, @class = "digits integer", @maxLength = 5 })%>
      </div>
    </div>
    <div class="fieldSeparator">
    </div>
    <div>
      <div>
        <label for="SourceCode">
          <span class="required">*</span> Source Code:</label>
        <%:Html.TextBoxFor(m => m.SourceCodeId, new { @class = "autocComplete" })%>      
      </div>
      <div>
        <label for="BillingMemoNumber">
          <span class="required">*</span> Billing Memo Number:</label>
        <%: Html.TextBoxFor(bmRecord => bmRecord.BillingMemoNumber, new { @maxLength = 11, @class = "alphaNumeric" })%>
      </div>
      <div>
        <label for="BillingReasonCode">
          <span class="required">*</span> Reason Code:</label>        
        <%:Html.TextBoxFor(m => m.ReasonCode, new { @class = "autocComplete upperCase" })%>
        <%:Html.HiddenFor(m => m.CouponAwbBreakdownMandatory)%> 
      </div>
      <div>
        <label for="OurRef">
          Our Reference:</label>
        <%: Html.TextBoxFor(bmRecord => bmRecord.OurRef, new { @maxLength = 20 })%>
      </div>
    </div>
    <div class="fieldSeparator">
    </div>
    <div>
      <div>
        <label for="YourInvoiceNumber">
          Your Invoice Number:</label>
        <%: Html.TextBoxFor(bmRecord => bmRecord.YourInvoiceNumber, new { @maxLength = 10 })%>
      </div>
      <div>
        <label for="BillingYear">
          Your Billing Year:</label>
        <%:Html.BillingYearDropdownList("YourInvoiceBillingYear", Model.YourInvoiceBillingYear)%>
      </div>
      <div>
        <label for="BillingMonth">
          Your Billing Month:</label>
        <%:Html.BillingMonthDropdownList("YourInvoiceBillingMonth", Model.YourInvoiceBillingMonth)%>
      </div>
      <div>
        <label for="BillingMonthPeriod">
          Your Billing Period:</label>
        <%:Html.StaticBillingPeriodDropdownList("YourInvoiceBillingPeriod", Model.YourInvoiceBillingPeriod)%>
      </div>
    </div>
    <div class="fieldSeparator">
    </div>
    <div>
      <div>
        <label for="CorrespondenceRefNumber">
          Correspondence Ref. Number:</label>
       <%-- <%: Html.TextBoxFor(bmRecord => bmRecord.CorrespondenceRefNumber, new { @class = "integer", @maxLength = 11 })%>--%>
       <%--SCP#391029: FW: CORRESPONDENCE_REF_NO equals "0" in PAX and Cargo--%>
          <%: Html.TextBox("CorrespondenceRefNumber", Model.CorrespondenceRefNumber == -1 ?  string.Empty : Model.CorrespondenceRefNumber.ToString(FormatConstants.CorrespondenceNumberFormat), new { @class = "numeric", maxLength = 11 })%>
          <%: Html.Hidden("UserCorrRefNo") %>
      </div>
      <div>
        <label for="FIMNumber">
          FIM Number:</label>
        <%: Html.TextBoxFor(bmRecord => bmRecord.FimNumber, new { @class = "integer", @maxLength = 11 })%>
      </div>
      <div>
        <label for="FIMCouponNumber">
          FIM Coupon Number:</label>
        <%: Html.TextBoxFor(bmRecord => bmRecord.FimCouponNumber, new { @class = "integer", @maxLength = 2 })%>
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
        <a class="ignoredirty" href="#" onclick="return openAttachment();" title="Add/Remove Attachments">Attachment</a>
      </div>
      <div>
        <label for="AirlineOwnUse">
          Airline Own Use:</label>
        <%: Html.TextBoxFor(bmRecord => bmRecord.AirlineOwnUse, new { @maxLength = 20 })%>
      </div>
    </div>
    <%if (ViewData[ViewDataConstants.BillingType].ToString() == BillingType.Payables)
    {%>
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
        <%:Html.TextBoxFor(bmRecord => bmRecord.NumberOfAttachments)%>
      </div>
      <div>
        <label for="isValidationFlag">
          IS Validation Flag:</label>
        <%:Html.TextBoxFor(bmRecord => bmRecord.ISValidationFlag)%>
      </div>
    </div>
  <%
    }%>
    <div class="fieldSeparator">
    </div>
    <div>
      <div>
        <label for="ReasonRemarks">
          Remarks:</label>
        <%: Html.TextAreaFor(bmRecord => bmRecord.ReasonRemarks, 10, 80, new { maxlength = "4000", @class="notValidCharsTextarea" })%>
      </div>
    </div>
    <div class="fieldSeparator">
    </div>
    <div class="amountFieldsDiv1">
      <div>
        <label for="TotalGrossAmountBilled">
          Total Gross Amount:</label>
        <%: Html.TextBoxFor(bmRecord => bmRecord.TotalGrossAmountBilled, new { @class = "breakDownDerived amount", maxLength = 14, min = -999999999999999.9, max = 999999999999999.9, watermark = ControlIdConstants.PositiveAmount })%>
      </div>
      <div>
        <label for="TotalISCAmountBilled">
          Total ISC Amount:</label>
        <%: Html.TextBoxFor(bmRecord => bmRecord.TotalIscAmountBilled, new { @class = "breakDownDerived amount", @maxLength = 14, min = -999999999999999.9, max = 999999999999999.9, watermark = ControlIdConstants.NegativeAmount })%>
      </div>
      <div>
        <label for="TotalOtherCommissionAmount">
          Total Other Commission Amount:</label>
        <%: Html.TextBoxFor(bmRecord => bmRecord.TotalOtherCommissionAmount, new { @class = "breakDownDerived amount", @maxLength = 14, min = -999999999999999.9, max = 999999999999999.9, watermark = ControlIdConstants.NegativeAmount })%>
      </div>
      <div>
        <label for="TotalUATPAmountBilled">
          Total UATP Amount:</label>
        <%: Html.TextBoxFor(bmRecord => bmRecord.TotalUatpAmountBilled, new { @class = "breakDownDerived amount", @maxLength = 14, min = -999999999999999.9, max = 999999999999999.9, watermark = ControlIdConstants.NegativeAmount })%>
      </div>
    </div>
    <div class="amountFieldsDiv2">
      <div>
        <label for="TotalHandlingFeeBilled">
          Total Handling Fee Amount:</label>
        <%: Html.TextBoxFor(bmRecord => bmRecord.TotalHandlingFeeBilled, new { @class = "breakDownDerived amount", @maxLength = 14, min = -999999999999999.9, max = 999999999999999.9, watermark = ControlIdConstants.NegativeAmount })%>
      </div>
      <div>
        <label for="TaxAmountBilled">
          Total Tax Amount:</label>
        <%: Html.TextBoxFor(bmRecord => bmRecord.TaxAmountBilled, new { @class = "breakDownDerived amount", @maxLength = 14, max = 999999999999999.9, min = -999999999999999.9, watermark = ControlIdConstants.PositiveAmount })%>
      </div>
      <div>
        <label for="TotalVATAmountBilled">
          Total VAT Amount:</label>
        <%: Html.TextBoxFor(bmRecord => bmRecord.TotalVatAmountBilled, new { @class = "amount amountTextfield", id = "VatAmount", @readonly = true, min = -999999999999999.9, max = 999999999999999.9 })%>
        <%:ScriptHelper.GenerateDialogueHtml("VAT Breakdown", "Billing Memo VAT Capture", "divVatBreakdown", 500, 900, "vatBreakdown")%>
      </div>
      <div>
        <label for="NetAmountBilled">
          Net Billed Amount:</label>
        <%: Html.TextBoxFor(bmRecord => bmRecord.NetAmountBilled, new { @class = "breakDownDerived amount", @readonly = "true", max = 999999999999999.9, min = 0, watermark = ControlIdConstants.PositiveAmount })%>
      </div>
    </div>
  </div>
  <div class="clear">
  </div>
</div>
<div id="childVatList" class="hidden">
</div>
<div id="childAttachmentList" class="">
</div> 
