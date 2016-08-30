<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.VatIdentifier>" %>
<div>
    <h2>
        Search Criteria</h2>
    <div class="searchCriteriaMedium">
        <div class="solidBox">
            <div class="fieldContainer horizontalFlow">
                <div>
                    <div>
                        <label>
                            VAT Identifier:</label>
                        <%: Html.TextBoxFor(model => model.Identifier, new { @class = "alphabetsOnly upperCase", @maxLength = 2 })%>
                    </div>
                    <div>
                        <label>
                            Billing Category:</label>
                        <%= Html.BillingCategoryDropdownListFor(model => model.BillingCategoryCode)%>
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


