$(document).ready(function () {

    $("#ISCalendarMaster").validate({
        rules: {
            Name: {
                required: true,
                maxlength: 100
            },
            Month: {
                required: true,
                maxlength: 2,
                number:true
            },
            Year: {
                required: true,
                maxlength: 4,
                number: true
            },
            Period: {
                required: true,
                maxlength: 2,
                number: true
            },
            EventCategory: {
                required: true,
                maxlength: 10
            },
            EventDescription: {
                required: true,
                maxlength: 255
            },
            EventDateTime: {
                required: true
            }
        },
        messages: {
            Name: " Event Name Required and should be of maximum 100 characters",
            Month: " Month Required",
            Year: " Year Required",
            Period: " Period Required and should be of maximum 2 characters",
            EventCategory: " Event Category Required and should be of maximum 10 characters",
            EventDescription: " Event Description Required and should be of maximum 255 characters",
            EventDateTime: " Event DateTime Required"
        },
        invalidHandler: function () {
            $('#errorContainer').show();
            $('#clientErrorMessageContainer').hide();
            $('#clientSuccessMessageContainer').hide();
        }
    });

});
