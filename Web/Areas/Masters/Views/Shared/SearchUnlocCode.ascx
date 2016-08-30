<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Common.UnlocCode>" %>
<div>
  <h2>
    Search Criteria</h2>
  <div class="searchCriteriaMedium">
    <div class="solidBox">
      <div class="fieldContainer horizontalFlow">
        <div>
        <div>
            <label>
              UN Location Code:</label>
            <%: Html.TextBoxFor(model => model.Id, new { @id = "Id", @Class = "alphabetsOnly upperCase", @maxLength = 5 })%>
             </div>
          <div>
            <label>
              UN Location Name:</label>
            <%: Html.TextBoxFor(model => model.Name, new { @id = "Name", @maxLength = 50 })%>
             </div> 
        </div>
      </div>
      <div class="clear">
      </div>
    </div>
  </div>
</div>
