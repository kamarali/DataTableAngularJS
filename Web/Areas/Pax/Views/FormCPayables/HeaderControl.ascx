<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.Sampling.SamplingFormC>" %>
<h2>
  Form C Header</h2>
<div class="solidBox dataEntry">
  <div class="fieldContainer horizontalFlow">
    <div>
      <div>
        <label>
          Provisional Billing Month:</label>
        <%: Html.ProvisionalBillingMonthDropdownList(ControlIdConstants.ProvisionalBillingMonthDropdown, Model.ProvisionalBillingMonth, Model.ProvisionalBillingYear)%>
      </div>
      <div>
        <%if (ViewData[ViewDataConstants.BillingType].ToString() == BillingType.Receivables)
          {%>
        <label>
          <span>*</span> Provisional Billing Member:</label>
        <%:Html.TextBoxFor(invoice => invoice.ProvisionalBillingMemberText, new { @class = "autocComplete" })%>
        <%
          }
          else
          {%>
        <label>
          From Member:</label>
        <%:Html.TextBoxFor(invoice => invoice.FromMemberText, new { @readOnly= true })%>
        <%
          }%>
        <%:Html.HiddenFor(invoice => invoice.ProvisionalBillingMemberId)%>
        <%:Html.HiddenFor(invoice => invoice.FromMemberId)%>
      </div>
      <div>
        <label>
          NIL Form C Indicator:</label>
        <%: Html.NilFormCIndicatorDropdownListFor(formC => formC.NilFormCIndicator)%>
      </div>
      <div id = "ListingCurrencyDiv">
        <label>
          <span>*</span> Currency of Listing/Evaluation:</label>
        <%: Html.CurrencyDropdownListFor(formC => formC.ListingCurrencyId)%>
      </div>
      <div class="hidden">
        <%: Html.TextBoxFor(formC => formC.InvoiceStatusId)%>
      </div>
    </div>
  </div>
  <div class="clear">
  </div>
</div>
<div class="clear">
</div>
