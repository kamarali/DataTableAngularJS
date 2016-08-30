$(document).ready(function () {
    $("#Description").keyup(function () {
        var textareaText = jQuery.trim($(this).val());
        if (textareaText.length > 1000) {
            textareaText = (textareaText.substr(0, 1000));
            $(this).val(textareaText);
        }
    });
    $("#MiscCodeMaster").validate({
        rules: {
            Group: {
                required: true,
                maxlength: 50
            },
            Name: {
                required: true,
                maxlength: 50
            },
            Description: {
                required: true,
                maxlength: 1000
                }

        },
        messages: {
            Group: "Group Code Required and should be of maximum 50 characters.",
            Name: "Misc Code Required and should be of maximum 50 characters.",
            Description: "Description Required and should be of maximum 1000 characters."
        },
        invalidHandler: function () {
            $('#errorContainer').show();
            $('#clientErrorMessageContainer').hide();
            $('#clientSuccessMessageContainer').hide();
        }
    });

});