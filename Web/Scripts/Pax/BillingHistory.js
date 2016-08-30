var corrUrl, invUrl, auditTrailUrl, initiateCorrUrl, memoType, clearSearchUrl, billingCode, initiateDuplicateRejectionUrl, checkIfBMExists;
/* constants from TransactionType enum */
var TransactionTypeRM1 = 2;
var TransactionTypeRM2 = 3;
var TransactionTypeRM3 = 4;
var TransactionTypeFormF = 10;
var TransactionTypeFormXf = 11;

function InitialiseBillingHistory(corrURL, invoiceURL, initiateCorrURL, auditTrailURL, clearSearchURL, initiateDuplicateRejectionURL, checkIfBillingMemoExists, isCorrespondenceOutsideTimeLimit) {
  corrUrl = corrURL;
  initiateCorrUrl = initiateCorrURL;
  invUrl = invoiceURL;
  _isCorrespondenceOutsideTimeLimitMethod = isCorrespondenceOutsideTimeLimit;
  clearSearchUrl = clearSearchURL;
  initiateDuplicateRejectionUrl = initiateDuplicateRejectionURL;
  checkIfBMExists = checkIfBillingMemoExists;
  SetTransaction();

  

  $("#CorrespondenceStatusId").bind("change", SetSubStatus);
  $("#CorrespondenceSubStatusId").bind("change", SetCorrespondenceOwner);


  $("#corrSearchCriteria").validate({
    rules: {
      FromDate: "required",
      ToDate: "required",
      CorrespondenceStatusId: "required"
    },
    messages: {
      FromDate: "From Date Required",
      ToDate: "To Date Required",
      CorrespondenceStatusId: "Correspondence Status Required"
    }
  });
  $("#invoiceSearchCriteria").validate({

      rules: {
      //CMP 500
      //BillingYearMonth: "required",
      //BillingPeriod: "required",
      //BilledMemberCode: "required",
      BillingTypeId: "required"
    },
      messages: {
      //CMP 500
      //BillingYearMonth: "Billing Year / Month Required",
      //BillingPeriod: "Billing Period Required",
      //BilledMemberCode: "Member Code Requiredgfgfdg",
      BillingTypeId: "Billing Type Required"
    }
  });

  //CMP#500
  $('#Search').click(function (evt) {

      var _billingTypeId = $("#BillingTypeId").val();
      var _billingYearMonth = $("#BillingYearMonth").val();
      var _billingCode = $("#BillingCode").val();
      var _billedMemberId = 0;
      _billedMemberId = ($("#BilledMemberCode").val() != "" && $("#BilledMemberId").val() != 0) ? $("#BilledMemberId").val() : 0;
      var _transactionMemoTypeId = $("#MemoTypeId option:selected").val();
      var _memoNumber = $("#MemoNumber").val();
      var _issuingAirline = $("#IssuingAirline").val();
      var _documentNumber = $("#DocumentNumber").val();
      //alert("_billingTypeId:" + _billingTypeId + "#_billingYearMonth:" + _billingYearMonth + "#_billingCode:" + _billingCode + "#_billedMemberId:" + _billedMemberId + "#_transactionMemoTypeId:" + _transactionMemoTypeId + "#_memoNumber:" + _memoNumber + "#_issuingAirline:" + _issuingAirline + "#_documentNumber:" + _documentNumber);
      var message = "All mandatory search fields have not been provided. Please review the search criteria provided and ensure that they comply with one of the options allowed for this screen. Move the mouse over each option to know which fields are mandatory – <br/>";
      message += "<a style='cursor: hand;' id='option1'>Option 1<div id='option1div' class='errortooltip'>Mandatory search fields for Option 1:   <br/>(i) Billing Type (Payables or Receivables)   <br/>(ii) Billing Year / Month (specific value other than Please Select)<br/>(iii) Billing Code (specific value other than All)<br/>(iv) Member Code</div>&nbsp;&nbsp;</a><script language='javascript'>$('#option1').bind('mousemove', function (event) {$('#option1div').css({top: event.pageY + 5 + 'px',left: event.pageX + 5 + 'px'}).show();}).bind('mouseout', function () {$('#option1div').hide();});</script>";
      message += "<a style='cursor: hand;' id='option2'>Option 2<div id='option2div' class='errortooltip'>Mandatory search fields for Option 2:   <br/>(i) Billing Type (Payables or Receivables)   <br/>(ii) Transaction Type selected as Rejection Memo, Billing Memo or Credit Memo<br/>(iii) Memo Number</div>&nbsp;&nbsp;</a><script language='javascript'>$('#option2').bind('mousemove', function (event) {$('#option2div').css({top: event.pageY + 5 + 'px',left: event.pageX + 5 + 'px'}).show();}).bind('mouseout', function () {$('#option2div').hide();});</script>";
      message += "<a style='cursor: hand;' id='option3'>Option 3<div id='option3div' class='errortooltip'>Mandatory search fields for Option 3:   <br/>(i) Billing Type (Payables or Receivables)   <br/>(ii) Transaction Type selected as Prime Coupon <br/>(iii) Issuing Airline <br/>(iv) Document Number</div>&nbsp;&nbsp;</a><script language='javascript'>$('#option3').bind('mousemove', function (event) {$('#option3div').css({top: event.pageY + 5 + 'px',left: event.pageX + 5 + 'px'}).show();}).bind('mouseout', function () {$('#option3div').hide();});</script>";

      var isValidSearchCriteria = false;

      if (_billingTypeId == "") {
          isValidSearchCriteria = false;
      }
      else {
          //Group 1: Billing Type, Billing Year/Month, MemberCode
          if (_billingYearMonth != "" && _billedMemberId != 0 && _billingCode != -1) {
              isValidSearchCriteria = true;
          }
          //Group 2: BillingType, Tranaction Type, Member Code
          else if (_transactionMemoTypeId != "" && (_transactionMemoTypeId == 4 || _transactionMemoTypeId == 5 || _transactionMemoTypeId == 6) && _memoNumber != "") {
              isValidSearchCriteria = true;
          }
          //Group 3: BillingType, Transaction Type, Issuing Airline, Document Number 
          else if (_transactionMemoTypeId != "" && _transactionMemoTypeId == 1 && _documentNumber != "" && _issuingAirline != "") {
              isValidSearchCriteria = true;
          }
      }
      if (!isValidSearchCriteria) {
          showClientErrorMessage(message);
          evt.preventDefault();
      }
  });


  /*
  SCP ID : 105268 - FW: Cargo Rejection for the same reason //SIS
  Code Change For: When multiple rejection memos are selected(using Select All check box) in billing 
  history screen 'initiate rejection' memo button should be disabled.
  */
  jQuery("#cb_BHSearchResultsGrid").click(function () {

      if ($(this).is(':checked')) {
          $("#InitiateRejection").attr('disabled', 'disabled');
          if ($.browser.mozilla) {
              $("#InitiateRejection").removeClass('primaryButton');
              $("#InitiateRejection").addClass('disabledButtonClassForMozilla');
          }
      }
      else {
          $("#InitiateRejection").removeAttr('disabled');
          if ($.browser.mozilla) {
              $("#InitiateRejection").removeClass('disabledButtonClassForMozilla');
              $("#InitiateRejection").addClass('primaryButton');
          }
      }

  });
  // End of SCP ID : 105268

  $("#BillingCode").change(function () {
      SetControlValues(false);
      $("#ReasonCodeId").val("");
      var _transactionMemoTypeId = $("#MemoTypeId option:selected").val();
      var _billingCodeId = $("#BillingCode option:selected").val();
      var _rejectionIdId = $("#RejectionStageId option:selected").val();

      disableReasonCodeForPrime(_transactionMemoTypeId, _billingCodeId, _rejectionIdId);
  });

  $("#MemoTypeId").change(function () {
      SetTransControl();
      $("#ReasonCodeId").val("");
      var _transactionMemoTypeId = $("#MemoTypeId option:selected").val();
      var _billingCodeId = $("#BillingCode option:selected").val();
      var _rejectionIdId = $("#RejectionStageId option:selected").val();

      disableReasonCodeForPrime(_transactionMemoTypeId, _billingCodeId, _rejectionIdId);
  });

  $("#RejectionStageId").change(function () {
      $("#ReasonCodeId").val("");
      var _transactionMemoTypeId = $("#MemoTypeId option:selected").val();
      var _billingCodeId = $("#BillingCode option:selected").val();
      var _rejectionIdId = $("#RejectionStageId option:selected").val();

      disableReasonCodeForPrime(_transactionMemoTypeId, _billingCodeId, _rejectionIdId);
  });

  //SCP 121308 : Reason Codes in PAX billing history screen appear multiple times.
  //need to make reason code textbox disable if transaction type = prime, rejection = "" and billing code = Form DE, Form AB, "".
  function disableReasonCodeForPrime(transactionTypeId, billingCode, rejectionId) {
      if ((transactionTypeId == 1 && billingCode == -1 && rejectionId == "") || ((transactionTypeId == -1 || transactionTypeId == 1) && (billingCode == 3 || billingCode == 5) && rejectionId == "")) {
          $("#ReasonCodeId").attr('disabled', 'disabled');
      }
      else {
          $("#ReasonCodeId").attr('disabled', false);
      }
  }


  SetControlValues(true);

  $("#FromDate").change(function () {
    var dateComparisonResult = validateDateRange('FromDate', 'ToDate');
    if (!dateComparisonResult) {
      alert("From date must be lesser than to date");
      $("#FromDate").val('');
    }
  });

  $("#ToDate").change(function () {
    var dateComparisonResult = validateDateRange('FromDate', 'ToDate');
    if (!dateComparisonResult) {
      alert("To date must be greater than from date");
      $("#ToDate").val('');
    }
  });

}

