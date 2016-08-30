<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Common.ExchangeRate>" %>
<div>
  <h2>
    Search Criteria</h2>
  <div class="searchCriteriaMedium">
    <div class="solidBox">
      <div class="fieldContainer horizontalFlow">
        <div>
        <div>
            <label>
             Currency:</label>
            <%: Html.CurrencyDropdownListFor(model => model.CurrencyId) %>
            
          </div>
           <div >
               <label>Effective From:</label>
                <%:Html.TextBox("EffectiveFromDate",null, new { @class = "datePicker", @id = "EffectiveFromDate"})%>
                
            </div>
            
            <div >
               <label>Effective To:</label>
                <%:Html.TextBox("EffectiveToDate", null, new { @class = "datePicker", @id = "EffectiveToDate" })%>
            </div>
          
        </div>
      </div>
      <div class="clear">
      </div>
    </div>
  </div>
</div>
