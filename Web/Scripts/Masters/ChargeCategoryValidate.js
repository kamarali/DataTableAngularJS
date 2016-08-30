$(document).ready(function () {

    $("#ChargeCategoryMaster").validate({
        rules: {
            BillingCategoryId: {
                required: true,
                min: 1
            },
            Name: {
                required: true,
                maxlength: 25
            },
             Description: {
                maxlength: 255
            }
        },
        messages: {
            BillingCategoryId: " Billing Category Required.",
            Name: " Charge Category Name Required and should be of maximum 25 characters",
            Description: " Description should be of maximum 255 characters"
        },
        invalidHandler: function () {
            $('#errorContainer').show();
            $('#clientErrorMessageContainer').hide();
            $('#clientSuccessMessageContainer').hide();
        }
    });

});