//Display Rejection Memo linked with correspondence.
//CMP612: Changes to PAX CGO Correspondence Audit Trail Download 
function ShowLinkedCorrRejectionMemo(correspondenceId) {
  $.ajax({
    type: "GET",
    url: "../Pax/BillingHistory/ShowLinkedRejectionMemo?correspondenceId=" + correspondenceId,
    dataType: "html",
    success: function (response) {
      $('#showRejection').html(response);
    },
    error: function (xhr, ajaxOptions, thrownError) {
      alert(xhr.statusText);
      alert(thrownError);
    }
  });
}

//Generate audit trail for rejection memo which is linked with correspondence.
//CMP612: Changes to PAX CGO Correspondence Audit Trail Download
function GenerateAuditTrailLinkedCorrRMs() {
  $('#successMessageForRM').addClass('hidden');
  $('#errorMessageForRM').addClass('hidden');

  var selectedTransactionIds = jQuery('#LinkedRejectionMemoGridId').getGridParam('selrow');
  
  //SCP310398 - SRM:Exception occurred in Report Download Service. - SIS Production - 10Nov
  checkUserSessionsForAjaxRequest();

  //SCP499580: SRM-Exception occurred in Report Download Service - Major - 12 Jul 2016
  if (selectedTransactionIds.length > 0 && selectedTransactionIds != undefined && selectedTransactionIds !=null) {
    $.ajax({
      type: "POST",
      url: "../Pax/BillingHistory/GenerateAuditTrailLinkedCorrRMs/",
      data: { rejectionMemoIds: selectedTransactionIds },
      dataType: "json",
      success: function (response) {
        if (response.IsFailed == false) {
          $('#successMessageForRM').removeClass('hidden');
          $('#successMessageContent').html(response.Message);
          // showClientSuccessMessage(response.Message);
        }
        else {
          $('#errorMessageForRM').removeClass('hidden');
          // showClientErrorMessage(response.Message);
        }
      },
      error: function () {
        $('#errorMessageForRM').removeClass('hidden');
      }
    });
  }
  else if (jQuery("#LinkedRejectionMemoGridId").jqGrid('getGridParam', 'records') > 0) {
    alert('Records exist in new grid \'Stage 3 Rejection Memos Linked with Correspondence\'; but none has been selected (by means of the checkbox)');
    }
    else{
    alert('No records exist in new grid \'Stage 3 Rejection Memos Linked with Correspondence\'');
  }
}

