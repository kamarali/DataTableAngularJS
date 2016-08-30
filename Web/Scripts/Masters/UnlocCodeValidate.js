$(document).ready(function () {

    $("#UnlocCodeMaster").validate({
        rules: {
            Name: {
                required: true,
                maxlength: 50
            },
            Id: {
                required: true,
                maxlength: 5,
                minlength:5
            }
        },
        messages: {
            Name: "UN Location Name Required and should be of maximum 50 characters",
            Id: "UN Location Code Required and should be of 5 characters"
        },
        invalidHandler: function () {
            $('#errorContainer').show();
            $('#clientErrorMessageContainer').hide();
            $('#clientSuccessMessageContainer').hide();
        }
    });

});
