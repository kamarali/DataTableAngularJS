$(document).ready(function () {

    $("#ContactsMaster").validate({
        rules: {
            ContactTypeName: {
                required: true,
                maxlength: 200
            },
            TypeId: {
                required: true
            },
            GroupId: {
                required: true
            },
            SubGroupId: {
                required: true
            }

        },
        messages: {
            ContactTypeName: "Contact Type Name Required and should be of maximum 200 characters.",
            TypeId: "Type Required.",
            GroupId: "Group Required.",
            SubGroupId: "Sub Group Required."
        },
        invalidHandler: function () {
            $('#errorContainer').show();
            $('#clientErrorMessageContainer').hide();
            $('#clientSuccessMessageContainer').hide();
        }
    });

});