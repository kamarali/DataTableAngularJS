$(document).ready(function () {
    $("#MinMaxAcceptableAmountMaster").validate({
        rules: {
            TransactionTypeId: {
                required: true
            },
            Min: {
                required: true,
                maxlength: 11
            },
            Max: {
                required: true,
                maxlength: 11
            },
           ClearingHouse: {
                required: function (element) {
                    var clearingHouse = $("#ClearingHouse").val();
                    if (clearingHouse != "") {
                        if (clearingHouse == "I" || clearingHouse == "i" || clearingHouse == "A" || clearingHouse == "a") {
                            return false;
                        }
                        else {
                            $("#ClearingHouse").val('');
                            return true;
                        }
                    } else {
                        return true;
                    }
                },
                maxlength: 1
            }
        },
        messages: {
            TransactionTypeId: "Transaction Type Required.",
            Min: "Min amount Required and should be of maximum 10 digit number.",
            Max: "Max amount Required and should be of maximum 10 digit number.",
            ClearingHouse: " Clearing House Required and should be either 'I' or 'A'"
        },
        invalidHandler: function () {
            $('#errorContainer').show();
            $('#clientErrorMessageContainer').hide();
            $('#clientSuccessMessageContainer').hide();
        }
    });

});