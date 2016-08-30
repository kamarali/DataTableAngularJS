$(document).ready(function () {
    $("#Description").keyup(function () {
        var textareaText = jQuery.trim($(this).val());
        if (textareaText.length > 255) {
            textareaText = (textareaText.substr(0, 255));
            $(this).val(textareaText);
        }
    });
    $("#TransactionTypeMaster").validate({
        rules: {
            Name: {
                required: true,
                maxlength: 20
            },
            Description: {
                required: true,
                maxlength: 255
            },
            BillingCategoryCode: {
                required: true,
                min: 1
            }
        },
        messages: {
            Name: " Transaction Type Required and should be of maximum 50 characters",
            Description: " Description Required and should be of maximum 255 characters",
            BillingCategoryCode: "Billing Category Code Required"
        },
        invalidHandler: function () {
            $('#errorContainer').show();
            $('#clientErrorMessageContainer').hide();
            $('#clientSuccessMessageContainer').hide();
        }
    });

});
