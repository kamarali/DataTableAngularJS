$(document).ready(function () {
   
    $("#OldIdecParticipationMaster").validate({
        rules: {
            MemberId: {
                required: true
            }
        },
        messages: {
            MemberId: "Member Code Numeric  Required."
        },
        invalidHandler: function () {
            $('#errorContainer').show();
            $('#clientErrorMessageContainer').hide();
            $('#clientSuccessMessageContainer').hide();
        }
    });

});