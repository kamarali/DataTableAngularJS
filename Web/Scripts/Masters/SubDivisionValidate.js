$(document).ready(function () {

    $("#SubDivisionMaster").validate({
        rules: {
            Id: {
                required: true,
                maxlength: 3
            },
            Name: {
                required: true,
                maxlength: 50
            },
            CountryId: {
                required: true
            }
        },
        messages: {
            Id: " Sub Division Code Required and should be of maximum 3 characters",
            Name: " Sub Division Name Required and should be of maximum 50 characters",
            CountryId: " Country Code Required"
        },
        invalidHandler: function () {
            $('#errorContainer').show();
            $('#clientErrorMessageContainer').hide();
            $('#clientSuccessMessageContainer').hide();
        }
    });

});