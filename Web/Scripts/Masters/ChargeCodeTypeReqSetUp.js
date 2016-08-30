var chargeCodeUrl;
var viewMode;

function InitialiseChargeCodeTypeReq(chargeCodeURL) {
  chargeCodeUrl = chargeCodeURL;

  $("#ChargeCodeTypeReqMaster").validate({
    rules: {
      ChargeCategoryId: { required: true, min: 1 },
      Id: { required: true, min: 1 },
      IsChargeCodeTypeRequired: "required"
    },
    messages: {
      ChargeCategoryId: "Please select a valid Charge Category",
      Id: "Please select a valid Charge Code",
      IsChargeCodeTypeRequired: "Please select Mandatory or Optional"
    },
    invalidHandler: function () {
      $('#errorContainer').show();
      $('#clientErrorMessageContainer').hide();
      $('#clientSuccessMessageContainer').hide();
    }
  });
}

$("#Id").change(function () {
  var isActive = $("#ChargeCodeTypeCreateMaster").val() == "ChargeCodeTypeCreateMaster";
  if (isActive) {
    $("#IsChargeCodeTypeRequired").val('');
  }
});

//Get charge code based on charge category and load into drop down list.
$("#ChargeCategoryId").change(function () {
  var chargeCategoryId = $("#ChargeCategoryId").val();
  $.ajax({
    type: "GET",
    url: chargeCodeUrl,
    data: { chargeCategoryId: chargeCategoryId },
    dataType: "json",
    success: function (data) {
      $("#Id").empty();
      if ($("#ChargeCodeTypeCreateMaster").val() == "ChargeCodeTypeCreateMaster") {
        $("#IsChargeCodeTypeRequired").val('');
        $("#Id").append("<option value=''>Please Select</option>");
      }
      else
        $("#Id").append("<option value='0'>All</option>");

      $.each(data, function (index, item) {
        $("#Id").append("<option value=" + item.Id + " >" + item.Name + "</option>");
      });
    }
  });
});

