_amountDecimals = 2;
_percentDecimals = 3;
var cpnExist = '';
isFromBillingHistory = false;
$(document).ready(function () {
  var couponExists = 'False';
  var validated = false;  
  SetPageWaterMark();
  // If pageMode is equal to Create or Edit register Control events and Validation rules 
  
  if (pageMode != 'View') {
    $('#ReasonRemarks').removeClass("validateCharacters");
    $("#rejectionMemoForm").validate({
      rules: {
        BatchSequenceNumber: "required",
        RecordSequenceWithinBatch: "required",
        SourceCodeId: "required",
        RejectionStage: "required",
        RejectionMemoNumber: "required",
        ReasonCode: "required",
        YourInvoiceNumber: "required",
        YourInvoiceBillingYear: "required",
        YourInvoiceBillingMonth: "required",
        YourInvoiceBillingPeriod: "required",
        ReasonRemarks: {
          allowedCharactersForTextAreaFields: true,
          ValidatePaxTextareaField: [80, 50]
        },
        YourRejectionNumber: {
          required: function (element) {
            var value = $("#RejectionStage").val();
            if (value != '2' && value != '3') {
              return false;
            } else {
              return true;
            }
          }
        },
        FimBMCMNumber: {
          required: function (element) {
            var FimBmCmIndicator = $("#FIMBMCMIndicatorId").val();
            // If FimNumber, BillingMemo number, Creditmemo number is selected in FimBmCmIndicator dropdown, "FimBMCMNumber" textbox value is mandatory.
            //SCP:37078 : Below code is change to make FimBMCMNumber field non mandetory in case of FIM Rejection Stage 2 and 3.
            if (FimBmCmIndicator == 3 || FimBmCmIndicator == 4 || (FimBmCmIndicator == 2 && $("#SourceCodeId").val() == '44')) {
              return true;
            }
            else {
              return false;
            }
          }
        },
        FimCouponNumber: {
          required: function (element) {
            var FimBmCmIndicator = $("#FIMBMCMIndicatorId").val();
            // If FimNumber is selected in FimBmCmIndicator dropdown, "FimCouponNumber" textbox value is mandatory.
            //SCP:37078 : Below code is change to make FimCouponNumber field non mandetory in case of FIM Rejection Stage 2 and 3.
            if (FimBmCmIndicator == 2 && $("#SourceCodeId").val() == '44') {
              return true;
            }
            else {
              return false;
            }
          },
          fimCouponNumber: true
        }
      },
      messages: {
        BatchSequenceNumber: "Valid Batch Number Required",
        RecordSequenceWithinBatch: "Valid Sequence Number Required",
        SourceCodeId: "Source Code Required",
        RejectionStage: "Rejection Stage",
        RejectionMemoNumber: "Rejection Memo Number Required/Invalid Rejection Memo Number",
        ReasonCode: "Reason Code Required",
        YourInvoiceNumber: "Your Invoice Number Required/Invalid Your Invoice Number",
        YourInvoiceBillingYear: "Your Invoice Billing year Required",
        YourInvoiceBillingMonth: "Your Invoice Billing Month Required",
        YourInvoiceBillingPeriod: "Your Invoice Billing Period Required",
        YourRejectionNumber: "Your Rejection Number Required for Stage 2 / 3",
        FimBMCMNumber: "FimBMCM Number Required/Invalid FimBMCM Number",
        FimCouponNumber: { required: "Fim Coupon Number Required" }
      },
      invalidHandler: function (form, validator) {
        validated = true;
        if ($('#RejectionStage').val() == "1") {
          $("#YourRejectionNumber").attr("readonly", "true");
          $('#RejectionStage').attr('disabled', 'disabled');
        }
        if ($('#RejectionStage').val() == "2" || $('#RejectionStage').val() == "3") {
          $('#RejectionStage').attr('disabled', 'disabled');
          $("#YourRejectionNumber").removeAttr("readonly");
        }
        if (couponExists == 'True') {
          $('#YourInvoiceBillingYear').attr('disabled', 'disabled');
          $('#YourInvoiceBillingMonth').attr('disabled', 'disabled');
          $('#YourInvoiceBillingPeriod').attr('disabled', 'disabled');
        }
        $.watermark.showAll();
      },
      submitHandler: function (form) {
        $('#RejectionStage').removeAttr('disabled');
        $('#YourInvoiceNumber').removeAttr('readOnly');
        $('#YourInvoiceBillingYear').removeAttr('disabled');
        $('#YourInvoiceBillingMonth').removeAttr('disabled');
        $('#YourInvoiceBillingPeriod').removeAttr('disabled');
        $("#FIMBMCMIndicatorId", '#content').removeAttr('disabled');
        calculateAmounts();
        // Call onSubmitHandler() function which will disable Submit buttons and will submit the form
        onSubmitHandler(form);
      }
    });

    trackFormChanges('rejectionMemoForm');

    $('#BatchSequenceNumber').change(function () {
      var batchNumber = $('#BatchSequenceNumber').val();
      if (!isNaN(parseInt(batchNumber)))
        $('#BatchSequenceNumber').val(parseInt(batchNumber));
    });

    $('#RecordSequenceWithinBatch').change(function () {
      var recordBatchNumber = $('#RecordSequenceWithinBatch').val();
      if (!isNaN(parseInt(recordBatchNumber)))
        $('#RecordSequenceWithinBatch').val(parseInt(recordBatchNumber));
    });

    $("#SourceCodeId").blur(function (element) {
      $("#ReasonCode").flushCache();
      var sourceCodeId = $("#SourceCodeId").val();
      if (!$("#SourceCodeId").prop('readonly')) {
        if (sourceCodeId != '') {
          $("#ReasonCode").removeAttr('readOnly');
        }
        else {
          $("#ReasonCode").val('');
          $("#ReasonCode").autocomplete('destroy');
          $("#ReasonCode").attr('readOnly', 'readOnly');
        }
      }
      $("#RejectionStage").attr('disabled', 'disabled');

      //257778: SYSTEM ISSUE
      if (sourceCodeId == '4' || sourceCodeId == '44' || sourceCodeId == '91') {
        $("#RejectionStage").val(1);
        $("#YourRejectionNumber").val('');
        $("#YourRejectionNumber").attr('readOnly', 'readOnly');
        removeAsteriskSpan('#LabelYourRejectionNumber');
        if (sourceCodeId == '44') {
          $("#FIMBMCMIndicatorId").val(2);
          $("#FIMBMCMIndicatorId").attr('disabled', true);
        }
        if ($("#FIMBMCMIndicatorId").val() == '')
          $("#FIMBMCMIndicatorId").val(1);
          
         }
      //257778: SYSTEM ISSUE
      else if (sourceCodeId == '5' || sourceCodeId == '45' || sourceCodeId == '92') {
        $("#RejectionStage").val(2);
        prefixAsterisk('#LabelYourRejectionNumber');
        $("#YourRejectionNumber").removeAttr('readOnly');
      }
      //257778: SYSTEM ISSUE
      else if (sourceCodeId == '6' || sourceCodeId == '46' || sourceCodeId == '93') {
        $("#RejectionStage").val(3);
        $("#YourRejectionNumber").removeAttr('readOnly');
        prefixAsterisk('#LabelYourRejectionNumber');
      }
      else {
        if (!isFromBillingHistory) {
          $("#RejectionStage").removeAttr('disabled');
          $("#YourRejectionNumber").removeAttr('readOnly');
          removeAsteriskSpan('#LabelYourRejectionNumber');
        }
      }

      // Disabling "Save" button again because it gets enabled when ajax call is completed for SourceCode autocomplete.
      // Note:- We have written code in site.js which enables all submit buttons on ajax call complete.
      if (!isFromBillingHistory) {
        $("#btnSave").attr("disabled", true);
        $("#btnSaveAndAddNew").attr("disabled", true);
      }
      // Call "EnableDisableFmBmCmIndicatorOnRejectionStage" function which will disable FmBmCmindicator fields if RejectionStage equals 2 or 3, else enable
      // EnableDisableFmBmCmIndicatorOnRejectionStage();
      //Above call is added in "linkingFieldsChnaged" method.
      // Call "linkingFieldsChanged" function which will enabled or disabled the save and fetch button on the base of rejection stage
      linkingFieldsChanged();

    });



    $("#YourRejectionNumber").focus(function (element) {
      if ($("#RejectionStage").val() == "1") {
        $("#YourRejectionNumber").val();
      }
    });

    $('#RejectionStage').change(function () {
      if ($(this).val() == '2' || $(this).val() == '3') {
        prefixAsterisk('#LabelYourRejectionNumber');
        // Call "EnableDisableFmBmCmIndicatorOnRejectionStage" function which will disable FmBmCmindicator fields if RejectionStage equals 2 or 3, else enable
        EnableDisableFmBmCmIndicatorOnRejectionStage();
      }
      else
        removeAsteriskSpan('#LabelYourRejectionNumber');
      // Call "EnableDisableFmBmCmIndicatorOnRejectionStage" function which will disable FmBmCmindicator fields if RejectionStage equals 2 or 3, else enable
      EnableDisableFmBmCmIndicatorOnRejectionStage();
    });

    $("#TotalGrossAmountBilled").blur(function () {
      setAmountDiff("#TotalGrossAmountBilled", "#TotalGrossAcceptedAmount", "#TotalGrossDifference");
      calculateAmount();
    });

    $("#TotalTaxAmountBilled").blur(function () {
      setAmountDiff("#TotalTaxAmountBilled", "#TotalTaxAmountAccepted", "#TotalTaxAmountDifference");
      calculateAmount();
    });

    $("#TotalVatAmountBilled").blur(function () {
      setAmountDiff("#TotalVatAmountBilled", "#TotalVatAmountAccepted", "#TotalVatAmountDifference");
      if (cpnExist != 'True' && $('#TotalVatAmountDifference').val() != 0)
        $('#vatBreakdown').show();
      calculateAmount();
    });

    $("#AllowedIscAmount").blur(function () {
      setAmountDiff("#AllowedIscAmount", "#AcceptedIscAmount", "#IscDifference");
      calculateAmount();
    });

    $("#AllowedUatpAmount").blur(function () {
      setAmountDiff("#AllowedUatpAmount", "#AcceptedUatpAmount", "#UatpAmountDifference");
      calculateAmount();
    });

    $("#AllowedHandlingFee").blur(function () {
      setAmountDiff("#AllowedHandlingFee", "#AcceptedHandlingFee", "#HandlingFeeAmountDifference");
      calculateAmount();
    });

    $("#AllowedOtherCommission").blur(function () {
      setAmountDiff("#AllowedOtherCommission", "#AcceptedOtherCommission", "#OtherCommissionDifference");
      calculateAmount();
    });

    $("#TotalGrossAcceptedAmount").blur(function () {
      setAmountDiff("#TotalGrossAmountBilled", "#TotalGrossAcceptedAmount", "#TotalGrossDifference");
      calculateAmount();
    });

    $("#TotalTaxAmountAccepted").blur(function () {
      setAmountDiff("#TotalTaxAmountBilled", "#TotalTaxAmountAccepted", "#TotalTaxAmountDifference");
      calculateAmount();
    });

    $("#TotalVatAmountAccepted").blur(function () {
      setAmountDiff("#TotalVatAmountBilled", "#TotalVatAmountAccepted", "#TotalVatAmountDifference");
      if (cpnExist != 'True' && $('#TotalVatAmountDifference').val() != 0)
        $('#vatBreakdown').show();
      calculateAmount();
    });

    $("#AcceptedIscAmount").blur(function () {
      setAmountDiff("#AllowedIscAmount", "#AcceptedIscAmount", "#IscDifference");
      calculateAmount();
    });

    $("#AcceptedUatpAmount").blur(function () {
      setAmountDiff("#AllowedUatpAmount", "#AcceptedUatpAmount", "#UatpAmountDifference");
      calculateAmount();
    });

    $("#AcceptedHandlingFee").blur(function () {
      setAmountDiff("#AllowedHandlingFee", "#AcceptedHandlingFee", "#HandlingFeeAmountDifference");
      calculateAmount();
    });

    $("#AcceptedOtherCommission").blur(function () {
      setAmountDiff("#AllowedOtherCommission", "#AcceptedOtherCommission", "#OtherCommissionDifference");
      calculateAmount();
    });

    function calculateAmounts() {
      setAmountDiff("#TotalGrossAmountBilled", "#TotalGrossAcceptedAmount", "#TotalGrossDifference");
      setAmountDiff("#TotalTaxAmountBilled", "#TotalTaxAmountAccepted", "#TotalTaxAmountDifference");
      setAmountDiff("#TotalVatAmountBilled", "#TotalVatAmountAccepted", "#TotalVatAmountDifference");
      if (cpnExist != 'True' && $('#TotalVatAmountDifference').val() != 0)
        $('#vatBreakdown').show();
      setAmountDiff("#AllowedIscAmount", "#AcceptedIscAmount", "#IscDifference");
      setAmountDiff("#AllowedUatpAmount", "#AcceptedUatpAmount", "#UatpAmountDifference");
      setAmountDiff("#AllowedHandlingFee", "#AcceptedHandlingFee", "#HandlingFeeAmountDifference");

      calculateAmount();
    }
  }

  enableDisableControlOnValue("#RejectionStage", "#YourRejectionNumber");

  $("#RejectionStage").attr('disabled', 'disabled');

  function calculateAmount() {
    calculateNetAmount("#TotalGrossDifference", "#TotalTaxAmountDifference", "#TotalVatAmountDifference", "#IscDifference", "#UatpAmountDifference", "#HandlingFeeAmountDifference", "#OtherCommissionDifference", "#TotalNetRejectAmount");
  }

  function setAmountDiff(sourceControl1Id, sourceControl2Id, targetControlId) {
    var sourceControl1Value = 0;
    sourceControl1Value = $(sourceControl1Id).val();
    var sourceControl2Value = 0;
    sourceControl2Value = $(sourceControl2Id).val();
    var difference = 0;

    if ($('#RejectionStage').val() == "2")
      difference = sourceControl2Value - sourceControl1Value;
    else
      difference = sourceControl1Value - sourceControl2Value;

    if (!isNaN(difference))
      $(targetControlId).val(difference.toFixed(_amountDecimals));
    else
      $(targetControlId).val(Number(0).toFixed(_amountDecimals));
  }

  function calculateNetAmount(sourceControl1, sourceControl2, sourceControl3, sourceControl4, sourceControl5, sourceControl6, sourceControl7, targetControl) {
    var grossAmount = 0;
    var taxAmount = 0;
    var vatAmount = 0;
    var iscAmount = 0;
    var uatpAmount = 0;
    var hfAmount = 0;
    var ocAmount = 0;

    if (!isNaN(Number($(sourceControl1).val())))
      grossAmount = Number($(sourceControl1).val());

    if (!isNaN(Number($(sourceControl2).val())))
      taxAmount = Number($(sourceControl2).val());

    if (!isNaN(Number($(sourceControl3).val())))
      vatAmount = Number($(sourceControl3).val());

    if (!isNaN(Number($(sourceControl4).val())))
      iscAmount = Number($(sourceControl4).val());

    if (!isNaN(Number($(sourceControl5).val())))
      uatpAmount = Number($(sourceControl5).val());

    if (!isNaN(Number($(sourceControl6).val())))
      hfAmount = Number($(sourceControl6).val());

    if (!isNaN(Number($(sourceControl7).val())))
      ocAmount = Number($(sourceControl7).val());

    var totalNetRejectAmount = grossAmount + iscAmount + ocAmount + uatpAmount + hfAmount + taxAmount + vatAmount;
    if (!isNaN(totalNetRejectAmount))
      $(targetControl).val(totalNetRejectAmount.toFixed(_amountDecimals));
  }

  function enableDisableControlOnValue(dropdownId, textBoxId) {
    if ($(dropdownId).val() == '1')
      $(textBoxId).attr('readonly', 'true');

    $(dropdownId).change(function () {
      if ($(this).val() == '1') {
        $(textBoxId).attr('readonly', 'true');
        $(textBoxId).val("");
      }
      else {
        $(textBoxId).removeAttr('readonly');
      }
    });
  }
});

