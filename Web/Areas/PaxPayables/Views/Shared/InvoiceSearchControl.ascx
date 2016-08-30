<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.SearchCriteria>" %>
<%@ Import Namespace="Iata.IS.Model.Enums" %>
<div class="searchCriteria">
  <div class="solidBox">
    <div class="fieldContainer horizontalFlow">
      <div>
        <div>
          <label>
            <span>*</span> Billing Year/Month:</label>
          <%:Html.BillingYearMonthDropdownForPayables(ControlIdConstants.BillingYearMonthDropDown, Model.BillingYear, Model.BillingMonth)%>
        </div>
        <div>
          <label>
           <span>*</span> Billing Period:</label>          
          <%:Html.StaticBillingPeriodDropdownListFor(model => model.BillingPeriod, true)%>
        </div>
        <div>
          <label>
            Billing Code:</label>
          <%:Html.BillingCodeDropdownListFor(m => m.BillingCode, true)%>
        </div>     
        <div>
          <label>
            Billing Member:</label>
                    <%:Html.HiddenFor(invoice => invoice.BillingMemberId)%>
        <%:Html.TextBoxFor(invoice => invoice.BillingMemberText, new { @class = "autocComplete" })%>
        </div>
      </div>
      <div>      
        <div>
          <label>
            Invoice/Credit Note Number:</label>
          <%:Html.TextBoxFor(m => m.InvoiceNumber, new { maxLength = 10 })%>
        </div>     
        <div>
          <label>
            SMI:</label>
          <%:Html.SettlementMethodDropdownListFor(m => m.SettlementMethodId, InvoiceType.Invoice)%>
        </div>
       <%-- <div>
          <label>
            File Name:</label>
          <%: Html.TextBox("FileName",Server.HtmlEncode(Model.FileName), new { @class = "xlargeTextField", maxLength = 100 })%>
        </div>--%>
       
      </div>
    </div>
    <div class="clear">
    </div>
  </div>
</div>
<div class="buttonContainer">
  <input class="primaryButton" type="submit" value="Search" />
  <input class="secondaryButton" type="button" onclick="resetForm();" value="Clear" />
</div>
<div class="clear">
</div>
