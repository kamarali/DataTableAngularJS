<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MiscUatp.Common.AddOnChargeName>" %>

<div>
    <h2>
        Search Criteria</h2>
    <div class="searchCriteriaMedium">
        <div class="solidBox">
            <div class="fieldContainer horizontalFlow">
                <div>
                    <div>
                        <label>
                             Add On Charge Name:</label>
                        <%: Html.TextBoxFor(model => model.Name, new { @class = "alphaNumericWithSpace", @maxLength = 30 })%>
                    </div>
                    <div>
                        <label>
                            Billing Category:</label>
                        <%: Html.BillingCategoryDropdownListFor(model => model.BillingCategoryId)%>
                    </div>
                </div>
            </div>
            <div class="clear">
            </div>
        </div>
    </div>
</div>
