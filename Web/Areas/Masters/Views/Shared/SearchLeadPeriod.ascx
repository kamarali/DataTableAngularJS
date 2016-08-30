<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Common.LeadPeriod>" %>
<div>
    <h2>
        Search Criteria</h2>
    <div class="searchCriteriaMax">
        <div class="solidBox">
            <div class="fieldContainer horizontalMediumFlow">
                <div>
                    <div>
                        <label>
                            Effective From Period:</label>
                        <%: Html.TextBoxFor(model => model.EffectiveFromPeriod, new { @class = "datePickerMaster", @id = "EffectiveFromPeriod" })%>
                    </div>
                    <div>
                        <label>
                            Effective To Period:</label>
                        <%: Html.TextBoxFor(model => model.EffectiveToPeriod, new { @class = "datePickerMaster", @id = "EffectiveToPeriod" })%>
                    </div>
                    <div>
                        <label>
                            Lead Period:</label>
                        <%: Html.TextBoxFor(model => model.Period, new { @id = "Period" })%>
                    </div>
                    <div>
                        <label>
                            Billing Category:</label>
                        <%: Html.BillingCategoryDropdownListFor(model => model.BillingCategoryId)%>
                    </div>
                    <div>
                        <label>
                            Clearing House:</label>
                        <%: Html.TextBoxFor(model => model.ClearingHouse, new { @class = "alphabet upperCase", @maxLength = 1 })%>
                    </div>                                    
                   </div>
            </div>
            <div class="clear">
            </div>
        </div>
    </div>
</div>
