$(document).ready(function () {

    $("#SourceCodeMaster").validate({
        rules: {
            SourceCodeIdentifier: {
                required: true,
                maxlength: 2,
                number: true
            },
            TransactionTypeId: {
                required: true
            },
            Description: {
                maxlength: 255
            }
        },
        messages: {
            SourceCodeIdentifier: " Source Code Required and should be of maximum 2 digit number",
            TransactionTypeId: " Transaction Type Required",
            Description: " Description should be of maximum 255 characters"
        },
        invalidHandler: function () {
            $('#errorContainer').show();
            $('#clientErrorMessageContainer').hide();
            $('#clientSuccessMessageContainer').hide();
        }
    });

});