function SetControlAccessibility(isCouponExists) {
  $("#SourceCodeId").attr("readonly", "true");
  if (isCouponExists == 'True')
    SetControlAccess();
}

function SetControlAccess() {
  $("#RejectionStage").attr("disabled", "disabled");
  $("#YourRejectionNumber").attr("readonly", "true");
  $("#SourceCodeId").autocomplete({ disabled: true });
  $("#ReasonCode").attr("readonly", "true");
  $("#ReasonCode").autocomplete({ disabled: true });
  $("#YourInvoiceNumber").attr("readonly", "true");
  $("#YourInvoiceBillingYear").attr("disabled", "disabled");
  $("#YourInvoiceBillingMonth").attr("disabled", "disabled");
  $("#YourInvoiceBillingPeriod").attr("disabled", "disabled");
  $("#FimCouponNumber").attr("readonly", "true");
  $("#FimBMCMNumber").attr("readonly", "true");
}

var _getLinkingDetailsMethod, _getLinkedMemoDetailsMethod;
var _billingMemberId, _billedMemberId, _rejectedInvoiceId;
function InitializeLinkingSettings(isCouponExists, getLinkingDetailsMethod, getLinkedMemoDetailsMethod, billingMemberId, billedMemberId, rejectedInvoiceId) {
  if (isCouponExists != false) {
    $('#btnGetLinkingData').attr("disabled", "disabled");
  }
  $('#ReasonCode,#YourInvoiceNumber,#YourInvoiceBillingYear, #YourInvoiceBillingMonth', '#content').blur(function () {
    linkingFieldsChanged();
  });

  $('#YourInvoiceBillingPeriod', '#content').blur(function () {
      linkingFieldsChanged();
    });


  $('#FimCouponNumber,#FimBMCMNumber, #FIMBMCMIndicatorId').blur(function () {
      linkingFieldsChangedOnFIMBM();
  });



  $('#RejectionStage,#YourRejectionNumber,#SourceCode', '#content').blur(function () {
    linkingFieldsChanged();
  });
  _getLinkingDetailsMethod = getLinkingDetailsMethod;
  _getLinkedMemoDetailsMethod = getLinkedMemoDetailsMethod;
  //Billing and billed member are considered for rejected invoice.
  _billingMemberId = billedMemberId;
  _billedMemberId = billingMemberId;
  _rejectedInvoiceId = rejectedInvoiceId;

  //it will disable the control when user comes from edit part
  linkingFieldsChanged();
}

