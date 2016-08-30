_amountDecimals = 2;

$(document).ready(function () {
  
  // SCP#391029: FW: CORRESPONDENCE_REF_NO equals "0" in PAX and Cargo
  $("#btnSave").bind("click", function () {
    UserCapturedCorrRefNo();
  });
  $("#btnSaveAndAddNew").bind("click", function () {
    UserCapturedCorrRefNo();
  });

  function UserCapturedCorrRefNo() {
    if ($('#CorrespondenceRefNumber').val().trim() == '') {
      $('#UserCorrRefNo').val('-1');
    }
    else {
      $('#UserCorrRefNo').val($('#CorrespondenceRefNumber').val());
    }
  }

  if ($('#ReasonCode').val() != '') {
    // Remove readOnly attribute of all text fields from div tag with "AmountFieldsDiv1" class
    $('.amountFieldsDiv1 input[type=text]').removeAttr("readOnly");

    // Remove readOnly attribute of all text fields from div tag with "AmountFieldsDiv2" class
    $('.amountFieldsDiv2 input[type=text]').not(id = '#NetAmountBilled').not(id = '#VatAmount').removeAttr("readOnly");

    // Show 'VatBreakdown' link
    $('#vatBreakdown').show();
  }

  // If pagemode is "Edit", call EnableDisableAmountFieldsOnreasonCode() function which will remove readonly attribute of memo amount fields within Div tags if 
  // couponBreakdownMandatory == 'False' and couponBreakdown does not exists
  if (pageMode == 'Edit') {
    EnableDisableAmountFieldsOnreasonCode($('#CouponAwbBreakdownMandatory').val(), couponBreakdownExists);
  }

  SetPageWaterMark();

  // If pageMode == Create/Edit, then add validation rules and register control events
  if (pageMode != 'View') {
    $('#ReasonRemarks').removeClass("validateCharacters");
    $('#YourInvoiceNumber').bind
    $("#billingMemoForm").validate({
      rules: {
        BatchSequenceNumber: "required",
        RecordSequenceWithinBatch: "required",
        SourceCodeId: "required",
        BillingMemoNumber: "required",
        ReasonCode: "required",
        ReasonRemarks: {
          allowedCharactersForTextAreaFields: true,
          ValidatePaxTextareaField: [80, 50]
        },
        CorrespondenceRefNumber: {
          required: function (element) {
            return ($('#ReasonCode').val() == "6A" || $('#ReasonCode').val() == "6B");
          }
        },
        YourInvoiceNumber: {
          required: function (element) {
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
        },

        FimNumber: { required: function (element) {
          return isValueRequiredFimCoupon('#FimCouponNumber');
        }
        },

        FimCouponNumber: { required: function (element) {
          return isValueRequiredFimCoupon('#FimNumber');
        }
        }
      },

      messages: {
        BatchSequenceNumber: "Valid Batch Sequence Number Required",
        RecordSequenceWithinBatch: "Valid Record Sequence Within Batch Required",
        SourceCodeId: "Source Code Required",
        BillingMemoNumber: "Billing Memo Number Required",
        ReasonCodeId: "Reason Code Required",
        YourInvoiceNumber: "Your Invoice Number Required",
        YourInvoiceBillingYear: "Your Billing Year Required",
        YourInvoiceBillingMonth: "Your Billing Month Required",
        YourInvoiceBillingPeriod: "Your Billing Period Required",
        FimNumber: "FIM Number Required",
        FimCouponNumber: "FIM Coupon Number Required",
        CorrespondenceRefNumber: "This field is Required for Reason Codes 6A or 6B"
      },
      invalidHandler: function (form, validator) {
        $.watermark.showAll();
      }
    });

    trackFormChanges('billingMemoForm');

    $('.primaryButton').click(function () {
      calculateNetAmount();
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


    $('#TotalGrossAmountBilled').blur(function () {
      calculateNetAmount();
    });

    $('#TotalIscAmountBilled').blur(function () {
      calculateNetAmount();

    });

    $('#TotalOtherCommissionAmount').blur(function () {
      calculateNetAmount();
    });

    $('#TotalUatpAmountBilled').blur(function () {
      calculateNetAmount();
    });

    $('#TotalHandlingFeeBilled').blur(function () {
      calculateNetAmount();
    });

    $('#TaxAmountBilled').blur(function () {
      calculateNetAmount();
    });

    $('#VatAmount').blur(function () {
      calculateNetAmount();
    });
  } // end if()

  function calculateNetAmount() {
    calculateNetRejectedAmount('#TotalGrossAmountBilled', "#TaxAmountBilled", '#VatAmount', '#TotalIscAmountBilled', '#TotalUatpAmountBilled', '#TotalHandlingFeeBilled', '#TotalOtherCommissionAmount', '#NetAmountBilled')
  }

  function calculateNetRejectedAmount(sourceControl1, sourceControl2, sourceControl3, sourceControl4, sourceControl5, sourceControl6, sourceControl7, targetControl) {
    var vatDiff = 0;
    var grossDiff = 0;
    var iscDiff = 0;
    var uatpDiff = 0;
    var hfDiff = 0;
    var ocDiff = 0;
    var taxDiff = 0;

    if (!isNaN(Number($(sourceControl1).val())))
      grossDiff = Number($(sourceControl1).val());

    if (!isNaN(Number($(sourceControl2).val())))
      taxDiff = Number($(sourceControl2).val());

    if (!isNaN(Number($(sourceControl3).val())))
      vatDiff = Number($(sourceControl3).val());

    if (!isNaN(Number($(sourceControl4).val())))
      iscDiff = Number($(sourceControl4).val());

    if (!isNaN(Number($(sourceControl5).val())))
      uatpDiff = Number($(sourceControl5).val());

    if (!isNaN(Number($(sourceControl6).val())))
      hfDiff = Number($(sourceControl6).val());

    if (!isNaN(Number($(sourceControl7).val())))
      ocDiff = Number($(sourceControl7).val());

    var netRejectedAmount = grossDiff + vatDiff + iscDiff + uatpDiff + hfDiff + ocDiff + taxDiff;
    if (!isNaN(netRejectedAmount))
      $(targetControl).val(netRejectedAmount.toFixed(2));
  }

  function isValueRequired(control1Id, control2Id, control3Id) {
    if ($(control1Id).val() != '' || $(control2Id).val() != '' || $(control3Id).val() != '')
      return true;
    else
      return false;
  }

  function isValueRequiredFimCoupon(control1Id) {
    if ($(control1Id).val() != '')
      return true;
    else
      return false;
  }

});

function setControlAccess(isException) {
  $("input[value=0]").val(''); // This is a temporary fix. Field should be made nullable.
  $('#BatchSequenceNumber').attr('readOnly', 'readOnly');
  $('#RecordSequenceWithinBatch').attr('readOnly', 'readOnly');
  $('#SourceCodeId').attr('readOnly', 'readOnly');

  var $reasonCode = $('#ReasonCode', '#content');

  if (($reasonCode.val() == '6A' || $reasonCode.val() == '6B') && isException == "False") {
    $('#CorrespondenceRefNumber').attr('readOnly', 'readOnly');
  }
}

function SourceCodeId_SetAutocompleteDisplay(item) {
  var sourceCode = item.SourceCodeName + '-' + item.SourceCodeDescription;
  return { label: sourceCode, value: item.SourceCodeName, id: item.Id };
}

function ReasonCodeId_SetAutocompleteDisplay(item) {
  var reasonCode = item.Code + '-' + item.Description;
  return { label: reasonCode, value: item.Code, id: item.Id };
}

// Following function removes readOnly attribute for text fields within memo amount div tag if isCouponBreakdownmandatory == 'False' and breakdownExists == 'False'
function EnableDisableAmountFieldsOnreasonCode(isCouponBreakdownmandatory, breakdownExists) {
  // If couponBreakdown is not mandatory and coupon does not exists for current memo, enable selected Memo amount fields
  if (isCouponBreakdownmandatory == 'False' && breakdownExists == 'False') {
    // Remove readOnly attribute of all text fields from div tag with "AmountFieldsDiv1" class
    $('.amountFieldsDiv1 input[type=text]').removeAttr("readOnly");

    // Remove readOnly attribute of all text fields from div tag with "AmountFieldsDiv2" class
    $('.amountFieldsDiv2 input[type=text]').not(id = '#NetAmountBilled').removeAttr("readOnly");
    $('#VatAmount').attr('readOnly', true);

    // Show 'VatBreakdown' link
    $('#vatBreakdown').show();
  }
  else {
    $('#vatBreakdown').hide();
    $('.amountFieldsDiv1 input[type=text]').attr("readOnly", true);
    $('.amountFieldsDiv2 input[type=text]').attr("readOnly", true);
  }
}

// Following function is used to remove readOnly attribute of BillingMemo amount fields at Memo level depending on ReasonCode if couponBreakdown is not mandatory.
// "selectedValue" parameter contains ReasonCode-IsCouponBreakdownMandatory value 
function onReasonCodeChange(selectedValue) {
  // Split selectedValue parameter to retrieve reasonCode 
  $('#ReasonCode').val(selectedValue.split('-')[0]);
  // Split selectedValue parameter to retrieve value whether Coupon breakdown is mandatory for selected reasonCode 
  isCouponBreakdownMandatory = selectedValue.split('-')[1];

  // If coupon breakdown is not mandatory and couponBreakdown does not exixts enable selected Memo amount fields, else keep it disabled 
  if (isCouponBreakdownMandatory == "False" && couponBreakdownExists == 'False') {
    // Remove readOnly attribute of all text fields from div tag with "AmountFieldsDiv1" class
    $('.amountFieldsDiv1 input[type=text]').removeAttr("readOnly");

    // Remove readOnly attribute of all text fields from div tag with "AmountFieldsDiv2" class
    $('.amountFieldsDiv2 input[type=text]').not(id = '#NetAmountBilled').not(id = '#VatAmount').removeAttr("readOnly");

    // Show 'VatBreakdown' link
    $('#vatBreakdown').show();

    // Set 'CouponAwbBreakdownMandatory' hidden field value
    $('#CouponAwbBreakdownMandatory').val(isCouponBreakdownMandatory);
  }
  else {
    // If coupon breakdown is mandatory, clear all text fields and add "readOnly" attribute to all text fields within div's which have amountFieldsDiv1 or amountFieldsDiv2 class 
    $('#vatBreakdown').hide();
    $('.amountFieldsDiv1 input[type=text]').attr("readOnly", true);
    $('.amountFieldsDiv2 input[type=text]').attr("readOnly", true);

    // Set 'CouponAwbBreakdownMandatory' hidden field value
    $('#CouponAwbBreakdownMandatory').val(isCouponBreakdownMandatory);
  }
} // end onReasonCodeChange()

// Following function keeps BillingMemo fields disabled, if ReasonCode autocomlete field is blank 
function onBlankReasonCode() {
  $('#vatBreakdown').hide();
  $('.amountFieldsDiv1 input[type=text]').attr("readOnly", true);
  $('.amountFieldsDiv2 input[type=text]').attr("readOnly", true);
} // end onBlankReasonCode()

