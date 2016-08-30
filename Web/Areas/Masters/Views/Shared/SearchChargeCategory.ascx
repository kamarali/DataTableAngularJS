<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MiscUatp.Common.ChargeCategory>" %>
<div>
    <h2>
        Search Criteria</h2>
    <div class="searchCriteriaMedium">
        <div class="solidBox">
            <div class="fieldContainer horizontalFlow">
                <div>
                    
                    <div>
                        <label>
                            Charge Category Name:</label>
                        <%: Html.TextBoxFor(model => model.Name, new { @id = "Name", @Class = "autocComplete", @maxLength = 50 })%>
                    </div>
                    <div>
                        <label>
                            Billing Category:</label>
                        <%: Html.BillingCategoryDropdownListFor(model => model.BillingCategoryId, new { @id = "BillingCategoryId"})%>
                    </div>
                    
                </div>
            </div>
            <div class="clear">
            </div>
        </div>
    </div>
</div>
