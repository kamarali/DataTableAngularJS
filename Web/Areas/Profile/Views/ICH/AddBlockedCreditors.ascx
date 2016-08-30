<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MemberProfile.BlockMember>" %>
<script type="text/javascript">
  $(document).ready(function () {
    // SCP90120: KAL: Issue with Blocking rules master [changed the method called to display the memberlist in dropdown list]
    registerAutocomplete('CreditorMemberCode', 'CreditorMemberId', '<%:Url.Action("GetMemberListForIchOrAch", "Data", new { area = "", menuType="bothAchIch" })%>', 0, true, null);
  });
</script>
<div>
  <div class="solidBox dataEntry">
    <div class="fieldContainer horizontalFlow">
      <div>
        <div>
          <label>
            <span class="required" style="color: Red;">*</span>Member Code:</label>
          <%:Html.TextBoxFor(model => model.BlockingRuleId, new {@class = "hidden",@id="creditorblockingRuleId"})%>
          <!--    CMP#596: Length of Member Accounting Code to be Increased to 12 
                  Desc: Non layout related IS-WEB screen changes.
                  Ref: FRS Section 3.1 Table 2, Section 3.2 Table 8,Section 3.3 Table 12, Section 3.5 Table20 */ -->
          <%: Html.TextBoxFor(ichModel => ichModel.MemberText, new { @id = "CreditorMemberCode", @Class = "autocComplete textboxWidth" })%>
          <%: Html.HiddenFor(ichModel => ichModel.MemberId, new { @id = "CreditorMemberId", @name = "CreditorMemberId" })%>
        </div>
      </div>
    </div>
    <div class="fieldContainer horizontalFlow">
      <div>
        <div>
          <label>
            PAX:</label>
          <%:Html.CheckBoxFor(model => model.Pax, new { @checked = true })%>
        </div>
        <div>
          <label>
            CGO:</label>
          <%:Html.CheckBoxFor(model => model.Cargo, new { @checked = true })%>
        </div>
        <div>
          <label>
            UATP:</label>
          <%:Html.CheckBoxFor(model => model.Uatp, new { @checked = true })%>
        </div>
        <div>
          <label>
            MISC:</label>
          <%:Html.CheckBoxFor(model => model.Misc, new { @checked = true })%>
        </div>
      </div>
    </div>
    <div class="buttonContainer">
      <input class="primaryButton ignoredirty" type="submit" value="Add" onclick="addBlockedCreditor();" />
      <input class="secondaryButton" type="submit" value="Exit" onclick="$AddCreditorsdialog.dialog('close');" />
    </div>
    <div class="clear">
    </div>
  </div>
