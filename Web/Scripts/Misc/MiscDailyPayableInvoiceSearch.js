
$(document).ready(function () {

    $("#MiscInvoiceSearchForm").validate({
        rules: {
            DeliveryDateFrom: "required",
            DeliveryDateTo: "required",
            AssociatedLocation: "required"
        },
        messages: {
            DeliveryDateFrom: "DeliveryDateFrom is required",
            DeliveryDateTo: "DeliveryDateTo is required",
            AssociatedLocation: "Billed Location ID required"
        }
    });

    //CMP #655: IS-WEB Display per Location ID          
    //2.11	MISC IS-WEB PAYABLES - VIEW DAILY BILATERAL INVOICES SCREEN
    $("#AssociatedLocation option").each(function (i, selected) {
        var selectedLocation = $("#BillingMemberLoc").val();
        var selectedLocationArray = selectedLocation.split(',');
        if (selectedLocation == '') {
            $(selected).attr('selected', 'selected');
        } else {
            var found = $.inArray(selected.text, selectedLocationArray) > -1;
            if (found) {
                $(selected).attr('selected', 'selected');
            }
        }
    });

    var firstOption = $("#AssociatedLocation option:selected:first").attr('title');
    $("#AssociatedLocation option:selected:first").filter(function () {
        $(this).removeAttr('selected');
        return $(this).text() == firstOption;
    }).attr('selected', true);

    var title = "At least one Location ID must be selected for successful search results. If no Location IDs are shown here, it means that you are not associated with any Location of your organization. Please contact your organization’s user(s) who have access to the Location Association module to review and associate you with appropriate Location(s).";
    $("#AssociatedLocation").attr("title", title);
    $("#AssociatedLocation option").attr("title", title);

    $("#btnSearch").bind('click', function () {
        BindSelectedLocation();
    });


});

function BindSelectedLocation() {
    var selectedLocationIds = '';
    $("#AssociatedLocation option:selected").each(function () {
        selectedLocationIds = selectedLocationIds + ',' + $(this).text();
    });
    $("#BillingMemberLoc").val(selectedLocationIds);
}


// Following function is used to check whether From date is less than To date.
$("#DeliveryDateFrom").change(function () {
    ValidateFromToDate('DeliveryDateFrom');
});

// Following function is used to check whether To date is greater than From date.
$("#DeliveryDateTo").change(function () {
    ValidateFromToDate('DeliveryDateTo');
});

function ValidateFromToDate(controlId) {
    var dateComparisonResult = validateDateRange('DeliveryDateFrom', 'DeliveryDateTo');
    if (!dateComparisonResult) {
        alert("Delivery Date To should be equal to or later than Delivery Date From");
        $('#' + controlId).val('');
    }
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
                showClientSuccessMessage(result.Message);
                $(gridId).trigger("reloadGrid");
            }
            else {
                showClientErrorMessage(result.Message);
            }
        }
    });
}

var isInvoiceSubmitPermitted = "False";
var isCreditNoteSubmitPermitted = "False";
var creditNoteTypeId;

function InitializeSubmissionParameters(hasInvoiceSubmitPermission, hasCreditNoteSubmitPermission, creditNoteType) {
    isInvoiceSubmitPermitted = hasInvoiceSubmitPermission;
    isCreditNoteSubmitPermitted = hasCreditNoteSubmitPermission;
    creditNoteTypeId = creditNoteType;
}

// Enable/disable Submit button based on whether user has permission to submit Invoice/Credit Note.
function GetSelectedRecordId(ids) {
    var $searchGrid = $("#searchGrid");
    var $SubmitInvoicesButton = $('#SubmitInvoicesButton');
    var selectedIds = $searchGrid.getGridParam('selarrrow');
    var hasCreditNote = false;
    var hasInvoice = false;
    // Check if user has permission to submit the invoice/credit note.
    if (isInvoiceSubmitPermitted == "False" || isCreditNoteSubmitPermitted == "False") {
        if (selectedIds) {

            if (selectedIds.length == 0) {
                // Disable Submit button.
                $SubmitInvoicesButton.attr('disabled', 'disabled');
                return;
            }

            for (i = 0; i < selectedIds.length; i++) {
                selectedId = selectedIds[i];
                var gridRow = $searchGrid.getRowData(selectedId);

                if (gridRow.InvoiceTypeId == creditNoteTypeId) // Credit note.
                {
                    hasCreditNote = true;
                }
                else // Invoice other than credit note.
                {
                    hasInvoice = true;
                }
            }
        }

        if ((hasCreditNote && isCreditNoteSubmitPermitted == "False") || (hasInvoice && isInvoiceSubmitPermitted == "False")) {
            // Disable Submit button.
            $SubmitInvoicesButton.attr('disabled', 'disabled');
        }
        else {
            // Enable Submit button.
            $SubmitInvoicesButton.removeAttr('disabled');
        }
    }
    else { // User has permissions to submit all kinds of invoices.
        // Enable Submit button.
        $SubmitInvoicesButton.removeAttr('disabled');
    }
}

function resetMiscInvoiceSearchForm(formId) {
    ResetForm(formId);
    $("#BillingCode").val("-1");
    $("#InvoiceStatus").val("-1");
    $("#SettlementMethodId").val("-1");
    $("#SubmissionMethodId").val("-1");
    $("#ChargeCategoryId").val("-1");
    $("#OwnerId").val("-1");
    $('#InvoiceTypeId').val("-1");
    $("#BillingPeriod").val("-1");
}

function callRejectLineItems(methodName, invoiceId, redirectUrl) {
    $.ajax({
        type: "POST",
        url: methodName + "/" + invoiceId,
        success: function (result) {
            if (result.ErrorCode) {
                if (confirm(result.ErrorCode) == false)
                    return;
            }

            if (result.Message) {
                alert(result.Message);
                return;
            }

            // Redirect user to Create Rejection invoice page.
            location.href = result.RedirectUrl;
        }
    });
}

var clearSearchUrl;
function ResetSearch(formId) {
    resetMiscInvoiceSearchForm(formId);
    $.ajax({
        type: "POST",
        url: clearSearchUrl,
        dataType: "json",
        success: function (response) {
            if (response) {
                $('#BillingYearMonth', '#content').val(response.Year + '-' + response.Month);
                $('#BillingPeriod', '#content').val(response.Period);
                $('#BilledMemberId').val('0');
            }
        }
    });
}