function SetTransControl() {
  var $MemoTypeId = $("#MemoTypeId").val();
  var $MemoNumber = $("#MemoNumber");
  var $IssuingAirline = $("#IssuingAirline");
  var $DocumentNumber = $("#DocumentNumber");
  var $CouponNumber = $("#CouponNumber");
  var $RejectionStageId = $("#RejectionStageId");
  var billingCode = $("#BillingCode").val();

  switch ($MemoTypeId) {
    case "4":
      $MemoNumber.attr('disabled', false);
      $IssuingAirline.val("");
      $IssuingAirline.attr('disabled', 'disabled');
      $DocumentNumber.val("");
      $DocumentNumber.attr('disabled', 'disabled');
      $CouponNumber.val("");
      $CouponNumber.attr('disabled', 'disabled');
      if (billingCode == 0 || billingCode == -1)
        $RejectionStageId.attr('disabled', false);
      break;

    case "5":
      $MemoNumber.attr('disabled', false);
      $IssuingAirline.val("");
      $IssuingAirline.attr('disabled', 'disabled');
      $DocumentNumber.val("");
      $DocumentNumber.attr('disabled', 'disabled');
      $CouponNumber.val("");
      $CouponNumber.attr('disabled', 'disabled');
      $RejectionStageId.val("");
      $RejectionStageId.attr('disabled', 'disabled');
      break;

    case "6":
      $MemoNumber.attr('disabled', false);
      $IssuingAirline.val("");
      $IssuingAirline.attr('disabled', 'disabled');
      $DocumentNumber.val("");
      $DocumentNumber.attr('disabled', 'disabled');
      $CouponNumber.val("");
      $CouponNumber.attr('disabled', 'disabled');
      $RejectionStageId.val("");
      $RejectionStageId.attr('disabled', 'disabled');
      break;

    case "-1":
      $MemoNumber.val("");
      $MemoNumber.attr('disabled', 'disabled');
      $RejectionStageId.val("");
      $RejectionStageId.attr('disabled', 'disabled');

      $IssuingAirline.val("");
      $IssuingAirline.attr('disabled', 'disabled');
      $DocumentNumber.val("");
      $DocumentNumber.attr('disabled', 'disabled');
      $CouponNumber.val("");
      $CouponNumber.attr('disabled', 'disabled');
      break;
    default:
      $MemoNumber.val("");
      $MemoNumber.attr('disabled', 'disabled');
      $RejectionStageId.val("");
      $RejectionStageId.attr('disabled', 'disabled');

      $IssuingAirline.attr('disabled', false);
      $DocumentNumber.attr('disabled', false);
      $CouponNumber.attr('disabled', false);
  }

}

function SetControlValues(isFromPageLoad) {
  var billingCode = $("#BillingCode").val();
  var $MemoTypeId = $("#MemoTypeId");
  var $RejectionStageId = $("#RejectionStageId");
  //$("#BillingYearMonth").val("")
  //$("#BillingPeriod").val("-1")
  switch (billingCode) {
    case "-1":
      $MemoTypeId.attr("disabled", false);
      $RejectionStageId.attr('disabled', false);
      break;
    case "0":
      $MemoTypeId.attr("disabled", false);

      if(isFromPageLoad == false){      
        $RejectionStageId.val("");
      }

      $RejectionStageId.attr('disabled', false);
      break;
    case "3":
      $MemoTypeId.val("1");
      $MemoTypeId.attr('disabled', 'disabled');
      $RejectionStageId.val("");
      $RejectionStageId.attr('disabled', 'disabled');
      break;
    case "6":
      $MemoTypeId.val("4");
      $MemoTypeId.attr('disabled', 'disabled');
      $RejectionStageId.val("2");
      $RejectionStageId.attr('disabled', 'disabled');
      break;
    case "7":
      $MemoTypeId.val("4");
      $MemoTypeId.attr('disabled', 'disabled');
      $RejectionStageId.val("3");
      $RejectionStageId.attr('disabled', 'disabled');
      break;
    default:
      $MemoTypeId.val("-1");
      $MemoTypeId.attr('disabled', 'disabled');
      $RejectionStageId.val("");
      $RejectionStageId.attr('disabled', 'disabled');
  }
  SetTransControl();
}

function SetTransaction() {
   
  if ($("#BillingTypeId").val() == "" || $("#BillingTypeId").val() == "2") {
    $("#InitiateRejection").attr('disabled', 'disabled');
    $("#InitiateCorrespondence").attr('disabled', 'disabled');
    $("#InitiateBilling").attr('disabled', 'disabled');

    // Buttons which have disabled attribute are not grayed out in Firefox, so if browser is Firefox remove "primaryButton" class and add "disabledButtonClassForMozilla" class 
    if ($.browser.mozilla) {
      $("#InitiateRejection").removeClass('primaryButton');
      $("#InitiateRejection").addClass('disabledButtonClassForMozilla');

      $("#InitiateCorrespondence").removeClass('primaryButton');
      $("#InitiateCorrespondence").addClass('disabledButtonClassForMozilla');

      $("#InitiateBilling").removeClass('primaryButton');
      $("#InitiateBilling").addClass('disabledButtonClassForMozilla');
    }
  }
  else {
    $("#InitiateRejection").removeAttr('disabled');
    $("#InitiateCorrespondence").removeAttr('disabled');
    $("#InitiateBilling").removeAttr('disabled');

    // If we remove "disabled" attribute from button and browser is Firefox, add "primaryButton" class and remove "disabledButtonClassForMozilla" class 
    if ($.browser.mozilla) {
      $("#InitiateRejection").removeClass('disabledButtonClassForMozilla');
      $("#InitiateRejection").addClass('primaryButton');

      $("#InitiateCorrespondence").removeClass('disabledButtonClassForMozilla');
      $("#InitiateCorrespondence").addClass('primaryButton');

      $("#InitiateBilling").removeClass('disabledButtonClassForMozilla');
      $("#InitiateBilling").addClass('primaryButton');
    }
  }
}

