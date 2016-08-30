<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Common.MinMaxAcceptableAmount>" %>
<div>
    <h2>
        Search Criteria</h2>
    <div class="searchCriteriaMedium">
        <div class="solidBox">
            <div class="fieldContainer horizontalFlow">
                <div>
                    <div>
                        <label>
                            Transaction Type:</label>
                            <%= Html.TransactionTypeDropdownListFor(model => model.TransactionTypeId)%>
                    </div>
                    <div>
                        <label>
                            Clearing House:</label>
                            <%: Html.TextBoxFor(model => model.ClearingHouse, new { @class = "alphabet upperCase", @maxLength = 1 })%>
                    </div>
                    <div>
                        <label>
                            Min:</label>
                            <%= Html.TextBoxFor(model => model.Min, new { @class = "amt_10_3 amount", @maxLength = 11 })%>
                    </div>
                    <div>
                        <label>
                            Max:</label>
                            <%= Html.TextBoxFor(model => model.Max, new { @class = "amt_10_3 amount", @maxLength = 11 })%>
                    </div>
                </div>
            </div>
            <div class="clear">
            </div>
        </div>
    </div>
</div>
