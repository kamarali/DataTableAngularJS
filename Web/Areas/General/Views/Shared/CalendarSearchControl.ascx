<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<div class="searchCriteria">
  <div class="solidBox">
    <div class="fieldContainer horizontalFlow">
      <div>
        <div>
          <label>
            Calendar Year:
          </label>
          <%:Html.IsCalendarBillingYearDropdownList(ControlIdConstants.ISCalendarSearchYear, ViewData[ViewDataConstants.ISCalendarSearchYear] != null ? Convert.ToInt32(ViewData[ViewDataConstants.ISCalendarSearchYear]) : 0)%>
        </div>
        <div>
          <label>
            Calendar Month:
          </label>
          <%:Html.MonthsDropdownList(ControlIdConstants.ISCalendarSearchMonth, ViewData[ViewDataConstants.ISCalendarSearchMonth] != null ? Convert.ToInt32(ViewData[ViewDataConstants.ISCalendarSearchMonth]) : 0)%>
        </div>
        <div>
          <label>
            Period:
          </label>
          <%:Html.StaticBillingPeriodDropdownList(ControlIdConstants.ISCalendarSearchPeriod, ViewData[ViewDataConstants.ISCalendarSearchPeriod] != null ? Convert.ToString(ViewData[ViewDataConstants.ISCalendarSearchPeriod]) : "0", TransactionMode.CalendarSearch)%>
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
