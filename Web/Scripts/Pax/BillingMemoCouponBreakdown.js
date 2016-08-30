_amountDecimals = 2;

$(document).ready(function () {
  SetPageWaterMark();
  $("#FlightDate").watermark(_dateWatermark);
  // Following condition registers control events and validation rules if pageMode is Create or Edit and not for View mode
  if (!$isOnView) {
    $("#billingMemoCouponBreakdownDetails").validate({
      rules: {
        TicketOrFimIssuingAirline: "required",
        TicketDocOrFimNumber: "required",
        TicketOrFimCouponNumber: "required",
        CheckDigit: "required",
        GrossAmountBilled: "required",
        CurrencyAdjustmentIndicator : "required"
      },
      messages: {
        TicketOrFimIssuingAirline: "Ticket Issuing Airline Required",
        TicketDocOrFimNumber: "Document Number Required",
        TicketOrFimCouponNumber: "Coupon Number Required",
        TicketDocOrFimNumber: "Valid Ticket/FIM Number Required",
        CheckDigit: { required: "Check Digit Required" },
        GrossAmountBilled: "Value is required and should be positive or 0",
        IscPercent: "Value should be between -99.999 and 99.999",
        UatpPercent: "Value should be between -99.999 and 99.999",
        OtherCommissionPercent: "Value should be between -99.999 and 99.999",
        NetAmountBilled: "Net Billed Amount Required and should be positive or 0",
        CurrencyAdjustmentIndicator: "Currency Adjustment Indicator value Required"
      }
    });

    trackFormChanges('billingMemoCouponBreakdownDetails');

    $('#TicketDocOrFimNumber').change(function () {
      var ticDocNumber = $('#TicketDocOrFimNumber').val();
      if (!isNaN(ticDocNumber))
        $('#TicketDocOrFimNumber').val(ticDocNumber);
    });

    // Calculate amounts on click of Save. Done to fix amount derivation issues on slow machines.
    $('.primaryButton').click(function () {
      if ($("#SourceCodeId").val() != 94) {
        setPercentage("#IscPercent", "#GrossAmountBilled", "#IscAmountBilled", _amountDecimals);
        setPercentage("#UatpPercent", "#GrossAmountBilled", "#UatpAmountBilled", _amountDecimals);
    }
      calculateAmount();
    });

    $("#IscPercent").blur(function () {
      setPercentage("#IscPercent", "#GrossAmountBilled", "#IscAmountBilled", 2);
      calculateNetAmount("#GrossAmountBilled", "#IscAmountBilled", "#OtherCommissionBilled", "#UatpAmountBilled", "#HandlingFeeAmount", "#TaxAmount", "#VatAmount", "#NetAmountBilled");
    });

    $("#IscAmountBilled").blur(function () {
      calculateAmount();
    });

    $("#GrossAmountBilled").blur(function () {
      setPercentage("#IscPercent", "#GrossAmountBilled", "#IscAmountBilled", 2);
      setPercentage("#OtherCommissionPercent", "#GrossAmountBilled", "#OtherCommissionBilled", 2);
      setPercentage("#UatpPercent", "#GrossAmountBilled", "#UatpAmountBilled", 2);
      calculateAmount();
    });

    $("#HandlingFeeAmount").blur(function () {
      calculateAmount();
    });

    $("#OtherCommissionPercent").blur(function () {
      setPercentage("#OtherCommissionPercent", "#GrossAmountBilled", "#OtherCommissionBilled", 2);
      calculateAmount();
    });

    $("#OtherCommissionBilled").blur(function () {
      calculateAmount();
    });

    $("#UatpPercent").blur(function () {
      setPercentage("#UatpPercent", "#GrossAmountBilled", "#UatpAmountBilled", 2);
      calculateAmount();
    });

    $("#UatpAmountBilled").blur(function () {
      calculateAmount();
    });

    $("#TaxAmount").blur(function () {
      calculateAmount();
    });

    $("#VatAmount").blur(function () {
      calculateAmount();
    });

    $("#ProrateSlipDetails").bind("keypress", function () { maxLength(this, 4000) });
    $("#ProrateSlipDetails").bind("paste", function () { maxLengthPaste(this, 4000) });

    enableDisableControlOnCheck("#ElectronicTicketIndicator", "#SettlementAuthorizationCode");
  }// end if()

//  var indicatorChkBox = document.getElementById("ElectronicTicketIndicator");
//  if (indicatorChkBox != null)
//    indicatorChkBox.checked = true;
});

