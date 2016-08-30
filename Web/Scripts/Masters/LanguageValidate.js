$(document).ready(function () {
    $("#Description").keyup(function () {
        var textareaText = jQuery.trim($(this).val());
        if (textareaText.length > 200) {
            textareaText = (textareaText.substr(0, 200));
            $(this).val(textareaText);
        }
    });
    $("#LanguageMaster").validate({
        rules: {
            Language_Code: {
                required: true,
                maxlength:2,
                minlength:2
            },

            Language_Desc: {
                required: true,
                maxlength: 200
            }
        },
        messages: {
            Language_Code: " Correct Language Code Required ",

            Language_Desc: " Description is required and should be of maximum 255 characters"
        },
        invalidHandler: function () {
            $('#errorContainer').show();
            $('#clientErrorMessageContainer').hide();
            $('#clientSuccessMessageContainer').hide();
        }
    });

});
