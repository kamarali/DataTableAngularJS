<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Common.CityAirport>" %>
<div>
  <h2>
    Search Criteria</h2>
  <div class="searchCriteriaMedium">
    <div class="solidBox">
      <div class="fieldContainer horizontalFlow">
        <div>
        <div>
            <label>
              City Airport Code:</label>
            <%: Html.TextBoxFor(model => model.Id, new { @id = "Id", @Class = "alphabetsOnly upperCase", @maxLength = 4 })%>
             </div>
          <div>
            <label>
              City Name:</label>
            <%: Html.TextBoxFor(model => model.Name, new { @id = "Name", @maxLength = 50 })%>
             </div> <div>
            <label>
              Country Code:</label>
            <%: Html.TextBoxFor(model => model.CountryId, new { @id = "CountryId", @Class = "alphabetsOnly upperCase", @maxLength = 2 })%>
          </div>
         
        </div>
      </div>
      <div class="clear">
      </div>
    </div>
  </div>
</div>
