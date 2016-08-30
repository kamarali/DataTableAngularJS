_amountDecimals = 2;
_primeCpn = false;
_FimSourceCode = 14;
$(document).ready(function () {
  // If pageMode == Create/Edit, register control events and validation rules
  if (!$isOnView) {
    $('#ElectronicTicketIndicator').change(function () {
        if ($(this).prop('checked'))
        $("#SettlementAuthorizationCode").removeAttr('readonly');
      else {
        $("#SettlementAuthorizationCode").attr('readonly', true);
        $("#SettlementAuthorizationCode").removeAttr('value');
      }
    });

    $("#IscPercent").blur(function () {
      setPercentage("#IscPercent", "#CouponGrossValueOrApplicableLocalFare", "#IscAmount");
      calculateAmount();
    });

    $('.primaryButton').click(function () {
      calculateAmounts();
    });

    $("#CouponGrossValueOrApplicableLocalFare").blur(function () {
      setPercentage("#IscPercent", "#CouponGrossValueOrApplicableLocalFare", "#IscAmount");
      setPercentage("#OtherCommissionPercent", "#CouponGrossValueOrApplicableLocalFare", "#OtherCommissionAmount");
      setPercentage("#UatpPercent", "#CouponGrossValueOrApplicableLocalFare", "#UatpAmount");
      calculateAmount();
    });

    $('#TicketDocOrFimNumber').change(function () {
      var ticDocNumber = $('#TicketDocOrFimNumber').val();
      if (!isNaN(ticDocNumber))
        $('#TicketDocOrFimNumber').val(ticDocNumber);
    });

    $('#BatchSequenceNumber').change(function () {
      var batchNumber = $('#BatchSequenceNumber').val();
      if (!isNaN(batchNumber))
        $('#BatchSequenceNumber').val(parseInt(batchNumber));
    });

    $('#RecordSequenceWithinBatch').change(function () {
      var recordBatchNumber = $('#RecordSequenceWithinBatch').val();
      if (!isNaN(recordBatchNumber))
        $('#RecordSequenceWithinBatch').val(parseInt(recordBatchNumber));
    });



    $("#HandlingFeeAmount").blur(function () {
      calculateAmount();
    });

    $("#TaxAmount").blur(function () {
      calculateAmount();
    });

    $("#VatAmount").blur(function () {
      calculateAmount();
    });

    $("#OtherCommissionPercent").blur(function () {
      setPercentage("#OtherCommissionPercent", "#CouponGrossValueOrApplicableLocalFare", "#OtherCommissionAmount");
      calculateAmount();
    });

    $("#OtherCommissionAmount").blur(function () {
      calculateAmount();
    });

    $("#UatpPercent").blur(function () {
      setPercentage("#UatpPercent", "#CouponGrossValueOrApplicableLocalFare", "#UatpAmount");
      calculateAmount();
    });

    $("#AgreementIndicatorSupplied").blur(function () {
      var agreementIndSupplied = $("#AgreementIndicatorSupplied").val();
      if (agreementIndSupplied == "") {
        setOriginalPmi("");
        $("#OriginalPmi").attr("readonly", false);
      }
      else if (agreementIndSupplied == "I" || agreementIndSupplied == 'J' || agreementIndSupplied == 'K') {
        setOriginalPmi("N");
        $("#OriginalPmi").attr("readonly", true);
      }
      else if (agreementIndSupplied == 'W' || agreementIndSupplied == 'V' || agreementIndSupplied == 'T') {
        setOriginalPmi("O");
        $("#OriginalPmi").attr("readonly", true);
      }
      else {
        $("#OriginalPmi").attr("readonly", false);
      }

      // Focus is set on ProrateMethodology field to solve IE8 issue which allowed to change originalPMI value even if control has readonly attribute  
      if ($("#OriginalPmi").attr("readonly")) {
        $("#ProrateMethodology").focus();
      }
    });

    $("#AgreementIndicatorSupplied").keyup(function () {
      var indSupp = $("#AgreementIndicatorSupplied").val();
      $("#AgreementIndicatorSupplied").val(indSupp.toUpperCase());
    });

    $("#OriginalPmi").keyup(function () {
      var origPmi = $("#OriginalPmi").val();
      $("#OriginalPmi").val(origPmi.toUpperCase());
    });

    $("#CurrencyAdjustmentIndicator").keyup(function () {
      var currAdjInd = $("#CurrencyAdjustmentIndicator").val();
      $("#CurrencyAdjustmentIndicator").val(currAdjInd.toUpperCase());
    });
  }

  function setOriginalPmi(value) {
    $("#OriginalPmi").val(value);
  }
});

