<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Reports.ReportSearchCriteria.ReportSearchCriteriaModel>" %>
<%@ Import Namespace="Iata.IS.Web.Util" %>
<h2>Search Criteria</h2>
<script type="text/javascript">
    $(document).ready(function () {
        $('#ToMonth').val('<%=ViewData["currentMonth"]%>');
        $('#ToYear').val('<%=ViewData["currentYear"]%>');
        $('#FromMonth').val('<%=ViewData["currentMonth"]%>');
        $('#FromYear').val('<%=ViewData["currentYear"]%>');
        $('#ToPeriod').val('<%=ViewData["PeriodNo"] %>');
        $('#FromPeriod').val('<%=ViewData["PeriodNo"] %>');
    });
 </script>
 <div class="solidBox">
   <div class="fieldContainer horizontalFlow">
     <div>
       <div>
         <label>
           <span class="required">* </span>Billing Year From:
         </label>
         <%: Html.BillingYearTwoDropdownListFor(model => model.FromYear)%>
       </div>
       <div>
         <label>
           <span class="required">* </span>Billing Month From:
         </label>
         <%: Html.BillingMonthDropdownListFor(model => model.FromMonth)%>
       </div>
       <div>
         <label>
           <span class="required">* </span>Billing Period From:
         </label>
         <%: Html.StaticBillingPeriodDropdownListFor(model => model.FromPeriod)%>
       </div>
     </div>
     <div>
       <div>
         <label>
           <span class="required">* </span>Billing Year To:
         </label>
         <%: Html.BillingYearTwoDropdownListFor(model => model.ToYear)%>
       </div>
       <div>
         <label>
           <span class="required">* </span>Billing Month To:
         </label>
         <%: Html.BillingMonthDropdownListFor(model => model.ToMonth)%>
       </div>
       <div>
         <label>
           <span class="required">* </span>Billing Period To:
         </label>
         <%: Html.StaticBillingPeriodDropdownListFor(model => model.ToPeriod)%>
       </div>
     </div>
     <div>
        <div>
           <label>
             <span class="required">* </span>Billing Member:
           </label>
           <!-- CMP #596: Length of Member Accounting Code to be Increased to 12 
            Desc: Increasing field size by specifying in-line width
            Ref: 3.1 Table 1 Row 4 -->
           <%:Html.TextBoxFor(model => model.BillingEntityCode, new { @id = "BillingEntityCode", @class = "autocComplete textboxWidth" })%>
           <%:Html.TextBoxFor(model => model.BillingEntityId, new { @id = "BillingEntityCodeId", @class = "hidden" })%>
       </div>
       <div>
           <label> Billed Member:</label>
           <!-- CMP #596: Length of Member Accounting Code to be Increased to 12 
            Desc: Increasing field size by specifying in-line width
            Ref: 3.1 Table 1 Row 5 -->
           <%:Html.TextBoxFor(model => model.BilledEntityCode, new { @id = "BilledEntityCode", @class = "autocComplete textboxWidth" })%>
           <%:Html.TextBoxFor(model => model.BilledEntityId, new { @id = "BilledEntityCodeId", @class = "hidden" })%>
       </div>
       <div>
         <label>Invoice Number:</label>
         <%:Html.TextBoxFor(model => model.InvoiceNo, new { maxLength = 10, @id = "InvoiceNumber" })%>
       </div>
     </div>
     <div>
       <div>
         <label>Charge Category:</label>
         <%--CMP609: MISC Changes Required as per ISW2. Added new parameter 'isIncludeInactive'. If it is true then method will return the all charge category for misc category including in-active.--%>
         <%:Html.ChargeCategoryDropdownListFor(model => model.ChargeCategory, isIncludeInactive: true)%>
       </div>
       <div>
         <label>Charge Code:</label>
         <%: Html.ChargeCodeDropdownList("ChargeCode", Model != null ? Model.ChargeCode : 0, Model != null ? Model.ChargeCategory : 0)%>
       </div>
     </div>
   </div>
   <div class="clear">
 </div>
</div>