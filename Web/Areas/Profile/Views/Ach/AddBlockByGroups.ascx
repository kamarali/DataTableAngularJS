<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MemberProfile.BlockGroup>" %>
<div>
  <div class="solidBox dataEntry">
    <div class="fieldContainer horizontalFlow">
      <div>
        <div>
          <label>
            <span class="required" style="color: Red;">*</span> Zone:</label>
          <%:Html.TextBoxFor(model => model.BlockingRuleId, new {@class = "hidden",@id="blockByGroupblockingRuleId"})%>
          <%:Html.IchZoneDropdownListFor(model => model.ZoneTypeId, new Dictionary<string,object>{ { "id", "AchZoneTypeId" },{"disabled","disable" }})%>
        </div>
        <div>
          <label>
            <span class="required" style="color: Red;">*</span> By/Against:</label>
          <%:Html.BlockedAgainstDropdownListFor(model => model.ByAgainst)%>
        </div>
      </div>
    </div>
    <div class="fieldContainer horizontalFlow">
      <div>
        <div>
          <label>
            PAX:</label>
          <%:Html.CheckBoxFor(model => model.Pax, new { id = "groupPax", @checked = true })%>
        </div>
        <div>
          <label>
            CGO:</label>
          <%:Html.CheckBoxFor(model => model.Cargo, new { id = "groupCargo", @checked = true })%>
        </div>
        <div>
          <label>
            UATP:</label>
          <%:Html.CheckBoxFor(model => model.Uatp, new { id = "groupUatp", @checked = true })%>
        </div>
        <div>
          <label>
            MISC:</label>
          <%:Html.CheckBoxFor(model => model.Misc, new { id = "groupMisc", @checked = true })%>
        </div>
      </div>
    </div>
    <div class="buttonContainer">
      <input class="primaryButton ignoredirty" type="submit" value="Add" onclick="if(addBlockByGroupsTableRow($('#BlocksByGroupBlocksGrid'))) $AddBlockGroupdialog.dialog('close');" />
      <input class="secondaryButton" type="submit" value="Exit" onclick="$AddBlockGroupdialog.dialog('close');" />
    </div>
    <div class="clear">
    </div>
  </div>
