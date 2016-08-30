<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Common.CountryIcao>" %>
<div>
    <h2>
        Search Criteria</h2>
    <div class="searchCriteriaMedium">
        <div class="solidBox">
            <div class="fieldContainer horizontalFlow">
                <div>
                    <div>
                        <label>
                            Country ICAO Code:</label>
                        <%: Html.TextBoxFor(model => model.CountryCodeIcao, new { @class = "alphabetsOnly upperCase", @maxlength = 2 })%>
                    </div>
                    <div>
                        <label>
                            Country Name:</label>
                        <%: Html.TextBoxFor(model => model.Name, new { @class = "alphabetsOnly upperCase", @maxlength = 50 })%>
                    </div>
                </div>
            </div>
            <div class="clear">
            </div>
        </div>
    </div>
</div>
