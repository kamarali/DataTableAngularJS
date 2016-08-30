<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Reports.SupportingMismatchDocument.SupportingMismatchDocument>" %>

<script type="text/javascript">
    $(document).ready(function () {
        
        $('#BillingMonth').val('<%=ViewData["CurrentMonth"] %>');
        $('#BillingYear').val('<%=ViewData["CurrentYear"] %>');
        $('#BillingPeriod').val('<%=ViewData["CurrentPeriod"] %>');
        $("#BillingPeriod option[value='']").remove();
        
        var category = '<%=ViewData["Category"].ToString() %>';
        if (category == 3) {
            $("#SettlementMethodStatusId option[value='6']").remove();
        }
    });
</script>
<h2>
  Search Criteria</h2>

<div class="solidBox">
  <div class="fieldContainer horizontalFlow">
      
      <div >
           <div>
               <label> <span class="required">* </span> Billing Month:</label>
                <%: Html.BillingMonthDropdownListFor(model => model.BillingMonth) %>
            </div>
            <div>
            <label>Period No:</label>
                <%: Html.StaticBillingPeriodDropdownListFor(model => model.BillingPeriod) %>
            </div>
            <div>
            <label><span class="required">* </span>Billing Year:</label>
            <%if ((ViewData["Category"]).Equals(3))
              { %>
                <%: Html.BillingTwoYearDropdownListFor(model => model.BillingYear) %>
                <% } %>
                <% else %>
                <%: Html.BillingYearLastThreeDropdownListFor(model => model.BillingYear) %>
            </div>
        </div>
    
      <div>
            <div>
             <label> Settlement Method: </label>
                  <%: Html.SettlementMethodStatusDropdownList(ControlIdConstants.SettlementMethodStatusId, -1)%>
            </div>
            <div>
                         <label>Invoice Number: </label>
                <%:Html.TextBoxFor(m => m.InvoiceNo, new {  maxLength = 10 })%>
            </div>

            <div>
            <label>Member Code: </label>
                   <span>
                    <%:Html.TextBoxFor(model => model.MemberCode, new { @class = "autocComplete textboxWidth" })%>
                    <%:Html.TextBoxFor(model => model.MemberId, new { @class = "hidden" })%>
                   </span>
            </div>
        </div>
    </div>
 
  <div class="clear">
  </div>

  </div>

