<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.SupportingDocuments.SupportingDocumentRecord>" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<div class="searchCriteria">
  <div class="solidBox dataEntry">
    <div class="fieldContainer horizontalFlow">
      <div>
        <div>
          <label><span>*</span>
            Billing Year/Month:
          </label>
          <%:Html.BillingYearMonthDropdown(ControlIdConstants.MismatchTransactionBillingYearMonth, Model.BillingYear, Model.BillingMonth)%>
        </div>
        <div>
          <label><span>*</span>
            Billing Period:</label>
          <%:Html.StaticBillingPeriodDropdownList(ControlIdConstants.MismatchTransactionBillingPeriod,Convert.ToString(Model.BillingPeriod)) %>
        </div>
        <div>
          <label><span>*</span>
            Billed Member:</label>
          <%:Html.Hidden("HiddenBilledMemberId",Model.BilledMemberId)%>
          <%:Html.TextBox(ControlIdConstants.MismatchTransactionBilledMember,Model.BilledMemberName, new { @class = "autocComplete" })%>
        </div>
      </div>
      <div>
        <div>
          <label><span>*</span>
            Invoice Number:</label>
          <%:Html.TextBox(ControlIdConstants.MismatchTransactionInvoiceNo, Model.InvoiceNumber, new { maxLength = 11 })%>
        </div>
        <div id="ChargeCat">
          <label>
            Charge Category:</label>
          <%:Html.ChargeCategoryDropdownList(ControlIdConstants.MismatchChargeCategory, Model.ChargeCategory, BillingCategoryType.Uatp, true)%>
        </div>
        <div class="verticalFlow">
          <div>
            <%:Html.CheckBox(ControlIdConstants.MismatchTransactionCases, true, new { value="Mismatch Cases"})%>
          </div>
          <div>
            <label>
              Mismatch Cases</label>
          </div>
        </div>
      </div>
    </div>
    <div class="clear">
    </div>
  </div>
</div>
<div class="buttonContainer">
  <input class="primaryButton" type="submit" value="Search" id="MismatchSearchButton" />
</div>
<div class="clear">
</div>
