$(document).ready(function () {

    $("#CountryMaster").validate({
        rules: {
            Name: {
                required: true,
                maxlength: 50
            },
            Id: {
                required: true,
                maxlength: 2
            },
           
            DsFormat: {
                required: function (element) {
                    var isDsFormat = $("#DsFormat").val();
                    if (isDsFormat != "") {
                        if (isDsFormat == "X" || isDsFormat == "P" || isDsFormat == "x" || isDsFormat == "p") {
                            return false;
                        }
                        else {
                            $("#DsFormat").val(''); 
                            return true;
                        }
                    } else {
                        return false;
                    }
                }
            }
        },
        messages: {
            Name: "Country Name Required and should be of maximum 50 characters",
            Id: "Country Code Required and should be of maximum 2 characters.",
            DsFormat: "Legal Format for Digital Signature should be either 'P' or 'X'."
        },
        invalidHandler: function () {
            $('#errorContainer').show();
            $('#clientErrorMessageContainer').hide();
            $('#clientSuccessMessageContainer').hide();
        }
    });

});

