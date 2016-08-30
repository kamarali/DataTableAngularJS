/// <reference path="jquery.watermark.min.js" />
_amountDecimals = 2;
var isCouponBreakdownExists = false;
var isOnCouponPage = false;

$(document).ready(function () {
  
  SetPageWaterMark();
  $("#FlightDate").watermark(_dateWatermark);
  // Following function removes readOnly attribute for text fields within memo amount div tag if isCouponBreakdownmandatory == 'False' and breakdownExists == 'False'
  function EnableDisableAmountFieldsOnreasonCode(isCouponBreakdownmandatory, breakdownExists) {
    // If couponBreakdown is not mandatory and coupon does not exists for current memo, enable selected Memo amount fields
    if (isCouponBreakdownmandatory == 'False' && breakdownExists == 'False') {
      // Remove readOnly attribute of all text fields from div tag with "AmountFieldsDiv1" class
      $('.AmountFieldsDiv1 input[type=text]').removeAttr("readOnly");

      // Remove readOnly attribute of all text fields from div tag with "AmountFieldsDiv2" class
      $('.AmountFieldsDiv2 input[type=text]').not(id = '#NetAmountBilled').removeAttr("readOnly");

      // Enable 'VatBreakdown' link
      $('#vatBreakdown').attr("disabled", false);
    }
    else {
      $('#vatBreakdown').attr("disabled", true);
      $('.AmountFieldsDiv1 input[type=text]').attr("readOnly", true);
      $('.AmountFieldsDiv2 input[type=text]').attr("readOnly", true);
    }
  }

  // If pagemode is "Edit" and if Creditmemo is being edited, call EnableDisableAmountFieldsOnreasonCode() function which will remove readonly attribute of memo amount fields within Div tags if 
  // couponBreakdownMandatory == 'False' and couponBreakdown does not exists
  if (pageMode == 'Edit' && editLevel == 'CreditMemoEdit') {
    EnableDisableAmountFieldsOnreasonCode($('#CouponAwbBreakdownMandatory').val(), couponBreakdownExists);
  }
  
  $('#TicketDocOrFimNumber').change(function () {
    var ticDocNumber = $('#TicketDocOrFimNumber').val();
    if (!isNaN(ticDocNumber))
      $('#TicketDocOrFimNumber').val(ticDocNumber);
  });
//below code commet because 'ElectronicTicketIndicator' always showing checked.
//  $('#ElectronicTicketIndicator').attr('checked', 'checked');

  $('#ElectronicTicketIndicator').change(function () {
      if ($(this).prop('checked')) {
      $('#SettlementAuthorizationCode').removeAttr('readOnly');
    }
    else {
      $('#SettlementAuthorizationCode').attr('readOnly', 'readOnly');
      $('#SettlementAuthorizationCode').val('');
    }
  });

  $("#IscPercent").blur(function () {
    setPercentage("#IscPercent", "#GrossAmountCredited", "#IscAmountBilled");
    calculateNetCouponAmount();
  });

  $("#GrossAmountCredited").blur(function () {
    setPercentage("#IscPercent", "#GrossAmountCredited", "#IscAmountBilled");
    setPercentage("#OtherCommissionPercent", "#GrossAmountCredited", "#OtherCommissionBilled");
    setPercentage("#UatpPercent", "#GrossAmountCredited", "#UatpAmountBilled");
    calculateNetCouponAmount();
  });

  $("#HandlingFeeAmount").blur(function () {
    calculateNetCouponAmount();
  });

  $("#OtherCommissionPercent").blur(function () {
    setPercentage("#OtherCommissionPercent", "#GrossAmountCredited", "#OtherCommissionBilled");
    calculateNetCouponAmount();
  });

  $("#OtherCommissionBilled").blur(function () {
    calculateNetCouponAmount();
  });

  $("#UatpPercent").blur(function () {
    setPercentage("#UatpPercent", "#GrossAmountCredited", "#UatpAmountBilled");
    calculateNetCouponAmount();
  });

  $("#TaxAmount").blur(function () {
    calculateNetCouponAmount();
  });

  $("#VatAmount").blur(function () {
    calculateNetCouponAmount();
  });

  $("#btnSaveAndAddNew, #btnSaveAndDuplicate, #SaveAndBackToOverview").click(function () {  
    setPercentage("#IscPercent", "#GrossAmountCredited", "#IscAmountBilled");
    setPercentage("#UatpPercent", "#GrossAmountCredited", "#UatpAmountBilled");
    calculateNetCouponAmount();
  });

  function calculateNetCouponAmount() {
    calculateNetAmount("#GrossAmountCredited", "#IscAmountBilled", "#OtherCommissionBilled", "#UatpAmountBilled", "#HandlingFeeAmount", "#TaxAmount", "#VatAmount", "#NetAmountCredited");
  }

  /*function setPercentage(sourceControl1Id, sourceControl2Id, targetControlId) {
    var sourceControl1Value = $(sourceControl1Id).val();
    var sourceControl2Value = $(sourceControl2Id).val();
    var percent = sourceControl1Value / 100 * sourceControl2Value;
    if (!isNaN(percent))
      $(targetControlId).val(percent.toFixed(_amountDecimals));
  }*/

//SCP:53676 - Error - Incorrect ISC Amount for BM with Reason Code 8E 
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

  $("#CurrencyAdjustmentIndicator").keyup(function () {
    var currAdjInd = $("#CurrencyAdjustmentIndicator").val();
    $("#CurrencyAdjustmentIndicator").val(currAdjInd.toUpperCase());
  });

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
    if (!isNaN(couponTotalAmount)) {
      if (couponTotalAmount == 0)
        $(targetControl).watermark();
      else
        $(targetControl).val(couponTotalAmount.toFixed(_amountDecimals));
    }
  }
  /*Auto derivations end*/

  function isValueRequired(control1Id, control2Id, control3Id) {
    if ($(control1Id).val() != '' || $(control2Id).val() != '' || $(control3Id).val() != '')
      return true;
    else
      return false;
  }

  $("#creditMemoCouponForm").validate({
    rules: {
      TicketOrFimIssuingAirline: "required",
      TicketDocOrFimNumber: "required",
      TicketOrFimCouponNumber: "required",
      CheckDigit: "required",
      AttachmentIndicatorOriginal: "required",
      GrossAmountCredited: "required",
      CurrencyAdjustmentIndicator: "required"
    },
    messages: {
      TicketOrFimIssuingAirline: "Ticket Issuing Airline Required",
      TicketDocOrFimNumber: "Valid Ticket Number Required",
      TicketOrFimCouponNumber: "Coupon Number required",
      CheckDigit: { required: "Check Digit Required" },
      NetAmountCredited: "Net Credited should be negative or 0",
      AttachmentIndicatorOriginal: "Attachment Indicator Required",
      IscPercent: "Value should be between -99.999 and 99.999",
      GrossAmountCredited: "Value is required and should be negative or 0",
      OtherCommissionPercent: "Value should be between -99.999 and 99.999",
      UatpPercent: "Value should be between -99.999 and 99.999",
      CurrencyAdjustmentIndicator: "Currency Adjustment Indicator value Required"
    }
  });

  trackFormChanges('creditMemoCouponForm');

  $("#ProrateSlipDetails").bind("keypress", function () { maxLength(this, 4000) });
  $("#ProrateSlipDetails").bind("paste", function () { maxLengthPaste(this, 4000) });

});

