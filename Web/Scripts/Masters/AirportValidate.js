$(document).ready(function () {

    $("#AirportMaster").validate({
        rules: {
            CountryCode: {
                required: true
            },
            Name: {
                required: true,
                maxlength: 50
            },
            Id: {
                required: true,
                maxlength: 4
            }
        },
        messages: {
            CountryCode: "Country Required",
            Name: "Airport Icao Name Required and should be of maximum 50 characters",
            Id: "Airport Icao Code Required and should be of maximum 4 characters"
        },
        invalidHandler: function () {
            $('#errorContainer').show();
            $('#clientErrorMessageContainer').hide();
            $('#clientSuccessMessageContainer').hide();
        }
    });

});
