$(document).ready(function () {
    $("#Description").keyup(function () {
        var textareaText = jQuery.trim($(this).val());
        if (textareaText.length > 255) {
            textareaText = (textareaText.substr(0, 255));
            $(this).val(textareaText);
        }
    });
    $("#VatIdentifierMaster").validate({
        rules: {
            Identifier: {
                required: true,
                maxlength: 2
            },
            BillingCategoryCode: {
                required: true,
                min: 1
            },
            Description: {
                required: true,
                maxlength: 255
            }
        },
        messages: {
            Identifier: "Vat Identifier Required and should be of maximum 2 characters",
            BillingCategoryCode: "Billing Category Required",
            Description: " Description Required and should be of maximum 255 characters"
        },
        invalidHandler: function () {
            $('#errorContainer').show();
            $('#clientErrorMessageContainer').hide();
            $('#clientSuccessMessageContainer').hide();
        }
    });

});