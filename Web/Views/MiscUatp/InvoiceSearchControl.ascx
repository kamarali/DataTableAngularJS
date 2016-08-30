<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MiscUatp.MiscSearchCriteria>" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<div class="searchCriteria" style="width:100%">
  <div class="solidBox" >
    <div class="fieldContainer horizontalFlow">
    <div style="width:85%; float:left;">
        <div>
          <label>
            <span>*</span> Billing Year/Month:</label>
          <%:Html.BillingYearMonthDropdown(ControlIdConstants.BillingYearMonthDropDown, Model.BillingYear, Model.BillingMonth)%>
          <%: Html.HiddenFor(m => m.BillingYear)%>
          <%: Html.HiddenFor(m => m.BillingMonth)%>
        </div>
        <div>
          <label>
            <span>*</span> Billing Period:</label>
          <%:Html.StaticBillingPeriodDropdownListFor(model => model.BillingPeriod, true)%>
        </div>
        <div>
          <label>
            Transaction Type :</label>
          <%:Html.InvoiceTypeDropDownListFor(m => m.InvoiceTypeId)%>
        </div>
        <div>
          <label>
            Invoice/Credit Note Status:</label>
          <%:Html.InvoiceStatusDropdownListFor(m => m.InvoiceStatus, true)%>
        </div>
        <div>
          <label>
            Billed Member:</label>
          <%:Html.HiddenFor(invoice => invoice.BilledMemberId)%>
           <!-- CMP#596: Length of Member Accounting Code to be Increased to 12 
                Desc: Non layout related IS-WEB screen changes.
                Ref: FRS Section 3.1 Table 2, Section 3.2 Table 8,Section 3.3 Table 12, Section 3.5 Table20 */ -->
          <%:Html.TextBoxFor(invoice => invoice.BilledMemberText, new { @class = "autocComplete textboxWidth" })%>
        </div>   
        
         <div>
          <label>
            Invoice/Credit Note Owner:</label>
          <%:Html.InvoiceOwnerDropDownListFor(m => m.OwnerId, true)%>
        </div>
          <div>
          <label>
            Invoice/Credit Note Number:</label>
          <%:Html.TextBoxFor(m => m.InvoiceNumber, new { maxLength = 10 })%>
        </div>
        <div>
          <label>
            SMI:</label>
          <%:Html.SettlementMethodDropdownListFor(m => m.SettlementMethodId,InvoiceType.Invoice, TransactionMode.MiscUatpInvoiceSearch)%>
        </div>
           <div>
          <label>
            Charge Category:</label>
          <%--CMP609: MISC Changes Required as per ISW2. Added new parameter 'isIncludeInactive'. If it is true then method will return the all charge category for misc category including in-active.--%>
          <%: Html.ChargeCategoryDropdownList(ControlIdConstants.ChargeCategory, Model.ChargeCategoryId, Model.BillingCategory, true, isIncludeInactive: true)%>
        </div>
         <% // Show Location only when BillingCategory is Miscellaneous
          if (Model.BillingCategory == BillingCategoryType.Misc)
          {%>
          <div>
          <label>
            Location:</label>
          <%:Html.TextBoxFor(invoice => invoice.LocationCode)%>
        </div>
        <%}%>
            <div>
          <label>
            File Name:</label>
            <%-- As per Robin's request, changed the maxLength from 30 to 100 --%>
          <%: Html.TextBox("FileName",Server.HtmlEncode(Model.FileName), new { @class = "xlargeTextField", maxLength = 100 })%>
        </div>
         <div>
          <label>
            Submission Method:</label>
          <%:Html.SubmissionMethodDropDownListFor(m => m.SubmissionMethodId, true,true)%>
        </div>
    </div>
    <div style="width:15%;">
        <% //CMP #655: IS-WEB Display per Location ID          
           //2.7	MISC IS-WEB RECEIVABLES - MANAGE INVOICE SCREEN
          if (Model.BillingCategory == BillingCategoryType.Misc)
          {%>
          <label>
            <span>*</span> Billed from Location ID:</label>     
          <%:Html.ListBox("AssociatedLocation", (MultiSelectList)ViewData["AssociatedLocation"], new { @style = "width: 150px;height:100px;" })%>
          <%: Html.HiddenFor(m=>m.BillingMemberLoc)%>
        <%}%>
    </div>
    </div>
    <div class="clear">
    </div>
  </div>
</div>
<div class="clear">
</div>
