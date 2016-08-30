<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.PrimeCoupon>" %>
<%@ Import Namespace="Iata.IS.Web.Util" %>
<h2>
  Prime Billing Details</h2>
<div class="solidBox dataEntry">
  <div class="fieldContainer horizontalFlow">
    <div id="divBlock1">
      <div>
        <label for="SourceCodeId">
          <span>*</span> Source Code:</label>
        <%:Html.TextBoxFor(couponRecord => couponRecord.SourceCodeId, new { @class = "autocComplete populated" })%>
      </div>
      <div>
        <label for="BatchSequenceNumber">
          <span>*</span> Batch Number:</label>
        <%:Html.TextBoxFor(couponRecord => couponRecord.BatchSequenceNumber, new { @class = "digits integer populated", @maxLength = 5 })%>
      </div>
      <div>
        <label for="RecordSequenceWithinBatch">
          <span>*</span> Sequence Number:</label>
        <%:Html.TextBoxFor(couponRecord => couponRecord.RecordSequenceWithinBatch, new { max = 99999, min = 0, @class = "digits integer populated", @maxLength = 5 })%>
      </div>
    </div>
    <div>
      <div>
        <label for="TicketOrFimIssuingAirline">
          <span>*</span> Issuing Airline:</label>
        <%:Html.TextBoxFor(couponRecord => couponRecord.TicketOrFimIssuingAirline, new { @class = "autocComplete populated", maxlength = 4 })%>
      </div>
      <div>
        <label for="couponNumber">
          <span>*</span> Coupon Number:</label>
        <%:Html.CouponNumberDropdownList("TicketOrFimCouponNumber", Model.TicketOrFimCouponNumber)%>
      </div>
      <div>
        <label for="TicketDocOrFimNumber">
          <span>*</span> Ticket/FIM Number:</label>
          <%--CMP # 480 : Data Issue-11 Digit Ticket FIM Numbers Being Captured--%>
        <%:Html.TextBoxFor(couponRecord => couponRecord.TicketDocOrFimNumber, new { max = 9999999999, min = 0, @class = "digits integer", @maxLength = 10 })%>
      </div>
      <div>
        <label for="checkDigit">
          <span>*</span> Check Digit:</label>
        <%:Html.TextBoxFor(couponRecord => couponRecord.CheckDigit, new { @class = "checkDigit", @maxLength = 1 })%>
      </div>
    </div>
  <div class="fieldSeparator">
  </div>
  <div>
    <div>
      <label for="CouponGrossValueOrApplicableLocalFare">
        Coupon Gross Value:</label>
      <%:Html.TextBoxFor(couponRecord => couponRecord.CouponGrossValueOrApplicableLocalFare, new { min = 0, max = 99999999999.9, watermark = ControlIdConstants.PositiveAmount, @class = "amount" })%>
    </div>
    <div>
      <label for="IscPercent">
        ISC:</label>
      <%:Html.TextBoxFor(couponRecord => couponRecord.IscPercent, new { @class = "percentageTextfield percent", min = -99.999, max = 99.999, watermark = ControlIdConstants.NegativePercentage })%>&nbsp;%&nbsp;
      <%:Html.TextBoxFor(couponRecord => couponRecord.IscAmount, new { @class = "amountTextfield amount", min = -99999999999.99, max = 99999999999.99, watermark = ControlIdConstants.NegativeAmount })%>&nbsp;Amount
    </div>
    <div>
      <label for="taxAmount">
        Tax Amount:</label>     
      <%:Html.TextBoxFor(couponRecord => couponRecord.TaxAmount, new { @class = "amountTextfield amount", @readOnly = true, min = -99999999999.9, max = 99999999999.9 })%>
      <%--CMP #672: Validation on Taxes in PAX FIM Billings--%>
      <span id="EnabletaxAmountLink"><%:ScriptHelper.GenerateDialogueHtml("Tax Breakdown", "Prime Billing Tax Capture", "divTaxBreakdown", 500, 500)%></span>
      <span id="DisabletaxAmountLink"></span>
    </div>
    <div>
      <label for="handlingFeeType">
        Handling Fee Type:</label>
      <%:Html.HandlingFeeTypeDropdownList(ControlIdConstants.HandlingFeeTypeId, Model.HandlingFeeTypeId)%>
    </div>
    <div>
      <label for="handlingFeeAmount">
        Handling Fee Amount:</label>
      <%:Html.TextBoxFor(couponRecord => couponRecord.HandlingFeeAmount, new { min = -999999999.9, max = 999999999.9, watermark = ControlIdConstants.NegativeAmount, @class = "amount" })%>
    </div>
  </div>
  <div>
    <div>
      <label for="OtherCommissionPercent">
        Other Commission:</label>
      <%:Html.TextBoxFor(couponRecord => couponRecord.OtherCommissionPercent, new { @class = "percentageTextfield percent", max = 99.999, min = -99.999, watermark = ControlIdConstants.NegativePercentage })%>&nbsp;%&nbsp;
      <%:Html.TextBoxFor(couponRecord => couponRecord.OtherCommissionAmount, new { @class = "amountTextfield amount", min = -99999999999.99, max = 99999999999.99, watermark = ControlIdConstants.NegativeAmount })%>&nbsp;Amount
    </div>
    <div>
      <label for="UatpPercent">
        UATP:</label>
      <%:Html.TextBoxFor(couponRecord => couponRecord.UatpPercent, new { @class = "percentageTextfield percent", max = 99.999, min = -99.999, watermark = ControlIdConstants.NegativePercentage })%>&nbsp;%&nbsp;
      <%:Html.TextBoxFor(couponRecord => couponRecord.UatpAmount, new { @class = "amountTextfield amount", min = -99999999999.99, max = 99999999999.99, watermark = ControlIdConstants.NegativeAmount })%>&nbsp;Amount
    </div>
    <div>
      <label for="vatAmount">
        VAT Amount:</label>
      <%:Html.TextBoxFor(couponRecord => couponRecord.VatAmount, new { @class = "amountTextfield amount", @readOnly = true, min = -99999999999.9, max = 99999999999.9 })%>
      <%:ScriptHelper.GenerateDialogueHtml("VAT Breakdown", "Prime Billing VAT Capture", "divVatBreakdown", 500, 900)%>
    </div>
    <div>
      <label for="couponTotalAmount">
        Coupon Total Amount:</label>
      <%:Html.TextBoxFor(couponRecord => couponRecord.CouponTotalAmount, new { @readOnly = true, min = 0, max = 99999999999.9, @class = "amount" })%>
    </div>
    <div>
      <label for="surchargeAmount">
        Surcharge Amt. (included within fare):</label>
      <%:Html.TextBoxFor(couponRecord => couponRecord.SurchargeAmount, new { min = -99999999999.9, max = 99999999999.9, watermark = ControlIdConstants.PositiveAmount, @class = "amount" })%>
    </div>
  </div>
  <div>
    <div>
      <label for="currencyAdjustmentIndicator">
        <span>*</span> Currency Adjustment Indicator:</label>
      <%:Html.TextBoxFor(couponRecord => couponRecord.CurrencyAdjustmentIndicator, new { @class = "alphabetsOnly", maxLength = 3 })%>
    </div>
    <div>
      <label for="ElectronicTicketIndicator">
        E-Ticket Indicator:</label>
      <%:Html.CheckBoxFor(couponRecord => couponRecord.ElectronicTicketIndicator)%>
    </div>
    <div>
      <label for="settlementAuthorizationCode">
        ESAC:</label>
      <%:Html.TextBoxFor(couponRecord => couponRecord.SettlementAuthorizationCode, new { @maxLength = 14 })%>
    </div>
  </div>
  <div class="fieldSeparator">
  </div>
  <div id="divFlightDetails">
    <div>
      <label for="airlineFlightDesignator">
        Airline Flight Designator:</label>
      <%:Html.TextBoxFor(couponRecord => couponRecord.AirlineFlightDesignator, new { @class = "alphabets upperCase", maxLength = 2 })%>
    </div>
    <div>
      <label for="flightNumber">
        Flight Number:</label>
      <%:Html.TextBoxFor(couponRecord => couponRecord.FlightNumber, new { @class = "integer", maxLength = 5 })%>
    </div>
    <div>
      <label for="flightDate">
        Flight Date:</label>
      <%: Html.TextBox(ControlIdConstants.FlightDate, (Model.FlightDate.HasValue == true ? Model.FlightDate.Value.ToString(FormatConstants.DateFormat) : string.Empty), new { @class = "datePicker" })%>
      <%-- TODO: Add watermark for MM-YY--%>
    </div>
    <div>
      <label for="FromAirportOfCoupon">From Airport:</label>
      <%:Html.TextBoxFor(couponRecord => couponRecord.FromAirportOfCoupon, new { @class = "upperCase", maxLength = 4 })%>
    </div>
    <div>
      <label for="ToAirportOfCoupon">To Airport:</label>
      <%:Html.TextBoxFor(couponRecord => couponRecord.ToAirportOfCoupon, new { @class = "upperCase", maxLength = 4 })%>
    </div>
    <div>
      <label for="cabinClass">
        Cabin Class:</label>
      <%:Html.TextBoxFor(couponRecord => couponRecord.CabinClass, new { @class = "alphabet upperCase", maxLength = 1 })%>
    </div>
    <div>
      <label for="filingReference">
        Filing Reference:</label>
      <%:Html.TextBoxFor(couponRecord => couponRecord.FilingReference, new { maxLength = 10 })%>
    </div>
  </div>
  <div class="fieldSeparator">
  </div>
  <div id="divProrateDetails">
    <div>
      <label for="AgreementIndicatorSupplied">
        Agreement Indicator:</label>
      <%:Html.TextBoxFor(couponRecord => couponRecord.AgreementIndicatorSupplied, new { @class = "alphabets upperCase", maxLength = 2 })%>
    </div>
    <div>
      <label for="originalPMI">
        Original PMI:</label>
      <%:Html.TextBoxFor(couponRecord => couponRecord.OriginalPmi, new { @class = "alphabet upperCase", maxLength = 1 })%>
    </div>
    <div>
      <label for="prorateMethodology">
        Prorate Methodology:</label>
      <%:Html.TextBoxFor(couponRecord => couponRecord.ProrateMethodology, new { maxLength = 3 })%>
    </div>
    <div style="display: none">
      <label for="nfpReasonCode">
        NFP Reason Code:</label>
      <%:Html.TextBoxFor(couponRecord => couponRecord.NfpReasonCode, new { maxLength = 2 })%>
    </div>
  </div>
  <div class="fieldSeparator">
  </div>
  <div id="divReasonCode">
    <div>
      <label for="reasonCode">
        Reason Code:</label>
      <%:Html.TextBoxFor(couponRecord => couponRecord.ReasonCode, new { maxLength = 2 })%>
    </div>
    <div>
      <label for="airlineOwnUse">
        Airline Own Use:</label>
      <%:Html.TextBoxFor(couponRecord => couponRecord.AirlineOwnUse, new { maxLength = 20 })%>
    </div>
    <div>
      <label for="ReferenceField1">
        Reference Field 1:</label>
      <%:Html.TextBoxFor(couponRecord => couponRecord.ReferenceField1, new { maxLength = 10 })%>
    </div>
    <div>
      <label for="ReferenceField2">
        Reference Field 2:</label>
      <%:Html.TextBoxFor(couponRecord => couponRecord.ReferenceField2, new { maxLength = 10 })%>
    </div>
    <div>
      <label for="ReferenceField3">
        Reference Field 3:</label>
      <%:Html.TextBoxFor(couponRecord => couponRecord.ReferenceField3, new { maxLength = 10 })%>
    </div>
    <div>
      <label for="ReferenceField4">
        Reference Field 4:</label>
      <%:Html.TextBoxFor(couponRecord => couponRecord.ReferenceField4, new { maxLength = 10 })%>
    </div>
    <div>
      <label for="ReferenceField5">
        Reference Field 5:</label>
      <%:Html.TextBoxFor(couponRecord => couponRecord.ReferenceField5, new { maxLength = 20 })%>
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
      <a class="ignoredirty" href="#" onclick="return openAttachment();">Attachment</a>
    </div>
  </div>
  <%if (ViewData[ViewDataConstants.BillingType].ToString() == BillingType.Payables)
    {%>
  <div class="fieldSeparator" id="payablesSeparatorDiv">
  </div>
  <div id="divPayables">
    <div>
      <label for="validatedPMI">
        Validated PMI:</label>
      <%:Html.TextBoxFor(couponRecord => couponRecord.ValidatedPmi)%>
    </div>
    <div>
      <label for="agreementIndicatorValidated">
        Agreement Indicator Validated:</label>
      <%:Html.TextBoxFor(couponRecord => couponRecord.AgreementIndicatorValidated)%>
    </div>
    <div>
      <label for="attachmentIndicatorValidated">
        Attachment Indicator - Validated:</label>
      <%:Html.TextBox(ControlIdConstants.AttachmentIndicatorValidated, Model.AttachmentIndicatorValidated == true ? "Yes" : "No")%>
    </div>
    <div>
      <label for="numberOfAttachments">
        Number Of Attachments:</label>
      <%:Html.TextBoxFor(couponRecord => couponRecord.NumberOfAttachments)%>
    </div>
    <div>
      <label for="isValidationFlag">
        IS Validation Flag:</label>
      <%:Html.TextBoxFor(couponRecord => couponRecord.ISValidationFlag)%>
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
 <%--CMP #672: Validation on Taxes in PAX FIM Billings--%>
<div id="promtUser" class="hidden"><p>Billing of taxes is not allowed for FIM (Source Code 14) transactions, even with a zero value. Click ‘OK’ to allow the system to proceed with remaining validations and remove/delete the tax breakdown information while saving the FIM. Else click ‘Cancel’ to abort the save operation of the FIM.</p></div>