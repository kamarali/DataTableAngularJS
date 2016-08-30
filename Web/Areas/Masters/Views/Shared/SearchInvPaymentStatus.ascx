<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Common.InvPaymentStatus>" %>
<div>
  <h2>
    Search Criteria</h2>
  <div class="searchCriteriaMedium">
    <div class="solidBox">
      <div class="fieldContainer horizontalFlow">
        <div>
          <div>
            <label>
              Payment Status Description:</label>
            <%: Html.TextBoxFor(model => model.Description, new { @Class = "alphaNumericWithSpace", @maxLength = 100 })%>
          </div>
          <div>
            <label>
              Applicable For:</label>
              <%: Html.InvPaymentStatusApplicableForDropdownListFor(model => model.ApplicableFor)%>
              
          </div>
        </div>
      </div>
      <div class="clear">
      </div>
    </div>
  </div>
</div>
