<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MemberProfile.BlockGroupException>" %>
<script type="text/javascript">
  $(document).ready(function () {
  // CMP#596: CIT CMP 596:- Auto Complete List of Block By Group field in Blocking Rule is not populating Date correctly in drop down.
//      registerAutocomplete('ExceptionMemberText', 'ExceptionMemberId', '<%:Url.Action("GetMemberList", "Data", new { area = "" })%>', 0, true, null, '', '', "#hdnIchZoneId");

  });
</script>
<div>
  <div class="solidBox dataEntry">
    <div class="fieldContainer horizontalFlow">
      <div>
        <div>
          <label>
            <span class="required" style="color: Red;">*</span>Member Code:</label>
          <!--    CMP#596: Length of Member Accounting Code to be Increased to 12 
                  Desc: Non layout related IS-WEB screen changes.
                  Ref: FRS Section 3.1 Table 2, Section 3.2 Table 8,Section 3.3 Table 12, Section 3.5 Table20 */ -->
          <%: Html.TextBoxFor(model => model.ExceptionMemberText, new { @Class = "autocComplete textboxWidth" })%>
          <%: Html.HiddenFor(model => model.ExceptionMemberId, new { @id = "ExceptionMemberId" })%>
          <input id="hdnIchZoneId" type="hidden" />
        </div>
      </div>
    </div>
    <div class="buttonContainer">
      <input class="primaryButton ignoredirty" type="submit" value="Add" onclick="addBlockbyGroupException();" />
      <input class="secondaryButton" type="submit" value="Exit" onclick="$AddBlockGroupExceptiondialog.dialog('close');" />
    </div>
    <div class="clear">
    </div>
  </div>