function linkingFieldsChanged() {
    if (!isFromBillingHistory) {
    if ($("#RejectionStage").val() == 1 && $("#FIMBMCMIndicatorId").val() == 1) {
      //disabled following control if rejection is first step rejection for PB
      $('#btnGetLinkingData', '#content').attr('disabled', 'disabled');
      // enabled Save button if rejection is first step rejection of PB.

      $("#btnSave").attr("disabled", false);
      $("#btnSaveAndAddNew").attr("disabled", false);
    }
    else {
      $('#btnGetLinkingData', '#content').attr('disabled', false);
      // Disable Save button if user modifies any control value.
      $("#btnSave").attr("disabled", true);
      $("#btnSaveAndAddNew").attr("disabled", true);
    }
    //EnableDisableFmBmCmIndicatorOnRejectionStage();
  }
  //Fin BM CM coupon number and MEMO Number are not get set as enabled in case of billing history
  // so adding this line outside the if loop
  EnableDisableFmBmCmIndicatorOnRejectionStage();
}



function linkingFieldsChangedOnFIMBM() {
    if (!isFromBillingHistory) {
        if ($("#RejectionStage").val() == 1 && $("#FIMBMCMIndicatorId").val() == 1) {
            //disabled following control if rejection is first step rejection for PB
            $('#btnGetLinkingData', '#content').attr('disabled', 'disabled');
            // enabled Save button if rejection is first step rejection of PB.

            $("#btnSave").attr("disabled", false);
            $("#btnSaveAndAddNew").attr("disabled", false);
        }
        else {
            $('#btnGetLinkingData', '#content').attr('disabled', false);
            // Disable Save button if user modifies any control value.
            $("#btnSave").attr("disabled", true);
            $("#btnSaveAndAddNew").attr("disabled", true);
        }
        //EnableDisableFmBmCmIndicatorOnRejectionStage();
    }
    //Fin BM CM coupon number and MEMO Number are not get set as enabled in case of billing history
    // so adding this line outside the if loop
    EnableDisableFmBmCmIndicator();
}






