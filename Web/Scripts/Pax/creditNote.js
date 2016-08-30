/// <reference path="jquery.watermark.min.js" />
_amountDecimals = 2;
var isCouponBreakdownExists = false;
var isOnCouponPage = false;

$(document).ready(function () {
  
  $("#FlightDate").watermark("DD-MM");
  SetPageWaterMark();

  // Following function removes readOnly attribute for text fields within memo amount div tag if isCouponBreakdownmandatory == 'False' and breakdownExists == 'False'
  function EnableDisableAmountFieldsOnreasonCode(isCouponBreakdownmandatory, breakdownExists) {
    // If couponBreakdown is not mandatory and coupon does not exists for current memo, enable selected Memo amount fields
    if (isCouponBreakdownmandatory == 'False' && breakdownExists == 'False') {
      // Remove readOnly attribute of all text fields from div tag with "AmountFieldsDiv1" class
      $('.AmountFieldsDiv1 input[type=text]').removeAttr("readOnly");

      // Remove readOnly attribute of all text fields from div tag with "AmountFieldsDiv2" class
      $('.AmountFieldsDiv2 input[type=text]').not(id = '#NetAmountCredited').not(id = '#VatAmount').removeAttr("readOnly");

      // Show 'VatBreakdown' link
      $('#vatBreakdown').show();
    }
    else {
      $('#vatBreakdown').hide();
      $('.AmountFieldsDiv1 input[type=text]').attr("readOnly", true);
      $('.AmountFieldsDiv2 input[type=text]').attr("readOnly", true);
    }
  }

  // If pagemode is "Edit" and if Creditmemo is being edited, call EnableDisableAmountFieldsOnreasonCode() function which will remove readonly attribute of memo amount fields within Div tags if 
  // couponBreakdownMandatory == 'False' and couponBreakdown does not exists
  if (pageMode == 'Edit' && editLevel == 'CreditMemoEdit') {
    EnableDisableAmountFieldsOnreasonCode($('#CouponAwbBreakdownMandatory').val(), couponBreakdownExists);
  }

  $('#ElectronicTicketIndicator').attr('checked', 'checked');
  
  // Register Control events and Validation rules only if pageMode is Create or Edit
  if (!$isOnView) {
  
    $('#ReasonRemarks').removeClass("validateCharacters");
    $("#creditMemoForm").validate({
      rules: {
        BatchSequenceNumber: "required",
        RecordSequenceWithinBatch: "required",
        SourceCodeId: "required",
        CreditMemoNumber: "required",
        AttachmentIndicatorOriginal: "required",
        ReasonCode: "required",
        ReasonRemarks: {
          allowedCharactersForTextAreaFields: true,
          ValidatePaxTextareaField: [80, 50]
        },
        FimNumber: {
          required: function (element) {
            if ($('#FimCouponNumber').val() != '') {
              return true;
            }
            else
              return false;
          }
        },
        FimCouponNumber: {
          required: function (element) {
            if ($('#FimNumber').val() != '') {
              return true;
            }
            else
              return false;
          }
        },
        YourInvoiceNumber: { required: function (element) {
          return isValueRequired('#YourInvoiceBillingYear', '#YourInvoiceBillingMonth', '#YourInvoiceBillingPeriod');
        }
        },
        YourInvoiceBillingYear: { required: function (element) {
          return isValueRequired('#YourInvoiceNumber', '#YourInvoiceBillingMonth', '#YourInvoiceBillingPeriod');
        }
        },
        YourInvoiceBillingMonth: { required: function (element) {
          return isValueRequired('#YourInvoiceBillingYear', '#YourInvoiceNumber', '#YourInvoiceBillingPeriod');
        }
        },
        YourInvoiceBillingPeriod: { required: function (element) {
          return isValueRequired('#YourInvoiceBillingYear', '#YourInvoiceBillingMonth', '#YourInvoiceNumber');
        }
        }
      },
      messages: {
        BatchSequenceNumber: "Batch Number Required",
        RecordSequenceWithinBatch: "Sequence Number Required",
        SourceCodeId: "Source Code Required",
        CreditMemoNumber: "Credit Memo Number Required",
        AttachmentIndicatorOriginal: "Attachment Indicator Original Required",
        FimNumber: "FIM Number Required",
        FimCouponNumber: "FIM Coupon Number Required",
        YourInvoiceNumber: "Your Invoice Number Required",
        YourInvoiceBillingYear: "Your Billing Year Required",
        YourInvoiceBillingMonth: "Your Billing Month Required",
        YourInvoiceBillingPeriod: "Your Billing Period Required",
        ReasonCode: "Reason Code Required"
      }
    });

    trackFormChanges('creditMemoForm');

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

    $("#FlightDate").blur(function () {
      var fdVal = $("#FlightDate").val();

      if (fdVal == "" || fdVal == "DD-MM") {
        return true;
      }
      if (fdVal.indexOf("-") != 2) {
        $("#FlightDate").val("");
        $("#FlightDate").focus();
        return;
      }
      var dd = fdVal.substr(0, fdVal.indexOf("-"));
      var mm = fdVal.substr(fdVal.indexOf("-") + 1);

      var y = new Date().getYear();

      if (y % 4 == 0) {
        if (isNaN(dd) || parseInt(dd) < 1 || parseInt(dd) > 31 || isNaN(mm) || parseInt(mm) < 1 || parseInt(mm) > 12 || (parseInt(dd) > 28 && parseInt(mm) == 2)) {

          $("#FlightDate").val("");
          $("#FlightDate").focus();
          return;
        }
      }
      else if (isNaN(dd) || parseInt(dd) < 1 || parseInt(dd) > 31 || isNaN(mm) || parseInt(mm) < 1 || parseInt(mm) > 12 || (parseInt(dd) > 29 && parseInt(mm) == 2)) {

        $("#FlightDate").val("");
        $("#FlightDate").focus();
        return;
      }
    });

    $("#CurrencyAdjustmentIndicator").keyup(function () {
      var currAdjInd = $("#CurrencyAdjustmentIndicator").val();
      $("#CurrencyAdjustmentIndicator").val(currAdjInd.toUpperCase());
    });
  } // end if()

  function calculateNetCouponAmount() {
    calculateNetAmount("#GrossAmountCredited", "#IscAmountBilled", "#OtherCommissionBilled", "#UatpAmountBilled", "#HandlingFeeAmount", "#TaxAmount", "#VatAmount", "#NetAmountCredited");
  }

  function setPercentage(sourceControl1Id, sourceControl2Id, targetControlId) {
    var sourceControl1Value = $(sourceControl1Id).val();
    var sourceControl2Value = $(sourceControl2Id).val();
    var percent = sourceControl1Value / 100 * sourceControl2Value;
    if (!isNaN(percent))
      $(targetControlId).val(percent.toFixed(_amountDecimals));
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
    if (!isNaN(couponTotalAmount)) {
      if (couponTotalAmount == 0) {
        $(targetControl).val(couponTotalAmount.toFixed(_amountDecimals));
        $(targetControl).watermark();
      }
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

  $("#ProrateSlipDetails").bind("keypress", function () { maxLength(this, 4000) });
  $("#ProrateSlipDetails").bind("paste", function () { maxLengthPaste(this, 4000) });

  if (isOnCouponPage == false) {
    $("#TotalGrossAmountCredited").blur(function () {
      calculateAmount();
    });

    $("#TotalIscAmountCredited").blur(function () {
      calculateAmount();
    });

    $("#TotalOtherCommissionAmountCredited").blur(function () {
      calculateAmount();
    });

    $("#TotalUatpAmountCredited").blur(function () {
      calculateAmount();
    });

    $("#TotalHandlingFeeCredited").blur(function () {
      calculateAmount();
    });

    $("#TaxAmount").blur(function () {
      calculateAmount();
    });

    $("#VatAmount").blur(function () {
      calculateAmount();
    });

    $('#Save').click(function () {
      calculateAmount();
    });
  }

  function calculateAmount() {
    calculateNetAmount("#TotalGrossAmountCredited", "#TotalIscAmountCredited", "#TotalOtherCommissionAmountCredited", "#TotalUatpAmountCredited", "#TotalHandlingFeeCredited", "#TaxAmount", "#VatAmount", "#NetAmountCredited");
  }
});


//To be updated once the functionality is done - Batch and sequence numbers will be non-editable in Create, Edit
function setControlAccess(isCouponExists) {
  isCouponBreakdownExists = isCouponExists;
  $('#BatchSequenceNumber').attr('readOnly', 'readOnly');
  $('#RecordSequenceWithinBatch').attr('readOnly', 'readOnly');

}

// Following function is executed when ReasonCode is modified and is used to enable disable CreditMemo amount fields at Memo level depending on ReasonCode.
// "selectedValue" parameter contains ReasonCode-IsCouponBreakdownMandatory value 
function onReasonCodeChange(selectedValue) {
  // Split selectedValue parameter to retrieve reasonCode 
  $('#ReasonCode').val(selectedValue.split('-')[0]);
  // Split selectedValue parameter to retrieve value whether Coupon breakdown is mandatory for selected reasonCode 
  isCouponBreakdownMandatory = selectedValue.split('-')[1];

  // If selected ReasonCode does not mandate coupon breakdown and coupon breakdown does not exist, remove readOnly attribute of selected Memo amount fields, else keep it disabled 
  if (isCouponBreakdownMandatory == "False" && couponBreakdownExists == 'False') {
    // Remove readOnly attribute of all text fields from div tag with "AmountFieldsDiv1" class
    $('.AmountFieldsDiv1 input[type=text]').removeAttr("readOnly");

    // Remove readOnly attribute of all text fields from div tag with "AmountFieldsDiv2" class
    $('.AmountFieldsDiv2 input[type=text]').not(id = '#NetAmountCredited').not(id = '#VatAmount').removeAttr("readOnly");

    // Show 'VatBreakdown' link
    $('#vatBreakdown').show();

    // Set 'CouponAwbBreakdownMandatory' hidden field value
    $('#CouponAwbBreakdownMandatory').val(isCouponBreakdownMandatory);
  }
  else {
    // If reasonCode mandates Coupon Breakdown, clear all Memo amount text fields and add readOnly attribute to text fields   
    $('#vatBreakdown').hide();
    $('.AmountFieldsDiv1 input[type=text]').attr("readOnly", true);
    $('.AmountFieldsDiv2 input[type=text]').attr("readOnly", true);

    // Set 'CouponAwbBreakdownMandatory' hidden field value
    $('#CouponAwbBreakdownMandatory').val(isCouponBreakdownMandatory);
  }
} // end onReasonCodeChange()

// Following function keeps CreditMemo Amount fields disabled, if ReasonCode field is blank 
function onBlankReasonCode() {
  $('#vatBreakdown').hide();
  $('.AmountFieldsDiv1 input[type=text]').attr("readOnly", true);
  $('.AmountFieldsDiv2 input[type=text]').attr("readOnly", true);
} // end onBlankReasonCode()


