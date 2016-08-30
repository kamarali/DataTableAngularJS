<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Reports.SupportingMismatchDocument.SupportingMismatchDocument>" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>

<script type="text/javascript">
    $(document).ready(function () {

        $('#BillingMonth').val('<%=ViewData["CurrentMonth"] %>');
        $('#BillingYear').val('<%=ViewData["CurrentYear"] %>');
        $('#BillingPeriod').val('<%=ViewData["CurrentPeriod"] %>');
        $("#BillingPeriod option[value='']").remove();
        
    });
</script>
<h2>
  Search Criteria</h2>

<div class="solidBox">
  <div class="fieldContainer horizontalFlow">
      
      <div >
           <div>
               <label>Billing Month: <span class="required">* </span> </label>
                <%: Html.BillingMonthDropdownListFor(model => model.BillingMonth) %>
            </div>
            <div>
            <label>Period No:<span class="required">* </span> </label>
                <%: Html.StaticBillingPeriodDropdownListFor(model => model.BillingPeriod) %>
            </div>
            <div>
            <label>Billing Year:<span class="required">* </span></label>
                <%: Html.BillingYearDropdownListFor(model => model.BillingYear) %>
            </div>
        </div>
    
      <div>
            <div>
             <label> Settlement Method: </label>
                  <%:Html.SettlementMethodDropdownListFor(m => m.SettlementMethodId ,InvoiceType.Invoice)%>
            </div>
            <div>
                         <label>Invoice Number: </label>
                <%:Html.TextBoxFor(m => m.InvoiceNo, new {  maxLength = 10 })%>
            </div>

            <div>
            <label>Member Code: </label>
                   <span>
                    <%:Html.TextBoxFor(model => model.MemberCode, new {@class="autocComplete" })%>
                    <%:Html.TextBoxFor(model => model.MemberId, new { @class = "hidden" })%>
                   </span>
            </div>
        </div>
    </div>
 
  <div class="clear">
  </div>

  </div>

