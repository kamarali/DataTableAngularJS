<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MiscUatp.Common.ChargeCode>" %>
<div>
  <h2>
    Search Criteria</h2>
  <div class="searchCriteriaMedium">
    <div class="solidBox">
      <div class="fieldContainer horizontalFlow">
        <div>
          <div>
            <label>
              Charge Category:</label>
            <%: Html.ChargeCategoryDropdownListWithAllFor(model => model.ChargeCategoryId)%>
          </div>
          <div>
            <label>
              Charge Code:</label>
            <%: Html.ChargeCodeDropdownList(model => model.Id, Model != null ? Model.ChargeCategoryId : 0, true)%>
          </div>
        </div>
      </div>
      <div class="clear">
      </div>
    </div>
  </div>
</div>
