_amountDecimals = 2;
var _rmStage;
isFromBillingHistory = false;
$(document).ready(function () {

  $("#ReasonRemarks").bind("keypress", function () { maxLength(this, 800) });
  $("#ReasonRemarks").bind("paste", function () { maxLengthPaste(this, 800) });

  $("#TotalGrossAcceptedAmount, #TotalGrossAmountBilled").blur(function () {
    setAmountDiff("#TotalGrossAmountBilled", "#TotalGrossAcceptedAmount", "#TotalGrossDifference");
    calculateAmount();
  });

  $("#TotalTaxAmountAccepted, #TotalTaxAmountBilled").blur(function () {
    setAmountDiff("#TotalTaxAmountBilled", "#TotalTaxAmountAccepted", "#TotalTaxAmountDifference");
    calculateAmount();
  });

  $("#TotalVatAmountAccepted, #TotalVatAmountBilled").blur(function () {
    setAmountDiff("#TotalVatAmountBilled", "#TotalVatAmountAccepted", "#TotalVatAmountDifference");
    if ($('#TotalVatAmountDifference').val() != 0)
      $('#vatBreakdown').show();
    calculateAmount();
  });

  $("#AcceptedIscAmount, #AllowedIscAmount").blur(function () {
    setAmountDiff("#AllowedIscAmount", "#AcceptedIscAmount", "#IscDifference");
    calculateAmount();
  });

  $("#AcceptedUatpAmount, #AllowedUatpAmount").blur(function () {
    setAmountDiff("#AllowedUatpAmount", "#AcceptedUatpAmount", "#UatpAmountDifference");
    calculateAmount();
  });

  $("#AcceptedHandlingFee, #AllowedHandlingFee").blur(function () {
    setAmountDiff("#AllowedHandlingFee", "#AcceptedHandlingFee", "#HandlingFeeAmountDifference");
    calculateAmount();
  });

  $("#AcceptedOtherCommission, #AllowedOtherCommission").blur(function () {
    setAmountDiff("#AllowedOtherCommission", "#AcceptedOtherCommission", "#OtherCommissionDifference");
    calculateAmount();
  });

  $("#ReasonCode").blur(function () {
    setControlAccess($("#ReasonCode").val(),reasonCode);
  });

  $("#YourInvoiceNumber").blur(function () {
    setControlAccess($("#YourInvoiceNumber").val(), yourInvoiceNumber);
  });


  $("#YourInvoiceBillingYear").blur(function () {
    setControlAccess($("#YourInvoiceBillingYear").val(), yourBillingYear);
  });

  $("#YourInvoiceBillingMonth").blur(function () {
    setControlAccess($("#YourInvoiceBillingMonth").val(), yourBillingMonth);
  });

  $("#YourInvoiceBillingPeriod").blur(function () {
    setControlAccess($("#YourInvoiceBillingPeriod").val(), yourBillingPeriod);
  });
});
function setControlAccess(newValue, oldValue) {
  if (isFromBillingHistory)
    return true;
  if (isFormDEViaISValue == 'True') {
    if (newValue != oldValue)
    $('#SaveButton').attr('disabled', 'disabled');
  }
}

function calculateAmount() {
  calculateNetAmount("#TotalGrossDifference", "#TotalTaxAmountDifference", "#TotalVatAmountDifference", "#IscDifference", "#UatpAmountDifference", "#HandlingFeeAmountDifference", "#OtherCommissionDifference", "#TotalNetRejectAmount");
}

function setAmountDiff(sourceControl1Id, sourceControl2Id, targetControlId) {
  var sourceControl1Value = 0;
  sourceControl1Value = $(sourceControl1Id).val();
  var sourceControl2Value = 0;
  sourceControl2Value = $(sourceControl2Id).val();
  var difference = 0;
  if (_rmStage == 2)
    difference = sourceControl2Value - sourceControl1Value;
  else if (_rmStage == 3)
    difference = sourceControl1Value - sourceControl2Value;  
  
  if (!isNaN(difference))
    $(targetControlId).val(difference.toFixed(_amountDecimals));
  else
    $(targetControlId).val(Number(0).toFixed(_amountDecimals));
}

