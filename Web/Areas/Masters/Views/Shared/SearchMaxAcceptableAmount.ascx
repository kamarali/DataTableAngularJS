<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Common.MaxAcceptableAmount>" %>
<div>
  <h2>
    Search Criteria</h2>
  <div class="searchCriteriaMax">
    <div class="solidBox">
      <div class="fieldContainer horizontalFlow">
        <div>
          <div>
            <label>
              Effective From Period:</label>
            <%: Html.TextBoxFor(model => model.EffectiveFromPeriod, new { @class = "upperCase" })%>
          </div>
          <div>
            <label>
              Effective To Period:</label>
            <%: Html.TextBoxFor(model => model.EffectiveToPeriod, new { @class = "upperCase" })%>
          </div>
          <div>
            <label>
              Transaction Type:</label>
            <%= Html.TransactionTypeDropdownListFor(model => model.TransactionTypeId)%>
          </div>
          <div>
            <label>
              Clearing House:</label>
            <%: Html.TextBoxFor(model => model.ClearingHouse, new { @class = "alphabet upperCase", @maxLength = 1 })%>
          </div>
          <div>
            <label>
              Amount:</label>
            <%= Html.TextBoxFor(model => model.Amount, new { @class = "amt_10_3 amount", @maxLength = 11 })%>
          </div>
        </div>
      </div>
      <div class="clear">
      </div>
    </div>
  </div>
</div>