function resetForm(formId) {
  $(':input', formId)
        .not(':button, :submit, :reset, :hidden')
        .val('')
        .removeAttr('selected');
  $("#BillingPeriod").val("-1");

  ResetSearch(formId);
}

function ResetSearch(formId) {
  $.ajax({
    type: "POST",
    url: clearSearchUrl,
    data: { entity: formId },
    dataType: "json",
    success: function (response) {

      if (response) {
        return;
      }
    },
    failure: function (response) {
      var failed = 'failed';
    }
  });
}

function ShowDetails(correspondenceUrl, invoiceUrl) {
    selectedInvoiceId = jQuery('#BHSearchResultsGrid').getGridParam('selrow');    

  var gridRow = $("#BHSearchResultsGrid").getRowData(selectedInvoiceId);
  if (gridRow.DisplayCorrespondenceStatus != '')
    location.href = corrUrl + '/' + selectedInvoiceId;
  else
    location.href = invUrl + '/' + selectedInvoiceId;
}

function ShowCorrespondence() {

    selectedInvoiceId = jQuery('#BHSearchResultsGrid').getGridParam('selrow');

    if (selectedInvoiceId && selectedInvoiceId != 0 && selectedInvoiceId != null) {
    location.href = corrUrl + '/' + selectedInvoiceId;
  }
  else {

  }
}

function ShowAuditTrail() {
    selectedInvoiceId = jQuery('#BHSearchResultsGrid').getGridParam('selrow');

    if (selectedInvoiceId && selectedInvoiceId != 0 && selectedInvoiceId!=null) {
    location.href = auditTrailUrl + '/' + selectedInvoiceId;
  }
}

function InitiateBillingMemo() {

  InvoiceId = null;
  memoType = 'BillingMemo';
  selectedTransactionId = jQuery('#BHSearchResultsGrid').getGridParam('selrow');

  var gridRow = $("#BHSearchResultsGrid").getRowData(selectedTransactionId);

  if (InvoiceId) {
    if (InvoiceId != gridRow.InvoiceId)
      return false;
  }
  else
    InvoiceId = gridRow.InvoiceId;
  var correspondenceNumber = gridRow.TransactionNumber;

  // If Authority to Bill is granted OR Correspondence Status is "Expired" allow user to create BillingMemo
  if (gridRow.AuthorityToBill == "Yes" || gridRow.DisplayCorrespondenceStatus == "Expired") {

    $.ajax({
      type: "POST",
      url: checkIfBMExists,
      data: { correspondenceRefNumber: correspondenceNumber },
      dataType: "json",
      success: function (response) {
        if (response) {
          if (response.Transactions) {
              for (i = 0; i < response.Transactions.length; i++) {
                  //SCP199693 - create BM and close correspondence at same time - related to Spira [IN:008756] [TC:082202]
                  //Desc: More details about existing BM are displayed.
                var memoNumber = 'No further action on this correspondence is possible as a Billing Memo refers to this correspondence. ' +
				             'Refer to Invoice No. ' + response.Transactions[i].InvoiceNumber +
				             ' of Period ' + response.Transactions[i].InvoicePeriod +
				             ', Batch No. ' + response.Transactions[i].BatchNumber +
				             ', Sequence No. ' + response.Transactions[i].SequenceNumber +
				             ', Billing Memo No. ' + response.Transactions[i].BillingMemoNumber + ' for details.';
                            //' - Invoice Status: ' + response.Transactions[i].InvoiceStatus + 
              var $dupBillingMemoDiv = $('#duplicateBMs');
              if (i == 0) {
                $dupBillingMemoDiv.text(memoNumber);
              }
              else {
                $dupBillingMemoDiv.text($dupBillingMemoDiv.text() + ', ' + memoNumber);
              }
            }
            $('#divDuplicateBillingMemos').dialog({ title: 'Warning', height: '190', width: '250', modal: true, resizable: false });

            return;
          }
          populateInvoices(response);
        }
      },
      failure: function (response) {
      },
      error: function (xhr, ajaxOptions, thrownError) {

        alert(xhr.statusText);
        alert(thrownError);
      }
    });
  }
  else {
    alert('Selected correspondence does not give authority to bill. Hence a billing memo cannot be created.');
  }
}

// Variables to calculate time limit for display of warning message..
var rejectedBillingYear;
var rejectedBillingMonth;
var rejectedBillingPeriod;
var rejectedSmi;
var rejectionTransType;
function InitiateRMRejection(type) {
  
  memoType = 'RejectionMemo';
  InvoiceId = null;
  selectedTransaction = null;
  selectedTransactionIds = jQuery('#BHSearchResultsGrid').getGridParam('selarrrow');

  if (selectedTransactionIds.length > 0) {
    for (i = 0; i < selectedTransactionIds.length; i++) {
      selectedTransactionId = selectedTransactionIds[i];

      var gridRow = $("#BHSearchResultsGrid").getRowData(selectedTransactionId);
      if (InvoiceId) {
        if (InvoiceId != gridRow.InvoiceId) {
          alert('Please select transactions which belong to same invoice.');
          return false;
        }
      }
      else
        InvoiceId = gridRow.InvoiceId;
      if (gridRow.TransactionType == 'PC' || gridRow.TransactionType == 'FD' || gridRow.TransactionType == 'RM' || gridRow.TransactionType == 'BM' || gridRow.TransactionType == 'CM') {
        if (selectedTransaction && selectedTransaction != gridRow.TransactionType) {
          alert('Only multiple prime coupon selection is allowed for initiating rejection.');
          return false;
        }
        else
        {
          selectedTransaction = gridRow.TransactionType;
          rejectedBillingYear = gridRow.BillingYear;
          rejectedBillingMonth = gridRow.BillingMonth;
          rejectedBillingPeriod = gridRow.BillingPeriod;
          rejectedSmi = gridRow.SettlementMethodId;
          if (gridRow.TransactionType == 'PC' || gridRow.TransactionType == 'BM' || gridRow.TransactionType == 'CM') {
            // Set the transaction type of rejection invoice. Will be used to calculate time limit.
            rejectionTransType = TransactionTypeRM1;
          }
          else if (gridRow.TransactionType == 'FD') {
            rejectionTransType = TransactionTypeFormF;
          }
          else if (gridRow.TransactionType == 'RM' && gridRow.BillingCodeId == 0) {
            if (gridRow.RejectionStage == 1)
              rejectionTransType = TransactionTypeRM2;
            else if (gridRow.RejectionStage == 2)
              rejectionTransType = TransactionTypeRM3;
          }
          else if (gridRow.TransactionType == 'RM' && gridRow.BillingCodeId == 6) { // Biling code: 6(Form F)
              rejectionTransType = TransactionTypeFormXf;            
          }
        }
      }
      else {
        alert('Only multiple prime coupon records, or single rejection record is allowed for initiating rejection.');
        return false;
      }
      billingCode = gridRow.BillingCodeId;

      if (gridRow.SourceCodeId == '14' && selectedTransactionIds && selectedTransactionIds.length > 1) {
        alert('You can not reject multiple FIM documents or FIM along with other coupons.');
        return false;
      }
    }
  }

  var idList = selectedTransactionIds.join(",");
  rejectTransactions(InvoiceId, idList);
}

