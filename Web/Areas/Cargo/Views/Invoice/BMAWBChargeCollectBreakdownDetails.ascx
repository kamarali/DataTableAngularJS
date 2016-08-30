<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Cargo.CargoBillingMemoAwb>" %>

<h2>
  AWB Charge Collect Billing Details</h2>
  <div class="solidBox dataEntry">
    <div class="fieldContainer horizontalFlow">
      <div>
       <div id="serialNoDiv">
        <label for="BdSerialNumber">
          Serial Number:
        </label>
        <%:Html.TextBoxFor(awbRecord => awbRecord.BdSerialNumber, new { @readOnly = true })%>
      </div>
        <div>
          <label >
            <span class="required">*</span> AWB Issuing Airline:</label>
          <%:Html.TextBoxFor(awbRecord => awbRecord.AwbIssueingAirline, new { @class = "autocComplete populated", @maxlength = 4 })%>
        </div>
        <div>
          <label for="AwbSerialNumber">
            <span class="required">*</span> AWB Serial Number:</label>
          <%:Html.TextBoxFor(awbRecord => awbRecord.AwbSerialNumberCheckDigit, new { @class = "", @maxLength = 8 })%>
          <%:Html.HiddenFor(awbRecord => awbRecord.AwbSerialNumber)%>
          <%:Html.HiddenFor(awbRecord => awbRecord.AwbCheckDigit)%>
          <%:Html.HiddenFor(awbRecord => awbRecord.OtherChargeVatSumAmount)%>
          <%:Html.HiddenFor(awbRecord => awbRecord.OtherChargeCount)%>
        </div>
        <%--<div>
          <label >
            Check Digit:</label>
          <%:Html.TextBoxFor(awbRecord => awbRecord.AwbCheckDigit, new { @class = "integer populated", @maxlength = 2 })%>
        </div>--%>
        <div>
          <label for="flightDate">
             AWB Issuing Date:</label>
          <%: Html.TextBox(ControlIdConstants.AwbDate, (Model.AwbDate.HasValue == true ? Model.AwbDate.Value.ToString(FormatConstants.DateFormat) : string.Empty), new { @class = "datePicker" })%>
          <%-- TODO: Add watermark for MM-YY--%>
        </div>
      </div>
      <div class="fieldSeparator">
      </div>
      <div>
        <div>
          <label for="FromAirportOfAwb">
             Origin:</label>
          <%:Html.TextBoxFor(awbRecord => awbRecord.ConsignmentOriginId, new { @class = "upperCase", maxLength = 4 })%>
        </div>
        <div>
          <label for="ToAirportOfAwb">
             Destination:</label>
          <%:Html.TextBoxFor(awbRecord => awbRecord.ConsignmentDestinationId, new { @class = "upperCase", maxLength = 4 })%>
        </div>
        <div>
          <label for="FromAirportOfAwb">
            <span class="required">*</span> From:</label>
          <%:Html.TextBoxFor(awbRecord => awbRecord.CarriageFromId, new { @class = "upperCase", maxLength = 4 })%>
        </div>
        <div>
          <label for="ToAirportOfAwb">
            <span class="required">*</span> To (or point of Transfer):</label>
          <%:Html.TextBoxFor(awbRecord => awbRecord.CarriageToId, new { @class = "upperCase", maxLength = 4 })%>
        </div>
        <div>
          <label for="flightDate">
           <span class="required">*</span> Date Of Carriage(or Transfer):</label>
          <%: Html.TextBox(ControlIdConstants.TransferDate, (Model.TransferDate.HasValue == true ? Model.TransferDate.Value.ToString(FormatConstants.DateFormat) : string.Empty), new { @class = "datePicker" })%>
          <%-- TODO: Add watermark for MM-YY--%>
        </div>
      </div>
      <div class="fieldSeparator">
      </div>
      <div>
        <div>
         <label>Weight Charges:</label>
         <%:Html.TextBoxFor(awbRecord => awbRecord.BilledWeightCharge, new { watermark = ControlIdConstants.PositiveAmount, @class = "amount amt_11_3" })%>
        </div>
        <div>
         <label>Valuation Charges:</label>
         <%:Html.TextBoxFor(awbRecord => awbRecord.BilledValuationCharge, new { @class = "amount amt_11_3", watermark = ControlIdConstants.PositiveAmount })%>
        </div>
        <div>
         <label>Amount Subjected To ISC:</label>
          <%:Html.TextBoxFor(awbRecord => awbRecord.BilledAmtSubToIsc, new { @class = "amountTextfield amount amt_11_3", watermark = ControlIdConstants.PositiveAmount })%>
        </div>
        <div>
         <label for="IscPercent">
         ISC </label>
         <%:Html.TextBoxFor(awbRecord => awbRecord.BilledIscPercentage, new { @class = "percentageTextfield percent amt_5_3",  watermark = ControlIdConstants.PositiveAmount })%>&nbsp;%&nbsp;
         <%:Html.TextBoxFor(awbRecord => awbRecord.BilledIscAmount, new { @class = "amountTextfield amount amt_11_3", @readOnly = true, watermark = ControlIdConstants.PositiveAmount })%>&nbsp;Amount
        </div>
       <%-- <div>
         <label>ISC Amount:</label>
         <%:Html.TextBoxFor(awbRecord => awbRecord.BilledIscAmount, new { @class = "amountTextfield amount", @readOnly = true, min = -99999999999.99, max = 99999999999.99, watermark = ControlIdConstants.PositiveAmount })%>
        </div>--%>
      </div>
      <div>
      <div>
         <label>Other Charges:</label>
          <%:Html.TextBoxFor(awbRecord => awbRecord.BilledOtherCharge, new { id = "OtherChargeAmount", @class = "amountTextfield amount amt_11_3", @readOnly = true })%>
        <%:ScriptHelper.GenerateDialogueHtml("OC Breakdown", "AWB Charge Collect Other Charge Capture", "divOtherCharge", 500, 900)%>
        </div>
        <div>
         <label>VAT Amount: </label>
         <%:Html.TextBoxFor(awbRecord => awbRecord.BilledVatAmount, new { id = "VatAmount", @class = "amountTextfield amount amt_11_3", @readOnly = true })%>
        <%:ScriptHelper.GenerateDialogueHtml("VAT Breakdown", "AWB VAT Capture", "divVatBreakdown", 500, 900, "vatBreakdown")%>
        </div>
        <div>
         <label><span class="required">*</span> Total Amount Billed:</label>
         <%:Html.TextBoxFor(awbRecord => awbRecord.TotalAmount, new { @readOnly = true, @class = "amount pos_amt_11_3", watermark = ControlIdConstants.PositiveAmount })%>
        </div>
      </div>
      <div class="fieldSeparator">
      </div>
      <div>
        <div>
          <label  >
            Currency Adjustment Indicator:</label>
          <%:Html.TextBoxFor(awbRecord => awbRecord.CurrencyAdjustmentIndicator, new { @class = "alphabetsOnly upperCase", maxLength = 3 })%>
        </div>
        <div>
          <label  >
            Billed Weight:</label>
          <%:Html.TextBoxFor(awbRecord => awbRecord.BilledWeight, new { @maxlength = 6, @class = "integer populated", min = 0, max = 999999 })%>
        </div>
        <div>
          <label >
            Proviso/Req/SPA:</label>
          <%--<%:Html.TextBoxFor(awbRecord => awbRecord.ProvisionalReqSpa, new { @class = "alphabets upperCase", maxLength = 1 })%>--%>
          <%:Html.ProvisoreqspaDropdownList(awbRecord => awbRecord.ProvisionalReqSpa)%>
        </div>
        <div>
          <label >
            Prorate %:</label>
          <%:Html.TextBoxFor(awbRecord => awbRecord.PrpratePercentage, new { @class = "integer populated", @maxlength = 2, min = 1, max = 99, watermark = ControlIdConstants.PositiveAmount })%>
        </div>
        <div>
          <label >
            KG/LB Indicator:</label>
           <%:Html.KgLbIndDropdownList(awbRecord => awbRecord.KgLbIndicator)%>
        </div>
      </div>
      <div>
        <div>
          <label  >
          Part Shipment:</label>
          <%:Html.CheckBox("chkPartShipment", (Model.PartShipmentIndicator=="P"?true:false))%>
          <%:Html.HiddenFor(awbRecord => awbRecord.PartShipmentIndicator, new { @class = "hidden", maxLength = 1 })%>
        </div>
        <div>
          <label for="CCAIndicator">
            CCA Indicator:</label>
          <%:Html.CheckBoxFor(awbRecord => awbRecord.CcaIndicator, new { @class = "alphabets upperCase", maxLength = 1 })%>
        </div>
        <div>
          <label for="filingReference">
            Filing Reference:</label>
          <%:Html.TextBoxFor(awbRecord => awbRecord.FilingReference, new { maxLength = 10 })%>
        </div>
      </div>
      <div class="fieldSeparator">
      </div>
      <div id="divReasonCode">
        <div>
          <label for="reasonCode">
            Reason Code:</label>
          <%--SCP#449352:Issue with Reason Code field--%>
          <%:Html.TextBoxFor(awbRecord => awbRecord.ReasonCode, new { @class = "alphabets upperCase", maxLength = 2 })%>
        </div>
        <div>
          <label for="ReferenceField1">
             Reference Field 1:</label>
          <%:Html.TextBoxFor(awbRecord => awbRecord.ReferenceField1, new { maxLength = 10 })%>
        </div>
        <div>
          <label for="ReferenceField2">
            Reference Field 2:</label>
          <%:Html.TextBoxFor(awbRecord => awbRecord.ReferenceField2, new { maxLength = 10 })%>
        </div>
        <div>
          <label for="ReferenceField3">
             Reference Field 3:</label>
          <%:Html.TextBoxFor(awbRecord => awbRecord.ReferenceField3, new { maxLength = 10 })%>
        </div>
        <div>
          <label for="ReferenceField4">
             Reference Field 4:</label>
          <%:Html.TextBoxFor(awbRecord => awbRecord.ReferenceField4, new { maxLength = 10 })%>
        </div>
        <div>
          <label for="ReferenceField5">
            <%--CMP#658--%>
        Weight Indicator (Reference Field 5):</label>
          <%:Html.TextBoxFor(awbRecord => awbRecord.ReferenceField5, new { maxLength = 20 })%>
        </div>
        <div>
          <label for="airlineOwnUse">
            Airline Own Use:</label>
          <%:Html.TextBoxFor(awbRecord => awbRecord.AirlineOwnUse, new { maxLength = 20 })%>
        </div>
      </div>
       <div class="fieldSeparator">
    </div>
    <div>
    <div>
      <label for="attachmentIndicatorOriginal">
        Attachment Indicator-Original:
      </label>
      <%: Html.AttachmentIndicatorTextBox(ControlIdConstants.AttachmentIndicatorOriginal, Model.AttachmentIndicatorOriginal)%>
      <a class="ignoredirty" id="attachmentBreakdown" href="#" onclick="return openAttachment();">Attachment</a>
    </div>
    <div>
        <%:ScriptHelper.GenerateDialogueHtml("Prorate Ladder", "Add/Edit Prorate Ladder Details", "divProrateLadder", 510, 970, "prorateLadder")%>
        <%: Html.HiddenFor(awb => awb.TotalProrateAmount) %>
        <%: Html.HiddenFor(awb => awb.ProrateCalCurrencyId)%>
      </div>
    </div>

    <%if (ViewData[ViewDataConstants.BillingType].ToString() == BillingType.Payables) {%>
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
          <%:Html.TextBoxFor(awb => awb.NumberOfAttachments)%>
        </div>
        <div>
          <label for="isValidationFlag">
            IS Validation Flag:</label>
          <%:Html.TextBoxFor(awb => awb.ISValidationFlag)%>
        </div>
      </div>
    <% }%>

    </div>
    <div class="clear">
    </div>
  </div>
 <div id="childVatList" class="hidden">
</div>
<div id="childAttachmentList" class="">
</div>
<div id="childOtherChargeList" class="hidden">
</div>
<div id="childProrateLadderList" class="hidden">
</div>

