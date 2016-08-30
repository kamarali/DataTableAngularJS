<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MemberProfile.SubDivision>" %>
<div>
    <h2>
        Search Criteria</h2>
    <div class="searchCriteriaMedium">
        <div class="solidBox">
            <div class="fieldContainer horizontalFlow">
                <div>
                    <div class="editor-label">
                        <label>
                            Sub Division Code:</label>
                        <%: Html.TextBoxFor(model => model.Id, new { @class = "alphaNumeric upperCase", @maxLength = 3 })%>
                    </div>
                    <div class="editor-label">
                        <label>
                            Sub Division Name:</label>
                        <%: Html.TextBoxFor(model => model.Name, new {@maxLength = 50 })%>
                    </div>
                    <div>
                        <label>
                            Country Code:</label>
                        <%: Html.TextBoxFor(model => model.CountryId, new { @id = "CountryId", @Class = "alphabetsOnly upperCase", @maxLength = 2 })%>
                    </div>
                </div>
            </div>
            <div class="clear">
            </div>
        </div>
    </div>
</div>
