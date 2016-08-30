$(document).ready(function () {
  $("#Description").keyup(function () {
    var textareaText = jQuery.trim($(this).val());
    if (textareaText.length > 255) {
      textareaText = (textareaText.substr(0, 255));
      $(this).val(textareaText);
    }
  });

    $("#AircraftTypeIcaoMaster").validate({
        rules: {
            Id: {
                required: true,
                maxlength: 4
            },
            Description: {
                maxlength: 255
            }
        },
        messages: {
            Id: "Aircraft Code Required and should be of maximum 4 characters",
            Description: "Description should be of maximum 255 characters"
        },
        invalidHandler: function () {
            $('#errorContainer').show();
            $('#clientErrorMessageContainer').hide();
            $('#clientSuccessMessageContainer').hide();
        }
    });

});