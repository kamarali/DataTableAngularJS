/// <reference path="site.js" />
var submitMethod;
var presentMethod;
$(document).ready(function () {

    $("#FormCSearchForm").validate({
        rules: {
            BillingYearMonth: "required"
        },
        messages: {
            BillingYearMonth: "Provisional Billing Month required"
        },
        submitHandler: function (form) {
            if ($("#BilledMemberText").val() == '')
                $("#BilledMemberId").val('0');

            // Call onSubmitHandler() function which will disable Submit buttons and will submit the form
            onSubmitHandler(form);
            // form.submit();
        }
    });

    $('#SubmitFormCButton').click(function () {
      getSelectedRows('#FormCSearchGrid', submitMethod);
      });

      $('#PresentFormCButton').click(function () {
        getSelectedRows('#FormCSearchGrid', presentMethod);
    });
});

function InitializeSubmitMethod(submitMethodAction) {
  submitMethod = submitMethodAction;
}

function InitializePresentMethod(presentMethodAction) {
  presentMethod = presentMethodAction;
}

function BilledMemberText_SetAutocompleteDisplay(item) {
  var memberCode = item.MemberCodeAlpha + "-" + item.MemberCodeNumeric + "-" + item.CommercialName;
  return { label: memberCode, value: memberCode, id: item.Id };
}

function getSelectedRows(gridId, action) {

  var selectedIds = jQuery(gridId).getGridParam('selarrrow');    

  $.ajax({
    type: "POST",
    //url: action + "/" + selectedIds,
    url: action,
    data: { transactionId: selectedIds.toString() },
    dataType: "json",
    success: function (result) {
      if (result.IsFailed == false) {        
        showClientSuccessMessage(result.Message);
      }
      else {
        showClientErrorMessage(result.Message);
      }
      $(gridId).trigger("reloadGrid");      
    }
  });
}

function resetSearch() {
  $('#BillingYearMonth').val('');
  $('#BilledMemberText').val('');
  $('#InvoiceStatus').val('-1');
}