function calculateNetAmount(sourceControl1, sourceControl2, sourceControl3, sourceControl4, sourceControl5, sourceControl6, sourceControl7, targetControl) {
  var grossAmount;
  var taxAmount;
  var vatAmount;
  var iscAmount;
  var uatpAmount;
  var hfAmount;
  var ocAmount;

  if (!isNaN(Number($(sourceControl1).val())))
    grossAmount = Number($(sourceControl1).val());
  else
    grossAmount = 0;

  if (!isNaN(Number($(sourceControl2).val())))
    taxAmount = Number($(sourceControl2).val());
  else
    taxAmount = 0;

  if (!isNaN(Number($(sourceControl3).val())))
    vatAmount = Number($(sourceControl3).val());
  else
    vatAmount = 0;

  if (!isNaN(Number($(sourceControl4).val())))
    iscAmount = Number($(sourceControl4).val());
  else
    iscAmount = 0;

  if (!isNaN(Number($(sourceControl5).val())))
    uatpAmount = Number($(sourceControl5).val());
  else
    uatpAmount = 0;

  if (!isNaN(Number($(sourceControl6).val())))
    hfAmount = Number($(sourceControl6).val());
  else
    hfAmount = 0;

  if (!isNaN(Number($(sourceControl7).val())))
    ocAmount = Number($(sourceControl7).val());
  else
    ocAmount = 0;

  var totalNetRejectAmount = grossAmount + iscAmount + ocAmount + uatpAmount + hfAmount + taxAmount + vatAmount;
  if (!isNaN(totalNetRejectAmount)) {
    $(targetControl).val(totalNetRejectAmount.toFixed(_amountDecimals));
    var samplingConstant = $('#SamplingConstant').val();
    if (!isNaN(samplingConstant)) {
      var totalnetAmtWithSamConst = totalNetRejectAmount * samplingConstant;
      $('#TotalNetRejectAmountAfterSamplingConstant').val(totalnetAmtWithSamConst.toFixed(_amountDecimals));
     }
  }

}


function InitializeEditRM(isCouponExists) {
  validateRM(isCouponExists);

  if (isCouponExists == 'True')
    InitializeEditRMFields();
}

function validateRM(isCouponExists) {
  SetPageWaterMark();

  $("#rejectionMemoForm").validate({
    rules: {
      BatchSequenceNumber: "required",
      RecordSequenceWithinBatch: "required",
      SourceCodeId: "required",
      RejectionMemoNumber: "required",
      ReasonCode: "required",
      YourInvoiceNumber: "required",
      YourInvoiceBillingYear: "required",
      YourInvoiceBillingMonth: "required",
      YourInvoiceBillingPeriod: "required",
      YourRejectionNumber: "required"
    },
    messages: {
      BatchSequenceNumber: { required: "Batch Number Required" },
      RecordSequenceWithinBatch: { required: "Sequence Number Required" },
      SourceCodeId: "Source Code Required",
      RejectionMemoNumber: "Rejection Memo Number Required",
      ReasonCode: "Reason Code Required",
      YourInvoiceNumber: "Your Invoice Number Required/Invalid Your Invoice Number",
      YourInvoiceBillingYear: "Your Invoice Billing year Required",
      YourInvoiceBillingMonth: "Your Invoice Billing Month Required",
      YourInvoiceBillingPeriod: "Your Invoice Billing Period Required",
      YourRejectionNumber: "Your Rejection Memo Number Required"
    },
    invalidHandler: function (form, validator) {

      if (isCouponExists == 'True') {
        InitializeEditRMFields();
      }
      $.watermark.showAll();
    },
    submitHandler: function (form) {
      $("#ReasonCode").removeAttr('readonly');
      $("#YourInvoiceNumber").removeAttr('readonly');
      $('#YourInvoiceBillingYear').removeAttr('disabled');
      $('#YourInvoiceBillingMonth').removeAttr('disabled');
      $('#YourInvoiceBillingPeriod').removeAttr('disabled');

      calculateAmounts();
      // Call onSubmitHandler() function which will disable Submit buttons and will submit the form
      onSubmitHandler(form);
      // form.submit();
    }
  });

  trackFormChanges('rejectionMemoForm');
}

