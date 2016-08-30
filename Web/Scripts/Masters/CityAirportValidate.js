$(document).ready(function () {

    $("#CityAirportMaster").validate({
        rules: {
            CountryId: {
                required: true
            },
            Name: {
                required: true,
                maxlength: 50
            },
            Id: {
                required: true,
                maxlength: 4
            },
            MainCity: {
                required: true,
                maxlength: 4
            },
            CityCodeNumeric: {
                maxlength: 5,
                number: true
            }
        },
        messages: {
            CountryId: "Country Name Required",
            Name: "City Name Required and should be of maximum 50 characters",
            Id: "City Airport Code Required and should be of maximum 4 characters",
            MainCity: "Main City Required and should be of maximum 4 characters",
            CityCodeNumeric: "City Code Numeric should be of maximum 5 digits"
        },
        invalidHandler: function () {
            $('#errorContainer').show();
            $('#clientErrorMessageContainer').hide();
            $('#clientSuccessMessageContainer').hide();
        }
    });

});