</div>
<script type="text/javascript">

  // Exception edit flag.
  var isEditExceptionFlag = false;
  var lastSelection = null;

  // Exception row count
  var rowCount = 0;

  function addBlockbyGroupException() {
    var ownerMember = $("#MemberId").val();
    var exceptionMember = $("#ExceptionMemberId").val();

    if (ownerMember == exceptionMember) {
      alert("Blocked by Group Member Exception can not be same as the member for which the blocking rule is being created.");
    }
    else {
      var isMemberCodeAdded = AddBlockMemberException('<%:Url.Action("GetMemberDetails", "Ich", new { area = "Profile"}) %>');

      if (isMemberCodeAdded)
        $AddBlockGroupExceptiondialog.dialog('close');
    }
  }

  function AddBlockMemberException(postUrl) {
    // Get selected groupId from BlockByGroup Jqgrid to add exception  
      var groupId = jQuery('#BlocksByGroupBlocksGrid').getGridParam('selrow');       

    // Check whether id contains "a" character, if yes selected BlockGroup is not yet added to database.
    var isTempRowId = groupId.indexOf('#');

    if (isTempRowId > 0)
      groupId = groupId.substring(0, groupId.length - 1);

    // Get selected Exception MemberId from Modal popup    
    var MemberId = $('#ExceptionMemberId').val();

    // If MemberId != "", get Member details from DB and add record to BlockGroupException grid     
    if (MemberId == "") {
      alert("Please select member.");
      return false;
    }
    else {
      if (ExceptionIdString == "") {
        $.ajax({
          type: "GET",
          url: '<%: Url.Action("BlockesByGroupExceptionsIdString", "Ich", new { area = "Profile"}) %>',
          data: { groupId: groupId },
          success: function (response) {
            ExceptionIdString = response;
        },
        dataType: "text", 
          async: false
        });
      } // end if()

      // Split exceptionIdArray string on comma and add to array
      var exceptionIdArray = ExceptionIdString.split(",");

      // Iterate through exceptionIdArray and check whether MemberId being added previously exists in the array
      for (var i = 0; i < exceptionIdArray.length; i++) {
        if (exceptionIdArray[i] == MemberId) {

          // Give alert message
          alert("Exception for " + $("#ExceptionMemberText").val() + " member already exists.");

          // return
          return;
        } // end if()
      } // end for()

      // Append newly added ExceptionMemberId to ExceptionIdString string
      ExceptionIdString += "," + MemberId;

      $.ajax({
          type: "GET",
          url: postUrl,
          data: { memberId: MemberId },
          success: function (result) {

              $('#BlocksByGroupExceptionsGrid').each(function () {

                  // Get result string which has MemberCode and MemberName 
                  var memberDetails = result.toString();
                  // Declare an array for member details
                  var detailsArray = new Array();
                  // Split result string and add it to array
                  
                  detailsArray = memberDetails.split('####');

                  // Get table
                  var $table = $(this);
                  // Get number of td's in the last table row
                  var n = $('tr:last td', this).length;
                  // Create dynamic "td" and append it to table body 
                  var tds = '<tr class="ui-widget-content jqgrow ui-row-ltr" name="' + groupId + '" id="' + rowCount + '">';

                  // Iterate and add "td"
                  for (var i = 1; i <= n; i++) {
                      switch (i) {
                          case 1:
                              tds += '<td> <a href="javascript:DeleteExceptionRow(' + rowCount + ')"><img src="<%: ResolveUrl("~/Content/Images/delete.png") %>" style="border-style: none;" title="Delete"></a>';
                              tds += '<input type="hidden" name="blockGroupExceptionItems[' + rowCount + '].ExceptionMemberId" value="' + MemberId + '" /> </td>';
                              tds += '<input type="hidden" name="blockGroupExceptionItems[' + rowCount + '].BlockGroupId" value="' + groupId + '" /> </td>';
                              break;

                          case 2: tds += '<td aria-describedby="BlocksByGroupExceptionsGrid_ExceptionMemberId" role="gridcell" title="">' + detailsArray[1] + "-" + detailsArray[2] + '</td>'; break;
                          case 3: tds += '<td aria-describedby="BlocksByGroupExceptionsGrid_ExceptionMemberId" role="gridcell" title="">' + detailsArray[0] + '</td>'; break;
                      } // end switch statement
                  } // end for()

                  // Append td
                  tds += '</tr>';
                  if ($('tbody', this).length > 0) {
                      $('tbody', this).append(tds);
                  } else {
                      $(this).append(tds);
                  }

                  // Increment table row count 
                  rowCount++;

                  // Increment Exception row count 
                  exceptionRowCount++;

                  // Set ExceptionMember autocomplete value to null
                  $('#ExceptionMemberText').val("");
                  $('#ExceptionMemberId').val(0);
              });
          }
      });
      return true;
    } // end else
  } // end AddBlockMemberException()

  // Following function is used to delete row from BlockByGroupExceptions JqGrid
  function DeleteExceptionRow(rowId) {
      var newcreditorMemberIdString = "";
    var confirmDelete = confirm("Are you sure you want to delete this record?");
    if (confirmDelete) {
      //Append hidden field to indicate that this row is deleted.
      $("#BlocksByGroupExceptionsGrid tr[id='" + rowId + "'] td:first").append('<input type="hidden" id="addedTd" name="blockGroupExceptionItems[' + rowId + '].IsDeleted" value="true" />');

      // Decrement ExceptionRow count;
      exceptionRowCount--;
    
      var memberId = $("#BlocksByGroupExceptionsGrid input[name='blockGroupExceptionItems[" + rowId + "].ExceptionMemberId']").val();
     
      // Split MemberId string on comma and add to array
      var memberIdArray = ExceptionIdString.split(",");
      // Iterate through memberIdArray and check whether MemberId being added previously exists in the array
      for (var i = 0; i < memberIdArray.length; i++) {

          if (memberIdArray[i] != memberId && memberIdArray[i] != "" && memberIdArray[i] != undefined && memberIdArray[i] != null) {

              newcreditorMemberIdString += "," + memberIdArray[i];
          }
      }

      ExceptionIdString = newcreditorMemberIdString;

      // Hide deleted row
      $('#BlocksByGroupExceptionsGrid tr[id=' + rowId + ']').hide();
    } // end if()
    else
      return;
  } // end DeleteExceptionRow()

  // Following function is used to delete row from Creditors grid which are populated from Database. 
  function DeleteExceptionRowFromDatabase(id) {

    // Get selected block group Id  
      var groupId = jQuery('#BlocksByGroupBlocksGrid').getGridParam('selrow');     

    // Ask user whether he wants to delete the row
    if (confirm("Are you sure you want to delete this record?")) {
      // Added deleted rows id to hidden field. If hidden field is "" add rowId. If more than one row 
      // is deleted append rowId's comma separated
      if ($('#DeletedExceptionRowString').val() == "")
        $('#DeletedExceptionRowString').val(groupId + '|' + id);
      else
    $('#DeletedExceptionRowString').val($('#DeletedExceptionRowString').val() + ',' + groupId + '|' + id);

      //Set flag for exception data change.
      isEditExceptionFlag = true;

      // Hide the row
      $('#' + id).hide();
    } // end if()
  } // end DeleteCreditorRowFromDatabase()

</script>
