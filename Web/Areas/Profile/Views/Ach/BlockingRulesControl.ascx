<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MemberProfile.BlockingRule>" %>
<script type="text/javascript" src="<%:Url.Content("~/Scripts/Member/MemberProfile.js")%>"></script>
<script src="<%:Url.Content("~/Scripts/Member/BlockingRule.js")%>" type="text/javascript"></script>
<script type="text/javascript">
    
    // Following variable is used to keep track whether user has saved all his changes after adding Group
    // and related Exceptions.
    var exceptionRowCount = 0;
  
    $(document).ready(function () {
        $("#Id").val('<%: ViewData["Id"]==null? "": ViewData["Id"].ToString() %>');
        $("#IsInEditMode").val('<%: ViewData["IsInEditMode"]==null? "":ViewData["IsInEditMode"].ToString() %>');

        registerAutocomplete('achMemberText', 'achMemberId', '<%:Url.Action("GetMemberListForIchOrAch", "Data", new { area = "",menuType="bothAchIch"})%>', 0, true, null);
        $("#Description").bind("keypress", function () { maxLength(this, 200) });
        $("#Description").bind("paste", function () { maxLengthPaste(this, 200) });
    });
    $(function () {
        $("#tabs").tabs({
            //TFS#9973:Firefox: v47 :ICHOPS:Manage Blocks:Not able to add records in Blocked Creditor,Blocked Debtor and Blocks By Group grids
            beforeLoad: function (event, ui) {
                if ($(ui.panel).html()) {    // If content already there...
                    event.preventDefault();      // ...don't load it again.
                }
            },
            ajaxOptions: {
                error: function (xhr, status, index, anchor) {
                    $(anchor.hash).html("Couldn't load this tab. We'll try to fix this as soon as possible. If this wouldn't be a demo.");
                }
            }
        //IATA SIS-P4-Third party : Jquery And Jquery UI 1.12.3 Upgradation
        }).on('tabsactivate', function (event, ui) {
            $('.ichBlkCred-dialog').remove();
            $('.ichBlkDebt-dialog').remove();
        });
    })

    function ACHMemberText_AutoCompleteValueChange(selectedId) {
        
        var flag = false;

        //Check Creditors grid.
        var blockedmember = $("#BlockedCreditorsGrid tr td input[name$='MemberId'][value='" + selectedId + "']");
        if (!IsMemberAdded(blockedmember)) {

            //Check Debtors grid.
            blockedmember = $("#BlockedDebtorsGrid tr td input[name$='MemberId'][value='" + selectedId + "']");
           // IsMemberAdded(blockedmember);
             if (!IsMemberAdded(blockedmember)) {

                 //Check Exceptions grid.
                 blockedmember = $("#BlocksByGroupExceptionsGrid tr td input[name$='MemberId'][value='" + selectedId + "']");
                 IsMemberAdded(blockedmember);
             }
        }
    }

    function IsMemberAdded(blockedmember) {
        var isExists = false;
        if (blockedmember.length > 0) {
            blockedmember.each(function () {
                var deleteEle = $(this).siblings("input[name$='IsDeleted']");
                if (deleteEle.length > 0)
                    isExists = deleteEle[0].value == 'false';
                else
                    isExists = true;

                if (isExists) {
                    $("#achMemberId").val('');
                    $("#achMemberText").val('');
                    alert("Please select other member name as the selected member name already exists in blocked rule.");
                    return false;
                }
            });
        }
        return isExists;
    }

  // Following function is used to modify CheckBox values i.e. Pax, Cargo etc in database if user has edited.
    function SaveBlockedMemberDetails() {
        // Check box values string for Creditors
        var creditorCheckboxList = '';
        // Check box values string for Debtors
        var debtorCheckBoxList = '';
        
         // CheckBox values string for BlockByGroups
         var blockByGroupCheckboxList = '';

        // If blocking Creditor is same as the member for which blocking rule is created, give an alert
        var ownerMemberId = $("#achMemberId").val();
        var creditorMemberId = $("#MemberCode").val();
        if (ownerMemberId == creditorMemberId && $("#MemberCode").val() != "") {
            alert("Blocked creditor can not be same as the member for which the blocking rule is being created.");
            return false;
        }

        // Modify database details if user has opened ACH tab in Edit mode
        if ($('#IsInEditMode').val()) {
            // variable to check whether Blocked Creditors grid exists   
            var firstTableRowCount = parseInt('<%: ViewData["BlockedCreditorsCount"] %>');
            if ($("#BlockedCreditorsGrid").length > 0) {

                firstTableRowCount = $('#BlockedCreditorsGrid tr:gt(0)').filter(function () { return $(this).css('display') !== 'none'; }).length;

                // Iterate through each row of BlockedCreditors grid and get checkbox values 
                $("#BlockedCreditorsGrid tr:gt(0)").each(function () {
                    var rowData = "";
                    $(this).find("td:gt(2)").each(function () {
                        var checkBoxName = $(this).attr("aria-describedby").replace(/BlockedCreditorsGrid_/, '');
                        rowData = rowData + checkBoxName + '_' + $(this).find('input:checkbox').prop('checked') + ",";
                    });

                    // Separate each row string with "|" character
                    var r = rowData.length - 1;
                    if (r > 0 && rowData[r] == ',')
                    { rowData = rowData.substring(0, r); }
                    creditorCheckboxList = creditorCheckboxList + $(this).attr("id") + "!" + rowData + "|";
                });

                var r = creditorCheckboxList.length - 1;
                if (r > 0 && creditorCheckboxList[r] == '|')
                { creditorCheckboxList = creditorCheckboxList.substring(0, r); }
            }

            // variable to check whether Blocked Debtors grid exists   
            var secondTableRowCount = parseInt('<%: ViewData["BlockedDebitorsCount"] %>');
            if ($("#BlockedDebtorsGrid").length > 0) {

                secondTableRowCount = $('#BlockedDebtorsGrid tr:gt(0)').filter(function () { return $(this).css('display') !== 'none'; }).length;
                // Iterate through each row of BlockedDebtors grid and get checkbox values 
                $("#BlockedDebtorsGrid tr:gt(0)").each(function () {
                    var rowData = "";
                    $(this).find("td:gt(2)").each(function () {
                        var checkBoxName = $(this).attr("aria-describedby").replace(/BlockedDebtorsGrid_/, '');
                        rowData = rowData + checkBoxName + '_' + $(this).find('input:checkbox').prop('checked') + ",";
                    });

                    // Separate each row string with "|" character
                    var r = rowData.length - 1;
                    if (r > 0 && rowData[r] == ',')
                    { rowData = rowData.substring(0, r); }
                    debtorCheckBoxList = debtorCheckBoxList + $(this).attr("id") + "!" + rowData + "|";
                });

                var r = debtorCheckBoxList.length - 1;
                if (r > 0 && debtorCheckBoxList[r] == '|')
                { debtorCheckBoxList = debtorCheckBoxList.substring(0, r); }
            }
            
      //Group changes
      // variable to check whether GroupBlocks grid exists   
      var thirdTableRowCount = parseInt('<%: ViewData["BlockedGroupCount"] %>');
      if ($("#BlocksByGroupBlocksGrid").length > 0) {

        thirdTableRowCount = $('#BlocksByGroupBlocksGrid tr:gt(0)').filter(function () { return $(this).css('display') !== 'none'; }).length;
        // Iterate through each row of BlockByGroups grid and get checkbox values
        $("#BlocksByGroupBlocksGrid tr:gt(0)").each(function () {
            var rowData = "";
            $(this).find("td:gt(2)").each(function () {
                var checkBoxName = $(this).attr("aria-describedby").replace(/BlocksByGroupBlocksGrid_/, '');
                if ($(this).find('input:checkbox').is(':checkbox')) {
                    rowData = rowData + checkBoxName + '_' + $(this).find('input:checkbox').prop('checked') + ",";
                }
            });

            // Separate each row string with "|" character
            var r = rowData.length - 1;
            if (r > 0 && rowData[r] == ',')
            { rowData = rowData.substring(0, r); }
            blockByGroupCheckboxList = blockByGroupCheckboxList + $(this).attr("id") + "!" + rowData + "|";
        });

        var r = blockByGroupCheckboxList.length - 1;
        if (r > 0 && blockByGroupCheckboxList[r] == '|')
        { blockByGroupCheckboxList = blockByGroupCheckboxList.substring(0, r); }
      }

            //If no data for add then show alert.
            if (firstTableRowCount <= 0 && secondTableRowCount <= 0 && thirdTableRowCount <= 0) {
                alert('Please add records to Grid.');
                return false;
            }

            // Execute UpdateBlockingMember action which will update values in database
            $.ajax({
                type: "POST",
                url: '<%: Url.Action("UpdateBlockingMember", "Ach", new { area = "Profile"}) %>',
                dataType: "json",
                data: { checkboxList: creditorCheckboxList + '|' + debtorCheckBoxList },
                success: function (response) {
                },
                async: false
            });
            
             // Execute UpdateBlockGroup action which will update values in database
              $.ajax({
                type: "POST",
                url: '<%: Url.Action("UpdateBlockGroup", "Ach", new { area = "Profile"}) %>',
                dataType: "json",
                data: { blockGroupCheckboxList: blockByGroupCheckboxList },
                success: function (response) {
                },
                async: false
              });

        } // end if()
        return true;
    } // end SaveBlockedMemberDetails()