function calculateAmount() {
  calculateNetAmount("#CouponGrossValueOrApplicableLocalFare", "#IscAmount", "#OtherCommissionAmount", "#UatpAmount", "#HandlingFeeAmount", "#TaxAmount", "#VatAmount", "#CouponTotalAmount");
}

function calculateAmounts() {
  calculateAmount();
}

function SetControlAccess() {
  $("#SourceCodeId").attr('readonly', 'true');
  $("#SourceCodeId").autocomplete({ disabled: true });
  $("#ElectronicTicketIndicator").attr('readonly', 'true');
  $("#BatchSequenceNumber").attr('readonly', 'true');
  $("#RecordSequenceWithinBatch").attr('readonly', 'true');
}



/*function setPercentage(sourceControl1Id, sourceControl2Id, targetControlId) {
 var sourceControl1Value = $(sourceControl1Id).val();
  var sourceControl2Value = $(sourceControl2Id).val();
  var percent = sourceControl1Value / 100 * sourceControl2Value;
  if (!isNaN(percent))
      $(targetControlId).val(percent.toFixed(_amountDecimals));
}*/

//SCP:65996 - Error Code 10216 - ISC amount is invalid
//Function is use to calculate percentage for amount.
 function setPercentage(sourceControl1Id, sourceControl2Id, targetControlId) {
    if ($(sourceControl1Id).val() != '' && $(sourceControl2Id).val() != '') {
        var sourceControl1Value = $(sourceControl1Id).val();
        var sourceControl2Value = $(sourceControl2Id).val();
        var percent = (sourceControl2Value * sourceControl1Value) / 100;

        percent = parseFloat(roundNumber(percent, _amountDecimals));
        if (!isNaN(percent))
            $(targetControlId).val(percent);
    }
    else {
        $(targetControlId).val(0.00);
    }
}

//SCP:65996 - Error Code 10216 - ISC amount is invalid
//Function is use to round value
function roundNumber(startValue, digits) {
    var decimalValue = 0;
    startValue = startValue * Math.pow(10, digits + 1);

    // Math.floor only in case of positive value and ignore decimals in case of negative value 
    // Math.floor rounds up the number to the integer closests to zero. 
    // Therefore, Math.floor(-10005.4)  = -100.06 which is logically incorrect as per ISPG standards
    // Thus parseInt(-10005.4) will return -10005 with decimal value 4 and the calculated startValue will be -100.50 here which is logically correct
    if (startValue >= 0) {
        decimalValue = parseInt(Math.floor(startValue) - Math.floor(startValue / 10) * 10);
        startValue = Math.floor(startValue / 10);
    }
    else {
        decimalValue = parseInt(startValue - parseInt(startValue / 10) * 10);
        if (decimalValue < 0)
            decimalValue = -(decimalValue);
        startValue = parseInt(startValue / 10);
    }
    // Add 1 in case of Positive value and subtract 1 in case of Negative value
    if (decimalValue >= 5) {
        if (startValue >= 0) {
            startValue = startValue + 1;
        }
        else {
            startValue = startValue - 1;
        }
    }
    startValue = startValue / parseFloat(Math.pow(10, (digits)));
    return startValue;
}




function calculateNetAmount(sourceControl1, sourceControl2, sourceControl3, sourceControl4, sourceControl5, sourceControl6, sourceControl7, targetControl) {
  var vatAmount;
  var taxAmount;
  var iscAmount;
  var uatpAmount;
  var hfAmount;
  var ocAmount;
  var grossAmount;

  if (!isNaN(Number($(sourceControl1).val())))
    grossAmount = Number($(sourceControl1).val());
  else
    grossAmount = 0;

  if (!isNaN(Number($(sourceControl2).val())))
    iscAmount = Number($(sourceControl2).val());
  else
    iscAmount = 0;

  if (!isNaN(Number($(sourceControl3).val())))
    ocAmount = Number($(sourceControl3).val());
  else
    ocAmount = 0;

  if (!isNaN(Number($(sourceControl4).val())))
    uatpAmount = Number($(sourceControl4).val());
  else
    uatpAmount = 0;

  if (!isNaN(Number($(sourceControl5).val())))
    hfAmount = Number($(sourceControl5).val());
  else
    hfAmount = 0;

  if (!isNaN(Number($(sourceControl6).val())))
    taxAmount = Number($(sourceControl6).val());
  else
    taxAmount = 0;

  if (!isNaN(Number($(sourceControl7).val())))
    vatAmount = Number($(sourceControl7).val());
  else
    vatAmount = 0; 
  
  var couponTotalAmount = grossAmount + iscAmount + ocAmount + uatpAmount + hfAmount + taxAmount + vatAmount;
  if (!isNaN(couponTotalAmount))
    $(targetControl).val(couponTotalAmount.toFixed(_amountDecimals));  
}

