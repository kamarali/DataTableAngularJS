$(document).ready(function () { 
    $("#SisMemberSubStatusMaster").validate({
        rules: {
            MemberStatusId: {
                required: true
            },
            MemberSubStatus: {
                required: true,
                maxlength: 1
            },
            Description: {
                required: true,
                maxlength: 254
            }
        },
        messages: {
            MemberStatusId: "Member Status Required.",
            MemberSubStatus: "Member Sub Status Required and should be of maximum 1 characters.",
            Description: "Description Required and should be of maximum 255 characters."
        },
        invalidHandler: function () {
            $('#errorContainer').show();
            $('#clientErrorMessageContainer').hide();
            $('#clientSuccessMessageContainer').hide();
        }
    });

});