</div>
<script type="text/javascript">
  function AddBlockMemberGroup(postUrl) {


    var zoneId = $('#ZoneTypeId').val();
    var blockedAgainst = $('#ByAgainst').val();
    var Pax = $('#Pax').prop('checked');
    var Cargo = $('#Cargo').prop('checked');
    var Uatp = $('#Uatp').prop('checked');
    var Misc = $('#Misc').prop('checked');
    var blockByGroupblockingRuleId = $('#blockByGroupblockingRuleId').val();
    $.ajax({
      type: "POST",
      url: postUrl,
      data: { blockedAgainst: blockedAgainst, zoneId: zoneId, pax: Pax, cgo: Cargo, uatp: Uatp, misc: Misc, blockingRuleId: blockByGroupblockingRuleId },
      dataType: "json",
      success: function (result) {
        if (result.IsFailed == false) {
          alert("Failed");
        }
        else {
          alert("Blocked group added successfully");
        }
      }
    });
    jQuery("#BlocksByGroupBlocksGrid").trigger('reloadGrid');
  }

  // Following variable is declared to set row count.
  var tableRowCount = 0;

  // Following function is used to add Row to BlockByGroups JQgrid when user clicks on Save of Modal Popup.
  function addBlockByGroupsTableRow(jQtable) {
    jQtable.each(function () {
      // Get table
      var $table = $(this);
      // Get selected value of Zone type dropdown
      var zoneId = $('#ZoneTypeId option:selected').val();
      // Get selected value of By/Against type dropdown
      var blockedAgainst = $('#ByAgainst option:selected').val();

      // Check whether checkboxes are checked
      var areCheckBoxesChecked = !$('#groupPax').prop('checked') && !$('#groupCargo').prop('checked') && !$('#groupUatp').prop('checked') && !$('#groupMisc').prop('checked');

      // Check whether zoneID and blockedAgainst are not equal to null, get Checkbox values from Popup window. 
      if (zoneId != "" && blockedAgainst != "" && !areCheckBoxesChecked) {
        // If BlockByGroupZoneIdString variable is null give call to "GetBlockByGroupsZoneId" which will return
        // ZoneId+ByAgainst string  
        if (BlockByGroupZoneIdString == "") {
          $.ajax({
            type: "GET",
            url: '<%: Url.Action("GetBlockByGroupsZoneId", "Ach", new { area = "Profile"}) %>',
            data: { blockingRuleId: $('#blockByGroupblockingRuleId').val() },
            success: function (response) {
              BlockByGroupZoneIdString = response;
          },
          dataType: "text", 
            async: false
          });
        } // end if()

        // Split ZoneId+ByAgainst string on comma and add to array
        var zoneIdArray = BlockByGroupZoneIdString.split(",");

        // Iterate through zoneIdArray and check whether ZoneId+ByAgainst being added, previously exists in the array
        for (var i = 0; i < zoneIdArray.length; i++) {
          if (zoneIdArray[i] == zoneId + blockedAgainst) {

            // Give alert message
            alert("Block by Group record for zone " + $('#ZoneTypeId option:selected').text() + " already exists.");

            // Check Pax checkbox
            $('#groupPax').prop('checked', true);
            // Check Cargo checkbox
            $('#groupCargo').prop('checked', true);
            // Check Uatp checkbox
            $('#groupUatp').prop('checked', true);
            // Check Misc checkbox
            $('#groupMisc').prop('checked', true);
            // Set Zone Type dropdown to default value
            $("#ZoneTypeId").val("3");
            // Set ByAgainst dropdown to default value
            $("#ByAgainst").val("");

            // return
            return;
          } // end if()
        } // end for()

        // Append newly added ZoneId and by/Against to BlockByGroupZoneIdString string
        BlockByGroupZoneIdString += "," + zoneId + blockedAgainst;

        var debtorblockingRuleId = $('#DebtorblockingRuleId').val();

        var Pax = "false"
        var PaxCheck = '';
        if ($('#groupPax').prop('checked')) {
          Pax = "true";
          PaxCheck = 'checked="checked"';
        }
        var Cargo = "false";
        var CargoCheck = '';
        if ($('#groupCargo').prop('checked')) {
          Cargo = "true";
          CargoCheck = 'checked="checked"';
        }
        var Uatp = "false";
        var UatpCheck = '';
        if ($('#groupUatp').prop('checked')) {
          Uatp = "true";
          UatpCheck = 'checked="checked"';
        }
        var Misc = "false";
        var MiscCheck = '';
        if ($('#groupMisc').prop('checked')) {
          Misc = "true";
          MiscCheck = 'checked="checked"';
        }

        // Get number of td's in the last table row
        var n = $('tr:last td', this).length;
        // Create dynamic "td" and append it to table body
        var tds = '<tr class="ui-widget-content jqgrow ui-row-ltr" id="' + tableRowCount + '">';
        // Iterate and add "td"
        for (var i = 1; i <= n; i++) {
          switch (i) {
            case 1:
              tds += '<td title="" role="gridcell" aria-describedby="BlocksByGroupBlocksGrid_Id"><a href="javascript:DeleteGroupRow(' + tableRowCount + ');"><img src="<%: ResolveUrl("~/Content/Images/delete.png") %>" style="border-style: none;" title="Delete"></a>';
              tds += '<input type="hidden" name="blockGroupItems[' + tableRowCount + '].IsDeleted" value="false" /></td>';
              tds += '<input type="hidden" name="blockGroupItems[' + tableRowCount + '].ByAgainst" value="' + blockedAgainst + '" /></td>';
              tds += '<input type="hidden" name="blockGroupItems[' + tableRowCount + '].ZoneTypeId" value="' + zoneId + '" /></td>';
              tds += '<input type="hidden" id="blockGroupItems[' + tableRowCount + '].TempRowCount" name="blockGroupItems[' + tableRowCount + '].TempRowCount" value="' + tableRowCount + '" /></td>';
              break;
            case 2: tds += '<td aria-describedby="BlocksByGroupBlocksGrid_IsBlockAgainst" title="" style="" role="gridcell">' + $('#ByAgainst option:selected').text() + '</td>'; break;
            case 3: tds += '<td aria-describedby="BlocksByGroupBlocksGrid_DisplayZoneType" title="" style="" role="gridcell">' + $('#ZoneTypeId option:selected').text(); +'</td>'; break;
            case 4: tds += '<td aria-describedby="BlocksByGroupBlocksGrid_Pax" title="" style="" role="gridcell"><input type="checkbox" name="blockGroupItems[' + tableRowCount + '].Pax" value="true"' + PaxCheck + ' ></td>'; break;
            case 5: tds += '<td aria-describedby="BlocksByGroupBlocksGrid_Cargo" title="" style="" role="gridcell"><input  type="checkbox" name="blockGroupItems[' + tableRowCount + '].Cargo" value="true"' + CargoCheck + '></td>'; break;
            case 6: tds += '<td aria-describedby="BlocksByGroupBlocksGrid_Uatp" title="" style="" role="gridcell"><input type="checkbox" name="blockGroupItems[' + tableRowCount + '].Uatp" value="true"' + UatpCheck + '></td>'; break;
            case 7: tds += '<td aria-describedby="BlocksByGroupBlocksGrid_Misc" title="" style="" role="gridcell"><input  type="checkbox" name="blockGroupItems[' + tableRowCount + '].Misc" value="true"' + MiscCheck + '></td>'; break;
          } // end switch() statement
        } // end for()

        // Append "td"
        tds += '</tr>';
        if ($('tbody', this).length > 0) {
          $('tbody', this).append(tds);
        } else {
          $(this).append(tds);
        }

        // Increment table row count 
        tableRowCount++;

        // Check Pax checkbox
        $('#groupPax').prop('checked', true);
        // Check Cargo checkbox
        $('#groupCargo').prop('checked', true);
        // Check Uatp checkbox
        $('#groupUatp').prop('checked', true);
        // Check Misc checkbox
        $('#groupMisc').prop('checked', true);
        // Set Zone Type dropdown to default value
        $("#ZoneTypeId").val("3");
        // Set ByAgainst dropdown to default value
        $("#ByAgainst").val("");

        // Close AddBlockGroup dialog
        $AddBlockGroupdialog.dialog('close');

      } // end if()
      else {
        if (zoneId == "" && blockedAgainst == "" && areCheckBoxesChecked == true)
          alert("Please select Zone Type, By/Against and atleast one billing category has to be selected");
        else if (zoneId == "" && blockedAgainst == "")
          alert("Please select Zone Type and By/Against");
        else if (zoneId == "" && areCheckBoxesChecked == true)
          alert("Please select Zone Type and atleast one billing category has to be selected");
        else if (zoneId == "")
          alert("Please select Zone Type");
        else if (blockedAgainst == "")
          alert("Please select By/Against");
        else
          alert("Atleast one billing category has to be selected");
      }
    });

    // Check whether checkboxes are checked
var checkBoxesChecked = !$('#groupPax').prop('checked') && !$('#groupCargo').prop('checked') && !$('#groupUatp').prop('checked') && !$('#groupMisc').prop('checked');

    if ($('#ZoneTypeId option:selected').val() != "" && $('#ByAgainst option:selected').val() != "" && !checkBoxesChecked)
      return true;
    else
      return false;

  } // end addBlockByGroupsTableRow()

  // Following function is used to delete temporary rows added to BlockByGroups grid
  function DeleteGroupRow(rowId) {
    // Ask user whether he wants to delete the row
    if (confirm("Are you sure you want to delete this record?")) {
      // Get innerHtml within the row
      var rowInnerHtml = document.getElementById('BlocksByGroupBlocksGrid').rows[rowId];
      if (rowInnerHtml) {
        // Insert new Cell i.e."<td>" to selected row. Row contains "6" "td's", so insert "7th" cell 
        var newCell = rowInnerHtml.insertCell(7);
        // Add hidden field to newly added cell, setting models "IsDeleted" property to true
        newCell.innerHTML = '<input type="hidden" name="blockGroupItems[' + rowId + '].IsDeleted" value="true" />';
      }

      // Get innerHtml within the row
      var exceptionRowInnerHtml = document.getElementById('BlocksByGroupExceptionsGrid').rows[rowId];
      if (exceptionRowInnerHtml) {
        // Insert new Cell i.e."<td>" to selected row. Row contains "3" "td's", so insert "3rd" cell 
        var newExceptionCell = exceptionRowInnerHtml.insertCell(3);
        // Add hidden field to newly added cell, setting models "IsDeleted" property to true
        newExceptionCell.innerHTML = '<input type="hidden" name="blockGroupExceptionItems[' + rowId + '].IsDeleted" value="true" />';
      }

      // Get related Exception rows for Group to be deleted 
      var relatedExceptionRows = $('#BlocksByGroupExceptionsGrid tr[name=' + rowId + ']').length;

      // Subtract related exception row count from global exceptionRowCount  
      exceptionRowCount = exceptionRowCount - relatedExceptionRows;

      var zoneId = $("#BlocksByGroupBlocksGrid input[name='blockGroupItems[" + rowId + "].ZoneTypeId']").val();

      var byAgainstGroup = $("#BlocksByGroupBlocksGrid input[name='blockGroupItems[" + rowId + "].ByAgainst']").val();

      var newGroupId = zoneId + byAgainstGroup;

      var newGroupIdString = "";
      // Split MemberId string on comma and add to array
      var groupIdArray = BlockByGroupZoneIdString.split(",");
      // Iterate through memberIdArray and check whether MemberId being added previously exists in the array
      for (var i = 0; i < groupIdArray.length; i++) {
       
          if (groupIdArray[i] != newGroupId && groupIdArray[i] != "" && groupIdArray[i] != undefined && groupIdArray[i] != null) {

              newGroupIdString += "," + groupIdArray[i];
          }
      }
    
      BlockByGroupZoneIdString = newGroupIdString;

      // Hide row from BlockByGroup grid
      $('#BlocksByGroupBlocksGrid tr[id=' + rowId + ']').hide();

      // Hide related rows from Exceptions grid
      $('#BlocksByGroupExceptionsGrid tr[name=' + rowId + ']').hide();

    } // end if()
  } // end DeleteRow()

  // Following function is used to delete row from BlockByGroups grid which are populated from Database. 
  function DeleteGroupBlockRowFromDatabase(id) {
    // Ask user whether he wants to delete the row
    if (confirm("Are you sure you want to delete this record?")) {
      // Add deleted rows id to hidden field. If hidden field is "" add rowId. If more than one row 
      // is deleted append rowId's comma separated
      if ($('#DeletedGroupByBlockString').val() == "")
        $('#DeletedGroupByBlockString').val(id);
      else
        $('#DeletedGroupByBlockString').val($('#DeletedGroupByBlockString').val() + ',' + id);

      // Declare variable to get string of ExceptionId
      var exceptionString = "";
      // Execute "BlockesByGroupExceptionsIdString" action which will return ExceptionId string for Group 
      // to be deleted
      $.ajax({
        type: "GET",
        url: '<%: Url.Action("BlockesByGroupExceptionsIdString", "Ach", new { area = "Profile"}) %>',
        data: { groupId: id },
        success: function (response) {
          exceptionString = response;
        },
        async: false
      });

      // Split Exception string on comma and add to array
      var exceptionIdArray = exceptionString.split(",");

      // Iterate through ExceptionId array and hide related rows from Exception grid
      for (var i = 0; i < exceptionIdArray.length; i++) {
        // Hide related Exception rows
        $('#BlocksByGroupExceptionsGrid tr[id=' + exceptionIdArray[i] + ']').hide();
      } // end for()

    // Hide the row from BockGroups grid
    var rowId = jQuery('#BlocksByGroupBlocksGrid').getGridParam('selrow');
    $('#BlocksByGroupBlocksGrid tr[id=' + rowId + ']').hide();
      
    } // end if()
  } // end DeleteCreditorRowFromDatabase()

</script>
