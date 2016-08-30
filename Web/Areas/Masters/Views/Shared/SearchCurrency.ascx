<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Common.Currency>" %>
<div>
  <h2>
    Search Criteria</h2>
  <div class="searchCriteriaMedium">
    <div class="solidBox">
      <div class="fieldContainer horizontalFlow">
        <div>
          <div>
            <label>
              Currency Alpha Code:</label>
            <%: Html.TextBoxFor(model => model.Code, new { @Class = "alphabetsOnly upperCase", @maxLength = 3 })%>
          </div>
          <div>
            <label>
              Currency Name:</label>
            <%: Html.TextBoxFor(model => model.Name, new {@maxLength = 50 })%>
          </div>
        </div>
      </div>
      <div class="clear">
      </div>
    </div>
  </div>
</div>