function GetLinkingDetails() {
  if (isFromBillingHistory)
    return;
  var reasonCode = $.trim($("#ReasonCode", '#content').val()).toUpperCase();
  var invNo = $.trim($("#YourInvoiceNumber", '#content').val());
  var billingYr = $.trim($("#YourInvoiceBillingYear", '#content').val());
  var billingMonth = $.trim($("#YourInvoiceBillingMonth", '#content').val());
  var billingPeriod = $.trim($("#YourInvoiceBillingPeriod", '#content').val());
  var fimCouponNo = $.trim($("#FimCouponNumber", '#content').val());
  var fimNo = $.trim($("#FimBMCMNumber", '#content').val());
  var fimBmCmIndicator = $.trim($("#FIMBMCMIndicatorId", '#content').val());
  var rejectionStage = $.trim($("#RejectionStage", '#content').val());
  var rmNo = $.trim($("#YourRejectionNumber", '#content').val());
  var baseMessage = 'Please make sure you have entered values in ALL of the below fields - \r\n';
  var message;

  if (fimBmCmIndicator != '' && rejectionStage == 1) {
    var errorMsg = false;
    var dataParameters = '';

    if (fimBmCmIndicator == 1) { // FIMBMCMInd. : None
      if (invNo != '' && billingYr != '' && billingMonth != '' && billingPeriod != '' && reasonCode != '') {
        dataParameters = { InvoiceNumber: invNo, BillingYear: billingYr, BillingMonth: billingMonth, BillingPeriod: billingPeriod, BillingMemberId: _billingMemberId, BilledMemberId: _billedMemberId, ReasonCode: reasonCode, FimBmCmIndicatorId: fimBmCmIndicator, RejectedInvoiceId: _rejectedInvoiceId, RejectionStage: rejectionStage };
      }
      else {
        errorMsg = true;
        message = baseMessage + '1. Your Invoice number\r\n2. Your Billing Year\r\n3. Your Billing Month\r\n4. Your Billing Period\r\n5. Reason Code';
      }
    }
    else if (fimBmCmIndicator == 2) { // FIM Number
      if (invNo != '' && billingYr != '' && billingMonth != '' && billingPeriod != '' && reasonCode != '' && fimNo != '' && fimCouponNo != '') {
        dataParameters = { InvoiceNumber: invNo, BillingYear: billingYr, BillingMonth: billingMonth, BillingPeriod: billingPeriod, BillingMemberId: _billingMemberId, BilledMemberId: _billedMemberId, ReasonCode: reasonCode, FimBmCmIndicatorId: fimBmCmIndicator, FimBMCMNumber: fimNo, FimCouponNumber: fimCouponNo, RejectedInvoiceId: _rejectedInvoiceId, RejectionStage: rejectionStage };
      }
      else {
        errorMsg = true;
        message = baseMessage + '1. Your Invoice number\r\n2. Your Billing Year\r\n3. Your Billing Month\r\n4. Your Billing Period\r\n5. Reason Code\r\n6. FIM Number\r\n7. FIM Coupon Number';
      }
    }
    else if (fimBmCmIndicator == 3) { // BM
      if (invNo != '' && billingYr != '' && billingMonth != '' && billingPeriod != '' && reasonCode != '' && fimNo != '') {
        dataParameters = { InvoiceNumber: invNo, BillingYear: billingYr, BillingMonth: billingMonth, BillingPeriod: billingPeriod, BillingMemberId: _billingMemberId, BilledMemberId: _billedMemberId, ReasonCode: reasonCode, FimBmCmIndicatorId: fimBmCmIndicator, FimBMCMNumber: fimNo, RejectedInvoiceId: _rejectedInvoiceId, RejectionStage: rejectionStage };
      }
      else {
        errorMsg = true;
        message = baseMessage + '1. Your Invoice number\r\n2. Your Billing Year\r\n3. Your Billing Month\r\n4. Your Billing Period\r\n5. Reason Code\r\n6. Billing Memo Number';
      }
    }
    else if (fimBmCmIndicator == 4) // CM
      if (invNo != '' && billingYr != '' && billingMonth != '' && billingPeriod != '' && reasonCode != '' && fimNo != '') {
        dataParameters = { InvoiceNumber: invNo, BillingYear: billingYr, BillingMonth: billingMonth, BillingPeriod: billingPeriod, BillingMemberId: _billingMemberId, BilledMemberId: _billedMemberId, ReasonCode: reasonCode, FimBmCmIndicatorId: fimBmCmIndicator, FimBMCMNumber: fimNo, RejectedInvoiceId: _rejectedInvoiceId, RejectionStage: rejectionStage };
      }
      else {
        errorMsg = true;
        message = baseMessage + '1. Your Invoice number\r\n2. Your Billing Year\r\n3. Your Billing Month\r\n4. Your Billing Period\r\n5. Reason Code\r\n6. FIM/Billing Memo Number/Credit Memo Number';
      }
      if (dataParameters != '')
        ajaxCallForLinkingDetails(dataParameters);
      else {
        errorMsg = true;
      }
  }
  else {
    if (rejectionStage == 2) {
      if (rejectionStage == 2 && rmNo != '' && invNo != '' && billingYr != '' && billingMonth != '' && billingPeriod != '' && reasonCode != '') {
        if (fimBmCmIndicator != '')
          dataParameters = { InvoiceNumber: invNo, BillingYear: billingYr, BillingMonth: billingMonth, BillingPeriod: billingPeriod, BillingMemberId: _billingMemberId, BilledMemberId: _billedMemberId, ReasonCode: reasonCode, RejectionMemoNumber: rmNo, RejectedInvoiceId: _rejectedInvoiceId, RejectionStage: rejectionStage, FimBmCmIndicatorId: fimBmCmIndicator, FimBMCMNumber: fimNo, FimCouponNumber: fimCouponNo };
        else
          dataParameters = { InvoiceNumber: invNo, BillingYear: billingYr, BillingMonth: billingMonth, BillingPeriod: billingPeriod, BillingMemberId: _billingMemberId, BilledMemberId: _billedMemberId, ReasonCode: reasonCode, RejectionMemoNumber: rmNo, RejectedInvoiceId: _rejectedInvoiceId, RejectionStage: rejectionStage };

        ajaxCallForLinkingDetails(dataParameters);
      }
      else {
        errorMsg = true;
        message = baseMessage + '1. Your Invoice number\r\n2. Your Rejection Memo Number\r\n3. Your Billing Year\r\n4. Your Billing Month\r\n5. Your Billing Period\r\n6. Reason Code';
      }
    }
    else if (rejectionStage == 3) {
    if (rejectionStage == 3 && rmNo != '' && invNo != '' && billingYr != '' && billingMonth != '' && billingPeriod != '' && reasonCode != '') {
      if (fimBmCmIndicator != '')
        dataParameters = { InvoiceNumber: invNo, BillingYear: billingYr, BillingMonth: billingMonth, BillingPeriod: billingPeriod, BillingMemberId: _billingMemberId, BilledMemberId: _billedMemberId, ReasonCode: reasonCode, RejectionMemoNumber: rmNo, RejectedInvoiceId: _rejectedInvoiceId, RejectionStage: rejectionStage, FimBmCmIndicatorId: fimBmCmIndicator, FimBMCMNumber: fimNo, FimCouponNumber: fimCouponNo };
      else
        dataParameters = { InvoiceNumber: invNo, BillingYear: billingYr, BillingMonth: billingMonth, BillingPeriod: billingPeriod, BillingMemberId: _billingMemberId, BilledMemberId: _billedMemberId, ReasonCode: reasonCode, RejectionMemoNumber: rmNo, RejectedInvoiceId: _rejectedInvoiceId, RejectionStage: rejectionStage };

      ajaxCallForLinkingDetails(dataParameters);
    }
    else {
      errorMsg = true;
      message = baseMessage + '1. Your Invoice number\r\n2. Your Rejection Memo Number\r\n3. Your Billing Year\r\n4. Your Billing Month\r\n5. Your Billing Period\r\n6. Reason Code';
    }
    }
    else if (rejectionStage == 0)
      errorMsg = true;
  }
  
  if (errorMsg == true)
  {
    if (message == undefined) {
      message = 'Please enter values in fields marked with - *';
    }
      alert(message);
    }
}

