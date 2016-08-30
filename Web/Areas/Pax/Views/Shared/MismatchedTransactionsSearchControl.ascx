<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.SupportingDocuments.SupportingDocumentRecord>" %>
<div class="searchCriteria">
  <div class="solidBox dataEntry">
    <div class="fieldContainer horizontalFlow">
      <div>
      <div>
        <label>
           Form C:
        </label>
        <%:Html.UnlinkedSupportingDocumentFormCYNDropdownList(ControlIdConstants.MismatchTransactionFormC, new { id = ControlIdConstants.MismatchTransactionFormC })%>
      </div>
      <div>
        <label>
          Billing Year/Month:
        </label>
        <%:Html.BillingYearMonthDropdown(ControlIdConstants.MismatchTransactionBillingYearMonth, Model.BillingYear, Model.BillingMonth)%>
      </div>
      <div>
          <label>
           Billing Period:</label>                   
         <%:Html.StaticBillingPeriodDropdownList(ControlIdConstants.MismatchTransactionBillingPeriod,Convert.ToString(Model.BillingPeriod)) %>
        </div>       
      </div>
     <div>      
        <div>
          <label>
            Billed Member:</label>
                    <%:Html.Hidden("HiddenBilledMemberId",Model.BilledMemberId)%>
        <%:Html.TextBox(ControlIdConstants.MismatchTransactionBilledMember,Model.BilledMemberName, new { @class = "autocComplete" })%>
        </div>    
        <div>
          <label>
            Invoice Number:</label>
          <%:Html.TextBox(ControlIdConstants.MismatchTransactionInvoiceNo, Model.InvoiceNumber, new { maxLength = 11 })%>
        </div>
        <div>
          <label>
            Batch Number:</label>
          <%:Html.TextBox(ControlIdConstants.MismatchTransactionBatchNumber,Model.BatchNumber, new { maxLength = 5 })%>
        </div>
      </div>
      <div>
       <div>
          <label>
            Sequence Number:</label>
          <%:Html.TextBox(ControlIdConstants.MismatchTransactionSequenceNumber,Model.SequenceNumber, new { maxLength = 5 })%>
        </div>
       <div>
          <label>
            Breakdown Serial Number:</label>
          <%:Html.TextBox(ControlIdConstants.MismatchTransactionCouponBreakdownSerialNumber,Model.BreakdownSerialNumber, new { maxLength = 5 })%>
        </div>
       <div class="verticalFlow">
       <div>
        <%:Html.CheckBox(ControlIdConstants.MismatchTransactionCases, true, new { value="Mismatch Cases"})%>
        </div>
        <div>
        <label>Mismatch Cases</label>
        </div>
        </div>
      </div>
    </div>
    <div class="clear">
    </div>
  </div>   
</div>
<div class="buttonContainer">
  <input class="primaryButton" type="submit" value="Search" id="MismatchSearchButton"  />  
</div>
<div class="clear">
</div>
