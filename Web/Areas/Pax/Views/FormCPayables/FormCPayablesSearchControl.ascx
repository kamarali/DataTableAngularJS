<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.SearchCriteria>" %>
<h2>
  Search Criteria</h2>
<div>
  <div>
    <div class="solidBox">
      <div class="fieldContainer horizontalFlow">
        <div>
          <div>
            <label>
              <span>*</span> Provisional Billing Month:</label>
            <%:Html.ProvisionalBillingMonthFormCDropdownList(ControlIdConstants.BillingYearMonthDropDown, Model.BillingYear, Model.BillingMonth)%>
          </div>
          <div>
            <label>
              Provisional Billed Member:</label>
            <%: Html.TextBoxFor(searchCriteria => searchCriteria.BilledMemberText, new { @class = "autocComplete" })%>
            <%: Html.HiddenFor(searchCriteria => searchCriteria.BilledMemberId) %>
          </div>          
        </div>
      </div>
      <div class="clear" />
    </div>
  </div>
</div>
<div>
  <div class="buttonContainer">
    <input class="primaryButton" type="submit" value="Search" />
    <input class="secondaryButton" type="button" value="Clear" id="ResetSearch" onclick="resetSearch();" />
  </div>
</div>