// This will be called when one or more coupon breakdown records exist.
function InitializeEditRMFields() {
  $("#ReasonCode").attr("readonly", "true");
  $("#ReasonCode").autocomplete({ disabled: true });
  $("#YourInvoiceNumber").attr("readonly", "true");
  $("#YourRejectionNumber").attr("readonly", "true");
  $("#YourInvoiceBillingYear").attr("disabled", "disabled");
  $("#YourInvoiceBillingMonth").attr("disabled", "disabled");
  $("#YourInvoiceBillingPeriod").attr("disabled", "disabled");
  $('#FetchButton').hide();
}

function InitializeRMCreate() {
  $('input[type=text]:not(.populated)').removeAttr('value');
  validateRM(false);  
}

var getLinkedFormDEMethod;
var billingMember;
var billedMember;
var provBillingMonth;
var provBillingYear;
var invoiceId;
var isFormDEViaISValue;
function InitializeFormFLinking(isFormDEViaIS, getLinkedFormDEMethodName, billingMemberId, billedMemberId, provisionalBillingMonth, provisionalBillingYear, invoiceGuid) {
  isFormDEViaISValue = isFormDEViaIS;
  if (isFormDEViaIS == 'True') {

    $('#FetchButton').show();
    getLinkedFormDEMethod = getLinkedFormDEMethodName;
    billingMember = billingMemberId;
    billedMember = billedMemberId;
    provBillingMonth = provisionalBillingMonth;
    provBillingYear = provisionalBillingYear;
    invoiceId = invoiceGuid;
    $('#FetchButton').click(function () { getFormDELinkingDetails() });

    if (!isFromBillingHistory)
      $('#SaveButton').attr('disabled', 'disabled');
  }
  else {
    registerBilledAmountsOnBlurEvent();
  }
}

function registerBilledAmountsOnBlurEvent() {
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
    if ($('#TotalVatAmountDifference').val() != 0)
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
}

function calculateAmounts() {
  setAmountDiff("#TotalGrossAmountBilled", "#TotalGrossAcceptedAmount", "#TotalGrossDifference");
  setAmountDiff("#TotalTaxAmountBilled", "#TotalTaxAmountAccepted", "#TotalTaxAmountDifference");
  setAmountDiff("#TotalVatAmountBilled", "#TotalVatAmountAccepted", "#TotalVatAmountDifference");
  if ($('#TotalVatAmountDifference').val() != 0)
    $('#vatBreakdown').show();
  setAmountDiff("#AllowedIscAmount", "#AcceptedIscAmount", "#IscDifference");
  setAmountDiff("#AllowedUatpAmount", "#AcceptedUatpAmount", "#UatpAmountDifference");
  setAmountDiff("#AllowedHandlingFee", "#AcceptedHandlingFee", "#HandlingFeeAmountDifference");
  
  calculateAmount();
}

