<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Cargo.AwbRecord>" %>
<h2>
  AWB Charge Collect Billing Details</h2>
<div class="solidBox dataEntry">
  <div class="fieldContainer horizontalFlow">
    <div id="divBlock1">
      <div>
        <label for="BatchSequenceNumber">
          <span>*</span> Batch Number:</label>
        <%:Html.TextBoxFor(awbRecord => awbRecord.BatchSequenceNumber, new { @class = "integer populated", @maxLength = 5 })%>
      </div>
      <div>
        <label for="RecordSequenceWithinBatch">
          <span>*</span> Sequence Number:</label>
        <%:Html.TextBoxFor(awbRecord => awbRecord.RecordSequenceWithinBatch, new { @class = "integer populated", @maxLength = 5 })%>
      </div>
      <div>
        <label for="IssuingAirline">
          <span>*</span> Issuing Airline:</label>
        <%:Html.TextBoxFor(awbRecord => awbRecord.AwbIssueingAirline, new { @class = "autocComplete populated", maxlength = 4 })%>
      </div>
      <div>
        <label for="awbserialnumber">
          <span>*</span> AWB Serial Number & Check Digit:</label>
        <%-- <%:Html.CouponNumberDropdownList("AWBSerialNumber", Model.AwbSerialNumber)%>--%>
        <%:Html.TextBoxFor(awbRecord => awbRecord.AwbSerialNumber, new { @class = "", @maxLength = 8, @minLength = 8 })%>
      </div>
      <div>
        <label for="AWBDate">
          <span>*</span>AWB Date:</label>
        <%: Html.TextBox(ControlIdConstants.AwbDate, (Model.AwbDate.HasValue == true ? Model.AwbDate.Value.ToString(FormatConstants.DateFormat) : string.Empty), new { @class = "datePicker" })%>
        <%-- TODO: Add watermark for MM-YY--%>
      </div>
    </div>
    <div class="fieldSeparator">
    </div>
    <div>
      <div>
        <label for="Weight Charges">
          Weight Charges:</label>
        <%-- <%:Html.TextBoxFor(awbRecord => awbRecord.OtherCommissionPercent, new { @class = "percentageTextfield percent", max = 99.999, min = -99.999, watermark = ControlIdConstants.NegativePercentage })%>&nbsp;%&nbsp;--%>
        <%:Html.TextBoxFor(awbRecord => awbRecord.WeightCharges, new { watermark = ControlIdConstants.PositiveAmount, @class = " amount amt_11_3" })%>
      </div>
      <div>
        <label for="ValuationCharges">
          Valuation Charges:</label>
        <%--<%:Html.TextBoxFor(awbRecord => awbRecord.UatpPercent, new { @class = "percentageTextfield percent", max = 99.999, min = -99.999, watermark = ControlIdConstants.NegativePercentage })%>&nbsp;%&nbsp;--%>
        <%:Html.TextBoxFor(awbRecord => awbRecord.ValuationCharges, new { @class = "amountTextfield amount amt_11_3", watermark = ControlIdConstants.PositiveAmount })%>
      </div>
      <div>
        <label for="Weight Charges Sub To ISC">
          Amount Subject to ISC:</label>
        <%:Html.TextBoxFor(awbRecord => awbRecord.AmountSubjectToIsc, new { watermark = ControlIdConstants.PositiveAmount, @class = " amount amt_11_3" })%>
      </div>
      <div>
        <label for="IscPercent">
          ISC:</label>
        <%:Html.TextBoxFor(awbRecord => awbRecord.IscPer, new { @class = "percentageTextfield percent", min = -99.999, max = 99.999, watermark = ControlIdConstants.Percentage })%>&nbsp;%&nbsp;
        <%:Html.TextBoxFor(awbRecord => awbRecord.IscAmount, new { @class = "amountTextfield  amount amt_11_3", @readOnly = true, watermark = ControlIdConstants.PositiveAmount })%>&nbsp;Amount
      </div>
    </div>
    <div>
      <div>
        <label for="othercharges">
          Other Charges:</label>
        <%:Html.TextBoxFor(awbRecord => awbRecord.OtherCharges, new { @class = "amountTextfield  amount amt_11_3", @readOnly = true, @id = "OtherChargeAmount", watermark = ControlIdConstants.PositiveAmount })%>
        <%:ScriptHelper.GenerateDialogueHtml("OC Breakdown", "Capture AWB Other Charges Breakdown", "divOtherCharge", 500, 900)%>
      </div>
      <div>
        <label for="vatAmount">
          VAT Amount:</label>
        <%:Html.TextBoxFor(awbRecord => awbRecord.VatAmount, new { @class = "amountTextfield  amount amt_11_3", @readOnly = true })%>
        <%:ScriptHelper.GenerateDialogueHtml("VAT Breakdown", "Capture AWB VAT Breakdown", "divVatBreakdown", 500, 900, "vatBreakdown")%>
      </div>
      <div>
        <label for="prepaidTotalAmount">
          AWB Total Amount:</label>
        <%:Html.TextBoxFor(awbRecord => awbRecord.AwbTotalAmount, new { @readOnly = true, @class = " amount pos_amt_11_3" })%>
      </div>
      <%--<div>
      <label for="surchargeAmount">
        Surcharge Amt. (included within fare):</label>
      <%:Html.TextBoxFor(awbRecord => awbRecord.SurchargeAmount, new { min = -99999999999.9, max = 99999999999.9, watermark = ControlIdConstants.PositiveAmount, @class = "amount" })%>
    </div>--%>
    </div>
    <div class="fieldSeparator">
    </div>
    <div id="divFlightDetails">
      <div>
        <label for="FromAirportOfAwb">
          <span>*</span>Origin:</label>
        <%:Html.TextBoxFor(awbRecord => awbRecord.ConsignmentOriginId, new { @class = "upperCase", maxLength = 4 })%>
      </div>
      <div>
        <label for="ToAirportOfAwb">
          <span>*</span>Destination:</label>
        <%:Html.TextBoxFor(awbRecord => awbRecord.ConsignmentDestinationId, new { @class = "upperCase", maxLength = 4 })%>
      </div>
      <div>
        <label for="FromAirportOfAwb">
          <span>*</span>From :</label>
        <%:Html.TextBoxFor(awbRecord => awbRecord.CarriageFromId, new { @class = "upperCase", maxLength = 4 })%>
      </div>
      <div>
        <label for="ToAirportOfAwb">
          <span>*</span>To (or Point of Transfer):</label>
        <%:Html.TextBoxFor(awbRecord => awbRecord.CarriageToId, new { @class = "upperCase", maxLength = 4 })%>
      </div>
      <div>
        <label for="flightDate">
          <span>*</span>Date Of Carriage(or Transfer):</label>
        <%: Html.TextBox(ControlIdConstants.DateOfCarriage, (Model.DateOfCarriage.HasValue == true ? Model.DateOfCarriage.Value.ToString(FormatConstants.DateFormat) : string.Empty), new { @class = "datePicker" })%>
        <%-- TODO: Add watermark for MM-YY--%>
      </div>
    </div>
    <div class="fieldSeparator">
    </div>
    <div>
      <div>
        <label for="currencyAdjustmentIndicator">
          <span>*</span> Currency Adjustment Indicator:</label>
        <%:Html.TextBoxFor(awbRecord => awbRecord.CurrencyAdjustmentIndicator, new { @class = "alphabetsOnly", maxLength = 3 })%>
      </div>
      <div>
        <label for="billedWeight">
          Billed Weight:</label>
        <%:Html.TextBoxFor(awbRecord => awbRecord.BilledWeight, new { @class = "integer", maxlength = 6 })%>
      </div>
      <div>
        <label for="KGLBIndicator">
          KG/LB Indicator:</label>
        <%:Html.KgLbIndDropdownList(awbRecord => awbRecord.KgLbIndicator)%>
      </div>
      <div>
        <label for="billedWeight">
          Proviso/Req/SPA:</label>
        <%--<%:Html.TextBoxFor(awbRecord => awbRecord.ProvisoReqSpa, new { @class = "alphabetsOnly", maxLength = 1 })%>--%>
        <%:Html.ProvisoreqspaDropdownList(awbRecord => awbRecord.ProvisoReqSpa)%>
      </div>
      <div>
        <label for="billedWeight">
          Prorate %:</label>
        <%:Html.TextBoxFor(awbRecord => awbRecord.ProratePer, new { @class = "integer", maxlength = 2, min = 1, max = 99.99 })%>
      </div>   
    </div>
    <div>
      <div>
        <label for="partshipment">
          Part Shipment:</label>
         <%:Html.CheckBox("chkPartShipment", (Model.PartShipmentIndicator=="P"?true:false))%>
        <%:Html.HiddenFor(awbRecord => awbRecord.PartShipmentIndicator, new { @class = "hidden", maxLength = 1 })%>
      </div>
      <div>
        <label for="CCAIndicator">
          CCA Indicator:</label>
        <%:Html.CheckBoxFor(awbRecord => awbRecord.CcaIndicator)%>
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
      <label for="ourRefNumber">
        Our Reference:</label>
      <%:Html.TextBoxFor(awbRecord => awbRecord.OurReference, new { maxLength = 20 })%>
    </div>
    <div>
      <label for="reasonCode">
        Reason Code:</label>
      <%:Html.TextBoxFor(awbRecord => awbRecord.ReasonCode, new { @class = "alphaNumeric upperCase", maxLength = 2 })%>
    </div>
    <div>
      <label for="airlineOwnUse">
        Airline Own Use:</label>
      <%:Html.TextBoxFor(awbRecord => awbRecord.AirlineOwnUse, new { maxLength = 20 })%>
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
      <%:Html.HiddenFor(awbRecord => awbRecord.OtherChargeVatSumAmount)%>
      <%:Html.HiddenFor(awbRecord => awbRecord.OtherChargeCount)%>
    </div>
  </div>
  <div class="fieldSeparator">
  </div>
  <div id="divAttachments">
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
        Attachment Indicator:
      </label>
      <%} %>
      <%: Html.AttachmentIndicatorTextBox(ControlIdConstants.AttachmentIndicatorOriginal, Model.AttachmentIndicatorOriginal)%>
      <a class="ignoredirty" id="attachmentBreakdown" href="#" onclick="return openAttachment();">Attachment</a>
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
      <%:Html.TextBoxFor(awbRecord => awbRecord.NumberOfAttachments)%>
    </div>
    <div>
      <label for="isValidationFlag">
        IS Validation Flag:</label>
      <%:Html.TextBoxFor(awbRecord => awbRecord.ISValidationFlag)%>
    </div>
  </div>
  <%
    }%>
</div>
<div class="clear">
</div>
</div>
<div id="childTaxList" class="hidden">
</div>
<div id="childVatList" class="hidden">
</div>
<div id="childAttachmentList" class="">
</div>
<div id="childOtherChargeList" class="hidden">
</div>