function resetForm() {
  $(':input', '#primeBillingDetailsForm')
    .not(':button, :submit, :reset, :hidden')
    .val('')
    .removeAttr('selected');
}

function SourceCodeId_SetAutocompleteDisplay(item) {
  var sourceCode = item.SourceCodeName + '-' + item.SourceCodeDescription;
  return { label: sourceCode, value: item.SourceCodeName, id: item.Id };
}

function ReasonCodeId_SetAutocompleteDisplay(item) {
  var reasonCode = item.Code + '-' + item.Description;
  return { label: reasonCode, value: item.Code, id: item.Id };
}


function InitializeCoupon(e) {
   
    if (e == "PrimeCpn") {//CMP #672: Validation on Taxes in PAX FIM Billings
        _primeCpn = true;
        if ($("#SourceCodeId").val() == _FimSourceCode) {
            $("#EnabletaxAmountLink").hide();
            $("#DisabletaxAmountLink").html('Tax Breakdown');
            $("#DisabletaxAmountLink").show();
        }
        else {
            $("#EnabletaxAmountLink").show();
            $("#DisabletaxAmountLink").hide();
        }
    }
   
  SetPageWaterMark();
  $("#FlightDate").watermark(_dateWatermark);
  // If pageMode == Create/Edit, then only set validation rules
  if (!$isOnView) {
      $("#primeBillingDetails").validate({
          rules: {
              BatchSequenceNumber: "required",
              RecordSequenceWithinBatch: "required",
              TicketOrFimIssuingAirline: "required",
              TicketDocOrFimNumber: "required",
              TicketOrFimCouponNumber: { required: true, ValidTicketOrFimCouponNumber: true },
              SourceCodeId: "required",
              CurrencyAdjustmentIndicator: "required",
              FromAirportOfCoupon: {
                  required: function (element) {
                      var sourceCodeValue = $("#SourceCodeId").val();
                      if (sourceCodeValue == 1 || sourceCodeValue == 14) {
                          return true;
                      }
                      else {
                          return false;
                      }
                  }
              },
              ToAirportOfCoupon: {
                  required: function (element) {
                      var sourceCodeValue = $("#SourceCodeId").val();
                      if (sourceCodeValue == 1 || sourceCodeValue == 14) {
                          return true;
                      }
                      else {
                          return false;
                      }
                  }
              },
              FlightNumber: {
                  required: function (element) {
                      var sourceCodeValue = $("#SourceCodeId").val();
                      if (sourceCodeValue == 1 || sourceCodeValue == 14) {
                          return true;
                      }
                      else {
                          return false;
                      }
                  }
              },
              FlightDate: {
                  required: function (element) {
                      var sourceCodeValue = $("#SourceCodeId").val();
                      if (sourceCodeValue == 1 || sourceCodeValue == 14) {
                          return true;
                      }
                      else {
                          return false;
                      }
                  }
              },
              AirlineFlightDesignator: {
                  required: function (element) {
                      var sourceCodeValue = $("#SourceCodeId").val();
                      if (sourceCodeValue == 1 || sourceCodeValue == 14) {
                          return true;
                      }
                      else {
                          return false;
                      }
                  }
              },
              CheckDigit: "required",
              HandlingFeeAmount: {
                  required: function (element) {
                      var value = $("#HandlingFeeType").val();
                      if (!value) {
                          return false;
                      } else {
                          return true;
                      }
                  }
              },
              AgreementIndicatorSupplied: { ValidOriginalPmi: true }
          },
          messages: {
              BatchSequenceNumber: "Valid Batch Number Required",
              RecordSequenceWithinBatch: "Valid Sequence Number Required",
              TicketOrFimIssuingAirline: "Issuing Airline Required",
              TicketDocOrFimNumber: "Valid Ticket/FIM Number Required",
              SourceCodeId: "Source Code Required",
              FromAirportOfCoupon: "From Airport Required for SourceCode 1 and 14",
              ToAirportOfCoupon: "To Airport Required for SourceCode 1 and 14",
              HandlingFeeAmount: "Handling Fee Amount Not Valid",
              FlightDate: "Flight Date Required for SourceCode 1 and 14",
              FlightNumber: "Flight Number Required for SourceCode 1 and 14",
              AirlineFlightDesignator: "Airline Flight Designator Required for SourceCode 1 and 14",
              CurrencyAdjustmentIndicator: "Currency Adjustment Indicator Required"
          },
          submitHandler: function (form) {

              //Validate Source Code from Prime Coupon, If FIM source code and Tax Breakdown exist then show message.  
              //CMP #672: Validation on Taxes in PAX FIM Billings
              var rowCount = $('#taxGrid tr').length - 1;
              var sourceCode = $("#SourceCodeId").val();

              if (rowCount > 0 && sourceCode == 14) {
                  $('#promtUser').dialog({ open: function () {
                      $(this).closest('.ui-dialog').find('.ui-dialog-buttonpane button')[1].focus();
                  },
                      closeOnEscape: false,
                      height: 205, width: 300, modal: true,
                      buttons: {
                          "OK": function () {
                              $(this).dialog("close");
                              onSubmitHandler(form);
                          },
                          Cancel: function () {
                              $(this).dialog("close");
                          }
                      },
                      resizable: false
                  }
                );
              }
              else {
                  onSubmitHandler(form);
              }
          }
      });

    trackFormChanges('primeBillingDetails');
  } // end if()

  // Initialize Conditional validation for TicketOrFimCouponNumber field
  conditionallyValidateTicketOrFimCouponNumber()

  // Initialize Conditional validation for OriginalPmi field
  conditionallyValidateOriginalPmi()
}

