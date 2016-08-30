<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Reports.ProcessingDashBoard.SisUsageReportModel>" %>
<%--CMP #659: SIS IS-WEB Usage Report.--%>
<h2>
    Search Criteria</h2>
<div class="solidBox">
    <div class="fieldContainer horizontalFlow">
        <div>
            <div>
                <label>
                    <span class="required">* </span>From Billing Year:
                </label>
                <%:
                Html.BillingYearDropdownListFor(model => model.FromBillingYear)  %>
            </div>
            <div>
                <label>
                    <span class="required">* </span>From Billing Month:
                </label>
                <%: Html.BillingMonthDropdownListFor(model => model.FromBillingMonth)  %>
            </div>
            <div>
                <label>
                    <span class="required">* </span>From Period:
                </label>
                <%: Html.StaticBillingPeriodDropdownListFor(model => model.FromPeriod)%>
            </div>
        </div>
        <div>
            <div>
                <label>
                    <span class="required">* </span>To Billing Year:
                </label>
                <%: Html.BillingYearDropdownListFor(model => model.ToBillingYear)  %></div>
            <div>
                <label>
                    <span class="required">* </span>To Billing Month:
                </label>
                <%: Html.BillingMonthDropdownListFor(model => model.ToBillingMonth)  %></div>
            <div>
                <label>
                    <span class="required">* </span>To Period:
                </label>
                <%: Html.StaticBillingPeriodDropdownListFor(model => model.ToPeriod)%></div>
        </div>
    </div>
    <div class="clear">
    </div>
</div>