// memoId is an optional parameter in below function. Not used for rejecting coupons.
function InitiateRejForSpecificTrans(gridName, transactionType, invoiceId, initiateRejMethod, dupRejMethod, billingCodeId, billingYear, billingMonth, billingPeriod, smi, memoId) {
  
  memoType = 'RejectionMemo';
  InvoiceId = invoiceId;
  selectedTransaction = transactionType;
  var idList;
  // For coupons..
  if (memoId == null || memoId == '') {
    if (selectedTransaction == "FD") {
      rejectionTransType = TransactionTypeFormF;
    }
    // For prime coupon
    else {
      rejectionTransType = TransactionTypeRM1;
    }
    selectedTransactionIds = jQuery('#' + gridName).getGridParam('selarrrow');
    for (i = 0; i < selectedTransactionIds.length; i++) {
      if (selectedTransactionIds[i]) {
        if (idList == null || idList == '')
          idList = selectedTransactionIds[i];
        else
          idList = idList + ',' + selectedTransactionIds[i];
      }
    }
  }
  else
    idList = memoId;

  rejectedBillingYear = billingYear;
  rejectedBillingMonth = billingMonth;
  rejectedBillingPeriod = billingPeriod;
  rejectedSmi = smi;

  invUrl = initiateRejMethod;
  initiateDuplicateRejectionUrl = dupRejMethod;
  billingCode = billingCodeId;
  rejectTransactions(InvoiceId, idList);
}

function rejectTransactions(InvoiceId, idList) {
  // selectedTransaction contains the transaction type.
  idList = idList + ';' + selectedTransaction;

  if (idList.indexOf(';') != 0 && idList.indexOf('undefined') != 0) {// if idList contains at least 1 transaction id to reject
    clearMessageContainer();
    $.ajax({
      type: "POST",
      url: invUrl,
      data: { invoiceId: InvoiceId, rejectedRecordIds: idList, billingYear: rejectedBillingYear, billingMonth: rejectedBillingMonth, billingPeriod: rejectedBillingPeriod, smi: rejectedSmi, rejectionTransactionType: rejectionTransType },
      dataType: "json",
      success: function (response) {
        //CMP#641: Time Limit Validation on Third Stage PAX Rejections
        if (response.Message && response.IsFailed) {
          if (response.ErrorCode == "BPAXNS_10969") {
            alert(response.Message);
          }
          else {
            alert('Rejection is not allowed to user.')
          }
        }
        else {
          $('#ProceedButton').unbind('click');
          if (response) {
            if (response.Transactions || response.IsTransactionOutsideTimeLimit == true) {
              if (response.Transactions) {
                if (response.Transactions.length > 0) {
                  $('#transactionsRejectedTitle').show();
                }
                else {
                  $('#transactionsRejectedTitle').hide();
                }

                for (i = 0; i < response.Transactions.length; i++) {
                  var rejMemoNumber = response.Transactions[i].MemoNumber + ' - ' +
                                  response.Transactions[i].InvoiceNumber;

                  if (i == 0) {
                    $('#dupRejections').text(rejMemoNumber);
                  }
                  else {
                    $('#dupRejections').text($('#dupRejections').text() + ', ' + rejMemoNumber);
                  }
                }
              }

              if (response.IsTransactionOutsideTimeLimit) {
                $('#outsideTimeLimitMessage').show();
              }
              else {
                $('#outsideTimeLimitMessage').hide();
              }

              $('#ProceedButton').bind('click', function () { ignoreDuplicateRejections(idList, InvoiceId); closeDialog('#divDuplicateRejections'); });
              $('#divDuplicateRejections').dialog({ title: 'Warning', height: '175', width: '250', modal: true, resizable: false });

              return;
            }

            populateInvoices(response);
          }
        }
      },
      failure: function (response) {
      },
      error: function (xhr, ajaxOptions, thrownError) {

        alert(xhr.statusText);
        alert(thrownError);
      }
    });
  }
  else {
    showClientErrorMessage('Please select transactions to reject.');
  }
}

function populateInvoices(response) {
  
    if (response.length > 0) {
      // If single open invoice found, redirect to the rejection memo create page of that invoice.
      if (response.length == 1) {
        location.href = response[0].RedirectUrl;
        return;
      }

    $("#ddlInvoice").empty().append('<option selected="selected" value="">Please Select</option>');
    for (i = 0; i < response.length; i++) {
      $("#ddlInvoice").append($("<option></option>").val(response[i].Id).html(response[i].InvoiceNumber));
    }
    $('#divBillingHistoryInvoice').dialog({ title: 'Invoices', height: '170', width: '250', modal: true, resizable: false });
    return;
  }
  if (response.Message) {
    if (response.isRedirect) {
      if (confirm('No Open Invoice Found. Please click on OK to create new invoice.'))
        location.href = response.RedirectUrl;
    }
  }
  else {
    clearMessageContainer();
  }
}

