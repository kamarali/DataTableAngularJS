<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MemberProfile.Contact>" %>
<div>
  <h2>
    Search Existing Contacts</h2>
  <div class="searchCriteria">
    <div class="fieldContainer horizontalFlow">
      <div>
        <div>
          <label>
            First Name:</label>
          <%: Html.TextBoxFor(model => model.FirstName, new { @id = "searchFirstName" })%>
        </div>
        <div>
          <label>
            Last Name:</label>
          <%: Html.TextBoxFor(model => model.LastName, new { @id = "searchLastName" })%>
        </div>
        <div>
          <label>
            Email Id:</label>
          <%:Html.TextBoxFor(model => model.EmailAddress, new { @id = "searchEmailAddress" })%>
        </div>
        <div>
          <label>
            Staff Id:</label>
          <%:Html.TextBoxFor(model => model.StaffId, new { @id = "searchStaffId" })%>
        </div>
      </div>
      <div class="buttonContainer">
        <input type="button" class="primaryButton" value="Search" id="btnSearch" name="Search"
          onclick="seachContacts('<%:Url.Action("ContactsData", "Member", new { area = "Profile"}) %>');" />
      </div>
    </div>
    <div class="clear">
    </div>
  </div>
</div>
<script type="text/javascript">
  function seachContacts(postUrl) {
    var firstName = $('#searchFirstName').val();
    var lastName = $('#searchLastName').val();
    var emailAddress = $('#searchEmailAddress').val();
    var staffId = $('#searchStaffId').val();
    var selectedMemberId = <%: Model.MemberId %>;
    
    var url = postUrl + "?" + $.param({ firstName: firstName, lastName: lastName, emailAddress: emailAddress, staffId: staffId, selectedMemberId: selectedMemberId });
    $("#ContactsGrid").jqGrid('setGridParam', { url: url }).trigger("reloadGrid", [{ page: 1}]);

  }
</script>
