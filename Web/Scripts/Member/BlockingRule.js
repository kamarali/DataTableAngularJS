
function initBlockingRuleValidation() {
  $("#BlockingRule").validate({

    rules: {
      MemberText: "required",
      ichMemberText: "required",
      RuleName: "required",
      Description: "required"
    },
    messages: {
      MemberText: "Member Code Required",
      ichMemberText: "Member Code Required",
      RuleName: "Blocking Rule Required",
      Description: "Description Required"

    },
    submitHandler: function (form) {

      if (!$("#IsInEditMode").val()) {

        // variable to check whether Blocked Creditors grid exists   
        var firstTableRowCount = $("#BlockedCreditorsGrid").length == 0 ? 0 : $('#BlockedCreditorsGrid tr:gt(0)').filter(function () { return $(this).css('display') !== 'none'; }).length;      //$("#BlockedCreditorsGrid tr:visible").length - 1;

        // variable to check whether Blocked Debtors grid exists   
        var secondTableRowCount = $("#BlockedDebtorsGrid").length == 0 ? 0 : $('#BlockedDebtorsGrid tr:gt(0)').filter(function () { return $(this).css('display') !== 'none'; }).length;      //$("#BlockedDebtorsGrid tr:visible").length - 1;

        // variable to check whether GroupBlocks grid exists   
        var thirdTableRowCount = $("#BlocksByGroupBlocksGrid").length == 0 ? 0 : $('#BlocksByGroupBlocksGrid tr:gt(0)').filter(function () { return $(this).css('display') !== 'none'; }).length;      //$("#BlocksByGroupBlocksGrid tr:visible").length - 1;

        //If no data for add then show alert.
        if (firstTableRowCount <= 0 && secondTableRowCount <= 0 && thirdTableRowCount <= 0) {
          alert('Please add records to Grid.');
          return;
        }
      }
      // Call onSubmitHandler() function which will disable Submit buttons and will submit the form
      onSubmitHandler(form);
      // form.submit();
    },
    invalidHandler: function () {
      $('#errorContainer').show();
    }
  });
}

function removeValueFromList(list, value, separator) {
  separator = separator || ",";
  var values = list.split(separator);
  for (var i = 0; i < values.length; i++) {
    if (values[i] == value) {
      values.splice(i, 1);
      return values.join(separator);
    }
  }
  return list;
}

function removeIndexFromList(list, index, separator) {
  separator = separator || ",";
  var values = list.split(separator);
  values.splice(index, 1);
  return values.join(separator);
}
