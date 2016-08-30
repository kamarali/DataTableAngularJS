<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Common.TimeLimit>" %>
<div>
  <h2>
    Search Criteria</h2>
  <div class="searchCriteriaMedium">
    <div class="solidBox">
      <div class="fieldContainer horizontalFlow">
        <div>        
          <div>
            <label>
             Time Limit:
            </label>
            <%: Html.TextBoxFor(model => model.Limit, new { @class = "integer", @maxLength = 3 })%>
          </div>
           <div>
            <label>
              Effective From Date/Period:
            </label>
            <%: Html.TextBoxFor(model => model.EffectiveFromPeriod, new { @class = "datePickerMaster" })%>
          </div>
          <div>
            <label>
              Effective To Date/Period:
            </label>
            <%: Html.TextBoxFor(model => model.EffectiveToPeriod, new { @class = "datePickerMaster" })%>
          </div>
          <div>
            <label>
              Settlement Method:
            </label>
            <%: Html.SettlementMethodDropdownListFor(model => model.SettlementMethodId,0)%>
          </div>
          <div>
            <label>
              Transaction Type:
            </label>
            <%: Html.TransactionTypeDropdownListFor(model => model.TransactionTypeId)%>
          </div>
        </div>
      </div>
      <div class="clear">
      </div>
    </div>
  </div>
</div>
