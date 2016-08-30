<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MemberProfile.ContactType>" %>
<div>
  <h2>
    Search Criteria</h2>
  <div class="fixedDiv">
    <div class="fieldContainer horizontalFlow">
      <div>
        <div class="threeColumn">
          <label>
            Group:</label>
          <%: Html.ContactTypeGroupDropdownListFor(model => model.GroupId)%>
        </div>
        <div class="threeColumn">
          <label>
            Subgroup:</label>
          <%: Html.ContactTypeSubGroupDropdownList(model => model.SubGroupId)%>
        </div>
        <div class="threeColumn">
          <label>
            Type:</label>
          <%: Html.TypeOfContactTypeDropdownListFor(model => model.TypeId)%>
        </div>
      </div>
    </div>
    <div class="buttonContainer">
      <input type="button" class="primaryButton" value="Search" id="btnSearchAssignment" name="Search" />
    </div>
    <div class="clear">
    </div>
  </div>
</div>
<script type="text/javascript">
    jQuery(document).ready(function () {

        $('#btnSearchAssignment').click(function () {
            //unload grid to load grid for new search criteria.
            $.jgrid.gridUnload('#list');
            //Get post data.
            var postData = {
                contactTypeCategory: '',
                groupId: $('#hdnGroupId').val(),
                subGroupId: $('#hdnSubGroupId').val(),
                typeId: $('#hdnTypeId').val(),
                columns: '',
                // SCP96750: insted of using control, ViewData is used get the selected member Id which is set from controller.
                selectedMemberId: '<%:ViewData["SelectedMemberId"] %>'
            };

            //On search click load grid.
            loadContactGrid('<%: Url.Action("GetMyGridDataJson", "Member", new { area = "Profile"}) %>', postData);
        });

        //Populate contact type sub groups and contact types based on contact groups selected.
        $('#GroupId').change(function () {
            $("#SubGroupId").attr('disabled', 'disabled');
            var groupIdList = $.map($('#GroupId :selected'), function (e) { return $(e).val(); })
            getSubGroups('<%: Url.Action("GetContactTypeSubGroups", "QueryAndDownloadDetails", new { area = "Profile"}) %>', groupIdList.join(','));
            $("#SubGroupId").removeAttr('disabled');
            $('#hdnSubGroupId').val('');
        });
    });
</script>
