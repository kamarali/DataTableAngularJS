<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Common.IchZone>" %>
<div>
    <h2>
        Search Criteria</h2>
    <div class="searchCriteriaMedium">
        <div class="solidBox">
            <div class="fieldContainer horizontalFlow">
                <div>
                    <div>
                        <label>
                            Zone:</label>
                        <%: Html.TextBoxFor(model => model.Zone, new {@class="alphabet upperCase", @maxLength = 1 })%>
                    </div>
                    <div>
                        <label>
                            Clearance Currency:</label>
                        <%: Html.TextBoxFor(model => model.ClearanceCurrency, new { @class = "alphabetsOnly upperCase", @maxLength = 3 })%>
                    </div>
                    <div>
                        <label>
                            Description:</label>
                        <%: Html.TextBoxFor(model => model.Description, new { @class = "alphaNumericWithSpace upperCase", @maxLength = 255 })%>
                    </div>
                </div>
            </div>
            <div class="clear">
            </div>
        </div>
    </div>
</div>

