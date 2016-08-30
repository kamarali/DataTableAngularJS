$(document).ready(function () {
    $("#Description").keyup(function () {
        var textareaText = jQuery.trim($(this).val());
        if (textareaText.length > 255) {
            textareaText = (textareaText.substr(0, 255));
            $(this).val(textareaText);
        }
    });
    $("#SisMemberStatusMaster").validate({
        rules: {
            MemberStatus: {
                required: true,
                maxlength: 1
            },
            Description: {
                required: true,
                maxlength: 255
            }
        },
        messages: {
            MemberStatusd: "Member Status Required and should be of maximum 1 characters.",
            Description: "Description Required and should be of maximum 255 characters."
        },
        invalidHandler: function () {
            $('#errorContainer').show();
            $('#clientErrorMessageContainer').hide();
            $('#clientSuccessMessageContainer').hide();
        }
    });

});