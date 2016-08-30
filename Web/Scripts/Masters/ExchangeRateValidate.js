$(document).ready(function () {

    $("#ExchangeRateMaster").validate({
        rules: {
            CurrencyId: {
                required: true
            },
            EffectiveFromDate: {
                required: true
            },
            EffectiveToDate: {
                required: true
            },
            FiveDayRateUsd: {
                required: true,
                maxlength: 16
            },
            FiveDayRateGbp: {
                required: true,
                maxlength: 16
            },
            FiveDayRateEur: {
                required: true,
                maxlength: 16
            }

        },
        messages: {
            CurrencyId: "Currency Required",
            EffectiveFromDate: "Effective From Date Required.",
            EffectiveToDate: "Effective To Date Required.",
            FiveDayRateUsd: "Five Day Rate Usd Required and should be of maximum 16 digit number.",
            FiveDayRateGbp: "Five Day Rate Gbp Required and should be of maximum 16 digit number.",
            FiveDayRateEur: "Five Day Rate Eur Required and should be of maximum 16 digit number."
        },
        invalidHandler: function () {
            $('#errorContainer').show();
            $('#clientErrorMessageContainer').hide();
            $('#clientSuccessMessageContainer').hide();
        }
    });

});
