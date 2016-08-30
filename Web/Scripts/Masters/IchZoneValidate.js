$(document).ready(function () {
    $("#IchZoneMaster").validate({
        rules: {
            Zone: {
                required: true,
                maxlength: 1
            },
            ClearanceCurrency: {
                required: true,
                maxlength: 3
            },
            Description: { 
                required: true,
                maxlength: 255
            }
        },
        messages: {
            Zone: " Zone Required and should be of maximum 1 character.",
            ClearanceCurrency: " ClearanceCurrency Required and should be of maximum 3 characters",
            Description: " Description Required and should be of maximum 255 characters"
        },
        invalidHandler: function () {
            $('#errorContainer').show();
            $('#clientErrorMessageContainer').hide();
            $('#clientSuccessMessageContainer').hide();
        }
    });

});
