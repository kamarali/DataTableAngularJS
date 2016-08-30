<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Cargo.PayableSearch>" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<div class="searchCriteria">
    <div class="solidBox">
        <div class="fieldContainer horizontalFlow">
            <div>
                <div>
                    <label>
                        <span>*</span> Billing Year/Month:</label>
                    <%:Html.BillingYearMonthDropdownForPayables(ControlIdConstants.BillingYearMonthDropDown, Model.BillingYear, Model.BillingMonth)%>
                </div>
                <div>
                    <label>
                        <span>*</span> Billing Period:</label>
                    <%:Html.StaticBillingPeriodDropdownListFor(model => model.BillingPeriod, true)%>
                </div>
                 <div>
          <label>
            Billing Code:</label>
            <%:Html.StaticCgoBillingCodeDropdownListFor(m => m.BillingCode, true)%>
        </div>
                <div>
          <label>
            <span>*</span> Billing Member:</label>
          <%:Html.HiddenFor(m => m.BillingMemberId)%>
          <%:Html.TextBoxFor(m => m.BillingMemberText, new { @class = "autocComplete" })%>
        </div>
                
            </div>
            <div>
            </div>
            <div>
                <div>
          <label>
            Invoice/Credit Note Status:</label>
          <%:Html.InvoiceStatusDropdownListFor(m => m.InvoiceStatus, true)%>
        </div>
                <div>
                    <label>
                        SMI:</label>
                    <%:Html.SettlementMethodDropdownListFor(m => m.SMI, InvoiceType.Invoice)%>
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
