<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Calendar.ISCalendar>" %>
<div>
    <h2>
        Search Criteria</h2>
    <div class="searchCriteriaMedium">
        <div class="solidBox">
            <div class="fieldContainer horizontalFlow">
                <div>
                    <div>
                        <label>
                            Month:</label>
                        <%: Html.TextBoxFor(model => model.Month, new { @id = "Month", @Class = "autocComplete", @maxLength = 6 })%>
                    </div>
                    <div>
                        <label>
                            Period:</label>
                        <%: Html.TextBoxFor(model => model.Period, new { @id = "Period", @Class = "autocComplete", @maxLength = 2 })%>
                    </div>
                    <div>
                        <label>
                            Event Type:</label>
                        <%: Html.TextBoxFor(model => model.EventCategory, new { @id = "EventCategory", @Class = "autocComplete", @maxLength = 10 })%>
                    </div>
                </div>
            </div>
            <div class="clear">
            </div>
        </div>
    </div>
</div>