var yourInvoiceNumber ;
var yourBillingYear ;
var yourBillingMonth ;
var yourBillingPeriod;
var reasonCode;
function getFormDELinkingDetails() {
  if (isFromBillingHistory)
    return true;
  reasonCode = $.trim($('#ReasonCode').val()).toUpperCase();
  var $yourInvoiceNumber = $('#YourInvoiceNumber');
  var $yourBillingYear = $('#YourInvoiceBillingYear');
  var $yourBillingMonth = $('#YourInvoiceBillingMonth');
  var $yourBillingPeriod = $('#YourInvoiceBillingPeriod');
   yourInvoiceNumber = $yourInvoiceNumber.val();
   yourBillingYear = $yourBillingYear.val();
   yourBillingMonth = $yourBillingMonth.val();
   yourBillingPeriod = $yourBillingPeriod.val();
   
  if (reasonCode != '' && yourInvoiceNumber != '' && yourBillingYear != '' && yourBillingMonth != '' && yourBillingPeriod != '') {
    $.ajax({
      type: "POST",
      url: getLinkedFormDEMethod,
      data: { reasonCode: reasonCode, yourInvoiceNumber: yourInvoiceNumber, yourBillingMonth: yourBillingMonth, yourBillingYear: yourBillingYear, yourBillingPeriod: yourBillingPeriod
      , billingMemberId: billingMember, billedMemberId: billedMember, rejectingInvoiceId: invoiceId, provisionalBillingMonth: provBillingMonth, provisionalBillingYear: provBillingYear
      },
      dataType: "json",
      success: function (result) {
        var hasError = false;
        if (result != null) {
          if (result.ErrorMessage != '') {
            if (result.ErrorMessage.indexOf("Exception") != -1) {
              hasError = true;
            }
          }

          if (hasError != true) {
            // set currency conversion factor
            showClientSuccessMessage(result.ErrorMessage + ' Linking is successful.');
            $('#SaveButton').attr('disabled', false);
            $('#CurrencyConversionFactor').val(result.CurrencyConversionFactor);
          }
          else {
            // Form D/E member migrated but data not found/ Reason code makes coupon breakdown mandatory 
            // but rejected invoice does not contain coupon breakdown.
            showClientErrorMessage(result.ErrorMessage);
            //Set the error is true.
            setAjaxError();
          }
        }
      }
    });
  }
  else {
    alert('Please select Reason Code, Your Invoice Number, Your Billing Month, Your Billing Year and Your Billing Period values.');
  }
}

var _getLinkingDetailsMethod;
var _getLinkedMemoDetailsMethod;
var _resonCode;
var _yourInvoiceNumber;
var _yourInvoiceBillingMonth;
var _yourInvoiceBillingYear;
var _yourRejectionNumber;
var _yourInvoiceBillingPeriod;

function InitializeFormXFLinking(isLinkingSuccessful, getLinkingDetailsMethod, getLinkedMemoDetailsMethod, billingMemberId, billedMemberId, rejectingInvoiceId, provisionalBillingMonth, provisionalBillingYear) {
  if (isLinkingSuccessful == 'True') {
    $('#FetchButton').show();
    $('#ReasonCode, #YourInvoiceNumber, #YourInvoiceBillingYear, #YourInvoiceBillingMonth, #YourRejectionNumber, #YourInvoiceBillingPeriod', '#content').blur(function () {
      linkingFieldsChanged();
    });

    _getLinkingDetailsMethod = getLinkingDetailsMethod;
    _getLinkedMemoDetailsMethod = getLinkedMemoDetailsMethod;
   
    //Billing and billed member are considered for rejected invoice.
    billingMember = billedMemberId;
    billedMember = billingMemberId;
    invoiceId = rejectingInvoiceId;
    provBillingMonth = provisionalBillingMonth;
    provBillingYear = provisionalBillingYear;
    $('#FetchButton').click(function () { getFormFLinkingDetails(); });
  }
  else {
    registerBilledAmountsOnBlurEvent();
  }
}

