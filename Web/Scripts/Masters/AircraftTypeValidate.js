$(document).ready(function () {
  $("#Description").keyup(function () {
    var textareaText = jQuery.trim($(this).val());
    if (textareaText.length > 255) {
      textareaText = (textareaText.substr(0, 255));
      $(this).val(textareaText);
    }
  });

  $("#AircraftTypeMaster").validate({
    rules: {
      Id: {
        required: true,
        maxlength: 3
      },
      IcaoCode: {
        required: true,
        maxlength: 4
      }
    },
    messages: {
      Id: "Aircraft Code Required and should be of maximum 3 characters",
      IcaoCode: "Aircraft Icao Code Required and should be of maximum 4 characters"
    },
    invalidHandler: function () {
      $('#errorContainer').show();
      $('#clientErrorMessageContainer').hide();
      $('#clientSuccessMessageContainer').hide();
    }
  });

});