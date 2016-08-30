var chargeCodeTypeUrl;
var chargeCodeDetailUrl;
var lineItemDetailsUrl;
var invoiceType;
var isActiveChargeCodeType;
var chargeCodeTypeReq;
var chargeCodeTypeId;
_amountDecimals = 3;

function InitialiseLineItem(chargeCodeTypeActionUrl, lineItemDetailsActionUrl, chargeCodeDetailsUrl) {
  chargeCodeTypeUrl = chargeCodeTypeActionUrl;
  lineItemDetailsUrl = lineItemDetailsActionUrl;
  chargeCodeDetailUrl = chargeCodeDetailsUrl;
  SetMiscPageWaterMark();
  // If pageMode == Create/Edit, register control events and validation rules
  AddPOLineItemNoValidator();
  if ($isOnView != true) {
    $("#LineItemForm").validate({
      rules: {
        ChargeCodeId: "required",
        Description: {
          required: true,
          maxlength: 240
        },
        EndDate: {
          required: true
        },
        //CMP515: If readonly then ignore validation. 
        LocationCode: {
          required: function (element) {
              if ($("#LocationCode").prop('readonly') == false)
              return true;
            else
              return false;
          }
        },
        //SCP220346: Inward Billing-XML file mandatory field
        ScalingFactor: {
          required: function (element) {
              if ($("#ScalingFactor").prop('readonly') == false)
              return true;
            else
              return false;
          },
          digits: true
        },
        POLineItemNumber: {
          validatePOLineItemNo: true
        },
        StartDate: { compareServiceDates: true },
        UomCodeId: "required",
        UnitPrice: "required",
        Quantity: "required",
        RejectionReasonCode: {required: function (element) {
		         if ($("#RejectionReasonCode").val()=='') {
		         $("#RejectionReasonCodeText").val('');
                    return true;
                  }
                  else {
                    return true;
                  }
                }
		    },
        ChargeCodeTypeId: {
          required: function (element) {
            if ($("#chargeCodeTypeDiv").css("display") == "block") {
              SetChargeCodeTypReqFlag();
              if (isActiveChargeCodeType && chargeCodeTypeReq) {
                return true;
              } else {
                return false;
              }
            }
            else {
              return false;
            }
          }
        },
        ChargeAmount: { required: function () {
          return $('#MinimumQuantityFlag:checked').val() != undefined;
        }
        }
      },
      messages: {
        ChargeCodeId: "Charge Code Required",
        ChargeCodeTypeId: "Charge Code Type Required",
        Description: "Description Required and should be of maximum 240 characters",
        EndDate: "End Date Required",
        LocationCode: "Location Code is missing. It should be provided either at Invoice level or Line Item level",
        UomCodeId: "UOM Code Required",
        UnitPrice: "Enter a valid unit price (dot should be used as a decimal separator).",
        Quantity: "Quantity Required and should be within 0.0001 and 99999999999999.9999",
        ChargeAmount: "Line Total is required and  should be within -99999999999999.999 and 99999999999999.999",
        TotalNetAmount: "Line Net Total should be within -99999999999999.999 and 99999999999999.999",
        TotalTaxAmount: "Tax Amount should be within -99999999999999.999 and 99999999999999.999",
        TotalVatAmount: "Vat Amount should be within -99999999999999.999 and 99999999999999.999",
        TotalAddOnChargeAmount: "Add On Charge Amount should be within -99999999999999.999 and 99999999999999.999",
        //SCP220346: Inward Billing-XML file mandatory field
        ScalingFactor: "Scaling factor is invalid (valid values 1-99999).",
        RejectionReasonCode: "Rejection Reason Code is mandatory"
      },
      submitHandler:
      function (form) {
        $('#UomCodeId').attr('disabled', false);
        $('#ChargeCodeId').attr('disabled', false);
        $('#ChargeCodeTypeId').attr('disabled', false);
        $('#SaveLineItem').attr('disabled', true);
        $('#UploadAttachment').attr('disabled', true);

        // Call onSubmitHandler() function which will disable Submit buttons and will submit the form
        onSubmitHandler(form);
      }
    });

    trackFormChanges('LineItemForm');
    validationStartAndEndDate();

    $("#Quantity").blur(function () {
      if ($('#MinimumQuantityFlag:checked').val() == undefined) {
        CalculateLineTotal();
      }
    });

    $("#UnitPrice").blur(function () {
      if ($('#MinimumQuantityFlag:checked').val() == undefined) {
        CalculateLineTotal();
      }
    });

    $("#ScalingFactor").blur(function () {
      if ($('#MinimumQuantityFlag:checked').val() == undefined) {
        CalculateLineTotal();
      }
    });


    $("#ChargeAmount").blur(function () {
      CalculateLineNetTotal();
    });

    $("#TaxAmount").blur(function () {
      CalculateLineNetTotal();
    });

    $("#VatAmount").blur(function () {
      CalculateLineNetTotal();
    });

    $("#TotalAddChargeAmount").blur(function () {
      CalculateLineNetTotal();
    });


    $("#ChargeCodeId").bind("change", OnChargeCodeChange);
    $("#RejectionReasonCodeText").bind("change", onBlankRejectionReasonCode);

  $("#MinimumQuantityFlag").bind("click", OnMinimumQuantityFlagClick);
    $("#ChargeAmount").bind("change", CalculateLineNetTotal);
    $("#AdditionalDetailDescription").bind("keypress", function () { maxLength(this, 80) });
    $("#AdditionalDetailDescription").bind("paste", function () { maxLengthPaste(this, 80) });

  }
}


