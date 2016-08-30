$(document).ready(function () {
    $("#LeadPeriodMaster").validate({
        rules: {
            EffectiveFromPeriod: {
                required: true,
                maxlength: 11
            },
            EffectiveToPeriod: {
                required: true,
                maxlength: 11
            },
            BillingCategoryId: {
                required: function () {
                    
                    var billingCategoryId = $('#BillingCategoryId').val();
                    if (billingCategoryId == 0)
                        return true;
                    else
                        return false;
                }
            },
            Period: {
                required: true
            },
            IsSamplingIndicator: {
                required: function (element) {
                    var samplingIndicator = $("#IsSamplingIndicator").val();
                    if (samplingIndicator == "TRUE" || samplingIndicator == "true") {
                        $('#SamplingIndicator').val('Y');
                    }
                    else {
                        $("#SamplingIndicator").val('N');

                    }
                    return false;

                }
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
            EffectiveFromPeriod: "Effective From Period Required",
            EffectiveToPeriod: "Effective To Period Required",
            Period: "Lead Period Required.",
            BillingCategoryId: "Billing Category Required.",
            SamplingIndicator: "Sampling Indicator Required.",
            ClearingHouse: " Clearing House Required and should be either 'I' or 'A'"
        },
        invalidHandler: function () {
            $('#errorContainer').show();
            $('#clientErrorMessageContainer').hide();
            $('#clientSuccessMessageContainer').hide();
        }
    });

});