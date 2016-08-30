<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.Common.Rfisc>" %>
<div>
  <h2>
    Search Criteria</h2>
  <div class="searchCriteriaMedium">
    <div class="solidBox">
      <div class="fieldContainer horizontalFlow">
        <div>
          <div>
            <label>
              RFISC Code:</label>
            <%: Html.TextBoxFor(model => model.Id, new { @class = "alphaNumeric upperCase", @maxLength = 3 })%>
          </div>
          <div>
            <label>
              RFIC Code:</label>
            <%: Html.RficDropdownListFor(model => model.RficId) %>
          </div>
          <div>
            <label>
              Group Name:</label>
            <%= Html.TextBoxFor(model => model.GroupName, new {@maxLength = 50 })%>
          </div>
          <div>
            <label>
              Commercial Name:</label>
            <%= Html.TextBoxFor(model => model.CommercialName, new {@maxLength = 50 })%>
          </div>
        </div>
      </div>
      <div class="clear">
      </div>
    </div>
  </div>
</div>
