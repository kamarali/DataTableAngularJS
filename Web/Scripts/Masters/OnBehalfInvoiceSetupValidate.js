
$(document).ready(function () {

    $("#OnBehalfInvoiceSetupMaster").validate({
        rules: {
            BillingCategoryId: {
                required: true,
                min:1
            },
            TransmitterCode: {
                required: true,
                maxlength: 50
            },
            ChargeCategoryId: {
                required: true,
                min: 1

            },
            ChargeCodeId: {
                required: true
            }
        },
        messages: {
            BillingCategoryId: " Billing Category Required.",
            TransmitterCode: " Transmitter Code Required and should be of maximum 50 characters",
            ChargeCategoryId: " Charge Category Required.",
            ChargeCodeId: " Charge Code Required."
        },
        invalidHandler: function () {
            $('#errorContainer').show();
            $('#clientErrorMessageContainer').hide();
            $('#clientSuccessMessageContainer').hide();
        }
    });

});