function ignoreDuplicateRejections(idList, InvoiceId) {
  $.ajax({
    type: "POST",
    url: initiateDuplicateRejectionUrl,
    data: { invoiceId: InvoiceId, rejectedRecordIds: idList },
    dataType: "json",
    success: function (response) {
      
      if (response) {
        populateInvoices(response);
      }
    },
    error: function (xhr, ajaxOptions, thrownError) {
      alert(xhr.statusText);
      alert(thrownError);
    }
  });
}

function InitiateCorrespondence() {
  InvoiceId = null;
  selectedTransaction = null;
  selectedReasonCode = null;
  selectedSourceCode = null;
  selectedTransactionIds = jQuery('#BHSearchResultsGrid').getGridParam('selarrrow');

  if ($('#BillingTypeId').val() == '1') {
    if (selectedTransactionIds.length > 0) {
      for (i = 0; i < selectedTransactionIds.length; i++) {
        selectedTransactionId = selectedTransactionIds[i];

        var gridRow = $("#BHSearchResultsGrid").getRowData(selectedTransactionId);

        if (gridRow.TransactionType == 'RM' && gridRow.RejectionStage == '3') {
          if (selectedTransaction && selectedTransaction != gridRow.TransactionType || selectedReasonCode && selectedReasonCode != gridRow.ReasonCode) {
            alert('Please select rejection memos of 3rd stage belonging to same Reason Code');
            return false;
          }
          else {
            selectedReasonCode = gridRow.ReasonCode;
            selectedTransaction = gridRow.TransactionType;
          }
        }
        else {
          alert('Please select rejection memos of 3rd stage');
          return false;
        }
        if (InvoiceId) {
          if (InvoiceId != gridRow.InvoiceId) {
            alert('Please select rejection memos of 3rd stage belonging to same invoice');
            return false;
          }
        }
        else
            InvoiceId = gridRow.InvoiceId;
        //CMP526 - Passenger Correspondence Identifiable by Source Code
        if (selectedSourceCode) {
            if (selectedSourceCode != gridRow.SourceCodeId) {
                alert('Please select rejection memos of 3rd stage belonging to the same Source Code');
                return false;
            }
        }
        else
            selectedSourceCode = gridRow.SourceCodeId;
      }
    }
  }

  var idList = selectedTransactionIds.join(",");
  $("#rejectionMemoIds").val(idList);
  

  $.ajax({
    type: "POST",
    url: initiateCorrUrl,
    data: { invoiceId: InvoiceId, rejectedRecordIds: idList },
    dataType: "json",
    success: function (response) {
      if (response) {
        if (response.isRedirect)
          IsCorrespondenceOutsideTimeLimit(InvoiceId);
        //location.href = corrUrl + "/" + InvoiceId;
        else {
          alert('Correspondence already created for selected rejection(s).');
          $("#rejectionMemoIds").val('');
          clearMessageContainer();
        }
      }
      else {
        alert('Correspondence already created for selected rejection(s).');
        $("#rejectionMemoIds").val('');
        clearMessageContainer();
      }
    },
    failure: function (response) {
      alert('Error occurred');
      $("#rejectionMemoIds").val('');
    }
  });
}

