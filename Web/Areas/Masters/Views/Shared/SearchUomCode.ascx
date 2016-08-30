<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MiscUatp.Common.UomCode>" %>
<div>
    <h2>
        Search Criteria</h2>
    <div class="searchCriteriaMedium">
        <div class="solidBox">
            <div class="fieldContainer horizontalFlow">
                <div>
                    <div>
                        <label>
                            UOM Code:</label>
                        <%: Html.TextBoxFor(model => model.Id, new { @class = "alphaNumeric upperCase", @maxLength = 3 })%>
                    </div>
                     <div>
                        <label>
                            UOM Code Type:</label>
                        <%: Html.UOMCodeTypeDropdownListFor(model => model.Type)%>
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

