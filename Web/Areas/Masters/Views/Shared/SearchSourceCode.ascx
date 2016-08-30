<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.SourceCode>" %>
<div>
    <h2>
        Search Criteria</h2>
    <div class="searchCriteriaMedium">
        <div class="solidBox">
            <div class="fieldContainer horizontalFlow">
                <div>
                    <div>
                        <label>
                            Source Code:</label>
                        <%: Html.TextBoxFor(model => model.SourceCodeIdentifier, new { @Class = "number", @maxLength = 2 })%>
                    </div>
                    <div>
                        <label>
                            Transaction Type:</label>
                        <%: Html.TransactionTypeDropdownListFor(model => model.TransactionTypeId) %>
                    </div>
                    <div>
                        <label>
                            Utilization Type:</label>
                        <%: Html.TextBoxFor(model => model.UtilizationType, new { @class = "alphaNumeric upperCase", @maxLength = 1 })%>
                    </div>
                </div>
            </div>
            <div class="clear">
            </div>
        </div>
    </div>
</div>