</div>
<script type="text/javascript">
  var j = 0;

  function addBlockedCreditor() {
     
    var ownerMember = $("#MemberId").val();
    var creditorMember = $("#CreditorMemberId").val();
    $("#MemberCode").val(creditorMember);
    if ($("#CreditorMemberId").val() == "")
        alert("Please select Member Code");

    else if (ownerMember == creditorMember && $("#CreditorMemberCode").val() != "") {

        if (ownerMember == null || ownerMember == "")
            alert("Please select valid Member Code");
        else
            alert("Blocked Creditor can not be same as the member for which the blocking rule is being created.");

        // Set MemberCode's auto complete text box value and text to "". 
        $('#CreditorMemberCode').val("");
        $('#CreditorMemberId').val(0);

    }

    else if (creditorMember == 0 || creditorMember == null) {
        alert("Please select valid Member Code");
        // Set MemberCode's auto complete text box value and text to "". 
        $('#CreditorMemberCode').val("");
        $('#CreditorMemberId').val(0);
    }
    else {
      addCreditorsTableRow($('#BlockedCreditorsGrid'));
    }
  } // end addBlockedCreditor()

  // Following function is used to add new row to Creditors JqGrid when user Block's Creditor. 
  function addCreditorsTableRow(jQtable) {
    jQtable.each(function () {
      // Get current table
      var $table = $(this);
      // Split Creditors member code
      var code = $("#CreditorMemberCode").val().split('-');
      // Get Current Creditors memberId
      var memberId = $('#CreditorMemberId').val();
      // Get Current Creditors memberCode
      var memberCode = $("#CreditorMemberCode").val();

      // Check whether checkboxes are checked
      var areCheckBoxesChecked = !$('#Pax').prop('checked') && !$('#Cargo').prop('checked') && !$('#Uatp').prop('checked') && !$('#Misc').prop('checked');

      // If MemberCode != "" and atleast one checkbox is checked, get checkbox values from Modal popup and add new row to table, else generate an alert.                
      if (memberCode != "" && !areCheckBoxesChecked) {

        // If creditorMemberIdString variable is null give call to "GetBlockedCreditorsId" which will return
        // Creditors MemberId string  
        if (creditorMemberIdString == "") {
          $.ajax({
            type: "GET",
            url: '<%: Url.Action("GetBlockedCreditorsId", "Ich", new { area = "Profile"}) %>',
            data: { blockingRuleId: $('#creditorblockingRuleId').val() },
            dataType: "text", 
            success: function (response) {
              creditorMemberIdString = response;
          },
            async: false
          });
        } // end if()

        // Split MemberId string on comma and add to array
        var memberIdArray = creditorMemberIdString.split(",");

        // Iterate through memberIdArray and check whether MemberId being added previously exists in the array
        for (var i = 0; i < memberIdArray.length; i++) {
          if (memberIdArray[i] == memberId) {

            // Give alert message
            alert("Blocked Creditor record for " + $("#CreditorMemberCode").val() + " already exists.");

            // Check Checkbox values on Add Creditors Modal Popup
            $('#Pax').prop('checked', true);
            $('#Cargo').prop('checked', true);
            $('#Uatp').prop('checked', true);
            $('#Misc').prop('checked', true);
            // Set MemberCode's autocomplete text box value and text to "". 
            $('#CreditorMemberCode').val("");
            $('#CreditorMemberId').val(0);

            // return
            return;
          } // end if()
        } // end for()

        // Append newly added CreditorMemberId to creditorMemberIdString string
        creditorMemberIdString += "," + memberId;

        // Get Creditors blocking ruleID
        var creditorblockingRuleId = $('#creditorblockingRuleId').val();

        // Get all checkbox values from Modal Popup for Pax, Uatp, Misc and Cargo
        var Pax = "false"
        var PaxCheck = '';
        if ($('#Pax').prop('checked')) {
          Pax = "true";
          PaxCheck = 'checked="checked"';
        }
        var Cargo = "false";
        var CargoCheck = '';
        if ($('#Cargo').prop('checked')) {
          Cargo = "true";
          CargoCheck = 'checked="checked"';
        }
        var Uatp = "false";
        var UatpCheck = '';
        if ($('#Uatp').prop('checked')) {
          Uatp = "true";
          UatpCheck = 'checked="checked"';
        }
        var Misc = "false";
        var MiscCheck = '';
        if ($('#Misc').prop('checked')) {
          Misc = "true";
          MiscCheck = 'checked="checked"';
        }

        // Concatenate all checkbox values to set Model property, so we can access it in Controllers action.
        var commaSepartedMemberType = Pax + "," + Cargo + "," + Uatp + "," + Misc;

        // Get number of td's in the last table row
        var n = $('tr:last td', this).length;
        // Create dynamic "tr" to add to JqGrid table
        var tds = '<tr class="ui-widget-content jqgrow ui-row-ltr" id="' + globalRowCount + '">';
        for (var i = 1; i <= n; i++) {
          switch (i) {
            case 1:
              tds += '<td> <a href="javascript:DeleteRow(' + globalRowCount + ');"><img src="<%: ResolveUrl("~/Content/Images/delete.png") %>" style="border-style: none;" title="Delete"></a>';
              tds += '<input type="hidden" id="items[' + globalRowCount + '].MemberId" name="items[' + globalRowCount + '].MemberId" value="' + memberId + '" /> </td>';
              // can not pass disable checkbox value in controller so using hidden member text field for it.
              tds += '<input type="hidden" name="items[' + globalRowCount + '].MemberText" value="' + commaSepartedMemberType + '" /> </td>';
              tds += '<input type="hidden" name="items[' + globalRowCount + '].IsDebtors" value="false" /> </td>';
              break;
            case 2: tds += '<td aria-describedby="BlockedCreditorsGrid_DisplayMemberCode" title="bn" style="" role="gridcell">' + code[0] + "-" + code[1] + '</td>'; break;
            case 3: tds += '<td aria-describedby="BlockedCreditorsGrid_DisplayMemberCommercialName" title="vnvbn" style="" role="gridcell">' + code[2] + '</td>'; break;
            case 4: tds += '<td aria-describedby="BlockedCreditorsGrid_Pax" title="" style="" role="gridcell"><input type="checkbox" name="items[' + globalRowCount + '].Pax" value="true"' + PaxCheck + ' ></td>'; break;
            case 5: tds += '<td aria-describedby="BlockedCreditorsGrid_Cargo" title="" style="" role="gridcell"><input  type="checkbox" name="items[' + globalRowCount + '].Cargo" value="true"' + CargoCheck + '></td>'; break;
            case 6: tds += '<td aria-describedby="BlockedCreditorsGrid_Uatp" title="" style="" role="gridcell"><input  type="checkbox" name="items[' + globalRowCount + '].Uatp" value="true"' + UatpCheck + '></td>'; break;
            case 7: tds += '<td aria-describedby="BlockedCreditorsGrid_Misc" title="" style="" role="gridcell"><input type="checkbox" name="items[' + globalRowCount + '].Misc" value="true"' + MiscCheck + '></td>'; break;
          } // end switch statement
        } // end for()

        // Append "td" to table
        tds += '</tr>';
        if ($('tbody', this).length > 0) {
          $('tbody', this).append(tds);
        } else {
          $(this).append(tds);
        }

        // Increment globalRow count(This variable is declared global because same count is used for Creditors and Debtors JqGrid table)
        globalRowCount++;

        // Check Checkbox values on Add Creditors Modal Popup
        $('#Pax').prop('checked', true);
        $('#Cargo').prop('checked', true);
        $('#Uatp').prop('checked', true);
        $('#Misc').prop('checked', true);
        // Set MemberCode's autocomplete text box value and text to "". 
        $('#CreditorMemberCode').val("");
        $('#CreditorMemberId').val(0);

        // Close Creditor dialog box
        $AddCreditorsdialog.dialog('close');
      } // end if()
      else {
        // If memberCode == "" and atleast one checkbox is not checked give an alert
        if (memberCode == "" && areCheckBoxesChecked == true)
          alert("Please select Member Code and atleast one billing category has to be selected");
        // If memberCode == "" and give an alert
        else if (memberCode == "")
          alert("Please select Member Code");
        // If atleast one checkbox is not checked give an alert
        else
          alert("Atleast one billing category has to be selected");
      } // end else
    });

  } // end addCreditorsTableRow()

  // Following function is used to delete temporary rows added to Creditors grid
  function DeleteRow(id) {
    // Ask user whether he wants to delete the row
    if (confirm("Are you sure you want to delete this record?")) {
      //Append hidden field to indicate that this row is deleted.
      $("#BlockedCreditorsGrid tr[id='" + id + "'] td:first").append('<input type="hidden" name="items[' + id + '].IsDeleted" value="true" />');

      var memberId = $("#BlockedCreditorsGrid input[name='items[" + id + "].MemberId']").val();

      var newcreditorMemberIdString = "";
      // Split MemberId string on comma and add to array
      var memberIdArray = creditorMemberIdString.split(",");
      // Iterate through memberIdArray and check whether MemberId being added previously exists in the array
      for (var i = 0; i < memberIdArray.length; i++) {

          if (memberIdArray[i] != memberId && memberIdArray[i] != "" && memberIdArray[i] != undefined && memberIdArray[i] != null) {

              newcreditorMemberIdString += "," + memberIdArray[i];
          }
      }

      creditorMemberIdString = newcreditorMemberIdString;
      // Hide the row
      $('#' + id).hide();
    } // end if()
  } // end DeleteRow()

  // Following function is used to delete row from Creditors grid which are populated from Database. 
  function DeleteCreditorRowFromDatabase(id) {
    // Ask user whether he wants to delete the row
    if (confirm("Are you sure you want to delete this record?")) {
      // Added deleted rows id to hidden field. If hidden field is "" add rowId. If more than one row 
      // is deleted append rowId's comma separated
      if ($('#DeletedBlockedCreditorString').val() == "")
        $('#DeletedBlockedCreditorString').val(id);
      else
        $('#DeletedBlockedCreditorString').val($('#DeletedBlockedCreditorString').val() + ',' + id);

      // Hide the row
      $('#' + id).hide();
    } // end if()
  } // end DeleteCreditorRowFromDatabase()
</script>
