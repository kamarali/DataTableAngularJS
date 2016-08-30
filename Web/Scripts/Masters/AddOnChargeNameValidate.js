$(document).ready(function () {

    $("#AddOnChargeNameMaster").validate({
        rules: {
            Name: {
                required: true,
                maxlength: 30
            }
        },
        messages: {
            Name: "Add On ChargeName Required and should be of maximum 30 characters"
        },
        invalidHandler: function () {
            $('#errorContainer').show();
            $('#clientErrorMessageContainer').hide();
            $('#clientSuccessMessageContainer').hide();
        }
    });

});
