<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MemberProfile.EBillingConfiguration>" %>
<div id="searchCriteriaDiv" class="solidBox dataEntry">
<h2>Select Country</h2>
  <div class="fieldContainer verticalFlow">
    <div class="oneColumnWidth">
     <input id="hiddenCountryIdAdd" type="hidden" />
     <input id="hiddenCountryIdRemove" type="hidden" />
      <%:Html.ListBox("BilledMemberDSSupportedByAtosFrom", (MultiSelectList)ViewData["DSSupportedBilledCountryListFrom"], new { size = "6"})%>
      </div>
      <div style="float: left; padding-top: 15px;" class="oneColumnWidth">
    <div>
     <input type="button" class="shiftButton" value=">"  onclick="SiftListItems('BilledMemberDSSupportedByAtosFrom','BilledMemberDSSupportedByAtosTo','hiddenCountryIdAdd','hiddenCountryIdRemove')"/>
    </div>
    <div>
       <input type="button" class="shiftButton" value="<" onclick="SiftListItemsBack('BilledMemberDSSupportedByAtosTo','BilledMemberDSSupportedByAtosFrom','hiddenCountryIdAdd','hiddenCountryIdRemove')" />      
       
    </div>
  </div>
    <div class="oneColumnWidth">
      <%:Html.ListBox("BilledMemberDSSupportedByAtosTo", (MultiSelectList)ViewData["FutureBilledDSSupportedCountryTo"], new { size = "6"})%>
      </div>
     
   <div id="billedFuturePeriod" class="hidden">
    <label>
       Future Period:</label>
     <%:Html.TextBoxFor(dsRequired => dsRequired.DSReqCountriesAsBilledFuturePeriod, new {@id = "BilledFuturePeriod"})%>
   </div>
     </div>
  <div class="clear"></div>
  </div>

