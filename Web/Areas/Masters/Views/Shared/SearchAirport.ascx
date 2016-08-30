<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Common.Airport>" %>
<div>
    <h2>
        Search Criteria</h2>
    <div class="searchCriteriaMedium">
        <div class="solidBox">
            <div class="fieldContainer horizontalFlow">
                <div>
                    <div>
                        <label>
                            Airport ICAO Code:</label>
                        <%: Html.TextBoxFor(model => model.Id, new { @id = "Id", @Class = "alphabetsOnly upperCase", @maxLength = 4 })%>
                    </div>
                    <div>
                        <label>
                            Airport Name:</label>
                        <%: Html.TextBoxFor(model => model.Name, new { @id = "Name", @Class = "alphaNumericWithSpace", @maxLength = 50 })%>
                    </div>
                    <div>
                        <label>
                            Country Code:</label>
                        <%: Html.TextBoxFor(model => model.CountryCode, new { @id = "CountryCode", @Class = "alphabetsOnly upperCase", @maxLength = 2 })%>
                    </div>
                </div>
            </div>
            <div class="clear">
            </div>
        </div>
    </div>
</div>