function calculateAmount() {
  calculateNetAmount("#GrossAmountBilled", "#IscAmountBilled", "#OtherCommissionBilled", "#UatpAmountBilled", "#HandlingFeeAmount", "#TaxAmount", "#VatAmount", "#NetAmountBilled");
}

$("#CurrencyAdjustmentIndicator").keyup(function () {
    var currAdjInd = $("#CurrencyAdjustmentIndicator").val();
    $("#CurrencyAdjustmentIndicator").val(currAdjInd.toUpperCase());
});

/*function setPercentage(sourceControl1Id, sourceControl2Id, targetControlId, decimalPlaces) {
  var sourceControl1Value = $(sourceControl1Id).val();
  var sourceControl2Value = $(sourceControl2Id).val();
  var percent = sourceControl1Value / 100 * sourceControl2Value;
  if (!isNaN(percent))
    $(targetControlId).val(percent.toFixed(decimalPlaces));
}*/
//SCP:53676 - Error - Incorrect ISC Amount for BM with Reason Code 8E 
//Function is use to calculate percentage for amount. 
function setPercentage(sourceControl1Id, sourceControl2Id, targetControlId, decimalPlaces) {
if ($(sourceControl1Id).val() != '' && $(sourceControl2Id).val() != '') {
var sourceControl1Value = $(sourceControl1Id).val();
  var sourceControl2Value = $(sourceControl2Id).val();
  var percent = (sourceControl2Value * sourceControl1Value) / 100;

percent = parseFloat(roundNumber(percent, decimalPlaces));
        if (!isNaN(percent))
            $(targetControlId).val(percent);
    }
    else {
        $(targetControlId).val(0.00);
    }
}

//SCP:53676 - Error - Incorrect ISC Amount for BM with Reason Code 8E 
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
    if(startValue >= 0)
    {      
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
  var vatAmount = 0;
  var taxAmount = 0;
  var iscAmount = 0;
  var uatpAmount = 0;
  var hfAmount = 0;
  var ocAmount = 0;
  var grossAmount = 0;

  if (!isNaN(Number($(sourceControl1).val())))
    grossAmount = Number($(sourceControl1).val());  

  if (!isNaN(Number($(sourceControl2).val())))
    iscAmount = Number($(sourceControl2).val());  

  if (!isNaN(Number($(sourceControl3).val())))
    ocAmount = Number($(sourceControl3).val());  

  if (!isNaN(Number($(sourceControl4).val())))
    uatpAmount = Number($(sourceControl4).val());  

  if (!isNaN(Number($(sourceControl5).val())))
    hfAmount = Number($(sourceControl5).val());  

  if (!isNaN(Number($(sourceControl6).val())))
    taxAmount = Number($(sourceControl6).val());  

  if (!isNaN(Number($(sourceControl7).val())))
    vatAmount = Number($(sourceControl7).val());  

  var couponTotalAmount = grossAmount + iscAmount + ocAmount + uatpAmount + hfAmount + taxAmount + vatAmount;
  
  if (!isNaN(couponTotalAmount))
    $(targetControl).val(couponTotalAmount.toFixed(_amountDecimals));
}

function enableDisableControlOnCheck(checkBoxId, textBoxId) {
  $(checkBoxId).change(function () {
      if ($(this).prop('checked') == true) {
      $(textBoxId).removeAttr('disabled');
      $(textBoxId).attr('enabled', 'enabled');
      $(textBoxId).removeAttr("readonly");
    }
    else {
      $(textBoxId).val("");
      $(textBoxId).attr('disabled', 'disabled');
      $(textBoxId).attr('readonly', 'true');
    }
  });
}