function getFormFLinkingDetails() {
  _resonCode = $.trim($("#ReasonCode", '#content').val()).toUpperCase();
  _yourInvoiceNumber = $.trim($("#YourInvoiceNumber", '#content').val());
  _yourInvoiceBillingYear = $.trim($("#YourInvoiceBillingYear", '#content').val());
  _yourInvoiceBillingMonth = $.trim($("#YourInvoiceBillingMonth", '#content').val());
  _yourRejectionNumber = $.trim($("#YourRejectionNumber", '#content').val());
  _yourInvoiceBillingPeriod = $.trim($("#YourInvoiceBillingPeriod", '#content').val());  
  if (_resonCode == '' || _yourInvoiceNumber == '' || _yourInvoiceBillingYear == '' || _yourInvoiceBillingMonth == '' || _yourInvoiceBillingPeriod == '' || _yourRejectionNumber == '') {
    alert('Please fill all the below fields:\n 1. Reason code\n2. Your Invoice Number\n3. Your Rejection Memo Number\n4. Your Billing Year\n5. Your Billing Month\n6. Your Billing Period');
    return;
  }
  $.ajax({
    type: 'POST',
    url: _getLinkingDetailsMethod,
    data: { reasonCode: _resonCode, yourInvoiceNumber: _yourInvoiceNumber, yourBillingMonth: _yourInvoiceBillingMonth,
      yourBillingYear: _yourInvoiceBillingYear, yourBillingPeriod: _yourInvoiceBillingPeriod, yourRejectionMemoNumber: _yourRejectionNumber, 
     billingMemberId: billingMember, billedMemberId: billedMember, rejectingInvoiceId: invoiceId, 
     provisionalBillingMonth: provBillingMonth, provisionalBillingYear: provBillingYear },
    dataType: "json",
    success: function (result) {
      PopulateLinkingDetails(result);
    }
  });
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
      //  $linkedCouponsGrid.addRowData($couponCurrent + 1, row);
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

function linkingFieldsChanged() {
  if (_resonCode != $.trim($("#ReasonCode", '#content').val()) ||
  _yourInvoiceNumber != $.trim($("#YourInvoiceNumber", '#content').val()) ||
  _yourInvoiceBillingYear != $.trim($("#YourInvoiceBillingYear", '#content').val()) ||
  _yourInvoiceBillingMonth != $.trim($("#YourInvoiceBillingMonth", '#content').val()) ||
  _yourRejectionNumber != $.trim($("#YourRejectionNumber", '#content').val()) ||
  _yourInvoiceBillingPeriod != $.trim($("#YourInvoiceBillingPeriod", '#content').val()))
  {
  // Enable Fetch button
   $('#btnGetLinkingData', '#content').attr('disabled', false);
   // Disable Save button if user modifies any control value.
  if(!isFromBillingHistory)
  $("#btnSaveAndAddNew").attr("disabled", true);
  }
}

function InitializeFormXFLinkingInCreateMode(isLinkingSuccessful) {
  if (isLinkingSuccessful == 'True') {
    // Save button is disabled in the start.
    if (!isFromBillingHistory)
    $("#btnSaveAndAddNew").attr("disabled", true);
  }
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
      $("#btnSaveAndAddNew").attr("disabled", false);
      if (response.Records.length > 0) {
        $('#CurrencyConversionFactor', '#content').val(response.CurrencyConversionFactor);
        var rejectionMemo = eval(response.Records);
        displayRecords(rejectionMemo);
      }
      else {
          $('#IsBreakdownAllowed', '#content').val(response.HasBreakdown);
          $('#CurrencyConversionFactor', '#content').val(response.CurrencyConversionFactor);

          if (response.MemoAmount != null) {
            populateFields(response.MemoAmount);
          }

          showClientSuccessMessage(response.ErrorMessage + " Linking is successful.");
        }
      // Enable Save button if linking is successful
    }
    else {
      showClientErrorMessage(response.ErrorMessage);
      $('#btnGetLinkingData', '#content').attr('disabled', 'disabled');

      //Set the error is true.
      setAjaxError();
    }
  }
}

// To get memo amounts on Form XF RM for a selected memo record.
function GetLinkedRecordDetails(selectedRecord) {
  var rejectedMemoId = selectedRecord.MemoId;
  var sourceCode = $.trim($("#SourceCodeId", '#content').val());
  var reasonCode = $.trim($("#ReasonCode", '#content').val());
  var fimBmCmIndicator = 10;
  var rejectionStage = 3;

  var dataParameters = '';
  if (sourceCode == '' || reasonCode == '') {    
    alert('Please enter Source Code and Reason Code');
  }
  else
  {
    dataParameters = { MemoId: rejectedMemoId, SourceCode: sourceCode, ReasonCode: reasonCode, FimBmCmIndicatorId: fimBmCmIndicator, RejectedInvoiceId: invoiceId, RejectionStage: rejectionStage }; 
  
    var jsonData = JSON.stringify(dataParameters);
    $.ajax({
      type: 'POST',
      url: _getLinkedMemoDetailsMethod,
      data: jsonData,
      dataType: "json",
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
         $('#IsBreakdownAllowed', '#content').val(response.HasBreakdown);
         populateFields(response.MemoAmount);
         showClientSuccessMessage(response.ErrorMessage + "Linking is successful.");
       }
     }
     else {
       showClientErrorMessage(response.ErrorMessage);
       //Set the error is true.
       setAjaxError();
       $('#btnGetLinkingData', '#content').attr('disabled', 'disabled');
     }
   }
 }

 function populateFields(selectedRecord){
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
   var $totalNetRejectAmount = $('#TotalNetRejectAmount', '#content');
   $totalNetRejectAmount.val(selectedRecord.TotalNetRejectAmount);
   var samplingConstant = $('#SamplingConstant', '#content').val();
   // populate net Reject amount after sampling constant
   $('#TotalNetRejectAmountAfterSamplingConstant', '#content').val($totalNetRejectAmount.val());// * samplingConstant);
 }

