$(document).ready(function () {

    $("#SampleDigitMaster").validate({
        rules: {
            ProvisionalBillingMonth: {
                required: true,
                maxlength: 6,
                minlength:6
            },
            DigitAnnouncementDateTime: {
                required: true,
                maxlength: 9,
                minlength: 9
            }
        },
        messages: {
            ProvisionalBillingMonth: " Billing Month Required and should be of 'YYYYMM' format.",
            DigitAnnouncementDateTime: " Digit Announcement Date Required."
        },
        invalidHandler: function () {
            $('#errorContainer').show();
            $('#clientErrorMessageContainer').hide();
            $('#clientSuccessMessageContainer').hide();
        }
    });

});