function GetSelectedRecordId(ids) {
    
  if ($("#BillingTypeId").val() == "" || $("#BillingTypeId").val() == "2") {
    SetTransaction();
    return false;
  }
  selectedTransactionIds = jQuery('#BHSearchResultsGrid').getGridParam('selarrrow');
  var rejectionEnabled = true;
  var billingMemoEnabled = false;
  var correspondenceEnabled = true;

  if (selectedTransactionIds && selectedTransactionIds != null && selectedTransactionIds.length > 1) {
    for (i = 0; i < selectedTransactionIds.length; i++) {
      selectedTransactionId = selectedTransactionIds[i];
      var gridRow = $("#BHSearchResultsGrid").getRowData(selectedTransactionId);

      if (rejectionEnabled) {
      // multiple selection allowed only for coupons.
        if ((gridRow.TransactionType == 'PC' || gridRow.TransactionType == 'FD') && gridRow.BillingCodeId != '3') {
          rejectionEnabled = true;
        }
        else
          rejectionEnabled = false;
    }
    
      if (correspondenceEnabled) {
        if (gridRow.TransactionType == 'RM' && gridRow.RejectionStage == '3' && $('#BillingTypeId').val() == '1')
          correspondenceEnabled = true;
        else
          correspondenceEnabled = false;
      }
    }

    $("#InitiateBilling").attr('disabled', 'disabled');

    // Buttons which have disabled attribute are not grayed out in Firefox, so if browser is Firefox remove "primaryButton" class and add "disabledButtonClassForMozilla" class 
    if ($.browser.mozilla) {
      $("#InitiateBilling").removeClass('primaryButton');
      $("#InitiateBilling").addClass('disabledButtonClassForMozilla');
    }

    if (rejectionEnabled) {
      $("#InitiateRejection").removeAttr('disabled');

      // If we remove "disabled" attribute from button and browser is Firefox, add "primaryButton" class and remove "disabledButtonClassForMozilla" class  
      if ($.browser.mozilla) {
        $("#InitiateRejection").removeClass('disabledButtonClassForMozilla');
        $("#InitiateRejection").addClass('primaryButton');
      }
    }
    else {
      $("#InitiateRejection").attr('disabled', 'disabled');

      // Buttons which have disabled attribute are not grayed out in Firefox, so if browser is Firefox remove "primaryButton" class and add "disabledButtonClassForMozilla" class
      if ($.browser.mozilla) {
        $("#InitiateRejection").removeClass('primaryButton');
        $("#InitiateRejection").addClass('disabledButtonClassForMozilla');
      }
    }

    if (correspondenceEnabled) {
      $("#InitiateCorrespondence").removeAttr('disabled');

      // If we remove "disabled" attribute from button and browser is Firefox, add "primaryButton" class and remove "disabledButtonClassForMozilla" class  
      if ($.browser.mozilla) {
        $("#InitiateCorrespondence").removeClass('disabledButtonClassForMozilla');
        $("#InitiateCorrespondence").addClass('primaryButton');
      }
    }
    else {
      $("#InitiateCorrespondence").attr('disabled', 'disabled');

      // Buttons which have disabled attribute are not grayed out in Firefox, so if browser is Firefox remove "primaryButton" class and add "disabledButtonClassForMozilla" class 
      if ($.browser.mozilla) {
        $("#InitiateCorrespondence").removeClass('primaryButton');
        $("#InitiateCorrespondence").addClass('disabledButtonClassForMozilla');
      }
    }
  }
  else {
    selectedInvoiceId = jQuery('#BHSearchResultsGrid').getGridParam('selarrrow');

    if (selectedInvoiceId && selectedInvoiceId.length > 0) {
      var gridRow = $("#BHSearchResultsGrid").getRowData(selectedInvoiceId);

      if (((gridRow.TransactionType == 'RM' && (gridRow.RejectionStage == '2' || gridRow.RejectionStage == '1')) || (gridRow.TransactionType == 'PC' || gridRow.TransactionType == 'FD' || gridRow.TransactionType == 'BM' || gridRow.TransactionType == 'CM')) && gridRow.BillingCodeId != '3') {
        if (gridRow.TransactionType == 'BM' && (gridRow.ReasonCode == '6A' || gridRow.ReasonCode == '6B')) {
          $("#InitiateRejection").attr('disabled', 'disabled'); // BMs with reason codes 6A, 6B cannot be rejected.

          // Buttons which have disabled attribute are not grayed out in Firefox, so if browser is Firefox remove "primaryButton" class and add "disabledButtonClassForMozilla" class 
          if ($.browser.mozilla) {
            $("#InitiateRejection").removeClass('primaryButton');
            $("#InitiateRejection").addClass('disabledButtonClassForMozilla');
          }
        }
        else {
          $("#InitiateRejection").removeAttr('disabled');

          // If we remove "disabled" attribute from button and browser is Firefox, add "primaryButton" class and remove "disabledButtonClassForMozilla" class  
          if ($.browser.mozilla) {
            $("#InitiateRejection").removeClass('disabledButtonClassForMozilla');
            $("#InitiateRejection").addClass('primaryButton');
          }
        }
      }
      else {
        $("#InitiateRejection").attr('disabled', 'disabled');

        // Buttons which have disabled attribute are not grayed out in Firefox, so if browser is Firefox remove "primaryButton" class and add "disabledButtonClassForMozilla" class 
        if ($.browser.mozilla) {
          $("#InitiateRejection").removeClass('primaryButton');
          $("#InitiateRejection").addClass('disabledButtonClassForMozilla');
        }
      }

    if (selectedInvoiceId && selectedInvoiceId != 0 && selectedInvoiceId.length > 0 && $('#BillingTypeId').val() == '1' && gridRow.TransactionType == 'RM' && gridRow.RejectionStage == '3') {
        $("#InitiateCorrespondence").removeAttr('disabled');

        // If we remove "disabled" attribute from button and browser is Firefox, add "primaryButton" class and remove "disabledButtonClassForMozilla" class  
        if ($.browser.mozilla) {
          $("#InitiateCorrespondence").removeClass('disabledButtonClassForMozilla');
          $("#InitiateCorrespondence").addClass('primaryButton');
        }
      }
      else {
        $("#InitiateCorrespondence").attr('disabled', 'disabled');

        // Buttons which have disabled attribute are not grayed out in Firefox, so if browser is Firefox remove "primaryButton" class and add "disabledButtonClassForMozilla" class 
        if ($.browser.mozilla) {
          $("#InitiateCorrespondence").removeClass('primaryButton');
          $("#InitiateCorrespondence").addClass('disabledButtonClassForMozilla');
        }
      }

  if (selectedInvoiceId && selectedInvoiceId != 0 && selectedInvoiceId.length > 0 && $('#BillingTypeId').val() == '1' && gridRow.TransactionType == '' && gridRow.AuthorityToBill == 'Yes' && gridRow.CorrInitiatingMember == loggedInMemberId && gridRow.DisplayCorrespondenceStatus != 'Closed') {
        $("#InitiateBilling").removeAttr('disabled');

        // If we remove "disabled" attribute from button and browser is Firefox, add "primaryButton" class and remove "disabledButtonClassForMozilla" class
        if ($.browser.mozilla) {
          $("#InitiateBilling").removeClass('disabledButtonClassForMozilla');
          $("#InitiateBilling").addClass('primaryButton');
        }
      }
      else {
        $("#InitiateBilling").attr('disabled', 'disabled');

        // Buttons which have disabled attribute are not grayed out in Firefox, so if browser is Firefox remove "primaryButton" class and add "disabledButtonClassForMozilla" class 
        if ($.browser.mozilla) {
          $("#InitiateBilling").removeClass('primaryButton');
          $("#InitiateBilling").addClass('disabledButtonClassForMozilla');
        }
      }

  if (selectedInvoiceId && selectedInvoiceId != 0 && selectedInvoiceId.length > 0 && gridRow.TransactionType == '' && gridRow.DisplayCorrespondenceStatus == 'Expired' && gridRow.CorrInitiatingMember == loggedInMemberId) {
        $("#InitiateBilling").removeAttr('disabled');

        // If we remove "disabled" attribute from button and browser is Firefox, add "primaryButton" class and remove "disabledButtonClassForMozilla" class  
        if ($.browser.mozilla) {
          $("#InitiateBilling").removeClass('disabledButtonClassForMozilla');
          $("#InitiateBilling").addClass('primaryButton');
        }
      }

  if (selectedInvoiceId && selectedInvoiceId != 0 && selectedInvoiceId.length > 0 && gridRow.TransactionType == '' && gridRow.DisplayCorrespondenceStatus == 'Expired' && (gridRow.CorrInitiatingMember != loggedInMemberId)) {
        $("#InitiateBilling").attr('disabled', 'disabled');

        // Buttons which have disabled attribute are not grayed out in Firefox, so if browser is Firefox remove "primaryButton" class and add "disabledButtonClassForMozilla" class 
        if ($.browser.mozilla) {
          $("#InitiateBilling").removeClass('primaryButton');
          $("#InitiateBilling").addClass('disabledButtonClassForMozilla');
        }
      }   
    }
    else {
      $("#InitiateBilling").attr('disabled', 'disabled');
      $("#InitiateCorrespondence").attr('disabled', 'disabled');
      $("#InitiateRejection").attr('disabled', 'disabled');

      // Buttons which have disabled attribute are not grayed out in Firefox, so if browser is Firefox remove "primaryButton" class and add "disabledButtonClassForMozilla" class 
      if ($.browser.mozilla) {
        $("#InitiateRejection").removeClass('primaryButton');
        $("#InitiateRejection").addClass('disabledButtonClassForMozilla');

        $("#InitiateCorrespondence").removeClass('primaryButton');
        $("#InitiateCorrespondence").addClass('disabledButtonClassForMozilla');

        $("#InitiateBilling").removeClass('primaryButton');
        $("#InitiateBilling").addClass('disabledButtonClassForMozilla');
      }
    }
  }
}

