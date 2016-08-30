<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.AutoBillingSearchCriteria>" %>

<div class="searchCriteria">
  <div class="solidBox">
    <div class="fieldContainer horizontalFlow">
      <div>
        <div>
          <label><span>*</span> Billed Member:</label>
          <%:Html.HiddenFor(invoice => invoice.BilledMemberId)%>
          <%:Html.TextBoxFor(invoice => invoice.BilledMemberText, new { @class = "autocComplete" })%>
        </div>
        <div>
          <label>Daily Revenue Recognition File Date:</label>
          <%:Html.TextBox(ControlIdConstants.PaxDailyRevenueRecognitionFileDate, Model.DailyRevenueRecognitionFileDate != null ? Model.DailyRevenueRecognitionFileDate.Value.ToString(FormatConstants.DateFormat) : null, new { @class = "datePicker" })%>          
        </div>
        <div>
          <label>Invoice Number:</label>
          <%:Html.TextBoxFor(invoice => invoice.InvoiceNumber, new { maxLength = 10 })%>
        </div>
        <div>
          <label>Source Code:</label>
          <%:Html.TextBoxFor(invoice => invoice.SourceCode, new { maxLength = 3 })%>
        </div>
        <div>
          <label>Prorate Methodology:</label>
          <%:Html.TextBoxFor(invoice => invoice.ProrateMethodology, new { maxLength = 3 })%>
        </div>
      </div>
      <div>
        <div>
          <label>Issuing Airline:</label>
          <%:Html.TextBoxFor(invoice => invoice.TicketIssuingAirline, new { @class = "digits integer", maxlength = 4 })%>
        </div>
        <div>
          <label>Ticket Document Number:</label>
          <%:Html.TextBoxFor(invoice => invoice.TicketDocNumber, new { max = 99999999999, min = 0, @class = "digits integer", @maxLength = 11 })%>
        </div>
        <div>
          <label>Coupon Number:</label>
          <%:Html.TextBoxFor(invoice => invoice.CouponNumber, new { @class = "digits integer" })%>
        </div>
      </div>
    </div>
    <div class="clear">
    </div>
  </div>
</div>
<div class="buttonContainer">
  <input class="primaryButton" type="submit" value="Search" />
  <input class="secondaryButton" type="button" onclick="resetForm();" value="Clear" />
</div>
<div class="clear">
</div>
