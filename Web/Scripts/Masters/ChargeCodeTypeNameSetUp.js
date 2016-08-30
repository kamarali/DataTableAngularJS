var chargeCodeUrl;

function InitialiseChargeCodeTypeName(chargeCodeURL) {
  chargeCodeUrl = chargeCodeURL;

  $("#ChargeCodeTypeNameMaster").validate({
    rules: {
      ChargeCategoryId: { required: true, min: 1 },
      ChargeCodeId: { required: true, min: 1 },
      Name: { required: true, maxlength: 50, allowedCharacters: true }
    },
    messages: {
      ChargeCategoryId: "Please select a valid Charge Category",
      ChargeCodeId: "Please select a valid Charge Code",
      Name: {
        required: "Charge Code Type Name is mandatory and should be a maximum of 50 characters",
        maxlength: "Charge Code Type Name is mandatory and should be a maximum of 50 characters",
        allowedCharacters: "Invalid characters entered in this field"
      }
    },
    invalidHandler: function () {
      $('#errorContainer').show();
      $('#clientErrorMessageContainer').hide();
      $('#clientSuccessMessageContainer').hide();
    }
  });
}


$("#ChargeCodeId").change(function () {
  var isActive = $("#ChargeCodeTypeNameCreateMaster").val() == "ChargeCodeTypeNameCreateMaster";
  if (isActive) {
    $("#Name").val('');
  }
});

//Get charge code based on charge category and load into drop down list.
$("#ChargeCategoryId").change(function () {
  var chargeCategoryId = $("#ChargeCategoryId").val();
  var isActive = $("#ChargeCodeTypeNameCreateMaster").val() == "ChargeCodeTypeNameCreateMaster";
  $.ajax({
    type: "GET",
    url: chargeCodeUrl,
    data: { chargeCategoryId: chargeCategoryId, isActiveChargeCodeTypeReq: isActive },
    dataType: "json",
    success: function (data) {
      $("#ChargeCodeId").empty();
      if (isActive) {
        $("#Name").val('');
        $("#ChargeCodeId").append("<option value=''>Please Select</option>");
      }
      else
        $("#ChargeCodeId").append("<option value='0'>All</option>");

      $.each(data, function (index, item) {
        $("#ChargeCodeId").append("<option value=" + item.Id + " >" + item.Name + "</option>");
      });
    }
  });
});

