<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.SearchCriteria>" %>
<div class="searchCriteria">
  <div class="solidBox">
    <div class="fieldContainer horizontalFlow">
      <div>
        <div>
          <label>
            Billing Month:</label>
          <!-- TODO : Billing Month Dropdown -->
        </div>
        <div>
          <label>
            Billing Period</label>
          <%= Html.BillingPeriodDropdownListFor(m => m.BillingPeriod)%>
        </div>
        <div>
          <label>
            Billing Code</label>
          <%= Html.BillingPeriodDropdownListFor(m => m.BillingCode)%>
        </div>
        <div>
          <label>
            Invoice Status</label>
          <%= Html.BillingPeriodDropdownListFor(m => m.InvoiceStatus)%>
        </div>
        <div>
          <label>
            Billed Member</label>
          <!-- TODO : Billed member Autocomplete --->
        </div>
      </div>
      <div>
        <div>
          <label>
            Invoice Owner</label>
          <%= Html.TextBoxFor(m => m.InvoiceOwner)%>
        </div>
        <div>
          <label>
            InvoiceNumber</label>
          <%= Html.TextBoxFor(m => m.InvoiceNumber)%>
        </div>
      </div>
      <div>
        <div>
          <label>
            SMI</label>
          <%= Html.TextBoxFor(m => m.SettlementMethod)%>
        </div>
        <div>
          <label>
            File Name</label>
          <%= Html.TextBoxFor(m => m.FileName, new { @class = "xlargeTextField" })%>
        </div>
        <div>
          <label>
            Submission Method</label>
          <%= Html.EditorFor(m => m.SubMethod)%>
        </div>
        <div>
          <label>
            Billing Member</label>
          <%=Html.TextBox("billingMember", "",new { @readonly = true })%>
        </div>
      </div>
    </div>
    <div class="clear">
    </div>
  </div>
</div>
<div class="searchButtonContainer">
  <% using (Html.BeginForm("InvoiceSearch", "Receivables", FormMethod.Post, new { style = "display:inline" }))
     { %>
  <input class="primaryButton" type="submit" value="Search" />
  <input class="secondaryButton" type="reset" value="Clear" />
  <% } %>
</div>
<div class="clear">
</div>
