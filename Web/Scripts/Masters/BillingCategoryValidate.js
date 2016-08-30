$(document).ready(function () {

    $("#BillingCategoryMaster").validate({
        rules: {
            CodeIsxml: {
                required: true
            },
            Description: {
                maxlength: 255
            }
        },
        messages: {
            CodeIsxml: "Billing Category Code Required and should be of maximum 25 characters",
            Description: " Description should be of maximum 255 characters"
        },
        invalidHandler: function () {
            $('#errorContainer').show();
            $('#clientErrorMessageContainer').hide();
            $('#clientSuccessMessageContainer').hide();
        }
    });

});
