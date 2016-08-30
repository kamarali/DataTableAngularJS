<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<div id="divAssignmentSearchResult">
  <div style="width: 100%; height: 330px; overflow: auto;">
    <table id="list">
    </table>
    <div id="pager">
    </div>
  </div>
</div>
<div>
  <input type="button" class="primaryButton" value="Save" id="btnSaveAssignment" name="btnSaveAssignment" />
  <input type="button" class="secondaryButton" value="Close" id="Button1" name="btnClose" onclick="if ($('#divType').val() =='divContactAssignmentSearch') { $('#divContactAssignmentSearch').dialog('close'); } else { $('#divContactAssignmentSearchResult').dialog('close'); }" />
  <input id="hdnIsICHTypeChanged" type="hidden" value="false" />
  <input id="hdnIsDataChanged" type="hidden" value="0" /></div>
  <input id="divType" type="hidden" />
<script type="text/javascript">
  var curContactGridPage;
  $(document).ready(function () {
    //Save Data
    $('#btnSaveAssignment').click(function () {
      saveContactAssignment('<%: Url.Action("SaveAllContactAssignment", "Member", new { area = "Profile"}) %>');
    });
  });
</script>


 