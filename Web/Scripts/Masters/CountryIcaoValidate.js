$(document).ready(function () {
    $("#CountryIcaoMaster").validate({
        rules: {
            Name: {
                required: true,
                maxlength: 50
            },
            CountryCodeIcao: {
                required: function (element) {
                    var countryCoede = $("#CountryCodeIcao").val();                    
                    if (countryCoede.length == 1 || countryCoede.length == 0) {                        
                        $('#CountryCodeIcao').val('');                        
                        return true;
                        maxlength: 2
                    }
                    else {
                        return false;
                    }
                }
            }
        },
        messages: {
            Name: "Country ICAO Name Required and should be of maximum 50 characters",
            CountryCodeIcao: "Country ICAO Code Required and should be of 2 characters."
        },
        invalidHandler: function () {
            $('#errorContainer').show();
            $('#clientErrorMessageContainer').hide();
            $('#clientSuccessMessageContainer').hide();
        }
    });

});

