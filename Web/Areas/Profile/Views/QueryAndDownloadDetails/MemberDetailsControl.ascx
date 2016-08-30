<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MemberProfile.QueryAndDownloadDetails>" %>
<div>
  <div id="searchCriteriaDiv" class="solidBox dataEntry">
    <div class="fieldContainer horizontalFlow">
      <div class="bottomLine">
        <h2>
          Report Criteria</h2>
        <div>
          <input type="radio" id="rdMemberDetails" name="rdDetails" value="Member Details" checked="checked" />Member Details
        </div>
        <div>
          <input type="radio" id="rdContactDetails" name="rdDetails" value="Contact Details" />Contact Details
        </div>
      </div>
    </div>
    <div class="fieldContainer horizontalFlow">
      <h2>
        Member Details</h2>
      <div>
        <div class="twoColumn">
          <label>
            Member Name</label>
          <%: Html.TextBoxFor(queryNDownloadDetails => queryNDownloadDetails.CommercialName, new { @class = "autocComplete largeTextField" })%>
          <%: Html.HiddenFor(queryNDownloadDetails => queryNDownloadDetails.Id)%>
          <%: Html.Hidden("userCategoryID", ViewData["UserCategoryID"])%>          
          <%: Html.Hidden("MemberId", ViewData["MemberId"])%>
          </div>
        <!-- Country Name -->
        <div>
          <label>
            Country</label>
          <%: Html.CountryCodeDropdownListFor(queryNDownloadDetails => queryNDownloadDetails.Country)%>
        </div>
        <div class="clear">
        </div>
      </div>
      <div class="topLine">
        <div>
          <label for="ach">
            ACH Members</label>
          <%: Html.CheckBox("ach")%>
        </div>
        <div>
          <label for="ich">
            ICH Members</label>
          <%: Html.CheckBox("ich")%>
        </div>
        <div>
          <label for="dual">
            Dual Members</label>
          <%: Html.CheckBox("dual")%>
        </div>
        <div>
          <label for="nonch">
            Non-CH Members</label>
          <%: Html.CheckBox("nonch")%>
        </div>
        <div>
          <label for="iata">
            IATA Members</label>
          <%: Html.CheckBox("iata")%>
        </div>
      </div>
      <div class="bottomLine" id="contactDetailsSeparator">
      </div>
    </div>
    <div class="fieldContainer verticalFlow" id="contactDetails">
      <div class="twoColumn">
        <div>
          <label>
            Contact Name</label>
          <select name="ContactName" id="ContactName">
            <option title="Please Select" value="">Please Select</option>
          </select>
        </div>
        <div>
          <label>
            Email</label>
          <select name="Email" id="Email">
            <option title="Please Select" value="">Please Select</option>
          </select>
        </div>
        <div>
          <strong>Report Display Options:</strong><br />
          <%: Html.RadioButton("tabularFormat", true, true, new { style = "display: inline;" })%>
          <label for="tabularFormat" style="display: inline;">
            Tabular Format</label>
          <%: Html.RadioButton("tabularFormat", false, false, new { id = "addressLabelFormat", style = "display: inline;" })%>
          <label for="addressLabelFormat" style="display: inline;">
            Address-Label Format</label>
        </div>
      </div>
      <div class="twoColumnWidth">
        
      </div>
    </div>
    <div class="fieldContainer horizontalFlow buttonContainer">
      <input type="button" class="secondaryButton" value="Clear All" onclick="clear_form_elements('#searchCriteriaDiv');" />
    </div>
    <div class="clear">
    </div>
  </div>
</div>
<script type="text/javascript">
    $(function () {
        var urlGetAvailMemberMeta = '<%: Url.Action("GetAvailableMemberProfileMetadata", "QueryAndDownloadDetails", new { area = "Profile"}) %>';
        var urlGetContactDetails = '<%: Url.Action("GetContactDetailsForMember", "QueryAndDownloadDetails", new { area = "Profile"}) %>';
        var urlGetContactTypeSubGroups = '<%: Url.Action("GetContactTypeSubGroups", "QueryAndDownloadDetails", new { area = "Profile"}) %>';
        var urlGetContactTypes = '<%: Url.Action("GetContactTypes", "QueryAndDownloadDetails", new { area = "Profile"}) %>';

        readyQueryAndDownload(urlGetAvailMemberMeta, urlGetContactDetails, urlGetContactTypes, urlGetContactTypeSubGroups);
        $(document).ready(function () { $('#CommercialName').focus(); });
    });

</script>
