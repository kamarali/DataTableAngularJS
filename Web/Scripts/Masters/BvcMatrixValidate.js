$(document).ready(function () {

    $("#BvcMatrixMaster").validate({
        rules: {
            EffectiveFrom: {
                required: true,
                maxlength: 8,
                minlength: 8
            },
            EffectiveTo: {
                required: true,
                maxlength: 8,
                minlength: 8
            }

        },
        messages: {
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
