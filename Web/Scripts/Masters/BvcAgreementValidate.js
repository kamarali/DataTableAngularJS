$(document).ready(function () {
    $("#BVCAgreementMaster").validate({
        rules: {
            BillingMemberText: {
                required: true
            },
            BilledMemberText: {
                required: true
            }
        },
        messages: {
            BillingMemberText: "Please select the billing member.",
            BilledMemberText: "Please select the billed member."
        },
        submitHandler: function (form) {
            var billingMemberId = $("#BillingMemberId").val();
            var billedMemberId = $("#BilledMemberId").val();

            if (billingMemberId == billedMemberId)
                showClientErrorMessage("Billing Member cannot be the same as the Billed Member.");
            else {
                form.submit();
            }
        }
    });

});


function activateRecord(methodName, value, gridId) {
    $('#successMessageContainer').hide();

    if (confirm("Are you sure you want to activate this record?")) {
        $.ajax({
            type: "POST",
            url: methodName + "/" + value,
            success: function (result) {
                // $('#errorContainer').hide();
                if (result.IsFailed == false) {
                    showClientSuccessMessage("Record has been activated");
                }
                else {
                    showClientErrorMessage(result.Message);
                }
                $(gridId).trigger("reloadGrid");
            }
        });
    }
}
function dactivateRecord(methodName, value, gridId) {
    $('#successMessageContainer').hide();

    if (confirm("Are you sure you want to deactivate this record?")) {
        $.ajax({
            type: "POST",
            url: methodName + "/" + value,
            success: function (result) {
                // $('#errorContainer').hide();
                if (result.IsFailed == false) {
                    showClientSuccessMessage("Record has been deactivated ");
                }
                else {
                    showClientErrorMessage(result.Message);
                }
                $(gridId).trigger("reloadGrid");
            }
        });
    }
}

