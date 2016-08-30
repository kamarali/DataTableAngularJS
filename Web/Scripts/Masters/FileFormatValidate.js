$(document).ready(function () {
    $("#Description").keyup(function () {
        var textareaText = jQuery.trim($(this).val());
        if (textareaText.length > 255) {
            textareaText = (textareaText.substr(0, 255));
            $(this).val(textareaText);
        }
    });
    $("#FileFormatMaster").validate({
        rules: {
            FileVersion: {
                required: true,
                maxlength: 20
            },
            Description: {
                required: true,
                maxlength: 255
            }
        },
        messages: {
            FileVersion: " File Version Required and should be of maximum 20 characters",
            Description: " Description Required and should be of maximum 255 characters"
        },
        invalidHandler: function () {
            $('#errorContainer').show();
            $('#clientErrorMessageContainer').hide();
            $('#clientSuccessMessageContainer').hide();
        }
    });

});
