var IsLegalArchivingUrl;
function ValidateSISUsageReport(IsLegalArchivingURL) {
    IsLegalArchivingUrl = IsLegalArchivingURL;

    $("#SISUsageReport").validate({
        rules: {

            FromBillingYear: {
                required: true
            },

            FromBillingMonth: {
                required: true
            },

            FromPeriod: {
                required: true
            },

            ToBillingYear: {
                required: true
            },

            ToBillingMonth: {
                required: true
            },

            ToPeriod: {
                required: true
            }
        },
        messages: {
            FromBillingYear: "From billing year is required.",
            FromBillingMonth: "From Billing Month is required.",
            FromPeriod: "From Period is required.",
            ToBillingYear: "To billing year is required.",
            ToBillingMonth: "To Billing Month is Required.",
            ToPeriod: "To Period is required."
        },
        submitHandler: function (form) {
            var FromDate = new Date($('#FromBillingYear').val() + '/' + $('#FromBillingMonth').val() + '/' + $('#FromPeriod').val());
            var ToDate = new Date($('#ToBillingYear').val() + '/' + $('#ToBillingMonth').val() + '/' + $('#ToPeriod').val());
            if (FromDate > ToDate)
                alert("From Billing Year/Month/Period : " + $('#FromBillingYear').val() + " / " + $('#FromBillingMonth').val() + " / " + $('#FromPeriod').val() + "\n     To Billing Year/Month/Period : " + $('#ToBillingYear').val() + " / " + $('#ToBillingMonth').val() + " / " + $('#ToPeriod').val() + "\nThe Combination of 'To' Billing Year/Month/Period Should Not Be Earlier Than The Combination of 'From' Billing Year/Month/Period.");
            else {
                $('#errorContainer').hide();
                form.submit();
            }
        },
        invalidHandler: function () {
            $('#errorContainer').show();
            $('#clientErrorMessageContainer').hide();
            $('#clientSuccessMessageContainer').hide();
            $.watermark.showAll();
        }
    });

    //CMP #659: SIS IS-WEB Usage Report.
    $("#SisIsWebUsageReport").validate({
        rules: {

            FromBillingYear: {
                required: true
            },

            FromBillingMonth: {
                required: true
            },

            FromPeriod: {
                required: true
            },

            ToBillingYear: {
                required: true
            },

            ToBillingMonth: {
                required: true
            },

            ToPeriod: {
                required: true
            }
        },
        messages: {
            FromBillingYear: "From billing year is required.",
            FromBillingMonth: "From Billing Month is required.",
            FromPeriod: "From Period is required.",
            ToBillingYear: "To billing year is required.",
            ToBillingMonth: "To Billing Month is Required.",
            ToPeriod: "To Period is required."
        },
        submitHandler: function (form) {
            var FromDate = new Date($('#FromBillingYear').val() + '/' + $('#FromBillingMonth').val() + '/' + $('#FromPeriod').val());
            var ToDate = new Date($('#ToBillingYear').val() + '/' + $('#ToBillingMonth').val() + '/' + $('#ToPeriod').val());
            if (FromDate > ToDate)
                alert("From Billing Year/Month/Period : " + $('#FromBillingYear').val() + " / " + $('#FromBillingMonth').val() + " / " + $('#FromPeriod').val() + "\nTo Billing Year/Month/Period : " + $('#ToBillingYear').val() + " / " + $('#ToBillingMonth').val() + " / " + $('#ToPeriod').val() + "\nThe Combination of 'To' Billing Year/Month/Period Should Not Be Earlier Than The Combination of 'From' Billing Year/Month/Period.");
            else {
                //This function is used to check legal archive process completed for the period.
                //CMP #659: SIS IS-WEB Usage Report.
                var billingMonth = $('#ToBillingMonth').val().length < 2 ? "0" + $('#ToBillingMonth').val() : $('#ToBillingMonth').val();
                var billingPeriod = $('#ToBillingYear').val() + billingMonth + '0' + $('#ToPeriod').val();
                
                $.ajax({
                    type: "GET",
                    url: IsLegalArchivingUrl,
                    data: { billingPeriod: billingPeriod },
                    async: false,
                    dataType: "json",
                    success: function (response) {
                        if (response) {
                            $('#errorContainer').hide();
                            form.submit();
                        }
                        else {
                            alert('Output processes for Billing Period ' + $('#ToBillingYear').val() + " / " + $('#ToBillingMonth').val() + " / " + $('#ToPeriod').val() + ' are still pending; and this is a prerequisite for inclusion of a Billing Period in this report. Please modify your Search Criteria');

                            return false;
                        }

                    }
                });


            }
        },
        invalidHandler: function () {
            $('#errorContainer').show();
            $('#clientErrorMessageContainer').hide();
            $('#clientSuccessMessageContainer').hide();
            $.watermark.showAll();
        }
    });
}