// Following function is executed when SourceCode is selected or modified
function onSourceCodeChange(sourceCodeId) {
  // Check E-Ticket Indicator if selected sourceCode value is present in array below
  var sourceCodeList = new Array("1", "2", "3", "23", "27", "90");
 
  //CMP #672: Validation on Taxes in PAX FIM Billings
  if (sourceCodeId == _FimSourceCode && _primeCpn == true) {
      $("#EnabletaxAmountLink").hide();
      $("#DisabletaxAmountLink").html('Tax Breakdown');
      $("#DisabletaxAmountLink").show();
  }
  else {
      $("#EnabletaxAmountLink").show();
      $("#DisabletaxAmountLink").hide();
  }
  // If sourceCode exists in above array check E-Ticket indicator checkbox, else uncheck
  if($.inArray(sourceCodeId, sourceCodeList) >= 0) {
    $("#ElectronicTicketIndicator").prop("checked", true);
  }
  else {
      $("#ElectronicTicketIndicator").prop("checked", false);
  }
} // end onSourceCodeChange()




function conditionallyValidateTicketOrFimCouponNumber() {
  // Following code is used for Conditional validation of TicketOrFimCouponNumber field
  $.validator.addMethod("ValidTicketOrFimCouponNumber", function (value, element) {
    if (this.optional(element)) // return true on optional element 
      return true;

    // Get value selected in TicketOrFimCouponNumber dropdown
    var TicketOrFimCouponNumber = value;
    // Initially set valid variable to true
    valid = true;
    // If SourceCode selected is not "14" and TicketOrFimCouponNumber selected is "9" throw validation message
    if ($("#SourceCodeId").val() != 14 && TicketOrFimCouponNumber == 9) {
      valid = false;
    }

    // return
    return valid;
  }, 'Coupon Number can be 9 only if Source Code selected is 14');
}

function conditionallyValidateOriginalPmi() {
  // Following code is used for Conditional validation of OriginalPmi field
  $.validator.addMethod("ValidOriginalPmi", function (value, element) {
    if (this.optional(element)) // return true on optional element 
      return true;

    // Get value added in OriginalPmi textbox
    var agreementIndicatorSupplied = value;
    // Initially set valid variable to false
    valid = false;
    // Get AgreementIndicatorSupplied field value
    var OriginalPmi = $("#OriginalPmi").val();
    // If AgreementIndicatorSupplied field value has length of 2 characters, Original Pmi field should have value of length 1 character 
     if (agreementIndicatorSupplied.length == 2 && OriginalPmi.length == 1) {
      valid = true;
    }
    else if (agreementIndicatorSupplied == "I" || agreementIndicatorSupplied == "J" || agreementIndicatorSupplied == "K" || agreementIndicatorSupplied == "W" || agreementIndicatorSupplied == "V" || agreementIndicatorSupplied == "T") {
      valid = true;
    }

    // return
    return valid;
  }, 'Invalid Agreement Indicator and Original PMI combination');
}
 