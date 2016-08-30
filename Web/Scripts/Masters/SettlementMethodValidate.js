$(document).ready(function () {
    $("#SettlementMethodMaster").validate({
        rules: {
            Description: {
                required: true
                },
            Name: {
                required: true,
                maxlength: 1
            }
        },
        messages: {
            Description: "Description is required.",
            Name: "Settlement Method Name is required."
        },
        invalidHandler: function () {
            $('#errorContainer').show();
            $('#clientErrorMessageContainer').hide();
            $('#clientSuccessMessageContainer').hide();
        }
    });

});