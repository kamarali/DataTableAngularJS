$(document).ready(function () {
  $("#UomCodeMaster").validate({
    rules: {
      Id: {
        required: true,
        maxlength: 3
      },
      Type: {
        required: true,
        maxlength: 1,
        min: 0,
        max: 9
      },
      Description: {
        maxlength: 255
      }
    },
    messages: {
      Id: "Uom Code Required and should be of maximum 3 alphaNumeric upperCase characters",
      //Type: "Uom Code Type Required and should be of maximum 1 integer number i.e. number 0-9.",
      Type: { required: "Uom Code Type Required." },
      Description: "Description should be of maximum 255 characters."
    },
    invalidHandler: function () {
      $('#errorContainer').show();
      $('#clientErrorMessageContainer').hide();
      $('#clientSuccessMessageContainer').hide();
    },
    submitHandler: function (form) {
      $("#Type").attr("disabled", false);
      // submit the form.
      onSubmitHandler(form);
    }
  });

});

