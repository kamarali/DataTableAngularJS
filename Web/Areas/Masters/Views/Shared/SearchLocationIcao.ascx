<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Common.LocationIcao>" %>
<div>
    <h2>
        Search Criteria</h2>
    <div class="searchCriteriaMedium">
        <div class="solidBox">
            <div class="fieldContainer horizontalFlow">
                <div>
                    <div>
                        <label>
                            Location ICAO Code:</label>
                        <%: Html.TextBoxFor(model => model.Id, new { @class = "alphabetsOnly upperCase", @maxLength = 4 })%>
                    </div>
                    <div>
                        <label>
                            Country Code:</label>
                        <%: Html.TextBoxFor(model => model.CountryCode, new { @class = "alphabetsOnly upperCase", @maxLength = 2 })%>
                    </div>
                    <div>
                        <label>
                            Description:</label>
                         <%: Html.TextBoxFor(model => model.Description, new { @id = "Description", @maxLength = 255 })%>
                    </div>
                </div>
            </div>
            <div class="clear">
            </div>
        </div>
    </div>
</div>
