var chargeCodeTypeUrl;
var lineItemDetailsUrl;
var invoiceType;
_amountDecimals = 3;

function InitialiseLineItem(chargeCodeTypeActionUrl, lineItemDetailsActionUrl) {
  // UOM code should be disabled and defaulted to EA for UATP. (UATP Web Review Issue)
    $('#UomCodeId').attr('disabled', true);
    chargeCodeTypeUrl = chargeCodeTypeActionUrl;
    lineItemDetailsUrl = lineItemDetailsActionUrl;
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
                POLineItemNumber: {
                    validatePOLineItemNo: true
                },
                StartDate: { compareServiceDates: true },                
                UnitPrice: "required",
                Quantity: "required",
                //        ChargeCodeTypeId: {
                //          required: function (element) {
                //            if ($("#chargeCodeTypeDiv").css("display") == "block")
                //              return true;
                //            else
                //              return false;
                //          }
                //        },
                ChargeAmount: { required: function () {
                    return $('#MinimumQuantityFlag:checked').val() != undefined;
                }
                }
            },
            messages: {
                ChargeCodeId: "Charge Code Required",
                //ChargeCodeTypeId: "Charge Code Type Required",
                Description: "Description Required and should be of maximum 240 characters",
                EndDate: "End Date Required",                
                UnitPrice: "Enter a valid unit price (dot should be used as a decimal separator).",
                Quantity: "Quantity Required and should be within 1 and 99999999999999.9999",
                ChargeAmount: "Line Total is required and  should be within -99999999999999.999 and 99999999999999.999",
                TotalNetAmount: "Line Net Total should be within -99999999999999.999 and 99999999999999.999",
                TotalTaxAmount: "Tax Amount should be within -99999999999999.999 and 99999999999999.999",
                TotalVatAmount: "Vat Amount should be within -99999999999999.999 and 99999999999999.999",
                TotalAddOnChargeAmount: "Add On Charge Amount should be within -99999999999999.999 and 99999999999999.999"
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

        // Commented as per UATP IS-Web Review.
//        $("#ScalingFactor").blur(function () {
//            if ($('#MinimumQuantityFlag:checked').val() == undefined) {
//                CalculateLineTotal();
//            }
//        });


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
        $("#ChargeCodeTypeId").bind("change", OnChargeCodeTypeChange);
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

function OnChargeCodeTypeChange() {

    // If invoice type is Credit Note, Rejection or Correspondence then no need to check for line item details are expected or not.  
    if (invoiceType != 2 && invoiceType != 3 && invoiceType != 4) {

        var selectedChargeCodeId = $("#ChargeCodeId").val() ? $("#ChargeCodeId").val() : 0;
        var selectedChargeCodeTypeId = $("#ChargeCodeTypeId").val() ? $("#ChargeCodeTypeId").val() : 0;

        $.ajax({
            type: "Post",
            url: lineItemDetailsUrl,
            data: { chargeCodeId: selectedChargeCodeId, chargeCodeTypeId: selectedChargeCodeTypeId },
            dataType: "json",
            success: SetControlProperties,
            failure: function (response) {
                $("#Quantity").removeAttr('readonly');
                $("#UnitPrice").removeAttr('readonly');
            }
        });

    }
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
        $('#ScalingFactor').attr('readonly', true);
        CalculateLineTotal();
    }
    else {
        $("#Quantity").removeAttr('readonly');
        $('#Quantity').val("1");
        $('#UnitPrice').val("0.0000");
        $("#UnitPrice").removeAttr('readonly');
        $('#MinimumQuantityIndicator').show();       
        // Commented as per UATP IS-Web Review.
        //$('#ScalingFactor').attr('readonly', false);
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

function OnChargeCodeChange() {
    var selectedChargeCodeId = $("#ChargeCodeId").val();
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

function PopulateChargeCodeTypes(response) {
    if (response.length > 0) {
        $("#chargeCodeTypeDiv").show();

        $("#ChargeCodeTypeId").empty();

        //Add option label for dropdown
        $("#ChargeCodeTypeId").append($("<option></option>").val('').html('Please Select'));
        for (i = 0; i < response.length; i++) {
            $("#ChargeCodeTypeId").append($("<option title='" + response[i].Name + "' ></option>").val(response[i].Id).html(response[i].Name));
        };

        OnChargeCodeTypeChange();
    }
    else {
        $("#ChargeCodeTypeId").empty();
        $("#chargeCodeTypeDiv").hide();
        OnChargeCodeTypeChange();
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
    if (scalingFactor != 0 && scalingFactor != undefined) {
        lineTotal = lineTotal / scalingFactor;
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
        $('#ChargeCodeTypeId').attr('disabled', true);
        $('#POLineItemNumber').attr('readonly', 'readonly');
        $('#Quantity').attr('readonly', 'readonly');
        $('#UomCodeId').attr('disabled', true);
        $('#UnitPrice').attr('readonly', 'readonly');
        $('#ScalingFactor').attr('readonly', 'readonly');
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
        $('#ScalingFactor').attr('readonly', true);
    }

}

function calculateAmounts() {
    // If Minimum Quantity flag is unchecked, derive amounts.
    if ($('#MinimumQuantityFlag:checked').val() == undefined) {
        CalculateLineTotal();
    }
}
