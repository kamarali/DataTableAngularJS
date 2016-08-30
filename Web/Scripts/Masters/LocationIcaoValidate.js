$(document).ready(function () {

    //SCPID : 107323 - ICAO location code - Incorrect reference to country master
    var value = $('#Description').val();
    value = $.trim(value);
    $('#Description').val(value);

    $("#LocationIcaoMaster").validate({
        rules: {

            Id: {
                required: true,
                maxlength: 4
            },
            CountryCode: {
                required: true
            },
            DsFormat: {
                maxlength: 1
            }
        },
        messages: {
            Id: " Country Code Required and should be of maximum 4 characters.",
            CountryCode: " Country Code Required.",
            Description: " Description Required and should be of maximum 255 characters."
        },
        invalidHandler: function () {
            $('#errorContainer').show();
            $('#clientErrorMessageContainer').hide();
            $('#clientSuccessMessageContainer').hide();
        }
    });

    
});

