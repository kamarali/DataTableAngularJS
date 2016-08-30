$(document).ready(function () {
    $("#InvPaymentStatusMaster").validate({
        rules: {
            Description: {
                required: true,
                maxlength: 100
            },
            ApplicableFor: {
                required: true,
                min: 1
            }
        },
        messages: {
            Description: "Payment Status Description Required and should be of maximum 100 characters.",
            ApplicableFor: "Applicable For is Required"
        },
        invalidHandler: function () {
            $('#errorContainer').show();
            $('#clientErrorMessageContainer').hide();
            $('#clientSuccessMessageContainer').hide();
        }
    });

});