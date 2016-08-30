<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.Common.TaxCode>" %>
<div>
    <h2>
        Search Criteria</h2>
    <div class="searchCriteriaMedium">
        <div class="solidBox">
            <div class="fieldContainer horizontalFlow">
                <div>
                    <div>
                        <label>
                            Tax Code:</label>
                        <%: Html.TextBoxFor(model => model.Id, new { @maxLength = 3, @class = "alphaNumeric upperCase" })%>
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

