$(document).ready(function () {
    $("#CgoRMReasonAcceptableDiffMaster").validate({
        rules: {
            TransactionTypeId: {
                required: true
            },
            ReasonCodeId: {
                required: true
            },
            EffectiveFrom: {
                required: true,
                maxlength: 8,
                minlength: 8
            },
            EffectiveTo: {
                required: true,
                maxlength: 8,
                minlength:8
            }

        },
        messages: {
            TransactionTypeId: " Transaction Type Required",
            ReasonCodeId: " Reason Code Required",
            EffectiveFrom: " Effective From Required and should be of 'YYYYMMPP' format.",
            EffectiveTo: "Effective To Required and should be of 'YYYYMMPP' format."
        },
        invalidHandler: function () {
            $('#errorContainer').show();
            $('#clientErrorMessageContainer').hide();
            $('#clientSuccessMessageContainer').hide();
        }
    });

});
