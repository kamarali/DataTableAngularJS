<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MemberProfile.BlockMember>" %>
<script type="text/javascript" src="<%:Url.Content("~/Scripts/autoCompleteDisplay.js")%>"></script>
<script type="text/javascript">
  $(document).ready(function () {
    // SCP90120: KAL: Issue with Blocking rules master [changed the method called to display the memberlist in dropdown list]
    registerAutocomplete('DebtorMemberCode', 'DebtorMemberId', '<%:Url.Action("GetMemberListForIchOrAch", "Data", new { area = "", menuType="bothAchIch" })%>', 0, true, null);    
  });
</script>
<div>
  <div class="solidBox dataEntry">
    <div class="fieldContainer horizontalFlow">
      <div>
        <div>
          <label>
            <span class="required" style="color: Red;">*</span>Member Code:</label>
          <%:Html.TextBoxFor(model => model.BlockingRuleId, new { @class = "hidden", @id = "DebtorblockingRuleId" })%>
          <!--    CMP#596: Length of Member Accounting Code to be Increased to 12 
                  Desc: Non layout related IS-WEB screen changes.
                  Ref: FRS Section 3.1 Table 2, Section 3.2 Table 8,Section 3.3 Table 12, Section 3.5 Table20 */ -->
          <%: Html.TextBoxFor(ichModel => ichModel.MemberText, new { @id = "DebtorMemberCode", @Class = "autocComplete textboxWidth" })%>
          <%: Html.HiddenFor(ichModel => ichModel.MemberId, new { @id = "DebtorMemberId" })%>
        </div>
      </div>
    </div>
    <div class="fieldContainer horizontalFlow">
      <div>
        <div>
          <label>
            PAX:</label>
          <%:Html.CheckBoxFor(model => model.Pax, new { id = "debtPax", @checked = true })%>
        </div>
        <div>
          <label>
            CGO:</label>
          <%:Html.CheckBoxFor(model => model.Cargo, new { id = "debtCargo", @checked = true })%>
        </div>
        <div>
          <label>
            UATP:</label>
          <%:Html.CheckBoxFor(model => model.Uatp, new { id = "debtUatp", @checked = true })%>
        </div>
        <div>
          <label>
            MISC:</label>
          <%:Html.CheckBoxFor(model => model.Misc, new { id = "debtMisc", @checked = true })%>
        </div>
      </div>
    </div>
    <div class="buttonContainer">
      <input class="primaryButton ignoredirty" type="submit" value="Add" onclick="addBlockedDebtor();" />
      <input class="secondaryButton" type="submit" value="Exit" onclick="$AddDebtorsdialog.dialog('close');" />
    </div>
    <div class="clear">
    </div>
  </div>
