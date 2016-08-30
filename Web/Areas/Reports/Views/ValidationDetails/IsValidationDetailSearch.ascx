<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.Common.ValidationExceptionSummary>" %>

<script type="text/javascript">
    $(document).ready(function () {
        $('#BillingYear').val('<%=ViewData["CurrentYear"] %>');
        $('#BillingMonth').val('<%=ViewData["CurrentMonth"] %>');
        $('#BillingPeriod').val('<%=ViewData["Period"] %>');
        $("#BillingPeriod option[value='']").remove();

    });
</script>

<h2>
  Search Criteria</h2>
<div class="solidBox">
  <div class="fieldContainer horizontalFlow">
    <div>
    <div>
        <label>
          Billing Year:<span style="color:Red">*</span></label>
        <%: Html.BillingYearDropdownListFor(searchCriteria => searchCriteria.BillingYear)%>
      </div>
      <div>
        <label>
          Billing Month:<span style="color:Red">*</span></label>
        <%: Html.BillingMonthDropdownListFor(searchCriteria => searchCriteria.BillingMonth)%>
      </div>
      <div>
        <label>
         Billing Period:</label>
        <%:
            Html.StaticBillingPeriodDropdownListFor(model => model.BillingPeriod)%>
      </div>
     
    </div>
    <div>
      <div>
        <label>
          Billing File Name:</label>
          <%: Html.TextBoxFor(m=>m.FileName, new { @Id = "filename" })%>
      </div>
      <div>
        <label>
          Billing File Submission Date(UTC):</label>
          <%: Html.TextBoxFor(model => model.FileSubmissionDate, new { @id = "fromSubmissionDate", @class = "datePicker", @readOnly = true })%>
        
      </div>
      
    </div>
    <div>
         <div>
        <label>
          Billed Member:</label>
          <%: Html.TextBoxFor(model => model.BilledOrganisation, new { @id = "billedOrganisation" })%>
      </div>
      <div>
        <label>Invoice Number:</label>
        <%: Html.TextBoxFor(model => model.InvoiceNo, new {@id ="InvoiceNo"})%>
      </div>
    </div>
  </div>
  <div class="clear">
  </div>
</div>