function ajaxCallForLinkingDetails(dataParameters) {
  var jsonData = JSON.stringify(dataParameters);
  $.ajax({
    type: 'POST',
    url: _getLinkingDetailsMethod,
    data: jsonData,
    dataType: "json",
    error: function () {
      $('#IsLinkingSuccessful').val(false);
    },
    success: function (result) {
      PopulateLinkingDetails(result);
    }
  });
}

function PopulateLinkingDetails(response) {

  var hasError = false;
  if (response != null) {
    if (response.ErrorMessage != '') {
      if (response.ErrorMessage.indexOf("Exception") != -1) {
        hasError = true;
      }
    }
    if (hasError != true) {
      if (response.Records.length > 0) {
        $('#CurrencyConversionFactor', '#content').val(response.CurrencyConversionFactor);
        var rejectionMemo = eval(response.Records);
        displayRecords(rejectionMemo);

      }
      else {
        if (response.IsLinkingSuccessful == true) {

          $('#IsLinkingSuccessful', '#content').val(true);
          $('#IsBreakdownAllowed', '#content').val(response.HasBreakdown);
          $('#CurrencyConversionFactor', '#content').val(response.CurrencyConversionFactor);

          if (response.MemoAmount != null) {
            populateFields(response.MemoAmount);
          }
          clearMessageContainer();
          showClientSuccessMessage(response.ErrorMessage + "Linking is successful.");

          // If rejected BillingMemo/CreditMemo has coupon breakdown present disable all Amount fields at Memo level
          if (response.HasBreakdown) {
            // If breakdown is present, set "readOnly" attribute to all text fields within div amountFieldsTable
            $('#vatBreakdown').hide();
            $('#amountFieldsTable input[type=text]').attr("readOnly", true);
          }// end if()
        }
        else {
          clearMessageContainer();
          showClientErrorMessage('Linking failed. Please enter the remaining data before saving if you intend to reject a non-migrated transaction.');
          $('#IsLinkingSuccessful', '#content').val(false);
        }
      }

      // Enable Save button if linking is successful
      $("#btnSave").attr("disabled", false);
      $("#btnSaveAndAddNew").attr("disabled", false);
    }
    else {
      clearMessageContainer();
      showClientErrorMessage(response.ErrorMessage);
      $('#btnGetLinkingData', '#content').attr('disabled', false);

      //Call site.js method to disable the save button.
      setAjaxError();
    }
  }

}

function GetLinkedRecordDetails(selectedRecord) {
  var rejectedMemoId = selectedRecord.MemoId;
  var sourceCode = $.trim($("#SourceCodeId", '#content').val());
  var reasonCode = $.trim($("#ReasonCode", '#content').val());
  var fimBmCmIndicator = $.trim($("#FIMBMCMIndicatorId", '#content').val());
  var rejectionStage = $.trim($("#RejectionStage", '#content').val());

  var dataParameters = '';
  if (sourceCode != '' && reasonCode != '' && rejectionStage != '') {
    if (fimBmCmIndicator == '')
      fimBmCmIndicator = 0;
    dataParameters = { MemoId: rejectedMemoId, SourceCode: sourceCode, ReasonCode: reasonCode, FimBmCmIndicatorId: fimBmCmIndicator, RejectedInvoiceId: _rejectedInvoiceId, RejectionStage: rejectionStage };
  }

  if (dataParameters != '') {
    var jsonData = JSON.stringify(dataParameters);
    $.ajax({
      type: 'POST',
      url: _getLinkedMemoDetailsMethod,
      data: jsonData,
      dataType: "json",
      error: function () {
        $('#IsLinkingSuccessful', '#content').val(false);
      },
      success: function (result) {
        PopulateLinkedMemoDetails(result);
      }
    });
  }

}

function PopulateLinkedMemoDetails(response) {
  var hasError = false;
  if (response != null) {
    if (response.ErrorMessage != '') {
      if (response.ErrorMessage.indexOf("Exception") != -1) {
        hasError = true;
      }
    }
    if (hasError != true) {
      if (response.MemoAmount != null) {

        $('#IsLinkingSuccessful', '#content').val(true);
        $('#IsBreakdownAllowed', '#content').val(response.HasBreakdown);
        populateFields(response.MemoAmount);
        clearMessageContainer();
        showClientSuccessMessage(response.ErrorMessage + "Linking is successful.");
      }
    }
    else {
      clearMessageContainer();
      showClientErrorMessage(response.ErrorMessage);
      $('#btnGetLinkingData', '#content').attr('disabled', 'disabled');
    }
  }
}

function populateFields(selectedRecord) {
  $('#TotalGrossAmountBilled', '#content').val(selectedRecord.TotalGrossAmountBilled);
  $('#TotalGrossAcceptedAmount', '#content').val(selectedRecord.TotalGrossAcceptedAmount);
  $('#TotalGrossDifference', '#content').val(selectedRecord.TotalGrossDifference);

  $('#TotalTaxAmountBilled', '#content').val(selectedRecord.TotalTaxAmountBilled);
  $('#TotalTaxAmountAccepted', '#content').val(selectedRecord.TotalTaxAmountAccepted);
  $('#TotalTaxAmountDifference', '#content').val(selectedRecord.TotalTaxAmountDifference);

  $('#TotalVatAmountBilled', '#content').val(selectedRecord.TotalVatAmountBilled);
  $('#TotalVatAmountAccepted', '#content').val(selectedRecord.TotalVatAmountAccepted);
  $('#TotalVatAmountDifference', '#content').val(selectedRecord.TotalVatAmountDifference);

  $('#AllowedIscAmount', '#content').val(selectedRecord.AllowedIscAmount);
  $('#AcceptedIscAmount', '#content').val(selectedRecord.AcceptedIscAmount);
  $('#IscDifference', '#content').val(selectedRecord.IscDifference);

  $('#AllowedUatpAmount', '#content').val(selectedRecord.AllowedUatpAmount);
  $('#AcceptedUatpAmount', '#content').val(selectedRecord.AcceptedUatpAmount);
  $('#UatpAmountDifference', '#content').val(selectedRecord.UatpAmountDifference);

  $('#AllowedHandlingFee', '#content').val(selectedRecord.AllowedHandlingFee);
  $('#AcceptedHandlingFee', '#content').val(selectedRecord.AcceptedHandlingFee);
  $('#HandlingFeeAmountDifference', '#content').val(selectedRecord.HandlingFeeAmountDifference);

  $('#AllowedOtherCommission', '#content').val(selectedRecord.AllowedOtherCommission);
  $('#AcceptedOtherCommission', '#content').val(selectedRecord.AcceptedOtherCommission);
  $('#OtherCommissionDifference', '#content').val(selectedRecord.OtherCommissionDifference);
  $('#TotalNetRejectAmount', '#content').val(selectedRecord.TotalNetRejectAmount);
}


var selectedRecord = -1;
var linkedRecords;
var linkedRecordsGrid;
// data fields
var BatchNumberDF = 'BatchNumber';
var RecordSequenceNumberDF = 'RecordSequenceNumber';
var RdbColumn = 'RdbColumn';

// display names
var BatchNumberDN = 'Batch Number';
var RecordSequenceNumberDN = 'Record Sequence Number';

