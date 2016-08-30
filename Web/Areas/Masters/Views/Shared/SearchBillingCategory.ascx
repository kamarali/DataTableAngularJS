<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Common.BillingCategory>" %>

<div>
  <h2>
    Search Criteria</h2>
  <div class="searchCriteriaMedium">
    <div class="solidBox">
      <div class="fieldContainer horizontalFlow">
        <div>
          <div>
            <label>
              Billing Category Code:</label>
            <%: Html.TextBoxFor(model => model.CodeIsxml, new { @id = "CodeIsxml", @Class = "autocComplete", @maxlength = 25 })%>
          </div>
          <div>
            <label>
              Description:</label>
            <%: Html.TextAreaFor(model => model.Description,3,60, new { @maxlength=255})%>
          </div>
        </div>
      </div>
      <div class="clear">
      </div>
    </div>
  </div>
</div>
