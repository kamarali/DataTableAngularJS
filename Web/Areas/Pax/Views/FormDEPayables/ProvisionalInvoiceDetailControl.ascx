<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.Sampling.ProvisionalInvoiceRecordDetail>" %>
<h2>
  Provisional Invoice Record Details</h2>
<form id="ProvisionalInvoiceForm" method="get" action="">
<div class="solidBox dataEntry">
  <div class="fieldContainer horizontalFlow">
    <div>
      <div>
        <label>
          <span>*</span> Provisional Invoice Number:</label>
        <%: Html.TextBoxFor(provisionalInvoiceRecord => provisionalInvoiceRecord.InvoiceNumber, new { maxLength = 10 })%>
      </div>
      <div>
        <label>
          <span>*</span> Provisional Invoice Date:</label>
        <%: Html.TextBoxFor(provisionalInvoiceRecord => provisionalInvoiceRecord.InvoiceDate, new { @class = "datePicker" })%>
      </div>
      <div>
        <label>
          <span>*</span> Provisional Billing Period No:</label>
        <%:Html.StaticBillingPeriodDropdownList("BillingPeriodNo", Model.BillingPeriodNo)%>
      </div>
      <div>
        <label>
          <span>*</span> Provisional Invoice Listing Currency:</label>
        <%:Html.CurrencyDropdownList(ControlIdConstants.InvoiceListingCurrencyId, Model.InvoiceListingCurrencyId.ToString())%>
      </div>
      <div>
        <label>
          <span>*</span> Provisional Invoice Listing Amount:</label>
        <%: Html.TextBoxFor(provisionalInvoiceRecord => provisionalInvoiceRecord.InvoiceListingAmount, new { @class = "amt_16_3 amount" })%>
      </div>
    </div>
    <%--CMP#648: Clearance Information in MISC Invoice PDFs. Desc: Convert Exchange Rate into nullable field.--%>
    <div>
      <div>
        <label>
          <span>*</span> Provisional Listing To Billing Rate:</label>
        <%: Html.TextBoxFor(provisionalInvoiceRecord => provisionalInvoiceRecord.ListingToBillingRate.HasValue.HasValue && provisionalInvoiceRecord.ListingToBillingRate.Value, new { @class = "exchangeRate" })%>
      </div>
      <div>
        <label>
          <span>*</span> Prov. Invoice Amount in Billing Currency:</label>
        <%: Html.TextBoxFor(provisionalInvoiceRecord => provisionalInvoiceRecord.InvoiceBillingAmount, new { @readOnly = true })%>
        <%: Html.TextBox("InvoiceId", Model.InvoiceId, new { @class = "hidden" })%>
      </div>
    </div>
  </div>
  <div class="clear">
  </div>
</div>
<div class="buttonContainer">
  <input class="primaryButton ignoredirty" type="submit" value="Save" />
</div>
</form>