function displayRecords(records) {
  linkedRecordsGrid = $('#linkedRecordsGrid', '#content');
  linkedRecordsGrid.jqGrid({
    autoencode: true,
    datatype: 'local',
    width: 475,
    height: 250,
    colNames: ['', BatchNumberDN, RecordSequenceNumberDN],
    colModel: [
                { name: RdbColumn, index: RdbColumn, sortable: false, width: 30, formatter: rdbFormatter }, // for radio button
                {name: BatchNumberDF, index: BatchNumberDF, sortable: false },
                { name: RecordSequenceNumberDF, index: RecordSequenceNumberDF, sortable: false }
              ]
  });

  $('#linkedRecords').dialog({ closeOnEscape: false, title: '', height: 400, width: 500, modal: true, resizable: false });
  // get IDs of all the rows of jqGrid
  var rowIds = linkedRecordsGrid.jqGrid('getDataIDs');

  // iterate through the rows and delete each of them
  for (var i = 0, len = rowIds.length; i < len; i++) {
    var currRow = rowIds[i];
    linkedRecordsGrid.jqGrid('delRowData', currRow);
  }
  selectedRecord = -1;
  // Populate data in tax grid with existing tax records
  if (records != null) {
    records = eval(records);
    linkedRecords = records;
    recordCurrent = 1;
    for (recordCurrent; recordCurrent < records.length + 1; recordCurrent++) {
      row = { RdbColumn: recordCurrent - 1, BatchNumber: records[recordCurrent - 1]["BatchSequenceNumber"], RecordSequenceNumber: records[recordCurrent - 1]["RecordSequenceWithinBatch"] };
      linkedRecordsGrid.jqGrid('addRowData', recordCurrent, row);
    }
  }
}

function rdbFormatter(cellValue, options, cellObject) {
  return '<input type="radio" name="rdbRecord" value=cellValue onclick="setSelectedRecord(' + cellValue + ');" />';
}

function setSelectedRecord(selectedIndex) {
  selectedRecord = selectedIndex;
}

function onLinkingDialogClose() {
  //populate details of selected index
  if (selectedRecord == -1) {
    alert('Please select at least one record.');
    return;
  }
  GetLinkedRecordDetails(linkedRecords[selectedRecord]);
  closeDialog('#linkedRecords');
}

function InitializeLinkingFieldsInEditMode(isLinkingSuccessful) {
  if (isLinkingSuccessful == "True") {
    disableLinkingFields();
  }
}

function disableLinkingFields() {
    $("#YourInvoiceNumber").attr("disabled", "disabled");
    $("#YourInvoiceBillingYear").attr("disabled", "disabled");
    $("#YourInvoiceBillingMonth").attr("disabled", "disabled");
    $("#YourInvoiceBillingPeriod").attr("disabled", "disabled");
    $("#FimCouponNumber").attr("disabled", "disabled");
    $("#FimBMCMNumber").attr("disabled", "disabled");
    $("#FIMBMCMIndicatorId").attr("disabled", "disabled");
}

function InitReferenceData(reasonCodeMethod) {
   registerAutocomplete('ReasonCode', 'ReasonCode', reasonCodeMethod, 0, false, onReasonCodeChange, '', null, '#RejectionStage', onBlankReasonCode);
  $('#RejectionStage').change(function () {
    $('#ReasonCode', '#content').val("");
    $('#ReasonCode').flushCache();
  });
}

// Following function is used to remove readOnly attribute of RejectionMemo amount fields at Memo level depending on ReasonCode if couponBreakdown is not mandatory.
// "selectedValue" parameter contains ReasonCode-IsCouponBreakdownMandatory value 
function onReasonCodeChange(selectedValue) {
    
  // Split selectedValue parameter to retrieve reasonCode 
  $('#ReasonCode').val(selectedValue.split('-')[0]);
  // Split selectedValue parameter to retrieve value whether Coupon breakdown is mandatory for selected reasonCode 
  isCouponBreakdownMandatory = selectedValue.split('-')[1];

  // ID : 200973 - ERROR IN FILE AUG-13
  // pageMode = False; It means the page is RM Create. Irrespective of any reason code, Amound fields should be disable at RM create.

  if (pageMode.toString() != "false") {
      // If coupon breakdown is not mandatory enable selected Memo amount fields, else keep it disabled 
      if (isCouponBreakdownMandatory == "False") {
          // Enable selected fields
          $('#amountFieldsTable input[type=text]').not(id = '#TotalGrossDifference').not(id = '#TotalTaxAmountDifference').not(id = '#TotalVatAmountDifference')
              .not(id = '#IscDifference').not(id = '#UatpAmountDifference').not(id = '#HandlingFeeAmountDifference').not(id = '#OtherCommissionDifference')
              .not(id = '#TotalNetRejectAmount').removeAttr("readOnly");

          // Set 'CouponAwbBreakdownMandatory' hidden field value
          $('#CouponAwbBreakdownMandatory').val(isCouponBreakdownMandatory);
      }
      else {
          // If coupon breakdown is mandatory, set "readOnly" attribute to all text fields within div amountFieldsTable
          $('#amountFieldsTable input[type=text]').attr("readOnly", true);

          // Set 'CouponAwbBreakdownMandatory' hidden field value
          $('#CouponAwbBreakdownMandatory').val(isCouponBreakdownMandatory);
      }
  } else {

      // If coupon breakdown is mandatory, set "readOnly" attribute to all text fields within div amountFieldsTable
      $('#amountFieldsTable input[type=text]').attr("readOnly", true);

      // Set 'CouponAwbBreakdownMandatory' hidden field value
      $('#CouponAwbBreakdownMandatory').val(isCouponBreakdownMandatory);

  }
  // Disabling "Save" button again because it gets enabled when ajax call is completed for ReasonCode autocomplete.
  // Note:- We have written code in site.js which enables all submit buttons on ajax call complete.
  if (!isFromBillingHistory) {
    $("#btnSave").attr("disabled", true);                 
    $("#btnSaveAndAddNew").attr("disabled", true);
  }
} // end onReasonCodeChange()

// Following function keeps Memo fields disabled, if ReasonCode field is blank 
function onBlankReasonCode() {
  $('#amountFieldsTable input[type=text]').attr("readOnly", true);
} // end onBlankReasonCode()

// Following function is used to Enable/Disable memo amount fields depending on whether ReasonCode mandates couponBreakdown and CouponBreakdown exists
function EnableDisableMemoAmountFieldsInEditMode(couponBreakdownExists) {
  // If couponBreakdown is not mandatory and coupon does not exists for current memo, enable selected Memo amount fields
  if ($('#CouponAwbBreakdownMandatory').val() == 'False' && couponBreakdownExists == 'False') {
    $('#amountFieldsTable input[type=text]').not(id = '#TotalGrossDifference').not(id = '#TotalTaxAmountDifference').not(id = '#TotalVatAmountDifference')
        .not(id = '#IscDifference').not(id = '#UatpAmountDifference').not(id = '#HandlingFeeAmountDifference').not(id = '#OtherCommissionDifference')
        .not(id = '#TotalNetRejectAmount').removeAttr("readOnly");
  }
  else {
    $('#amountFieldsTable input[type=text]').attr("readOnly", true);
  }
} // end EnableDisablememoAmountFieldsInEditMode()

