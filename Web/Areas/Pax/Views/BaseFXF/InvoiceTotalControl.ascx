<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.InvoiceTotal>" %>
<h2>
  Invoice Total</h2>
<div class="solidBox dataEntry">
  <div class="fieldContainer horizontalFlow">
    <div>
      <div>
        <label>
          Total Gross Amount:
        </label>
        <%: Html.TextBoxFor(model => model.TotalGrossValue, new { readOnly = true, @class = "amount" })%>
      </div>
      <div>
        <label>
          Total ISC Amount:
        </label>
        <%: Html.TextBoxFor(model => model.TotalIscAmount, new { readOnly = true, @class = "amount" })%>
      </div>
      <div>
        <label>
          Total Other Commission Amount:
        </label>
        <%: Html.TextBoxFor(model => model.TotalOtherCommission, new { readOnly = true, @class = "amount" })%>
      </div>
      <div>
        <label>
          Total UATP Amount:
        </label>
        <%: Html.TextBoxFor(model => model.TotalUatpAmount, new { readOnly = true, @class = "amount" })%>
      </div>
      <div>
        <label>
          Handling Fee Amount:
        </label>
        <%: Html.TextBoxFor(model => model.TotalHandlingFee, new { readOnly = true, @class = "amount" })%>
      </div>
      <div>
        <label>
          Total Tax Amount:
        </label>
        <%: Html.TextBoxFor(model => model.TotalTaxAmount, new { readOnly = true, @class = "amount" })%>
      </div>
      <div>
        <label>
          Total VAT Amount:
        </label>
        <%: Html.TextBoxFor(model => model.TotalVatAmount, new { readOnly = true, @class = "amount" })%>
      </div>
      <div>
        <label>
          Net Total Before Sampling Constant:
        </label>
        <%: Html.TextBoxFor(model => model.TotalGrossValue, new { readOnly = true, @class = "amount" })%>
      </div>
      <div>
        <label>
          Sampling Constant:
        </label>
        <%: Html.TextBoxFor(model => model.SamplingConstant, new { readOnly = true, roundTo = 3 })%>
      </div>
      <div>
        <label>
          Net Total After Sampling Constant:
        </label>
        <%: Html.TextBoxFor(model => model.NetAmountAfterSamplingConstant, new { readOnly = true, @class = "amount" })%>
      </div>
      <div>
        <label>
          Net Billing Amount:
        </label>
        <%: Html.TextBoxFor(model => model.NetBillingAmount, new { readOnly = true, @class = "amount" })%>
      </div>
      <div>
        <label>
          No. of Billing Records:
        </label>
        <%: Html.TextBoxFor(model => model.NoOfBillingRecords, new { readOnly = true })%>
      </div>
    </div>
  </div>
  <div class="clear">
  </div>
</div>