function SetAuthorityToBill(cellValue, options, rowObject) {

  if (isinvoiceSearch)
    return "&nbsp;";

  if (cellValue.toString().toLowerCase() === 'true') return "Yes";
  return "NA";
}

SetSubStatus();
function SetSubStatus() {

  var selectedId = $("#CorrespondenceSubStatusId").val();
  $("#CorrespondenceSubStatusId").empty();
  var status = $("#CorrespondenceStatusId").val();
  if (status == "1") {
    $("#CorrespondenceSubStatusId").append($("<option></option>").val("-1").html("All"));
    $("#CorrespondenceSubStatusId").append($("<option selected></option>").val("1").html("Received"));
    $("#CorrespondenceSubStatusId").append($("<option></option>").val("3").html("Saved"));
    $("#CorrespondenceSubStatusId").append($("<option></option>").val("2").html("Responded"));
    $("#CorrespondenceSubStatusId").append($("<option></option>").val("4").html("Ready For Submit"));
    $("#CorrespondenceSubStatusId").append($("<option></option>").val("7").html("Pending"));
  }
  else if (status == "2") {
    $("#CorrespondenceSubStatusId").append($("<option></option>").val("-1").html("Please Select"));
  }
  else if (status == "3") {
    $("#CorrespondenceSubStatusId").append($("<option></option>").val("-1").html("All"));
    $("#CorrespondenceSubStatusId").append($("<option></option>").val("5").html("Billed"));
    $("#CorrespondenceSubStatusId").append($("<option></option>").val("6").html("Due To Expiry"));
    $("#CorrespondenceSubStatusId").append($("<option></option>").val("8").html("Accepted By Correspondence Initiator"));
  }
  else if (status == "-1") {
    $("#CorrespondenceSubStatusId").append($("<option></option>").val("-1").html("All"));
    $("#CorrespondenceSubStatusId").append($("<option></option>").val("1").html("Received"));
    $("#CorrespondenceSubStatusId").append($("<option></option>").val("3").html("Saved"));
    $("#CorrespondenceSubStatusId").append($("<option></option>").val("2").html("Responded"));
    $("#CorrespondenceSubStatusId").append($("<option></option>").val("4").html("Ready For Submit"));
    $("#CorrespondenceSubStatusId").append($("<option></option>").val("5").html("Billed"));
    $("#CorrespondenceSubStatusId").append($("<option></option>").val("6").html("Due To Expiry"));
    $("#CorrespondenceSubStatusId").append($("<option></option>").val("7").html("Pending"));
    $("#CorrespondenceSubStatusId").append($("<option></option>").val("8").html("Accepted By Correspondence Initiator"));
  }
  //TFS#9984 : Forefox:v45 - By selecting value from "Correspondence Status" related value is not updated in "Correspondence Sub Status".
  //Note:Again Changes are done as per TFS#9992.
  if ($("#CorrespondenceSubStatusId > option[value='" + selectedId + "']").length == 0) {
        $("#CorrespondenceSubStatusId").val($("#CorrespondenceSubStatusId option:first").val());
        return;
  }
  else {
        $("#CorrespondenceSubStatusId").val(selectedId);
  }
}
SetCorrespondenceOwner();
function SetCorrespondenceOwner() {
  var selectedId = $("#CorrespondenceSubStatusId").val();
  if (selectedId == "1") {
    $("#CorrespondenceOwnerId").val('-1');
    $("#CorrespondenceOwnerId").attr('disabled', 'disabled');
  }
  else
    $("#CorrespondenceOwnerId").removeAttr('disabled');
}


var _isCorrespondenceOutsideTimeLimitMethod;
function IsCorrespondenceOutsideTimeLimit(invoiceId) {
  var frm = $("#frmInitiateCorrespondence");
  $.ajax({
    type: "POST",
    url: _isCorrespondenceOutsideTimeLimitMethod,
    data: { invoiceId: invoiceId },
    dataType: "json",
    success: function (response) {
      if (response.IsFailed == false) {
        $("#invoiceId").val(InvoiceId);
        $("#frmInitiateCorrespondence").submit();
        //location.href = corrUrl + "/" + InvoiceId;
        return;
      }
      alert(response.Message);
      return;
    },
    error: function (response) {
      alert(response);
    }
  });
}

function validateDateRange(startDateId, endDateId) {
  var startDateVal = $('#' + startDateId).datepicker("getDate");
  var endDateVal = $('#' + endDateId).datepicker("getDate");

  return endDateVal >= startDateVal;
}

//CMP508:Audit Trail Download with Supporting Documents
function DownloadFile(instantDownloadUrl, enqueueUrl) {
    var includeSuppDocs = $('#IncludeSuppDocs').prop('checked');
    if (!includeSuppDocs) {    
        location.href = instantDownloadUrl;
    }
      else {
        //SCP310398 - SRM:Exception occured in Report Download Service. - SIS Production - 10Nov
        checkUserSessionsForAjaxRequest();
        $.ajax({
            type: "POST",
            url: enqueueUrl,
            dataType: "json",
            success: function (response) {
              // SCP227747: Cargo Invoice Data Download
              if (response.IsFailed) {
                showClientErrorMessage(response.Message);
              }
              else {
                showClientSuccessMessage(response.Message);
              }
            }
        });
    }
}
