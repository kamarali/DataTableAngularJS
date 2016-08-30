<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.Sampling.SamplingFormEDetail>" %>
<%@ Import Namespace="Iata.IS.Web.Util" %>
<h2>
  Form E Details
</h2>
<div class="solidBox dataEntry">
  <div class="fieldContainer horizontalFlow">
    <div class="bottomLine">
      <h3>
        Universe Amounts</h3>
      <div>
        <label>
          <span>*</span> Gross Total Of Universe:</label>
        <%: Html.TextBoxFor(formEDetails => formEDetails.GrossTotalOfUniverse, new { @class="amt_16_3 amount", watermark = ControlIdConstants.PositiveAmount })%>
      </div>
      <div>
        <label>
          <span>*</span> Gross Total Of Form C:</label>
        <%: Html.TextBoxFor(formEDetails => formEDetails.GrossTotalOfUaf, new { @class = "amt_16_3 amount", watermark = ControlIdConstants.PositiveAmount })%>
      </div>
      <div>
        <label>
          <span>*</span> Universe Adjusted Gross Amount:</label>
        <%: Html.TextBoxFor(formEDetails => formEDetails.UniverseAdjustedGrossAmount, new { @readonly = true, @class = "amount" })%>
      </div>
    </div>
    <div class="bottomLine">
      <h3>
        Sampling Constant Calculation</h3>
      <div>
        <label>
          <span>*</span> Gross Total Of Sample:</label>
        <%: Html.TextBoxFor(formEDetails => formEDetails.GrossTotalOfSample, new { @readonly = true, @class = "amount" })%>
      </div>
      <div>
        <label>
          <span>*</span> Gross Total Of Form C Sample Coupon:</label>
        <%: Html.TextBoxFor(formEDetails => formEDetails.GrossTotalOfUafSampleCoupon, new { @class = "amt_16_3 amount", watermark = ControlIdConstants.PositiveAmount })%>
      </div>
      <div>
        <label>
          <span>*</span> Sample Adjusted Gross Amount:</label>
        <%: Html.TextBoxFor(formEDetails => formEDetails.SampleAdjustedGrossAmount, new { @readonly = true, @class = "amount" })%>
      </div>
      <div>
        <label>
          <span>*</span> Sampling Constant:</label>
        <%: Html.TextBoxFor(formEDetails => formEDetails.SamplingConstant, new { @readonly = true, roundTo = 3, @class = "pos_num_7_3" })%>
      </div>
    </div>
    <div>
      <h3>
        Total of Form D Amounts X Sampling Constant</h3>
      <div>
        <label>
          <span>*</span> Gross:</label>
        <%: Html.TextBoxFor(formEDetails => formEDetails.TotalOfGrossAmtXSamplingConstant, new { @readonly = true, @class = "amount" })%>
      </div>
      <div>
        <label>
          <span>*</span> ISC:</label>
        <%: Html.TextBoxFor(formEDetails => formEDetails.TotalOfIscAmtXSamplingConstant, new { @readonly = true, @class = "amount" })%>
      </div>
      <div>
        <label>
          <span>*</span> Other Commission:</label>
        <%: Html.TextBoxFor(formEDetails => formEDetails.TotalOfOtherCommissionAmtXSamplingConstant, new { @readonly = true, @class = "amount" })%>
      </div>
      <div>
        <label>
          <span>*</span> UATP:</label>
        <%: Html.TextBoxFor(formEDetails => formEDetails.UatpCouponTotalXSamplingConstant, new { @readonly = true, @class = "amount" })%>
      </div>
      <div>
        <label>
          <span>*</span> Handling Fee:</label>
        <%: Html.TextBoxFor(formEDetails => formEDetails.HandlingFeeTotalAmtXSamplingConstant, new { @readonly = true, @class = "amount" })%>
      </div>
      <div>
        <label>
          <span>*</span> Tax:</label>
        <%: Html.TextBoxFor(formEDetails => formEDetails.TaxCouponTotalsXSamplingConstant, new { @readonly = true, @class = "amount" })%>
      </div>
      <div>
        <label>
          <span>*</span> VAT:</label>
        <%: Html.TextBoxFor(formEDetails => formEDetails.VatCouponTotalsXSamplingConstant, new { @readonly = true, @class = "amount" })%>
      </div>
    </div>
    <div class="bottomLine">
      <div>
        <label>
          <span>*</span> Net Amount Due In Currency of Listing:</label>
        <%: Html.TextBoxFor(formEDetails => formEDetails.NetAmountDue, new { @readonly = true, @class = "amount" })%>
      </div>
      <div>
        <label>
          <span>*</span> Net Amount Due In Currency Of Billing:</label>
        <%: Html.TextBoxFor(formEDetails => formEDetails.NetAmountDueInCurrencyOfBilling, new { @readonly = true, @class = "amount" })%>
      </div>
    </div>
    <div class="hidden">
      <%: Html.TextBox(ControlIdConstants.HiddenGrossValue, Model.TotalGrossValue)%>
      <%: Html.TextBox(ControlIdConstants.HiddenIscAmount, Model.TotalIscAmount)%>
      <%: Html.TextBox(ControlIdConstants.HiddenOtherCommission, Model.TotalOtherCommission)%>
      <%: Html.TextBox(ControlIdConstants.HiddenUatpmount, Model.TotalUatpAmount)%>
      <%: Html.TextBox(ControlIdConstants.HiddenHandlingFee, Model.TotalHandlingFee)%>
      <%: Html.TextBox(ControlIdConstants.HiddenTaxAmount, Model.TotalTaxAmount)%>
      <%: Html.TextBox(ControlIdConstants.HiddenVatAmount, Model.TotalVatAmount)%>
      <%--CMP#648: Clearance Information in MISC Invoice PDFs. Desc: Convert Exchange Rate into nullable field.--%>
      <%--<%: Html.TextBox(ControlIdConstants.HiddenListingToBillingRate, Model.Invoice.ExchangeRate.HasValue ? Model.Invoice.ExchangeRate.Value : null)%>--%>
      <%--SCP#475855: IS web issue (View a form E in passenger payable invoices, get an error - it says Sorry, an error occurred.) --%>
      <%: Html.TextBox(ControlIdConstants.HiddenListingToBillingRate, Model.Invoice.ExchangeRate)%>
    </div>
    <h3>
      Form B Total Amounts</h3>
    <div>
      <div>
        <label>
          Gross:</label>
        <%: Html.TextBoxFor(formEDetails => formEDetails.ProvisionalFormBGrossBilled, new { @class = "amt_16_3 amount", watermark = ControlIdConstants.PositiveAmount })%>
      </div>
      <div>
        <label>
          ISC:</label>
        <%: Html.TextBoxFor(formEDetails => formEDetails.ProvisionalFormBIscAmount, new { @class = "amt_16_3 amount", watermark = ControlIdConstants.PositiveAmount })%>
      </div>
      <div>
        <label>
          Other Commission:</label>
        <%: Html.TextBoxFor(formEDetails => formEDetails.ProvisionalFormBOtherCommissionAmount, new { @class = "amt_16_3 amount", watermark = ControlIdConstants.PositiveAmount })%>
      </div>
      <div>
        <label>
          UATP:</label>
        <%: Html.TextBoxFor(formEDetails => formEDetails.ProvisionalFormBUatpAmount, new { @class = "amt_16_3 amount", watermark = ControlIdConstants.PositiveAmount })%>
      </div>
      <div>
        <label>
          Handling Fee:</label>
        <%: Html.TextBoxFor(formEDetails => formEDetails.ProvisionalFormBHandlingFeeAmountBilled, new { @class = "amt_10_3 amount", watermark = ControlIdConstants.PositiveAmount })%>
      </div>
    </div>
    <div class="bottomLine">
      <div>
        <label>
          Tax:</label>
        <%: Html.TextBoxFor(formEDetails => formEDetails.ProvisionalFormBTaxAmount, new { @class = "amt_16_3 amount", watermark = ControlIdConstants.PositiveAmount })%>
      </div>
      <div>
        <label>
          VAT:</label>
        <%: Html.TextBoxFor(formEDetails => formEDetails.ProvisionalFormBVatAmountBilled, new { @class = "amt_16_3 amount", watermark = ControlIdConstants.PositiveAmount })%>
      </div>
      <div>
        <label>
          <span>*</span> Total Amount:</label>
        <%: Html.TextBoxFor(formEDetails => formEDetails.TotalAmountFormB, new { @readonly = true, @class = "amt_16_3 amount" })%>
      </div>
    </div>
    <div>
      <h3>
        Net Adjustment</h3>
      <div>
        <label>
          <span>*</span> Net Billed/Credited Amount:</label>
        <%: Html.TextBoxFor(formEDetails => formEDetails.NetBilledCreditedAmount, new { @readonly = true, @class = "amount" })%>
      </div>
      <div>
        <label>
          <span>*</span> Number Of Billing Records:</label>
        <%: Html.TextBoxFor(formEDetails => formEDetails.NumberOfBillingRecords, new { @readonly = true })%>
      </div>
    </div>
  </div>
  <div class="clear">
  </div>
</div>