function validationStartAndEndDate() {
    jQuery.validator.addMethod('compareServiceDates',
    function (value, element) {
        if ($("#EndDate").val() != '') {
            var startDate = $("#StartDate").datepicker("getDate");
            var endDate = $("#EndDate").datepicker("getDate");
            if (startDate != null && startDate.getTime() >= endDate.getTime()) {
                return false;
            }
            else return true;
        }

        return true;
    },
    "Service Start Date should not be greater than or equal to Service End Date. Only service end date needs to be supplied, if the service start and end date is same.");
}

function AddPOLineItemNoValidator() {
    jQuery.validator.addMethod('validatePOLineItemNo',
    function (value, element) {
        if (value != '') {
            if ($("#PONumber").val() == '') {
                return false;
            }
        }
        return true;
    },
    "PO Line Number can not be entered, as PO Number is not populated at Invoice level.");
}

function SetControlProperties(response) {
  if (response == true) {
    $('#Quantity').val("1");
    $('#Quantity').attr('readonly', 'readonly');
    $('#UnitPrice').val("0.0000");
    $('#UnitPrice').attr('readonly', 'readonly');
    $('#MinimumQuantityFlag').removeAttr('checked');
    $('#MinimumQuantityIndicator').hide();
    $('#UomCodeId').attr('disabled', true);
    // default to EA when line item detail is expected.
    $('#UomCodeId').val("EA");
    $('#ScalingFactor').attr('disabled', true);
    CalculateLineTotal();
  }
  else {
    $("#Quantity").removeAttr('readonly');
    $('#Quantity').val("1");
    $('#UnitPrice').val("0.0000");
    $("#UnitPrice").removeAttr('readonly');
    $('#MinimumQuantityIndicator').show();
    $('#UomCodeId').removeAttr('disabled');
    $('#ScalingFactor').attr('disabled', false);
  }
}



function OnMinimumQuantityFlagClick() {

  var isMinimumQuantityFlagChecked = $('#MinimumQuantityFlag:checked').val();

  if (isMinimumQuantityFlagChecked != undefined) {
    $('#ChargeAmount').removeAttr('readonly');
    $('#ChargeAmount').val('');
   
    // Calculate line net total as line total value is changed to 0.
    CalculateLineNetTotal();
  }
  else {
    CalculateLineTotal();
  }
  $.watermark.show('#ChargeAmount');
}

function SetChargeCodeTypReqFlag()
{
  //CMP #636: Standard Update Mobilization
  var selectedChargeCodeId = $("#ChargeCodeId").val();
  //CMP #636: Standard Update Mobilization
  $.ajax({
    type: "Post",
    url: chargeCodeDetailUrl,
    data: { chargeCodeId: selectedChargeCodeId },
    dataType: "json",
    async: false,
    success: function (responce) {
      chargeCodeTypeReq = responce.chargeCodeTypeReq;
      isActiveChargeCodeType = responce.isActiveChargeCodeType;
    }
  });
}

function OnChargeCodeChange() {
  var selectedChargeCodeId = $("#ChargeCodeId").val();
  chargeCodeTypeId = $("#ChargeCodeTypeId").val();
  //CMP #636: Standard Update Mobilization
  SetChargeCodeTypReqFlag();

  if (selectedChargeCodeId == 0) {
    SetControlProperties(false);
    return;
  }
  $.ajax({
    type: "Post",
    url: chargeCodeTypeUrl,
    data: { chargeCodeId: selectedChargeCodeId },
    dataType: "json",
    success: PopulateChargeCodeTypes,
    failure: function (response) {
      $("#ChargeCodeTypeId").val("");
      $("#chargeCodeTypeDiv").hide();
    }
  });
}

//CMP #636: Standard Update Mobilization
function PopulateChargeCodeTypes(response) {
  if (chargeCodeTypeReq != null) {
    $("#chargeCodeTypeDiv").show();

    $("#ChargeCodeTypeId").empty();

    //Add option label for dropdown
    $("#ChargeCodeTypeId").append($("<option></option>").val('').html(''));
  }
  else {
    $("#ChargeCodeTypeId").empty();
    //Add option label for dropdown
    $("#ChargeCodeTypeId").append($("<option></option>").val('').html(''));
    $("#chargeCodeTypeDiv").hide();
  }

  if (response.length > 0) {
    for (i = 0; i < response.length; i++) {
      $("#ChargeCodeTypeId").append($("<option title='" + response[i].Name + "' ></option>").val(response[i].Id).html(response[i].Name));
    };
    $("#ChargeCodeTypeId").val(chargeCodeTypeId);
  }
}