//To be updated once the functionality is done - Batch and sequence numbers will be non-editable in Create, Edit
function setControlAccess(isCouponExists) {
  isCouponBreakdownExists = isCouponExists;
  $('#BatchSequenceNumber').attr('readOnly', 'readOnly');
  $('#RecordSequenceWithinBatch').attr('readOnly', 'readOnly');

  if (isCouponExists == 'True') {
    $('#YourInvoiceNumber').attr('readOnly', 'readOnly');
    $('#YourInvoiceBillingYear').attr('disabled', 'disabled');
    $('#YourInvoiceBillingMonth').attr('disabled', 'disabled');
    $('#YourInvoiceBillingPeriod').attr('disabled', 'disabled');
    $('#CorrespondenceRefNumber').attr('readOnly', 'readOnly');
    $('#FimNumber').attr('readOnly', 'readOnly');
    $('#FimCouponNumber').attr('readOnly', 'readOnly');
  }
}

// Following function is executed when ReasonCode is modified and is used to enable disable CreditMemo amount fields at Memo level depending on ReasonCode.
// "selectedValue" parameter contains ReasonCode-IsCouponBreakdownMandatory value 
function onReasonCodeChangeInCreateMode(selectedValue) {
  // Split selectedValue parameter to retrieve reasonCode 
  $('#ReasonCode').val(selectedValue.split('-')[0]);
  // Split selectedValue parameter to retrieve value whether Coupon breakdown is mandatory for selected reasonCode 
  isCouponBreakdownMandatory = selectedValue.split('-')[1];

  // If selected ReasonCode does not mandate coupon breakdown, remove readOnly attribute of selected Memo amount fields, else keep it disabled 
  if (isCouponBreakdownMandatory == "False") {
    // Remove readOnly attribute of all text fields from div tag with "AmountFieldsDiv1" class
    $('.AmountFieldsDiv1 input[type=text]').removeAttr("readOnly");

    // Remove readOnly attribute of all text fields from div tag with "AmountFieldsDiv2" class
    $('.AmountFieldsDiv2 input[type=text]').not(id = '#NetAmountCredited').not(id = '#VatAmount').removeAttr("readOnly");

    // Enable 'VatBreakdown' link
    $('#vatBreakdown').attr("disabled", false);

    // Set 'CouponAwbBreakdownMandatory' hidden field value
    $('#CouponAwbBreakdownMandatory').val(isCouponBreakdownMandatory);
  }
  else {
    // If reasonCode mandates Coupon Breakdown, clear all Memo amount text fields and add readOnly attribute to text fields   
    $('#vatBreakdown').attr("disabled", true);
    $('.AmountFieldsDiv1 input[type=text]').attr("readOnly", true);
    $('.AmountFieldsDiv2 input[type=text]').attr("readOnly", true);

    // Set 'CouponAwbBreakdownMandatory' hidden field value
    $('#CouponAwbBreakdownMandatory').val(isCouponBreakdownMandatory);
  }
} // end onReasonCodeChange()

