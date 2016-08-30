$(document).ready(function () {
//CMP#642: Changes for Please Select.
//    var precision = $.trim($('#Precision').val());
//    if (precision == '') {
//        $('#Precision').val('0');
//    }
//    if (isNaN(precision)) {
//        $('#Precision').val('0');
//    }
    $("#CurrencyMaster").validate({
        rules: {
            Id: {
                required: true,
                maxlength: 5
            },
            Code: {
                required: true,
                maxlength: 3
            },
            Name: {
                required: true,
                maxlength: 50
            },
            Precision: {
                required: true,
                maxlength: 1
            }
        },
        messages: {
          Id: "Currency Numeric Code Required and should be of maximum 5 digit.",
          Code: "Currency Alpha Code Required and should be of maximum 3 characters.",
            Name: "Currency Name Required and should be of maximum 50 characters.",
            Precision: "Currency Precision is Required."
        },
        invalidHandler: function () {
            $('#errorContainer').show();
            $('#clientErrorMessageContainer').hide();
            $('#clientSuccessMessageContainer').hide();
        }
    });

});