function CalculateLineTotal() {
  $('#ChargeAmount').attr('readonly', 'readonly');
  CalculateLineTotalWithParams("#ChargeAmount", "#UnitPrice", "#Quantity", "#ScalingFactor");
  // Calculate line net total whenever line total is calculated.
  CalculateLineNetTotal();
  $.watermark.show('#ChargeAmount');
}

function CalculateLineTotalWithParams(lineTotalControl, unitPriceControl, quantityControl, scalingFactorControl) {
  var unitPrice;
  var quantity;
  var scalingFactor;

  if (!isNaN(Number($(unitPriceControl).val())))
    unitPrice = Number($(unitPriceControl).val());
  else
    unitPrice = 0;

  if (!isNaN(Number($(quantityControl).val())))
    quantity = Number($(quantityControl).val());
  else
    quantity = 0;

  if (!isNaN(Number($(scalingFactorControl).val())))
    scalingFactor = Number($(scalingFactorControl).val());
  else
    scalingFactor = 1;

  var lineTotal = (quantity * unitPrice);
  if(scalingFactor != 0 && scalingFactor != undefined)
{
lineTotal = lineTotal  / scalingFactor;
}
  if (!isNaN(lineTotal))
    $(lineTotalControl).val(lineTotal.toFixed(_amountDecimals));
}

function CalculateLineNetTotal() {
  CalculateLineNetTotalWithParams("#TotalNetAmount", "#ChargeAmount", "#TaxAmount", "#VatAmount", "#TotalAddChargeAmount");
}

function CalculateLineNetTotalWithParams(lineNetTotalControl, lineTotalControl, taxAmountControl, vatAmountControl, additionalChargeControl) {
  var lineTotal;
  var tax;
  var vat;
  var additionalCharge;

  if (!isNaN(Number($(lineTotalControl).val())))
    lineTotal = Number($(lineTotalControl).val());
  else
    lineTotal = 0;

  if (!isNaN(Number($(taxAmountControl).val())))
    tax = Number($(taxAmountControl).val());
  else
    tax = 0;

  if (!isNaN(Number($(vatAmountControl).val())))
    vat = Number($(vatAmountControl).val());
  else
    vat = 0;

  if (!isNaN(Number($(additionalChargeControl).val())))
    additionalCharge = Number($(additionalChargeControl).val());
  else
    additionalCharge = 0;

  var lineNetTotal = lineTotal + tax + vat + additionalCharge;
  $(lineNetTotalControl).val(lineNetTotal.toFixed(_amountDecimals));
}

function SetControllAccess(isLineItemDetailExists) {
  if (isLineItemDetailExists) {
    $('#ChargeCodeId').attr('disabled', true);
    $('#POLineItemNumber').attr('readonly', 'readonly');
    $('#Quantity').attr('readonly', 'readonly');
    $('#UomCodeId').attr('disabled', true);
    $('#UnitPrice').attr('readonly', 'readonly');
    $('#ScalingFactor').attr('disabled', true);
    $('#ChargeAmount').attr('readonly', 'readonly');
    $('#MinimumQuantityFlag').attr('disabled', true);
    $('#OriginalLineItemNumber').attr('readonly', 'readonly');
  }
}

function InitializeControlProperties(fieldMetaExists) {  
  if (fieldMetaExists) {
    $('#Quantity').attr('readonly', 'readonly');    
    $('#UnitPrice').attr('readonly', 'readonly');
    $('#MinimumQuantityFlag').removeAttr('checked');
    $('#MinimumQuantityIndicator').hide();
    $('#UomCodeId').attr('disabled', true);
    $('#ScalingFactor').attr('disabled', true);
  }

}

//CMP#502 : [3.5] Rejection Reason for MISC Invoices
function onRejectionReasonCodeChange(selectedValue) {
  // Split selectedValue parameter to retrieve reasonCode
  $('#RejectionReasonCode').val('');
  $('#RejectionReasonCode').val(selectedValue.split('-')[0]);
  $('#RejectionReasonCodeText').val(selectedValue);
}

function onBlankRejectionReasonCode() {
  // Split selectedValue parameter to retrieve reasonCode
  $('#RejectionReasonCode').val('');
  $('#RejectionReasonCodeText').val('');
}

function calculateAmounts() {
  // If Minimum Quantity flag is unchecked, derive amounts.
  if ($('#MinimumQuantityFlag:checked').val() == undefined) {
    CalculateLineTotal();
  }
}