// Following function keeps CreditMemo Amount fields disabled, if ReasonCode field is blank 
function onBlankReasonCode() {
  $('#vatBreakdown').attr("disabled", true);
  $('.AmountFieldsDiv1 input[type=text]').attr("readOnly", true);
  $('.AmountFieldsDiv2 input[type=text]').attr("readOnly", true);
} // end onBlankReasonCode()

// Following function is executed when ReasonCode is changed. (selectedValue parameter value contains "ReasonCode-CouponBreakdownMandatoryValue") 
function onReasonCodeChangeInEditMode(selectedValue) {
  // Get Previous breakdown mandatory value. i.e. before changing ReasonCode
  previousBreakdownMandatoryValue = $('#CouponAwbBreakdownMandatory').val();

  // Split selectedValue parameter to retrieve value whether Coupon breakdown is mandatory for selected reasonCode 
  isCouponBreakdownMandatory = selectedValue.split('-')[1];

  // If previous reasonCode was not mandating CouponBreakdown, new reasonCode mandates couponBreakdown and CouponBreakdown does not exist for given CreditMemo
  // popup an Confirm box telling user that previously captured Memo amounts will now be derived from CouponBreakdown 
  if (previousBreakdownMandatoryValue == 'False' && isCouponBreakdownMandatory == 'True' && couponBreakdownExists == 'False') {

    // Split selectedValue parameter to retrieve reasonCode 
    $('#ReasonCode').val(selectedValue.split('-')[0]);

    // Set Coupon Breakdown field value
    $('#CouponAwbBreakdownMandatory').val(isCouponBreakdownMandatory);

    // If user continues set "ReadOnly" attribute to text fields
    $('#vatBreakdown').attr("disabled", true);
    $('.AmountFieldsDiv1 input[type=text]').attr("readOnly", true);
    $('.AmountFieldsDiv2 input[type=text]').attr("readOnly", true);
  }
  // If previous reasonCode was mandating CouponBreakdown, new reasonCode does not mandate couponBreakdown and CouponBreakdown does not exist for given CreditMemo,
  // Remove ReadOnly attribute of all memo amount text fields except for "NetAmountCredited" textbox 

  else if (previousBreakdownMandatoryValue == 'True' && isCouponBreakdownMandatory == 'False' && couponBreakdownExists == 'False') {
    // Split selectedValue parameter to retrieve reasonCode 
    $('#ReasonCode').val(selectedValue.split('-')[0]);

    // Remove readOnly attribute of all text fields from div tag with "AmountFieldsDiv1" class
    $('.AmountFieldsDiv1 input[type=text]').removeAttr("readOnly");

    // Remove readOnly attribute of all text fields from div tag with "AmountFieldsDiv2" class
    $('.AmountFieldsDiv2 input[type=text]').not(id = '#NetAmountCredited').not(id = '#VatAmount').removeAttr("readOnly");

    // Enable 'VatBreakdown' link
    $('#vatBreakdown').attr("disabled", false);

    // Set 'CouponAwbBreakdownMandatory' hidden field value
    $('#CouponAwbBreakdownMandatory').val(isCouponBreakdownMandatory);
  }
  // If previous reasonCode was not mandating CouponBreakdown, new reasonCode also does not mandate couponBreakdown and CouponBreakdown does not exist for given CreditMemo
  // only set ReasonCode and CouponAwbBreakdownMandatory fields value
  else if (previousBreakdownMandatoryValue == 'False' && isCouponBreakdownMandatory == 'False' && couponBreakdownExists == 'False') {
    // Split selectedValue parameter to retrieve reasonCode 
    $('#ReasonCode').val(selectedValue.split('-')[0]);

    // Set 'CouponAwbBreakdownMandatory' hidden field value
    $('#CouponAwbBreakdownMandatory').val(isCouponBreakdownMandatory);
  }
  else {
    // Split selectedValue parameter to retrieve reasonCode 
    $('#ReasonCode').val(selectedValue.split('-')[0]);

    // Set Coupon Breakdown field value
    $('#CouponAwbBreakdownMandatory').val(isCouponBreakdownMandatory);
  }
}


