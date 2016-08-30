<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.BMCoupon>" %>
<h2>
  Billing Memo Coupon Details</h2>
  <input type="hidden" value="<%: Model.BillingMemo.SourceCodeId%>" id="SourceCodeId" />
<div class="solidBox dataEntry">
  <div class="fieldContainer horizontalFlow">
    <div id="divBlock1">
      <div id="serialNoDiv">
        <label for="SerialNo">Serial Number:</label>
        <%:Html.TextBoxFor(couponRecord => couponRecord.SerialNo, new { @readOnly = true })%>
      </div>
      <div>
        <label for="TicketOrFimIssuingAirline">
          <span class="required">*</span> Ticket Issuing Airline:</label>
        <%:Html.TextBoxFor(couponRecord => couponRecord.TicketOrFimIssuingAirline, new { @class = "autocComplete populated", maxlength = 4 })%>
      </div>      
      <div>
        <label for="TicketOrFimCouponNumber">
          <span class="required">*</span> Coupon No.:</label>
        <%:Html.NonFimCouponNumberDropdownList("TicketOrFimCouponNumber", Model.TicketOrFimCouponNumber)%>
      </div>
      <div>
        <label for="TicketDocOrFimNumber">
          <span class="required">*</span> Ticket/Document No.:</label>
          <%--CMP # 480 : Data Issue-11 Digit Ticket FIM Numbers Being Captured--%>
        <%: Html.TextBoxFor(bmCb => bmCb.TicketDocOrFimNumber, new { max = 9999999999, min = 0, @class = "digits integer", maxLength = 10 })%>
      </div>
      <div>
        <label for="CheckDigit">
          <span class="required">*</span> Check Digit:</label>
        <%:Html.TextBoxFor(bmCb => bmCb.CheckDigit, new { @class = "checkDigit", maxLength = 1 })%>
      </div>
    </div>
    <div>
      <div>
        <label for="ElectronicTicketIndicator">
          Electronic Ticket Indicator:</label>
        <%:Html.CheckBoxFor(bmCb => bmCb.ElectronicTicketIndicator)%>
      </div>
      <div>
        <label for="SettlementAuthorizationCode">
          ESAC:</label>
        <%: Html.TextBoxFor(bmCb => bmCb.SettlementAuthorizationCode, new { maxlength = 14 })%>
      </div>
    </div>
    <div class="fieldSeparator">
    </div>
    <div>
      <div>
        <label for="GrossAmountBilled">
         <span>*</span> Gross Amount:</label>
        <%:Html.TextBoxFor(bmCb => bmCb.GrossAmountBilled, new { max = 999999999999999.9, min = 0, watermark = ControlIdConstants.PositiveAmount, @class = "amount" })%>
      </div>
      <div>
        <label for="isc">
          ISC:</label>
        <%:Html.TextBoxFor(bmCb => bmCb.IscPercent, new { @class = "percentageTextfield percent", min = -99.999, max = 99.999, watermark = ControlIdConstants.NegativePercentage })%>&nbsp;%&nbsp;
        <%:Html.TextBoxFor(bmCb => bmCb.IscAmountBilled, new { @class = "amountTextfield amount", min = -99999999999.9, max = 99999999999.9, watermark = ControlIdConstants.NegativeAmount })%>&nbsp;Amount
      </div>
      <div>
        <label for="taxAmount">
          Tax Amount:</label>
        <%:Html.TextBoxFor(bmCb => bmCb.TaxAmount, new { @class = "amountTextfield amount", @readOnly = true, min = -99999999999.9, max = 99999999999.9 })%>
        <%:ScriptHelper.GenerateDialogueHtml("Tax Breakdown", "Billing Memo Coupon Breakdown Tax Capture", "divTaxBreakdown", 500, 500)%>
      </div>
      <div>
        <label for="HandlingFeeAmount">
          Handling Fee:</label>
        <%: Html.TextBoxFor(bmCb => bmCb.HandlingFeeAmount, new { min = -999999999.9, max = 999999999.9, watermark = ControlIdConstants.NegativeAmount, @class = "amount" })%>
      </div>
    </div>
    <div>
      <div>
        <label for="otherCommission">
          Other Commission:</label>
        <%:Html.TextBoxFor(bmCb => bmCb.OtherCommissionPercent, new { @class = "percentageTextfield percent", max = 99.999, min = -99.999, watermark = ControlIdConstants.NegativePercentage })%>&nbsp;%&nbsp;
        <%:Html.TextBoxFor(bmCb => bmCb.OtherCommissionBilled, new { @class = "amountTextfield amount", min = -99999999999.9, max = 99999999999.9, watermark = ControlIdConstants.NegativeAmount })%>&nbsp;Amount
      </div>
      <div>
        <label for="uatp">
          UATP:</label>
        <%:Html.TextBoxFor(bmCb => bmCb.UatpPercent, new { @class = "percentageTextfield percent", min = -99.999, max = 99.999, watermark = ControlIdConstants.NegativePercentage })%>&nbsp;%&nbsp;
        <%:Html.TextBoxFor(bmCb => bmCb.UatpAmountBilled, new { @class = "amountTextfield amount", min = -99999999999.9, max = 99999999999.9, watermark = ControlIdConstants.NegativeAmount })%>&nbsp;Amount
      </div>
      <div>
        <label for="vatAmount">
          VAT Amount:</label>
        <%:Html.TextBoxFor(bmCb => bmCb.VatAmount, new { @class = "amountTextfield amount", @readOnly = true, max = 99999999999.9, min = -99999999999.9 })%>
        <%:ScriptHelper.GenerateDialogueHtml("VAT Breakdown", "Billing Memo Coupon Breakdown VAT Capture", "divVatBreakdown", 500, 900)%>
      </div>
      <div>
        <label for="CurrencyAdjustmentIndicator">
          <span class="required">*</span> Currency Adjustment Indicator:</label>
        <%:Html.TextBoxFor(bmCb => bmCb.CurrencyAdjustmentIndicator, new { @class = "alphabetsOnly", maxLength = 3 })%>
      </div>
    </div>
    <div>
      <div>
        <label for="NetAmountBilled">
          <span class="required">*</span> Net Billed Amount:</label>
        <%: Html.TextBoxFor(bmCb => bmCb.NetAmountBilled, new { @class = "amountTextfield amount", @readOnly = true, max = 999999999999999.9, min = 0 })%>
      </div>
    </div>
    <div class="fieldSeparator">
    </div>
    <div>
      <div>
        <label for="AirlineFlightDesignator">
          Airline Flight Designator:</label>
        <%:Html.TextBoxFor(couponRecord => couponRecord.AirlineFlightDesignator, new { @class = "alphabets upperCase", maxLength = 2 })%>
      </div>
      <div>
        <label for="FlightNumber">
          Flight Number:</label>
        <%: Html.TextBoxFor(bmCb => bmCb.FlightNumber, new { @class = "integer", maxLength = 5 })%>
      </div>
      <div>
        <label for="FlightDate">
          Flight Date:</label>
        <%: Html.TextBox(ControlIdConstants.FlightDate, (Model.FlightDate.HasValue == true ? Model.FlightDate.Value.ToString(FormatConstants.DateFormat) : string.Empty), new { @class = "datePicker" })%>
        <%-- TODO: Add watermark for Date--%>
      </div>
    </div>
    <div>
      <div>
        <label for="FromAirportOfCoupon">
          From Airport:</label>
        <%: Html.TextBoxFor(bmCb => bmCb.FromAirportOfCoupon, new { maxLength = 4, @class = "upperCase" })%>
        <%-- TODO: Add From Airport Auto populate for Billing Memo Coupon Breakdown--%>
      </div>
      <div>
        <label for="ToAirportOfCoupon">
          To Airport:</label>
        <%: Html.TextBoxFor(bmCb => bmCb.ToAirportOfCoupon, new { maxLength = 4, @class = "upperCase" })%>
        <%-- TODO: Add To Airport Auto populate for Billing Memo Coupon Breakdown--%>
      </div>
      <div>
        <label for="CabinClass">
          Cabin Class:</label>
        <%: Html.TextBoxFor(bmCb => bmCb.CabinClass, new { @class = "upperCase", maxLength = 1 })%>
      </div>
    </div>
    <div class="fieldSeparator">
    </div>
    <div>
      <div>
        <label for="AgreementIndicatorSupplied">
          Agreement Indicator:</label>
        <%: Html.TextBoxFor(bmCb => bmCb.AgreementIndicatorSupplied, new { @class = "twoChars" })%>
      </div>
      <div>
        <label for="OriginalPmi">
          Original PMI:</label>
        <%:Html.TextBoxFor(bmCb => bmCb.OriginalPmi, new { @class = "alphabet upperCase", maxLength = 1 })%>
      </div>
      <div>
        <label for="ValidatedPmi">
          Validated PMI:</label>
        <%: Html.TextBoxFor(bmCb => bmCb.ValidatedPmi, new { maxLength = 1 })%>
      </div>
      <div>
        <label for="ProrateMethodology">
          Prorate Methodology:</label>
        <%: Html.TextBoxFor(bmCb => bmCb.ProrateMethodology, new { maxLength = 3 })%>
      </div>
    </div>
    <div class="fieldSeparator">
    </div>
    <div>
      <div>
        <label for="ReasonCode">
          Reason Code:</label>
        <%: Html.TextBoxFor(bmCb => bmCb.ReasonCode, new { @class = "upperCase", maxLength = 2 })%>
        <%-- TODO: Add Auto populate Reason Code for Billing Memo Coupon Breakdown--%>
      </div>
      <div>
        <label for="AirlineOwnUse">
          Airline Own Use:</label>
        <%: Html.TextBoxFor(bmCb => bmCb.AirlineOwnUse, new { maxLength = 20 })%>
      </div>
      <div>
        <label for="ReferenceField1">
          Reference Field 1:</label>
        <%: Html.TextBoxFor(bmCb => bmCb.ReferenceField1, new { maxLength = 10 })%>
      </div>
      <div>
        <label for="ReferenceField2">
          Reference Field 2:</label>
        <%: Html.TextBoxFor(bmCb => bmCb.ReferenceField2, new { maxLength = 10 })%>
      </div>
      <div>
        <label for="ReferenceField3">
          Reference Field 3:</label>
        <%: Html.TextBoxFor(bmCb => bmCb.ReferenceField3, new { maxLength = 10 })%>
      </div>
    </div>
    <div>
      <div>
        <label for="ReferenceField4">
          Reference Field 4:</label>
        <%: Html.TextBoxFor(bmCb => bmCb.ReferenceField4, new { maxLength = 10 })%>
      </div>
      <div>
        <label for="ReferenceField5">
          Reference Field 5:</label>
        <%: Html.TextBoxFor(bmCb => bmCb.ReferenceField5, new { maxLength = 20 })%>
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
          <%:Html.TextBoxFor(bmCb => bmCb.NumberOfAttachments)%>
        </div>
        <div>
          <label for="isValidationFlag">
            IS Validation Flag:</label>
          <%:Html.TextBoxFor(bmCb => bmCb.ISValidationFlag)%>
        </div>
      </div>
    <% }%>
  </div>
  <div class="clear">
  </div>
  <%:Html.TextAreaFor(bmCb => bmCb.ProrateSlipDetails, new { @class = "hidden", @id = "hiddenprorateSlip" })%>
</div>
<div id="childTaxList" class="hidden">
</div>
<div id="childVatList" class="hidden">
</div>
<div id="childAttachmentList" class="">
</div>