// Following function is executed when ReasonCode is modified and is used to enable disable Memo amount fields at Rejectionmemo level depending on ReasonCode.
// "selectedValue" parameter contains ReasonCode-IsCouponBreakdownMandatory value 
function onReasonCodeChangeForFormF(selectedValue) {
  // Split selectedValue parameter to retrieve reasonCode 
  $('#ReasonCode').val(selectedValue.split('-')[0]);
  // Split selectedValue parameter to retrieve value whether Coupon breakdown is mandatory for selected reasonCode 
  isCouponBreakdownMandatory = selectedValue.split('-')[1];
  // If coupon breakdown is not mandatory enable selected Memo amount fields, else keep it disabled 
  if (isCouponBreakdownMandatory == "False") {
    // Enable selected fields
    $('#amountFieldsTable input[type=text]').not(id = '#TotalGrossDifference').not(id = '#TotalTaxAmountDifference').not(id = '#TotalVatAmountDifference')
            .not(id = '#IscDifference').not(id = '#UatpAmountDifference').not(id = '#HandlingFeeAmountDifference').not(id = '#OtherCommissionDifference')
            .not(id = '#TotalNetRejectAmount').not(id = '#SamplingConstant').not(id = '#TotalNetRejectAmountAfterSamplingConstant').removeAttr("readOnly");

    // Set 'CouponAwbBreakdownMandatory' hidden field value
    $('#CouponAwbBreakdownMandatory').val(isCouponBreakdownMandatory);
  }
  else {
    $('#amountFieldsTable input[type=text]').attr("readOnly", true);

    // Set 'CouponAwbBreakdownMandatory' hidden field value
    $('#CouponAwbBreakdownMandatory').val(isCouponBreakdownMandatory);
  }
} // end onReasonCodeChange()

// Following function keeps Memo fields disabled, if ReasonCode field is blank 
function onBlankReasonCodeForFormF() {
  $('#amountFieldsTable input[type=text]').attr("readOnly", true);
} // end onBlankReasonCode()

// Following function is  executed when ReasonCode is modified and is used to enable disable Memo amount fields at Rejectionmemo level depending on ReasonCode.
// "selectedValue" parameter contains ReasonCode-IsCouponBreakdownMandatory value 
function onReasonCodeChangeForFormXF(selectedValue) {
  // Split selectedValue parameter to retrieve reasonCode 
  $('#ReasonCode').val(selectedValue.split('-')[0]);
  // Split selectedValue parameter to retrieve value whether Coupon breakdown is mandatory for selected reasonCode 
  isCouponBreakdownMandatory = selectedValue.split('-')[1];

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
} // end onReasonCodeChange()

// Following function keeps Memo fields disabled, if ReasonCode field is blank 
function onBlankReasonCodeForFormXF() {
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

function checkReasonCode1A(isBreakDownExists) {
  var reasonCode = $('#ReasonCode', '#content').val();
  if ((reasonCode == '1A' || reasonCode == '1a') && isBreakDownExists == "True") {
    alert('The selected Reason Code allows only single coupon rejection.');
    return false;
  }

  return true;
}