</script>
<% using (Html.BeginForm("BlockingRuleDetails", "Ach", FormMethod.Post, new { id = "BlockingRule" }))
   {%>
   <%: Html.AntiForgeryToken() %>
<div>
  <%: Html.HiddenFor(model => model.DeletedBlockedCreditorString)%>
  <%: Html.HiddenFor(model => model.DeletedBlockedDebtorString)%>
  <%: Html.HiddenFor(model => model.DeletedGroupByBlockString)%>
  <%: Html.HiddenFor(model => model.DeletedExceptionRowString)%>
  <%: Html.HiddenFor(model => model.IsInEditMode) %>
  <input type="hidden" id="MemberCode" />
  <div class="solidBox dataEntry">
    <div class="fieldContainer horizontalFlow">
      <div>
        <div style=" width:250px;">
          <label>
            <span class="required">*</span>Member Code:</label>
          <%if (Model != null)
            {%>
          <!--    CMP#596: Length of Member Accounting Code to be Increased to 12 
                  Desc: Non layout related IS-WEB screen changes.
                  Ref: FRS Section 3.1 Table 2, Section 3.2 Table 8,Section 3.3 Table 12, Section 3.5 Table20 */ -->
          <%: Html.TextBoxFor(model => model.MemberText, new { @id = "achMemberText", @disabled = true, @class = "autocComplete textboxWidth" })%>
          <%: Html.HiddenFor(model => model.MemberText) %>
          <%: Html.HiddenFor(model => model.MemberId, new { @id = "achMemberId" })%>
          <%
            }%>
          <%if (Model == null)
            {%>
            <!--    CMP#596: Length of Member Accounting Code to be Increased to 12 
                  Desc: Non layout related IS-WEB screen changes.
                  Ref: FRS Section 3.1 Table 2, Section 3.2 Table 8,Section 3.3 Table 12, Section 3.5 Table20 */ -->
          <%: Html.TextBoxFor(model => model.MemberText, new { @id = "achMemberText", @class = "autocComplete textboxWidth" })%>
          <%: Html.HiddenFor(model => model.MemberText) %>
          <%: Html.HiddenFor(model => model.MemberId, new { @id = "achMemberId" })%>
          <%
            }%>
        </div>
        <div style=" width:250px;">
          <label>
            <span class="required">*</span>Blocking Rule:</label>
          <%:Html.TextBoxFor(model => model.RuleName, new { @maxLength = 15 })%>
        </div>
        <div style=" width:420px;">
          <label>
             <span class="required">*</span>Description:</label>
          <%: Html.TextAreaFor(model => model.Description,3,60, null)%>
        </div>
        <div>
        </div>
        <div style=" width:250px;">
          <label>
            Clearing House:</label>
          <%:Html.TextBoxFor(model => model.ClearingHouse, new { @readonly = true })%>
          <%: Html.HiddenFor(model => model.ClearingHouse) %>
          <%:Html.HiddenFor(model=>model.Id) %>
        </div>
      </div>
    </div>
    <div class="clear">
    </div>
    <div id="tabs" class="ui-tabs-hide">
      <ul>
        <li>
          <%:Html.ActionLink("Blocked Creditors", "BlockedCreditors", "Ach",
            new { area = "Profile", blockingRuleId = Model != null ? Model.Id : 0, memberId = Model != null ? Model.MemberId : 0 }, null)%>
        </li>
        <li>
          <%:Html.ActionLink("Blocked Debtors", "BlockedDebitors", "Ach",
            new { area = "Profile", blockingRuleId = Model != null ? Model.Id : 0, memberId = Model != null ? Model.MemberId : 0 }, null)%>
        </li>
         <li>
          <%:Html.ActionLink("Blocks By Group", "BlocksByGroup", "Ach",
                                                    new { area = "Profile", blockingRuleId = Model != null ? Model.Id : 0 }, null)%>
        </li>
      </ul>
    </div>
  </div>
  <div>
    <div class="buttonContainer">
      <input type="submit" class="primaryButton" value="Save" id="btnSave" name="btnSave" onclick="javascript:return SaveBlockedMemberDetails();" />
      <%: Html.LinkButton("Back", Url.Action("AchBlockingRules", "Ach"))%>
    </div>
  </div>
</div>
<%
  }%>