// Following function will disable FmBmCM indicator fields RejectionStage equals 2 or 3, else enable
function EnableDisableFmBmCmIndicatorOnRejectionStage() {
  if (pageMode == 'View')
    return;
  if (($("#RejectionStage").val() == 2) || $("#RejectionStage").val() == 3) 
  {
    // If RM being created is standalone execute If block else execute Else block
    if (!isFromBillingHistory) {
      // If RM has coupon breakdown disable FIM related fields
      if (cpnExist == "True") {
        $("#FimBMCMNumber").attr("readOnly", true);
        $("#FimCouponNumber").attr("readOnly", true);
        $("#FIMBMCMIndicatorId").attr("disabled", true);
      }
      else {
        // If FIM Indicator value is 1 i.e. None, disable "FimBMCMNumber" and "FimCouponNumber" fields and clear there values
        if ($("#FIMBMCMIndicatorId").val() == 1) {
          $("#FimBMCMNumber").attr("readOnly", true);
          $("#FimCouponNumber").attr("readOnly", true);
          // Clear field values
          $("#FimBMCMNumber").val('');
          $("#FimCouponNumber").val('');
        }
        // If FIM indicator has value other than 2 i.e. BillingMemo or CreditMemo, disable "FimCouponNumber" field and enable "FimBMCMNumber" field and clear "FimCouponNumber" field value
        else if ($("#FIMBMCMIndicatorId").val() != 2) {
          //$("#FIMBMCMIndicatorId").attr("disabled", false);
          $("#FimCouponNumber").val('');
          $("#FimBMCMNumber").attr("readOnly", false);
          $("#FimCouponNumber").attr("readOnly", true);
        }
        // If FIM Indicator has value 2 i.e. FIM, enable "FimBMCMNumber" and "FimCouponNumber" fields
        else {
          $("#FimBMCMNumber").attr("readOnly", false);
          $("#FimCouponNumber").attr("readOnly", false);
        }

        // If SourceCode is equal to 44 i.e. FIM rejection disable "FIMBMCMIndicatorId" field
        if ($("#SourceCodeId").val() == '44')
          $("#FIMBMCMIndicatorId").attr("disabled", true);

        // If SourceCode is equal to 44 i.e. FIM rejection disable "FIMBMCMIndicatorId" field
        if ($("#SourceCodeId").val() == '44' || $("#SourceCodeId").val() == '45' || $("#SourceCodeId").val() == '46') {
          $("#FIMBMCMIndicatorId").val('2');
          //SCP:37078 Comment below code to make FIMBMCMIndicatorId field to editable.
//          $("#FIMBMCMIndicatorId").attr("disabled", true);
          $("#FimBMCMNumber").attr("readOnly", false);
          $("#FimCouponNumber").attr("readOnly", false);
        }
      }
    }
    else {
      // Disable FIM related fields
        if ($("#FimBMCMNumber").val() != '') {
            $("#FimBMCMNumber").attr("readOnly", true);
        } else {
            $("#FimBMCMNumber").removeAttr("readonly");
        }
        if ($("#FimCouponNumber").val() != '') {
            $("#FimCouponNumber").attr("readOnly", true);
        } else {
            $("#FimCouponNumber").removeAttr("readonly");
        }
      $("#FIMBMCMIndicatorId").attr("disabled", true);
    }
  }
  else if ($("#RejectionStage").val() == 1) 
  {
    // If RM being created is standalone execute If block else execute Else block
    if (!isFromBillingHistory) 
    {
      // If RM has coupon breakdown disable FIM related fields
      if (cpnExist == "True") 
      {
        $("#FimBMCMNumber").attr("readOnly", true);
        $("#FimCouponNumber").attr("readOnly", true);
        $("#FIMBMCMIndicatorId").attr("disabled", true);
      }
      else 
      {
        // If FIM Indicator value is 1 i.e. None, disable "FimBMCMNumber" and "FimCouponNumber" fields and clear there values
        if ($("#FIMBMCMIndicatorId").val() == 1) 
        {
          $("#FimBMCMNumber").attr("readOnly", true);
          $("#FimCouponNumber").attr("readOnly", true);
          // Clear field values
          $("#FimBMCMNumber").val('');
          $("#FimCouponNumber").val('');
        }
        // If FIM indicator has value other than 2 i.e. BillingMemo or CreditMemo, disable "FimCouponNumber" field and enable "FimBMCMNumber" field and clear "FimCouponNumber" field value
        else if ($("#FIMBMCMIndicatorId").val() != 2) 
        {
          //$("#FIMBMCMIndicatorId").attr("disabled", false);
          // Enable fields
          $("#FimCouponNumber").val('');
          $("#FimBMCMNumber").attr("readOnly", false);
          $("#FimCouponNumber").attr("readOnly", true);
        }
        // If FIM Indicator has value 2 i.e. FIM, enable "FimBMCMNumber" and "FimCouponNumber" fields
        else 
        {
          $("#FimBMCMNumber").attr("readOnly", false);
          $("#FimCouponNumber").attr("readOnly", false);
        }

        // If SourceCode is equal to 44 i.e. FIM rejection disable "FIMBMCMIndicatorId" field
        if ($("#SourceCodeId").val() == '44')
          $("#FIMBMCMIndicatorId").attr("disabled", true);
      }
    }
    else {
      // Disable FIM related fields
      $("#FimBMCMNumber").attr("readOnly", true);
      $("#FimCouponNumber").attr("readOnly", true);
      $("#FIMBMCMIndicatorId").attr("disabled", true);
    }
  }
} // end EnableDisableFmBmCmIndicatorOnRejectionStage()



