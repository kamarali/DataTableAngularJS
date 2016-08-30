<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Reports.MemberLocations.MemberLocationModel>" %>

  <script type="text/javascript">

      $(document).ready(function () {
      // SCP186215: Member Code Mismatch between Member and Location Details
          registerAutocomplete('ParticipantText', 'ParticipantID', '<%:Url.Action("GetAllMemberList", "Data", new { area = "", userCategoryId = ViewData["UserCategoryID"], isFromMemContactRprt = "true" })%>', 0, true, null);
      });
  </script>

<div class="solidBox">
  <div class="fieldContainer horizontalFlow">    
      <div>
        <label>
          Member Code:</label>
          <%:Html.TextBoxFor(model => model.ParticipantText, new { @Id = "ParticipantText", @class = "autocComplete" })%>
          <%: Html.HiddenFor(model => model.ParticipantID, new { @id = "ParticipantID" })%>          
          <p>
        <label>
          Location ID:</label>
        <%: Html.TextBoxFor(searchCriteria => searchCriteria.LocationId, new { @Id = "locationId", @class = "alphaNumeric", @maxLength = 7 })%>
        </p>       
      </div>            
  </div>
  <div class="clear">
  </div>
</div>
