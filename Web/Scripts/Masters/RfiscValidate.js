$(document).ready(function () {

    $("#RfiscMaster").validate({
        rules: {
            Id: {
                required: true,
                maxlength: 3
            },
            RficId: {
                required: true,
                maxlength: 1
            },
            CommercialName: {
                required: true,
                maxlength: 50
            }
        },
        messages: {
            Id: "RFISC Code Required and should be of maximum 3 characters",
            RficId: "RFIC Code Required.",
            CommercialName: " Commercial Name should be of maximum 50 characters"
        },
        invalidHandler: function () {
            $('#errorContainer').show();
            $('#clientErrorMessageContainer').hide();
            $('#clientSuccessMessageContainer').hide();
        }
    });

});
