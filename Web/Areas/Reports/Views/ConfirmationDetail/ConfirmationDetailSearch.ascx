<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Reports.ConfirmationDetails.ConfirmationDetailModel>" %>


  <%-- Function call passenger is data value confirmation details report --%>

  <script type="text/javascript">

      $(document).ready(function () {
          $('#MmbrId').val('<%= ViewData["MembrId"] %>');
          $('#ClearanceMonth').val('<%= ViewData["CurrentMonth"] %>');
          $('#BillingPeriod').val('<%= ViewData["CurrentPeriod"] %>');
          $('#billingYear').val('<%= ViewData["CurrentYear"] %>');
          /*CMP #596: Length of Member Accounting Code to be Increased to 12 
          Desc: The list of Members shown in the auto-complete should exclude Type B Members, Applying New auto-complete logic #MW1.
          Ref: FRS Section 3.4 Table 15 Row 42 
          SCP368582 - User Access Problem*/
          registerAutocomplete('BillingAirlineNumberCode', 'BillingAirlineNumber', '<%:Url.Action("GetMemberListForPaxCgoContainsLoginMember", "Data", new { area = "" })%>', 0, true, null);
          registerAutocomplete('BilledAirlineNumberCode', 'BilledAirlineNumber', '<%:Url.Action("GetMemberListForPaxCgoContainsLoginMember", "Data", new { area = "" })%>', 0, true, null);
          registerAutocomplete('IssuingAirline', 'IssuingAirline', '<%:Url.Action("GetTicketIssuingAirlineList", "Data", new { area = "" })%>', 0, true, null);
      });
  </script>

<div class="solidBox">
  <div class="fieldContainer horizontalFlow">
  <div>
      <div>
        <label>
          Clearance Month:<span style="color:Red">*</span></label>          
        <%: Html.BillingMonthDropdownListFor(model => model.ClearanceMonth)%>            
         <%: Html.HiddenFor(model => model.MmbrId)%>
      </div>
      <div>
      <label>
        Period No:<span style="color:Red">*</span>
      </label>
      <%: Html.StaticBillingPeriodDropdownListFor(model => model.BillingPeriod,true)%>
      </div>     
      <div>
      <label>
        Billing Year:<span style="color:Red">*</span>
      </label>
      <%: Html.BillingYearDropdownListFor(model => model.billingYear)%>
      </div>      
     <div>
     <label>
     Billing Airline: 
     </label>
     <%:Html.TextBoxFor(model => model.BillingAirlineNumberCode, new { @Id = "BillingAirlineNumberCode", @class = "autocComplete" })%>     
     <%: Html.HiddenFor(model => model.BillingAirlineNumber, new { @id = "BillingAirlineNumber" })%>
     </div>
     
     </div>     
     <div>    
     <div>
     <label>
     Billed Airline:
     </label>
     <%:Html.TextBoxFor(model => model.BilledAirlineNumberCode, new { @Id = "BilledAirlineNumberCode", @class = "autocComplete" })%>     
     <%: Html.HiddenFor(model => model.BilledAirlineNumber, new { @id = "BilledAirlineNumber" })%>
     </div> 
     <div>
     <label>
     Invoice Number:
     </label>
     <%: Html.TextBoxFor(model => model.InvoiceNumber, new { @Id = "invoiceNumber", @class = "alphaNumeric", @maxLength = 10 })%>
     </div>     
      <div>
      <label>
        Issuing Airline 
      </label>      
      <%: Html.TextBoxFor(model => model.IssuingAirline, new { @Id = "IssuingAirline", @class = "autocComplete" })%>     
      </div>     
      <div>
     <label>
        Original PMI:
     </label>
     <%: Html.TextBoxFor(model => model.OriginalPMI, new { @Id = "originalPMI", @class = "alphabet", @maxLength = 1 })%>
     </div>   
     
     </div>
     <div>
     <div>
     <label>
        Validated PMI:
     </label>
     <%: Html.TextBoxFor(model => model.ValidatedPMI, new {  @class = "alphabet", @maxLength = 1 })%>
     </div>   
      <div>
      <label>
      Agreement Indicator - Supplied: 
      </label>
      <%: Html.TextBoxFor(searchCriteria => searchCriteria.AgreementIndicatorSupplied, new { @Id = "agreIndSupplied", @class = "alphaNumeric", @maxLength = 2 })%>
      </div>  
       <div>
      <label>
      Agreement Indicator - Validated: 
      </label>
      <%: Html.TextBoxFor(searchCriteria => searchCriteria.AgreementIndicatorValidated, new { @Id = "agreIndValidated", @class = "alphaNumeric", @maxLength = 2 })%>
      </div>  
       <div>
      <label>
      ATPCO Reason Code: 
      </label>
      <%: Html.TextBoxFor(searchCriteria => searchCriteria.ATPCOReasonCode, new { @Id = "atpcoReasonCode", @class = "alphabet", @maxLength = 1 })%>
      </div>  
      </div>      
  </div>
  <div class="clear">
  </div>
</div>







