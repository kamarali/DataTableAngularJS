<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.Common.Rfic>" %>
<div>
  <h2>
    Search Criteria</h2>
  <div class="searchCriteriaMedium" style="width: 730px">
    <div class="solidBox">
      <div class="fieldContainer horizontalFlow">
        <div>
          <div class="editor-label">
            <label> RFIC Code:</label>
              <%: Html.TextBoxFor(model => model.Id, new { @class = "alphaNumeric", @maxLength = 1 })%>
          </div>
          <div class="editor-label">
            <label>Description:</label>
            <%: Html.TextBoxFor(model => model.Description, new { @id = "Description", @maxLength = 255 })%>
          </div>
        </div>
      </div>
      <div class="clear">
      </div>
    </div>
  </div>
</div>
