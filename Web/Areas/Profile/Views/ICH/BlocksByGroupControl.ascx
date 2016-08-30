<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MemberProfile.BlockGroup>" %>
<script type="text/javascript">

  // Following function is used to display BlockGroupExceptions on selecting BlockGroup.
  function DisplayExceptions(ids) {
    if (ids != null && lastSelection == ids)
      return true;

    // Check whether Exception row count i.e. User has added any Exception against Group, if yes alert 
    // an message to save details first and then proceed.
    if (exceptionRowCount > 0 || isEditExceptionFlag) {
      alert('You have unsaved Exception records. Please Save the records and proceed');
      if (lastSelection != null)
        $('#BlocksByGroupBlocksGrid').setSelection(lastSelection, false);
      return false;
    } // end if()
    else {
      if (ids == null) {
        ids = 0;
        if (jQuery("#BlocksByGroupExceptionsGrid").jqGrid('getGridParam', 'records') > 0) {
          jQuery("#BlocksByGroupExceptionsGrid").trigger('reloadGrid');
        }
      } // end if()
      else {
        lastSelection = ids;
        // Check whether id contains "#" character, if yes selected BlockGroup is not yet added to database.
        var isTempRowId = ids.indexOf('#');
        if (isTempRowId > 0) {
          $('#BlocksByGroupExceptionsGrid').clearGridData();
        }
        else {
          jQuery("#BlocksByGroupExceptionsGrid").jqGrid('setGridParam', { url: '<%: Url.Action("BlockesByGroupExceptionsGridData", "ICH", new { area = "Profile"})%>?groupId=' + ids, page: 1 });
          jQuery("#BlocksByGroupExceptionsGrid").trigger('reloadGrid');
        }
      } // end else
    } // end else
    return true;
  }
</script>
<div style="float: left;">
  <h2>
    Blocks By Group</h2>
  <div>
    <%Html.RenderPartial("~/Areas/Profile/Views/ICH/BlocksByGroupBlocksControl.ascx", ViewData["BlocksByGroupBlocksGrid"]); %>
  </div>
  <div class="buttonContainer">
    <input type="button" class="primaryButton" value="Add" onclick="ShowAddBlockGroupdialog();" />
  </div>
</div>
<div id="divAddBlocekeByGroups">
  <% Html.RenderPartial("AddBlockByGroups");%></div>
<div id="divAddBlockByGroupsException">
  <% Html.RenderPartial("AddBlockByGroupException");%></div>
<div style="float: left; padding-left: 20px;">
  <h2>
    Exceptions</h2>
  <div>
    <%Html.RenderPartial("~/Areas/Profile/Views/ICH/BlocksByGroupExceptionsControl.ascx", ViewData["BlocksByGroupExceptionsGrid"]); %>
  </div>
  <div class="buttonContainer">
    <input type="button" class="primaryButton" value="Add" onclick="ShowAddBlockGroupExceptiondialog();" />
  </div>
</div>
<div class="clear"></div>
<script type="text/javascript">
  var $AddBlockGroupdialog;
  var $AddBlockGroupExceptiondialog;
  $(document).ready(function () {
      $AddBlockGroupdialog = $('<div></div>')
		.html($("#divAddBlocekeByGroups"))
		.dialog({
		    autoOpen: false,
		    title: 'Add Block by group',
		    height: 200,
		    width: 700,
		    modal: true,
		    resizable: false
		});

      $AddBlockGroupExceptiondialog = $('<div></div>')
		.html($("#divAddBlockByGroupsException"))
		.dialog({
		    autoOpen: false,
		    title: 'Add Exception',
		    height: 200,
		    width: 400,
		    modal: true,
		    resizable: false,
		    close: function (event, ui) {
		        $('#ExceptionMemberText').unautocomplete();
		    },
		    open: function (event, ui) { registerAutocomplete('ExceptionMemberText', 'ExceptionMemberId', '<%:Url.Action("GetIchMemberListForZone", "Data", new { area = "" })%>', 0, true, null, '', '', "#hdnIchZoneId"); }
		});
  });

  function ShowAddBlockGroupdialog() {

    if (exceptionRowCount > 0)
      alert('You have unsaved Exception records. Please Save the records and proceed');
    else {
      $AddBlockGroupdialog.dialog('open');
      return false;
    } // end else
  } // end ShowAddBlockGroupdialog()

  function ShowAddBlockGroupExceptiondialog() {


      var groupId = jQuery('#BlocksByGroupBlocksGrid').getGridParam('selrow');      
     
    if (groupId == null) {
      alert("Please select group.");
    }
  else {
      
      var oldZoneId = $("#hdnIchZoneId").val();
      var newZoneId = $("#BlocksByGroupBlocksGrid input[name='blockGroupItems[" + groupId + "].ZoneTypeId']").val();
    
      if (!newZoneId) {
          var ret = jQuery("#BlocksByGroupBlocksGrid").jqGrid('getRowData', groupId);
          newZoneId = ret.ZoneTypeId;
      }
      

      if (oldZoneId != newZoneId) {
        $("#hdnIchZoneId").val(newZoneId);
        $('#ExceptionMemberText').val("");
        $('#ExceptionMemberText').flushCache();
      }

      $AddBlockGroupExceptiondialog.dialog('open');
      return false;
    }
  } 
      
</script>
