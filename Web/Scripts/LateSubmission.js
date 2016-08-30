$(document).ready(function () {

    jQuery("#LateSubmissionInvoiceDetailsGrid").jqGrid('setCaption', "Details: ")
    $(".ui-icon-circle-triangle-n").attr("style", "display:none;");

    if ($("#lateSubWindowStatus").val().toLowerCase() == "false") {
        $(".primaryButton").attr("disabled", true);
    }
});

// Following function is used to refresh InvoiceStatus grid for specified time interval
function refreshDetailInvoiceStatusGrid() {

    var grid = $("#LateSubmissionInvoiceDetailsGrid");
    grid.trigger("reloadGrid");
    t = setTimeout("refreshDetailInvoiceStatusGrid()", 300000);
}

// Following function is used to refresh master grid 
function refreshMasterInvoiceGrid() {

    var grid = $("#LateSubmissionGrid");
    grid.trigger("reloadGrid");
}

// display selected member (on row click) details
function DisplayDetails(ids) {

    if (ids == null) {
        ids = 0;
        if (jQuery("#LateSubmissionInvoiceDetailsGrid").jqGrid('getGridParam', 'records') > 0) {

            jQuery("#LateSubmissionInvoiceDetailsGrid").jqGrid('setCaption', "Invoice Detail: " + ids)
				.trigger('reloadGrid');
        }
    } else {
        var name = $("#" + ids).find("td").eq(1).html();
        var userCategory = $("#categoryId").val();
        jQuery("#LateSubmissionInvoiceDetailsGrid").jqGrid('setGridParam', { url: "GetSelectedMemberInvoiceDetail?memberId=" + ids + "&catId=" + userCategory });
        jQuery("#LateSubmissionInvoiceDetailsGrid").jqGrid('setCaption', "Details: " + name).trigger('reloadGrid');
    }
}

//  function is used to accept Invoices for LateSubmission
function AcceptInvoices(url) {
    // Get selected Invoices Id on InvoiceStatusGrid  
    var selectedInvoiceIds = $("#LateSubmissionInvoiceDetailsGrid").jqGrid('getGridParam', 'selarrrow');

    if (selectedInvoiceIds != "" && selectedInvoiceIds.length > 0) {

        $.ajax({
            type: "POST",
            url: url,
            data: { invoiceIds: selectedInvoiceIds.toString() },
            success: function (response) {
                // SCP52187: Deleted invoice appearing in Late submissions
                //  Error and success message style has been changed.Before it showned in green color despite the message is error
                if (response.IsFailed) {
                    if (response.Message) {
                        showClientErrorMessage(response.Message);
                    }
                } else {
                    if (response.Message) {
                        showClientSuccessMessage(response.Message);
                    }
                }
                
                // On Success call "refreshInvoiceStatusGrid()" function which will refresh the detail grid
                refreshDetailInvoiceStatusGrid();
                refreshMasterInvoiceGrid();
            },
            error: function (xhr, textStatus, errorThrown) {
                alert('An error occurred! ' + errorThrown);
            },
            async: false
        });
    }
    else {
        alert('Please select atleast one Invoice.');
    }
}

//  function is used to reject Invoices for LateSubmission
function RejectInvoices(url) {
    // Get selected Invoices Id on InvoiceStatusGrid  
    var selectedInvoiceIds = $("#LateSubmissionInvoiceDetailsGrid").jqGrid('getGridParam', 'selarrrow');
    var catId = $("#categoryId").val()

    if (selectedInvoiceIds != "") {

        $.ajax({
            type: "POST",
            url: url,
            data: { invoiceIds: selectedInvoiceIds.toString(),catId: catId },
            success: function (response) {
              //  if (response) {
                    $("#clientSuccessMessageContainer").show();
                    $("#clientSuccessMessage").html(response);
               // }
                // On Success call "refreshInvoiceStatusGrid()" function which will refresh the detail grid
                    refreshDetailInvoiceStatusGrid();
                    refreshMasterInvoiceGrid();
            },
            error: function (xhr, textStatus, errorThrown) {
                alert('An error occurred! ' + errorThrown);
            },
            async: false
        });
    }
    else {
        alert('Please select atleast one Invoice.');
    }
}

//  function is used to reject Invoices for LateSubmission
//  function is used to reject Invoices for LateSubmission
function LateSubmissionWindowClose(url) {

  if (confirm("Are you sure you want to close the Late Submission Window?")) {
    var catId = $("#categoryId").val()


    $.ajax({
      type: "POST",
      url: url,
      data: { catId: catId },
      success: function (response) {
        if (response) {
          $(".primaryButton").attr("disabled", true);
          $("#clientSuccessMessageContainer").show();
          $("#clientSuccessMessage").html(response);
        }

        // On Success call "refreshInvoiceStatusGrid()" function which will refresh the Invoice grid
        refreshDetailInvoiceStatusGrid();
        refreshMasterInvoiceGrid();
      },
      error: function (xhr, textStatus, errorThrown) {
        alert('An error occurred! ' + errorThrown);
      },
      async: false
    });

  }
}