</div>
<script type="text/javascript">
  var j = 0;

  function addBlockedDebtor() {
    var ownerMember = $("#MemberId").val();
    var debtorMember = $("#DebtorMemberId").val();

    if (ownerMember == debtorMember && $("#DebtorMemberCode").val() != "") {
      alert("Blocked debtor can not be same as the member for which the blocking rule is being created.");
    }
    else {
      addDebtorsTableRow($('#BlockedDebtorsGrid'));
    }
  } // end addBlockedDebtor()

  // Following function is used to add new row to Creditors JqGrid when user Block's Debtor. 
  function addDebtorsTableRow(jQtable) {
    jQtable.each(function () {
      // Get current table
      var $table = $(this);
      // Split Debtors member code
      var code = $('#DebtorMemberCode').val().split('-');
      // Get Current Debtors memberId
      var memberId = $('#DebtorMemberId').val();
      // Get Current Debtors memberCode
      var memberCode = $("#DebtorMemberCode").val();
      // Check whether checkboxes are checked
      var areCheckBoxesChecked = !$('#debtPax').prop('checked') && !$('#debtCargo').prop('checked') && !$('#debtUatp').prop('checked') && !$('#debtMisc').prop('checked');

      // Check whether user has entered membername on popup, if not give alert
      if (memberCode != "" && !areCheckBoxesChecked) {
        // If DebtorMemberIdString variable is null give call to "GetBlockedDebtorsId" which will return
        // Debtors MemberId string  
        if (DebtorMemberIdString == "") {
          $.ajax({
            type: "GET",
            url: '<%: Url.Action("GetBlockedDebtorsId", "Ich", new { area = "Profile"}) %>',
            data: { blockingRuleId: $('#DebtorblockingRuleId').val() },
            success: function (response) {
              DebtorMemberIdString = response;
          },
          dataType: "text", 
            async: false
          });
        } // end if()

        // Split MemberId string on comma and add to array
        var memberIdArray = DebtorMemberIdString.split(",");

        // Iterate through memberIdArray and check whether MemberId being added previously exists in the array
        for (var i = 0; i < memberIdArray.length; i++) {
          if (memberIdArray[i] == memberId) {

            // Give alert message
            alert("Blocked Debtor record for " + $('#DebtorMemberCode').val() + " already exists.");

            // Check Checkbox values on Add Debtors Modal Popup
            $('#debtPax').prop('checked', true);
            $('#debtCargo').prop('checked', true);
            $('#debtUatp').prop('checked', true);
            $('#debtMisc').prop('checked', true);
            // Set MemberCode's autocomplete text box value and text to "". 
            $('#DebtorMemberCode').val("");
            $('#DebtorMemberId').val(0);

            // return
            return;
          } // end if()
        } // end for()

        // Append newly added CreditorMemberId to creditorMemberIdString string
        DebtorMemberIdString += "," + memberId;

        // Get Debtors blocking ruleID
        var debtorblockingRuleId = $('#DebtorblockingRuleId').val();

        // Get all checkbox values from Modal Popup for Pax, Uatp, Misc and Cargo
        var Pax = "false"
        var PaxCheck = '';
        if ($('#debtPax').prop('checked')) {
          Pax = "true";
          PaxCheck = 'checked="checked"';
        }
        var Cargo = "false";
        var CargoCheck = '';
        if ($('#debtCargo').prop('checked')) {
          Cargo = "true";
          CargoCheck = 'checked="checked"';
        }
        var Uatp = "false";
        var UatpCheck = '';
        if ($('#debtUatp').prop('checked')) {
          Uatp = "true";
          UatpCheck = 'checked="checked"';
        }
        var Misc = "false";
        var MiscCheck = '';
        if ($('#debtMisc').prop('checked')) {
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
              tds += '<td><a href="javascript:DeleteDebtorRow(' + globalRowCount + ');"><img src="<%: ResolveUrl("~/Content/Images/delete.png") %>" style="border-style: none;" title="Delete"></a>';
              tds += '<input type="hidden" name="items[' + globalRowCount + '].MemberId" value="' + memberId + '" /> </td>';
              tds += '<input type="hidden" name="items[' + globalRowCount + '].MemberText" value="' + commaSepartedMemberType + '" /> </td>';
              tds += '<input type="hidden" name="items[' + globalRowCount + '].IsDebtors" value="true" /> </td>';
              break;
            case 2: tds += '<td aria-describedby="BlockedDebtorsGrid_DisplayMemberCode" title="bn" style="" role="gridcell">' + code[0] + "-" + code[1] + '</td>'; break;
            case 3: tds += '<td aria-describedby="BlockedDebtorsGrid_DisplayMemberCommercialName" title="vnvbn" style="" role="gridcell">' + code[2] + '</td>'; break;
            case 4: tds += '<td aria-describedby="BlockedDebtorsGrid_Pax" title="" style="" role="gridcell"><input type="checkbox" name="items[' + globalRowCount + '].Pax" value="true"' + PaxCheck + ' ></td>'; break;
            case 5: tds += '<td aria-describedby="BlockedDebtorsGrid_Cargo" title="" style="" role="gridcell"><input  type="checkbox" name="items[' + globalRowCount + '].Cargo" value="true"' + CargoCheck + '></td>'; break;
            case 6: tds += '<td aria-describedby="BlockedDebtorsGrid_Uatp" title="" style="" role="gridcell"><input type="checkbox" name="items[' + globalRowCount + '].Uatp" value="true"' + UatpCheck + '></td>'; break;
            case 7: tds += '<td aria-describedby="BlockedDebtorsGrid_Misc" title="" style="" role="gridcell"><input  type="checkbox" name="items[' + globalRowCount + '].Misc" value="true"' + MiscCheck + '></td>'; break;
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

        // Check Checkbox values on Add Debtors Modal Popup
        $('#debtPax').prop('checked', true);
        $('#debtCargo').prop('checked', true);
        $('#debtUatp').prop('checked', true);
        $('#debtMisc').prop('checked', true);
        $('#DebtorMemberCode').val("");
        $('#DebtorMemberId').val(0);

        $AddDebtorsdialog.dialog('close');
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

  } // end addDebtorsTableRow

  // Following function is used to delete temporary rows added to Debtors grid
  function DeleteDebtorRow(id) {
    // Ask user whether he wants to delete the row
    if (confirm("Are you sure you want to delete this record?")) {
      //Append hidden field to indicate that this row is deleted.
        $("#BlockedDebtorsGrid tr[id='" + id + "'] td:first").append('<input type="hidden" name="items[' + id + '].IsDeleted" value="true" />');

        var memberId = $("#BlockedDebtorsGrid input[name='items[" + id + "].MemberId']").val();

        var newcreditorMemberIdString = "";
        // Split MemberId string on comma and add to array
        var memberIdArray = DebtorMemberIdString.split(",");
        // Iterate through memberIdArray and check whether MemberId being added previously exists in the array
        for (var i = 0; i < memberIdArray.length; i++) {

            if (memberIdArray[i] != memberId && memberIdArray[i] != "" && memberIdArray[i] != undefined && memberIdArray[i] != null) {

                newcreditorMemberIdString += "," + memberIdArray[i];
            }
        }

        DebtorMemberIdString = newcreditorMemberIdString;
      // Hide the row
      $('#' + id).hide();
    } // end if()
  } // end DeleteDebtorRow()

  // Following function is used to delete row from Debtors grid which are populated from Database. 
  function DeleteDebtorRowFromDatabase(id) {
    // Ask user whether he wants to delete the row
    if (confirm("Are you sure you want to delete this record?")) {
      // Added deleted rows id to hidden field. If hidden field is "" add rowId. If more than one row 
      // is deleted append rowId's comma separated
      if ($('#DeletedBlockedDebtorString').val() == "")
        $('#DeletedBlockedDebtorString').val(id);
      else
        $('#DeletedBlockedDebtorString').val($('#DeletedBlockedDebtorString').val() + ',' + id);

      // Hide the row
      $('#' + id).hide();
    } // end if()
  } // end DeleteDebtorRowFromDatabase()

</script>
