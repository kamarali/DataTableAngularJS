<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Common.Tolerance>" %>
<%@ Import Namespace="Iata.IS.Web.Util" %>
<div>
  <h2>
    Search Criteria</h2>
  <div class="searchCriteriaMax">
    <div class="solidBox">
      <div class="fieldContainer horizontalFlow">
        <div>
          <div>
            <label>
              Billing Category:</label>
            <%= Html.BillingCategoryDropdownListFor(model => model.BillingCategoryId)%>
          </div>
          <div>
            <label>
              Clearing House:</label>
            <%: Html.TextBoxFor(model => model.ClearingHouse, new { @class = "alphabet upperCase", @maxLength = 1 })%>
          </div>
          <div>
            <label>
              Type:</label>
            <%: Html.TextBoxFor(model => model.Type, new { @class = "alphabet upperCase", @maxLength = 1 })%>
          </div>
          <div>
            <label>
              Effective From Period:</label>
            <%: Html.TextBoxFor(model => model.EffectiveFromPeriod, new { @class = "upperCase", @id = "EffectiveFromPeriod" })%>
          </div>
          <div>
            <label>
              Effective To Period:</label>
            <%: Html.TextBoxFor(model => model.EffectiveToPeriod, new { @class = "upperCase", @id = "EffectiveToPeriod" })%>
          </div>
        </div>
      </div>
      <div class="clear">
      </div>
    </div>
  </div>
</div>
