$(document).ready(function () {

    $("#TaxCodeMaster").validate({
        rules: {
            TaxCodeTypeId: {
                required: function (element) {
                    var taxCodeTypeId = $("#TaxCodeTypeId").val();
                    if (taxCodeTypeId != "") {
                        if (taxCodeTypeId == "T" || taxCodeTypeId == "t" || taxCodeTypeId == "V" || taxCodeTypeId == "v") {
                            return false;
                        }
                        else {
                            $("#TaxCodeTypeId").val('');
                            return true;
                        }
                    } else {
                        return true;
                    }
                },
                maxlength: 2
            },
            Description: {
                required: true,
                maxlength: 255
            },
            Id: {
                required: true,
                maxlength: 3
            }
        },
        messages: {
            TaxCodeTypeId: " Tax Code Type Required and should be either 'T' or 'V'",
            Description: " Description Required and should be of maximum 255 characters",
            Id: "Tax Code Required and should be of maximum 3 characters"
        },
        invalidHandler: function () {
            $('#errorContainer').show();
            $('#clientErrorMessageContainer').hide();
            $('#clientSuccessMessageContainer').hide();
        }
    });

});
