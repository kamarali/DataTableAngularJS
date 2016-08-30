<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.CMCoupon>" %>

<div class="solidBox dataEntry">
  <div class="fieldContainer horizontalFlow">
    <div>
      <div id="serialNoDiv">
        <label for="SerialNo">Serial Number:</label>
         <%:Html.TextBoxFor(cmCoupon => cmCoupon.SerialNo, new { @readOnly = true })%>
      </div>
      <div>
        <label for="issuingAirline">
          <span>*</span> Ticket Issuing Airline:</label>
         <%:Html.TextBoxFor(cmCoupon => cmCoupon.TicketOrFimIssuingAirline, new { @class = "autocComplete populated", maxlength = 4 })%>
      </div>
      <div>
        <label for="couponNumber">
          <span>*</span> Coupon No.:</label>
        <%:Html.NonFimCouponNumberDropdownList("TicketOrFimCouponNumber", Model.TicketOrFimCouponNumber)%>
      </div>
      <div>
        <label for="ticketNumber">
          <span>*</span> Ticket/Document No.:</label>
          <%--CMP # 480 : Data Issue-11 Digit Ticket FIM Numbers Being Captured--%>
        <%: Html.TextBoxFor(cmCoupon => cmCoupon.TicketDocOrFimNumber, new { max = 9999999999, min = 0, @class = "digits integer", maxLength = 10 })%>
      </div>      
      <div>
        <label for="checkDigit">
          <span>*</span> Check Digit:</label>
        <%:Html.TextBoxFor(cmCoupon => cmCoupon.CheckDigit, new { @class = "checkDigit", maxLength = 1 })%>
      </div>
    </div>
    <div>
      <div>
        <label for="eTicketIndicator">
          Electronic Ticket Indicator:</label>
        <%: Html.CheckBoxFor(cmCoupon => cmCoupon.ElectronicTicketIndicator)%>
      </div>
      <div>
        <label for="settlementAuthorizationCode">
          ESAC:</label>
        <%: Html.TextBoxFor(cmCoupon => cmCoupon.SettlementAuthorizationCode, new { maxLength = 14 })%>
      </div>
    </div>
    <div class="fieldSeparator">
    </div>
    <div>
      <div>
        <label for="couponGross">
         <span>*</span> Gross Amount:</label>
        <%: Html.TextBoxFor(m => m.GrossAmountCredited, new { min = -999999999.90, max = 999999999.90, watermark = ControlIdConstants.PositiveAmount, @class = "amount" })%>
      </div>
      <div>
        <label for="isc">
          ISC:</label>
        <%: Html.TextBoxFor(m => m.IscPercent, new { @class = "percentageTextfield percent", max = 99.999, min = -99.999, watermark = ControlIdConstants.NegativePercentage })%>&nbsp;%&nbsp;
        <%: Html.TextBoxFor(m => m.IscAmountBilled, new { @class = "amountTextfield amount", @readOnly = true, min = -99999999999.9, max = 99999999999.9 })%>&nbsp;Amount
      </div>
      <div>
        <label for="taxAmount">
          TAX Amount:</label>
        <%: Html.TextBoxFor(m => m.TaxAmount, new { @class = "amountTextfield amount", @readOnly = true, min = -99999999999.9, max = 99999999999.9 })%>
        <%: ScriptHelper.GenerateDialogueHtml("Tax Breakdown", "Credit Memo Coupon Tax Capture", "divTaxBreakdown", 500, 500)%>
      </div>
      <div>
        <label for="handlingFeeAmount">
          Handling Fee:</label>
        <%: Html.TextBoxFor(m => m.HandlingFeeAmount, new { min = -999999999.9, max = 999999999.9, watermark = ControlIdConstants.PositiveAmount, @class = "amount" })%>
      </div>
    </div>
    <div>
      <div>
        <label for="otherCommission">
          Other Commission:</label>
        <%: Html.TextBoxFor(m => m.OtherCommissionPercent, new { @class = "percentageTextfield percent", min = -99.999, max = 99.999, watermark = ControlIdConstants.NegativePercentage })%>&nbsp;%&nbsp;
        <%: Html.TextBoxFor(m => m.OtherCommissionBilled, new { @class = "amountTextfield amount", min = -99999999999.9, max = 99999999999.9, watermark = ControlIdConstants.PositiveAmount })%>&nbsp;Amount
      </div>
      <div>
        <label for="uatp">
          UATP:</label>
        <%: Html.TextBoxFor(m => m.UatpPercent, new { @class = "percentageTextfield percent", min = -99.999, max = 99.999, watermark = ControlIdConstants.NegativePercentage })%>&nbsp;%&nbsp;
        <%: Html.TextBoxFor(m => m.UatpAmountBilled, new { @class = "amountTextfield amount", @readOnly = true, min = -99999999999.9, max = 99999999999.9 })%>&nbsp;Amount
      </div>
      <div>
        <label for="vatAmount">
          VAT Amount:</label>
        <%: Html.TextBoxFor(m => m.VatAmount, new { @class = "amountTextfield amount", @readOnly = true, min = -99999999999.9, max = 99999999999.9 })%>
        <%: ScriptHelper.GenerateDialogueHtml("VAT Breakdown", "Credit Memo Coupon VAT Capture", "divVatBreakdown", 500, 900)%>
      </div>
      <div>
        <label for="currencyAdjustmentIndicator">
          <span class="required">*</span> Currency Adjustment Indicator:</label>
        <%:Html.TextBoxFor(cmCoupon => cmCoupon.CurrencyAdjustmentIndicator, new { @class = "alphabetsOnly", maxLength = 3 })%>
      </div>
    </div>
    <div>
      <div>
        <label for="netAmount">
          Net Credited Amount:</label>
        <%: Html.TextBoxFor(m => m.NetAmountCredited, new { @readOnly = true, min = -999999999999999.9, max = 0, @class = "amount" })%>
      </div>
    </div>
    <div class="fieldSeparator">
    </div>
    <div>
      <div>
        <label for="airlineFlightDesignator">
          Airline Flight Designator:</label>
        <%: Html.TextBoxFor(m => m.AirlineFlightDesignator, new { @class = "alphabets upperCase", maxLength = 2 })%>
      </div>
      <div>
        <label for="flightNumber">
          Flight Number:</label>
        <%: Html.TextBoxFor(cmCoupon => cmCoupon.FlightNumber, new { @class = "integer", maxLength = 5 })%>
      </div>
      <div>
        <label for="flightDate">
          Flight Date:</label>
        <%: Html.TextBox(ControlIdConstants.FlightDate, (Model.FlightDate.HasValue == true ? Model.FlightDate.Value.ToString(FormatConstants.DateFormat) : string.Empty), new { @class = "datePicker" })%>
      </div>
    </div>
    <div>
      <div>
        <label for="fromAirport">
          From Airport:</label>
        <%: Html.TextBoxFor(cmCoupon => cmCoupon.FromAirportOfCoupon, new { maxLength = 4, @class = "upperCase" })%>
      </div>
      <div>
        <label for="toAirport">
          To Airport:</label>
        <%: Html.TextBoxFor(cmCoupon => cmCoupon.ToAirportOfCoupon, new { maxLength = 4, @class = "upperCase" })%>
      </div>
      <div>
        <label for="cabinClass">
          Cabin Class:</label>
        <%: Html.TextBoxFor(cmCoupon => cmCoupon.CabinClass, new { @class = "upperCase",  maxLength = 1 })%>
      </div>
    </div>
    <div class="fieldSeparator">
    </div>
    <div>
      <div>
        <label for="AgreementIndicatorSupplied">
          Agreement Indicator:</label>
        <%: Html.TextBoxFor(cmCoupon => cmCoupon.AgreementIndicatorSupplied, new { maxLength = 2 })%>
      </div>
      <div>
        <label for="originalPMI">
          Original PMI:</label>
        <%: Html.TextBoxFor(cmCoupon => cmCoupon.OriginalPmi, new { maxLength = 1 })%>
      </div>
      <div>
        <label for="ValidatedPMI">
          Validated PMI:</label>
        <%: Html.TextBoxFor(cmCoupon => cmCoupon.ValidatedPmi, new { maxLength = 1 })%>
      </div>
      <div>
        <label for="prorateMethodology">
          Prorate Methodology:</label>
        <%: Html.TextBoxFor(cmCoupon => cmCoupon.ProrateMethodology, new { maxLength = 3 })%>
      </div>
    </div>
    <div class="fieldSeparator">
    </div>
    <div>
      <div>
        <label for="reasonCode">
          Reason Code:</label>
        <%: Html.TextBoxFor(cmCoupon => cmCoupon.ReasonCode, new { maxLength = 2 })%>
      </div>
      <div>
        <label for="airlineOwnUse6">
          Airline Own Use:</label>
        <%: Html.TextBoxFor(cmCoupon => cmCoupon.AirlineOwnUse, new { maxLength = 20 })%>
      </div>
      <div>
        <label for="airlineOwnUse1">
          Reference Field 1:</label>
        <%: Html.TextBoxFor(cmCoupon => cmCoupon.ReferenceField1, new { maxLength = 10 })%>
      </div>
      <div>
        <label for="airlineOwnUse2">
          Reference Field 2:</label>
        <%: Html.TextBoxFor(cmCoupon => cmCoupon.ReferenceField2, new { maxLength = 10 })%>
      </div>
      <div>
        <label for="airlineOwnUse3">
          Reference Field 3:</label>
        <%: Html.TextBoxFor(cmCoupon => cmCoupon.ReferenceField3, new { maxLength = 10 })%>
      </div>
    </div>
    <div>
      <div>
        <label for="airlineOwnUse4">
          Reference Field 4:</label>
        <%: Html.TextBoxFor(cmCoupon => cmCoupon.ReferenceField4, new { maxLength = 10 })%>
      </div>
      <div>
        <label for="airlineOwnUse5">
          Reference Field 5:</label>
        <%: Html.TextBoxFor(cmCoupon => cmCoupon.ReferenceField5, new { maxLength = 20 })%>
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
        <%: ScriptHelper.GenerateDialogueHtml("Prorate Slip", "Add/Edit Prorate Slip Details", "divProrateSlip", 400, 970)%>
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
          <%:Html.TextBoxFor(cmCoupon => cmCoupon.NumberOfAttachments)%>
        </div>
        <div>
          <label for="isValidationFlag">
            IS Validation Flag:</label>
          <%:Html.TextBoxFor(cmCoupon => cmCoupon.ISValidationFlag)%>
        </div>
      </div>
    <% }%>
  </div>
  <div class="clear">
  </div>
  <%: Html.TextAreaFor(cmCoupon => cmCoupon.ProrateSlipDetails, new { @class = "hidden", @id = "hiddenprorateSlip" })%>
</div>
<div class="clear">
</div>
<div class="hidden" id="childTaxList">
</div>
<div class="hidden" id="childVatList">
</div>
<div id="childAttachmentList" class="">
</div> 

