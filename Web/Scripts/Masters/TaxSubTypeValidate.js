$(document).ready(function () {

    $("#TaxSubTypeMaster").validate({
        rules: {
            SubType: {
                required: true,
                maxlength: 20
            },
            Type: {
                required: true,
                maxlength: 1
            }
        },
        messages: {
            SubType: " Sub Type Required and should be of maximum 20 characters",
            Type: " Type Required and should be of maximum one character"
        },
        invalidHandler: function () {
            $('#errorContainer').show();
            $('#clientErrorMessageContainer').hide();
            $('#clientSuccessMessageContainer').hide();
        }
    });

});