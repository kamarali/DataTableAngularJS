<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Common.Country>" %>
<div>
  <h2>
    Search Criteria</h2>
  <div class="searchCriteriaMedium">
    <div class="solidBox">
      <div class="fieldContainer horizontalFlow">
        <div>
         <div>
            <label>
              Country Code:</label>
             <%: Html.TextBoxFor(country => country.Id, new { @class = "alphabetsOnly upperCase", @maxLength = 2 })%>
          </div>
          <div>
            <label>
              Country Name:</label>
            <%: Html.TextBoxFor(model => model.Name, new { @id = "Name",@maxlength = 50 })%>
          </div>
          
        </div>
      </div>
      <div class="clear">
      </div>
    </div>
  </div>
</div>