// Following function will disable FmBmCM indicator fields RejectionStage equals 2 or 3, else enable
function EnableDisableFmBmCmIndicator() {
    if (pageMode == 'View')
        return;
    if (($("#RejectionStage").val() == 2) || $("#RejectionStage").val() == 3) {
        // If RM being created is standalone execute If block else execute Else block
        if (!isFromBillingHistory) {
            // If RM has coupon breakdown disable FIM related fields
            if (cpnExist == "True") {
                $("#FimBMCMNumber").attr("readOnly", true);
                $("#FimCouponNumber").attr("readOnly", true);
                $("#FIMBMCMIndicatorId").attr("disabled", true);
            }
            else {
                // If FIM Indicator value is 1 i.e. None, disable "FimBMCMNumber" and "FimCouponNumber" fields and clear there values
                if ($("#FIMBMCMIndicatorId").val() == 1) {
                    $("#FimBMCMNumber").attr("readOnly", true);
                    $("#FimCouponNumber").attr("readOnly", true);
                    // Clear field values
                    $("#FimBMCMNumber").val('');
                    $("#FimCouponNumber").val('');
                }
                // If FIM indicator has value other than 2 i.e. BillingMemo or CreditMemo, disable "FimCouponNumber" field and enable "FimBMCMNumber" field and clear "FimCouponNumber" field value
                else if ($("#FIMBMCMIndicatorId").val() != 2) {
                    //$("#FIMBMCMIndicatorId").attr("disabled", false);
                    $("#FimCouponNumber").val('');
                    $("#FimBMCMNumber").attr("readOnly", false);
                    $("#FimCouponNumber").attr("readOnly", true);
                }
                // If FIM Indicator has value 2 i.e. FIM, enable "FimBMCMNumber" and "FimCouponNumber" fields
                else {
                    $("#FimBMCMNumber").attr("readOnly", false);
                    $("#FimCouponNumber").attr("readOnly", false);
                }

                // If SourceCode is equal to 44 i.e. FIM rejection disable "FIMBMCMIndicatorId" field
                if ($("#SourceCodeId").val() == '44')
                    $("#FIMBMCMIndicatorId").attr("disabled", true);

                // If SourceCode is equal to 44 i.e. FIM rejection disable "FIMBMCMIndicatorId" field
                if ($("#SourceCodeId").val() == '44' || $("#SourceCodeId").val() == '45' || $("#SourceCodeId").val() == '46') {
                    $("#FIMBMCMIndicatorId").val('2');
                    //SCP:37078 Comment below code to make FIMBMCMIndicatorId field to editable.
                    //          $("#FIMBMCMIndicatorId").attr("disabled", true);
                    $("#FimBMCMNumber").attr("readOnly", false);
                    $("#FimCouponNumber").attr("readOnly", false);
                }
            }
        }
        else {
            // Disable FIM related fields
//            if ($("#FimBMCMNumber").val() != '') {
//                $("#FimBMCMNumber").attr("readOnly", true);
//            } else {
//                $("#FimBMCMNumber").removeAttr("readonly");
//            }
//            if ($("#FimCouponNumber").val() != '') {
//                $("#FimCouponNumber").attr("readOnly", true);
//            } else {
//                $("#FimCouponNumber").removeAttr("readonly");
//            }
            $("#FIMBMCMIndicatorId").attr("disabled", true);
        }
    }
    else if ($("#RejectionStage").val() == 1) {
        // If RM being created is standalone execute If block else execute Else block
        if (!isFromBillingHistory) {
            // If RM has coupon breakdown disable FIM related fields
            if (cpnExist == "True") {
                $("#FimBMCMNumber").attr("readOnly", true);
                $("#FimCouponNumber").attr("readOnly", true);
                $("#FIMBMCMIndicatorId").attr("disabled", true);
            }
            else {
                // If FIM Indicator value is 1 i.e. None, disable "FimBMCMNumber" and "FimCouponNumber" fields and clear there values
                if ($("#FIMBMCMIndicatorId").val() == 1) {
                    $("#FimBMCMNumber").attr("readOnly", true);
                    $("#FimCouponNumber").attr("readOnly", true);
                    // Clear field values
                    $("#FimBMCMNumber").val('');
                    $("#FimCouponNumber").val('');
                }
                // If FIM indicator has value other than 2 i.e. BillingMemo or CreditMemo, disable "FimCouponNumber" field and enable "FimBMCMNumber" field and clear "FimCouponNumber" field value
                else if ($("#FIMBMCMIndicatorId").val() != 2) {
                    //$("#FIMBMCMIndicatorId").attr("disabled", false);
                    // Enable fields
                    $("#FimCouponNumber").val('');
                    $("#FimBMCMNumber").attr("readOnly", false);
                    $("#FimCouponNumber").attr("readOnly", true);
                }
                // If FIM Indicator has value 2 i.e. FIM, enable "FimBMCMNumber" and "FimCouponNumber" fields
                else {
                    $("#FimBMCMNumber").attr("readOnly", false);
                    $("#FimCouponNumber").attr("readOnly", false);
                }

                // If SourceCode is equal to 44 i.e. FIM rejection disable "FIMBMCMIndicatorId" field
                if ($("#SourceCodeId").val() == '44')
                    $("#FIMBMCMIndicatorId").attr("disabled", true);
            }
        }
        else {
            // Disable FIM related fields
            $("#FimBMCMNumber").attr("readOnly", true);
            $("#FimCouponNumber").attr("readOnly", true);
            $("#FIMBMCMIndicatorId").attr("disabled", true);
        }
    }
} // end EnableDisableFmBmCmIndicator()





function GetLinkingDetailsOnSave() {
  if (($("#RejectionStage").val() == 1 && $("#FIMBMCMIndicatorId").val() == 1)) {
    GetLinkingDetails();
  }
}


function checkReasonCode1A(isBreakDownExists) {
  var reasonCode = $('#ReasonCode','#content').val();
  if ((reasonCode == '1A' || reasonCode == '1a') && isBreakDownExists == "True") {
    alert('The selected Reason Code allows only single coupon rejection.');
    return false;
  }

  return true;
}

function SetSourceCode(isPostBack) {
  var $sourceCodeId = $('#SourceCodeId');

  if (isPostBack == 'False') {
    if ($sourceCodeId.val() == '1' || $sourceCodeId.val() == '2' || $sourceCodeId.val() == '3' || $sourceCodeId.val() == '8' || $sourceCodeId.val() == '21' || $sourceCodeId.val() == '23' || $sourceCodeId.val() == '25') {
      $sourceCodeId.val('4');
    }
    else if ($sourceCodeId.val() == '14')
      $sourceCodeId.val('44');
    else if ($sourceCodeId.val() == '4')
      $sourceCodeId.val('5');
    else if ($sourceCodeId.val() == '5')
      $sourceCodeId.val('6');
    else if ($sourceCodeId.val() == '44')
      $sourceCodeId.val('45');
    else if ($sourceCodeId.val() == '45')
      $sourceCodeId.val('46');
    else if ($sourceCodeId.val() == '1')
      $sourceCodeId.val('4');
    else if ($sourceCodeId.val() == '90' || $sourceCodeId.val() == '94')
      $sourceCodeId.val('91');
    else if ($sourceCodeId.val() == '91')
      $sourceCodeId.val('92');
    else if ($sourceCodeId.val() == '92')
      $sourceCodeId.val('93');
    else if ($sourceCodeId.val() == '9')
      $sourceCodeId.val('4');
    else if ($sourceCodeId.val() == '24')
      $sourceCodeId.val('4');
    else if ($sourceCodeId.val() != '')
      $sourceCodeId.val('4');
    else
      $sourceCodeId.val('0');
  }
  if ($sourceCodeId.val() == '0') {
    $sourceCodeId.val('');
  }
  else
    $sourceCodeId.attr('readonly', 'true');
}

function SetBillingHistoryControlData() {
  var $FimBMCMNumber = $('#FimBMCMNumber');
  var $FimCouponNumber = $('#FimCouponNumber');
  var $TotalGrossAmountBilled = $('TotalGrossAmountBilled');
  var $TotalGrossAcceptedAmount = $('TotalGrossAcceptedAmount');

  $('#YourInvoiceNumber').addClass('populated');
  $('#YourRejectionNumber').addClass('populated');
  $('#SourceCodeId').addClass('populated');
  $FimBMCMNumber.addClass('populated');
  $FimCouponNumber.addClass('populated');
  $('#YourInvoiceNumber').attr('readonly', 'true');
  $('#YourRejectionNumber').attr('readonly', 'true');  
  $('#YourInvoiceBillingYear').attr('disabled', 'disabled');
  $('#YourInvoiceBillingMonth').attr('disabled', 'disabled');
  $('#YourInvoiceBillingPeriod').attr('disabled', 'disabled');
  $('#FIMBMCMIndicatorId').attr('disabled', 'disabled');
}
