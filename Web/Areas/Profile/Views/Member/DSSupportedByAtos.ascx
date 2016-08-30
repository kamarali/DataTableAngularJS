<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MemberProfile.EBillingConfiguration>" %>
<div id="searchCriteriaDiv" class="solidBox dataEntry">
  <h2>
    Select Country</h2>
  <div class="fieldContainer verticalFlow">
    <div class="oneColumnWidth">
      <input id="hiddenBillingCountryIdAdd" type="hidden" />
      <input id="hiddenBillingCountryIdRemove" type="hidden" />
      <%:Html.ListBox("BillingMemberDSSupportedByAtosFrom", (MultiSelectList)ViewData["DSSupportedBillingCountryListFrom"], new { size = "6"})%>
       </div>
    <div style="float: left; padding-top: 15px;" class="oneColumnWidth">
      <div>
        <input type="button" class="shiftButton" value=">" onclick="SiftListItems('BillingMemberDSSupportedByAtosFrom','BillingMemberDSSupportedByAtosTo','hiddenBillingCountryIdAdd','hiddenBillingCountryIdRemove')" />
      </div>
      <div>
        <input type="button" class="shiftButton" value="<" onclick="SiftListItemsBack('BillingMemberDSSupportedByAtosTo','BillingMemberDSSupportedByAtosFrom','hiddenBillingCountryIdAdd','hiddenBillingCountryIdRemove')" />
      </div>
    </div>
    <div class="oneColumnWidth">
      <%:Html.ListBox("BillingMemberDSSupportedByAtosTo", (MultiSelectList)ViewData["FutureBillingDSSupportedCountryTo"], new { size = "6"})%>
      </div>
    <div id="billingFuturePeriod" class="hidden">
      <label>
        Future Period:</label>
      <%:Html.TextBoxFor(dsRequired => dsRequired.DSReqCountriesAsBillingFuturePeriod, new { @id = "BillingFuturePeriod" })%>
    </div>
     </div>
  <div class="clear">
  </div>
 </div>