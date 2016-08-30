/// <reference path="site.js" />
$(document).ready(function () {

    $("#InvoiceSearchForm").validate({
        rules: {
            BillingYearMonth: "required"
        },
        messages: {
            BillingYearMonth: "Billing Year / Month required"
        }
    });

    loadGrid();

});

function BilledMemberText_SetAutocompleteDisplay(item) {
  var memberCode = item.MemberCodeAlpha + "-" + item.MemberCodeNumeric + "(" + item.CommercialName + ")";
  return { label: memberCode, value: memberCode, id: item.Id };
}


function submitInvoices(gridId, urlAction) {

    var selectedIds = jQuery(gridId).getGridParam('selarrrow');

  $.ajax({
    type: "POST",
    //url: urlAction + "/" + selectedIds,
    url: urlAction,
    data: { id: selectedIds.toString() },
    dataType: "json",
    success: function (result) {
        if (result.IsFailed == false) {
            if (result.AlertMessage != '') {
                showDualClientMessage(result.Message, result.AlertMessage);
            }
            else { showClientSuccessMessage(result.Message); }
            $(gridId).trigger("reloadGrid");
        }
      else {
        showClientErrorMessage(result.Message);
      }
    },
    error: function (result) {
      
    }
  });
}

//SCP - 85037: Maintain pagination on grid
function loadGrid() {

    jQuery("#searchGrid").jqGrid('setGridParam', {
        onPaging: function () {
            //alert('my over written event');

            var nextPg = $("#searchGrid").getGridParam("page");
                        
            if ($("#hdnPageNo").val() == '0') {
                $("#hdnPageNo").val(nextPg);

                $("#searchGrid").setGridParam({ page: nextPg }).trigger("reloadGrid");                
                return;
            }
        }
    });
}