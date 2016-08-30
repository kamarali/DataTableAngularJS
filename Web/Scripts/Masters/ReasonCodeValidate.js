$(document).ready(function () {
  $("#Description").keyup(function () {
    var textareaText = jQuery.trim($(this).val());
    if (textareaText.length > 255) {
      textareaText = (textareaText.substr(0, 255));
      $(this).val(textareaText);
    }
  });
    $("#ReasonCodeMaster").validate({
        rules: {
            Code: {
                required: true,
                maxlength: 5
            },
            TransactionTypeId: {
                required: true,
                maxlength: 2,
                number: true
            },
           Description: {
                maxlength: 255
            }
        },
        messages: {
            Code: " Reason Code Required and should be of maximum 5 characters",
            TransactionTypeId: " Transaction Type Required",
            Description: " Description should be of maximum 255 characters"
        },
        invalidHandler: function () {
            $('#errorContainer').show();
            $('#clientErrorMessageContainer').hide();
            $('#clientSuccessMessageContainer').hide();
        }
    });

});
