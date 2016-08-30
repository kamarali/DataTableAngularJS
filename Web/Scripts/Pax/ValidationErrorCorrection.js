
var muCorrespondenceTransactionType = 37;

var ExceptionSummaryGridDatasUrl, ExceptionDetailsGridDataUrl, UpdateCorrectLinkingErrorUrl, IsDisplayLinkingButtonUrl, UpdateUrl, ValidateErrorUrl, BatchUpdatedCountUrl;

function UpdatePopUpClick() {
   
  $('#InvoiceUpdatePopUp').dialog({ title: 'Update Validation Error', height: 350, width: 1000, modal: true, resizable: false });

  s = $('#UatpExceptionDetailsGrid').getGridParam('selrow');

  var rowcells = jQuery("#UatpExceptionDetailsGrid").jqGrid('getCell', s, 'FieldName');

  ss = $('#UatpExceptionSummary').getGridParam('selrow');
  var sss = $('#UatpExceptionDetailsGrid').getGridParam('selrow');


  $('#FieldName').val(jQuery("#UatpExceptionDetailsGrid").jqGrid('getCell', s, 'FieldName'));
  $('#FieldValue').val(jQuery("#UatpExceptionDetailsGrid").jqGrid('getCell', s, 'FieldValue'));
  $('#UpdateFileName').val(jQuery("#UatpExceptionSummary").jqGrid('getCell', ss, 'FileName'));
  $('#ErrorDescription').val(jQuery("#UatpExceptionSummary").jqGrid('getCell', ss, 'ErrorDescription'));
  $('#UpdateExceptionCode').val(jQuery("#UatpExceptionSummary").jqGrid('getCell', ss, 'ExceptionCode'));
  $('#BatchUpdateAllowed').val(jQuery("#UatpExceptionSummary").jqGrid('getCell', ss, 'BatchUpdateAllowed'));
  $('#ExceptionSummaryId').val(jQuery("#UatpExceptionSummary").jqGrid('getCell', ss, 'Id'));
  $('#ExceptionDetailId').val(jQuery("#UatpExceptionDetailsGrid").jqGrid('getCell', sss, 'ExceptionDetailId'));
  $('#ErrorLevel').val(jQuery("#UatpExceptionDetailsGrid").jqGrid('getCell', sss, 'ErrorLevel'));
  $('#PkReferenceId').val(jQuery("#UatpExceptionDetailsGrid").jqGrid('getCell', sss, 'PkReferenceId'));
  //SCP ID : 252342 - SRM: ICH invoice in ready for billing status
  $('#LastUpdatedOn').val(jQuery("#UatpExceptionDetailsGrid").jqGrid('getCell', sss, 'LastUpdatedOn'));
 
  if ($('#BatchUpdateAllowed').val() == "No") {
      
      if (!($('#BatchUpdateButtonAlloweds').is(':disabled'))) {
        
          $('#BatchUpdateButtonAlloweds').attr('disabled', true);  
      }
      
  }
else {
    
    $('#BatchUpdateButtonAlloweds').removeAttr('disabled');
  }
}



function CorrectLinkingErrorClick() {

  $('#CorrectLinkingErrorPopUp').dialog({ title: 'Correct Linking Error', height: 350, width: 1000, modal: true, resizable: false });

  s = $('#UatpExceptionDetailsGrid').getGridParam('selrow');

  var rowcells = jQuery("#UatpExceptionDetailsGrid").jqGrid('getCell', s, 'FieldName');

  ss = $('#UatpExceptionSummary').getGridParam('selrow');
  var sss = $('#UatpExceptionDetailsGrid').getGridParam('selrow');


  $('#YourInvoiceNo').val(jQuery("#UatpExceptionDetailsGrid").jqGrid('getCell', s, 'YourInvoiceNo'));
  $('#ProvisionalInvoiceNo').val(jQuery("#UatpExceptionDetailsGrid").jqGrid('getCell', s, 'YourInvoiceNo'));
  $('#CorrespondenceRejInvoiceNo').val(jQuery("#UatpExceptionDetailsGrid").jqGrid('getCell', s, 'YourInvoiceNo'));
  $('#YourInvoiceBillingDate').val(jQuery("#UatpExceptionDetailsGrid").jqGrid('getCell', s, 'YourInvoiceBillingDate'));
  $('#CorrectLinkingFileName').val(jQuery("#UatpExceptionSummary").jqGrid('getCell', ss, 'FileName'));
  $('#CorrectLinkingErrorDescription').val(jQuery("#UatpExceptionSummary").jqGrid('getCell', ss, 'ErrorDescription'));
  $('#CorrectLinkingExceptionCode').val(jQuery("#UatpExceptionSummary").jqGrid('getCell', ss, 'ExceptionCode'));
  $('#YourRejectionMemoNo').val(jQuery("#UatpExceptionDetailsGrid").jqGrid('getCell', s, 'YourRejectionMemoNo'));

  $('#BmCmIndicator').val(jQuery("#UatpExceptionDetailsGrid").jqGrid('getCell', sss, 'BmCmIndicator'));
  if ($('#BmCmIndicator').val() != 1 && $('#BmCmIndicator').val() != "") {
    $('#YourBmCmNo').val(jQuery("#UatpExceptionDetailsGrid").jqGrid('getCell', s, 'YourBmCmNo'));
  }
  $('#TranscationId').val(jQuery("#UatpExceptionDetailsGrid").jqGrid('getCell', s, 'TranscationId'));
  $('#CorrespondenceRefNo').val(jQuery("#UatpExceptionDetailsGrid").jqGrid('getCell', s, 'CorrespondenceRefNo'));
  $('#ReasonCode').val(jQuery("#UatpExceptionDetailsGrid").jqGrid('getCell', s, 'ReasonCode'));
  $('#RejectionStage').val(jQuery("#UatpExceptionDetailsGrid").jqGrid('getCell', s, 'RejectionStage'));
  $('#InvoiceID').val(jQuery("#UatpExceptionSummary").jqGrid('getCell', ss, 'InvoiceID'));
  $('#PkReferenceId').val(jQuery("#UatpExceptionDetailsGrid").jqGrid('getCell', s, 'PkReferenceId'));
  //SCP ID : 252342 - SRM: ICH invoice in ready for billing status
  $('#LastUpdatedOn').val(jQuery("#UatpExceptionDetailsGrid").jqGrid('getCell', s, 'LastUpdatedOn'));

  $('#BatchSeqNo').val(jQuery("#UatpExceptionDetailsGrid").jqGrid('getCell', s, 'LineItemOrBatchNo'));
  $('#BatchRecordSeq').val(jQuery("#UatpExceptionDetailsGrid").jqGrid('getCell', s, 'LineItemDetailOrSequenceNo'));

  $('#FimBmCmNo').val(jQuery("#UatpExceptionDetailsGrid").jqGrid('getCell', s, 'FimBmCmNo'));
  $('#FimCouponNo').val(jQuery("#UatpExceptionDetailsGrid").jqGrid('getCell', s, 'FimCouponNo'));
  $('#InvoiceNo').val(jQuery("#UatpExceptionSummary").jqGrid('getCell', ss, 'InvoiceNo'));
  $('#FimBmCmIndicator').val(jQuery("#UatpExceptionDetailsGrid").jqGrid('getCell', s, 'FimBmCmIndicator'));
  $('#CouponNo').val(jQuery("#UatpExceptionDetailsGrid").jqGrid('getCell', s, 'CouponNo'));
  $('#SourceCodeId').val(jQuery("#UatpExceptionDetailsGrid").jqGrid('getCell', s, 'SourceCodeId'));

  var FimBmCmIndicator = jQuery("#UatpExceptionDetailsGrid").jqGrid('getCell', s, 'FimBmCmIndicator');

  if (FimBmCmIndicator == 1) {
    $('#FimBmCmIndicatorDisplay').val('None');
  } else if (FimBmCmIndicator == 2) {
    $('#FimBmCmIndicatorDisplay').val('FIM');
  } else if (FimBmCmIndicator == 3) {
    $('#FimBmCmIndicatorDisplay').val('Billing Memo');
  }
  else if (FimBmCmIndicator == 4) {
    $('#FimBmCmIndicatorDisplay').val('Credit Memo');
  }

  if ($('#TranscationId').val() == 2) {

    $('#rmpopup').show();
    $('#rmpopup1').show();
    $('#bmpopup').hide();
    $('#formdpopup').hide();
  }
  else if ($('#TranscationId').val() == 12) {
    $('#rmpopup').hide();
    $('#rmpopup1').hide();
    $('#bmpopup').show();
    $('#formdpopup').hide();
  }

  else if ($('#TranscationId').val() == 9) {
    $('#rmpopup').hide();
    $('#rmpopup1').hide();
    $('#bmpopup').hide();
    $('#formdpopup').show();
  }
}


function CloseCorrectLinkingErrorClick() {
  $('#CorrectLinkingErrorPopUp').dialog('close');
}

function closeUpdate() {
    //$('#InvoiceUpdatePopUp').Close();

    updateform.resetForm();
  $('#InvoiceUpdatePopUp').dialog('close');

}



function ValidateUpdatePopup() {
  updateform = $("#UpdateValidationForm").validate({ });

  $('#InvoiceUpdatePopUp').bind('dialogclose', function (event) {
    updateform.resetForm();
  });
}

function updatebuttonclick(isbatchupdated) {
  
  if ($('#ErrorDescription').val() != 'Invalid Original PMI' && ($('#NewValue').val() == null || $('#NewValue').val() == "")) {
      alert("New Value is required");
      return;
  }
  else {


    var billingCategory = "1";

    $.ajax({
        type: "POST",
        url: UpdateUrl,
        data: { filename: $('#UpdateFileName').val(), ExceptionCode: $('#UpdateExceptionCode').val(), ErrorDescription: $('#ErrorDescription').val(), FieldName: $('#FieldName').val(), FieldValue: $('#FieldValue').val().toString(), NewValue: $('#NewValue').val(), exceptionSummaryId: $('#ExceptionSummaryId').val(), exeptionDetailsId: $('#ExceptionDetailId').val(), isBatchUpdate: isbatchupdated, billingCat: billingCategory, errorLevel: $('#ErrorLevel').val(), pkReferenceId: $('#PkReferenceId').val(), lastUpdatedOn: $('#LastUpdatedOn').val() },
        dataType: "json",
        success: function (result) {
            if (result) {
               
                if (result == "Error") {
                    alert("Invalid value entered. Please enter valid value");
                   
                }
                else if (result == 2) {
                    alert("Unable to perform update(s) as submission and/or late submission deadlines have passed");
                    //TFS#9906 :IE:Version 11 - Update Validation Error screen is not performing any action.
                    if (isbatchupdated == 1) {
                        $('#BatchUpdatePopup').dialog('close');
                    }  
                    $('#InvoiceUpdatePopUp').dialog('close');
                    $("#UatpExceptionSummary").trigger("reloadGrid");
                }
                else  if (result == "InvoiceDeleted") {
                    alert("This invoice has been deleted.");
                } //SCP252342 - SRM: ICH invoice in ready for billing status
                else if (result == -1) {
                    alert("The transaction or invoice that you are attempting to correct has already been successfully corrected from another session. Please refresh the error correction screen to view and correct remaining errors, if any.");
                    //TFS#9906 :IE:Version 11 - Update Validation Error screen is not performing any action.
                    if (isbatchupdated == 1) {
                        $('#BatchUpdatePopup').dialog('close');
                    }  
                    $('#InvoiceUpdatePopUp').dialog('close');
                    $("#UatpExceptionSummary").trigger("reloadGrid");
                }
                else {
                    // $('#frmValidationError').reload();
                    //TFS#9906 :IE:Version 11 - Update Validation Error screen is not performing any action.
                    if (isbatchupdated == 1) {
                        $('#BatchUpdatePopup').dialog('close');
                    }  
                    $('#InvoiceUpdatePopUp').dialog('close');
                    $("#UatpExceptionSummary").trigger("reloadGrid");
                    window.parent.location.href = window.parent.location.href;

                    // $("#UatpExceptionDetailsGrid").trigger("reloadGrid");
                }
            }
            else {
                alert("Unhandled error. Please contact SIS Ops");
            }
        }
    });
  }
}

function DisplayDetails(id) {
  
  s = $('#UatpExceptionSummary').getGridParam('selrow');

  var rowcells = jQuery("#UatpExceptionSummary").jqGrid('getCell', s, 'Id');

  var exceptionCode = jQuery("#UatpExceptionSummary").jqGrid('getCell', s, 'ExceptionCode');

  var billingCategory = "1";

  // Create URL to call "GetInvoiceAndFileGridData" action passing it filter criteria
  var url = ExceptionDetailsGridDataUrl + "?" + $.param({ rowcells: rowcells, billingCategoryType: billingCategory, exceptionCode: exceptionCode });
  $("#UatpExceptionDetailsGrid").setGridParam({ url: url }).trigger("reloadGrid");


//  $.ajax({
//    type: "POST",
//    url: ExceptionDetailsGridDataUrl,
//    data: { rowcells: rowcells, billingCategoryType: billingCategory, exceptionCode: exceptionCode },
//    dataType: "json",
//    success: function (result) {
//      if (result) {

//        // Create URL to call "GetInvoiceAndFileGridData" action passing it filter criteria
//          var url = ExceptionDetailsGridDataUrl + "?" + $.param({ rowcells: rowcells,billingCategoryType: billingCategory, exceptionCode: exceptionCode });
//        $("#UatpExceptionDetailsGrid").setGridParam({ url: url }).trigger("reloadGrid");
//      }
//    }
//  });

  $.ajax({
    type: "POST",
    url: IsDisplayLinkingButtonUrl,
    data: { exceptionCode: exceptionCode },
    dataType: "json",
    success: function (result) {
      if (result) {
        if (result == 1) {
          $('#CorrectLinkingErrorButton1').removeAttr('disabled');
          $('#UpdateButton1').attr('disabled', 'disabled');
          // Buttons which have disabled attribute are not grayed out in Firefox, so if browser is Firefox remove "primaryButton" class and add "disabledButtonClassForMozilla" class 
          if ($.browser.mozilla) {
            $("#CorrectLinkingErrorButton1").removeClass('disabledButtonClassForMozilla');
            $("#CorrectLinkingErrorButton1").addClass('primaryButton');
            $("#UpdateButton1").removeClass('primaryButton');
            $("#UpdateButton1").addClass('disabledButtonClassForMozilla');
          }
        }
        else {
          $('#UpdateButton1').removeAttr('disabled');
          $('#CorrectLinkingErrorButton1').attr('disabled', 'disabled');
          // Buttons which have disabled attribute are not grayed out in Firefox, so if browser is Firefox remove "primaryButton" class and add "disabledButtonClassForMozilla" class 
          if ($.browser.mozilla) {
            $("#UpdateButton1").removeClass('disabledButtonClassForMozilla');
            $("#UpdateButton1").addClass('primaryButton');
            $("#CorrectLinkingErrorButton1").removeClass('primaryButton');
            $("#CorrectLinkingErrorButton1").addClass('disabledButtonClassForMozilla');
          }         
        }
      }
    }
  });

}

//Reset function
function resetForm() {
  $(':input', '#ValidationErrorCorrection')
        .not(':button, :submit, :reset, :hidden')
        .val('')
        .removeAttr('selected');
  $("#ChargeCategoryId").val("-1");
  ResetSearch();
}

function ResetSearch() {
  $.ajax({
    type: "POST",
    url: clearSearchUrl,
    dataType: "json",
    success: function (response) {
      if (response) {
        $('#BillingYearMonth', '#content').val(response.Year + '-' + response.Month + '-' + response.Period);
      }
    }
  });
}

function SelectFirstRow() {

  //If last contact deleted successfully then clear the form elements showing information of last record.
  var recs = $("#UatpExceptionSummary").getGridParam("records");
  if (recs == 0) {
     
    $("#UatpExceptionDetailsGrid").trigger("reloadGrid");
  }
  //clear_form_elements(".tempEdit");
  //alert('Remove This');
else {

    var selecRow = $('#UatpExceptionSummary').getGridParam('selrow');
    if (selecRow == null) {
        var rows = $("#UatpExceptionSummary").getDataIDs();
        $("#UatpExceptionSummary").jqGrid('setSelection', rows[0], true);
     // $('#UatpExceptionSummary').setSelection(rows[1], true);
    }
    else {
        $("#UatpExceptionSummary").jqGrid('setSelection', selecRow, true);
      //$("#UatpExceptionSummary").setSelection(selecRow, true);
    }
  }
}

function SelectFirstDetailRow() {
 
  //If last contact deleted successfully then clear the form elements showing information of last record.
  var recs = $("#UatpExceptionDetailsGrid").getGridParam("records");
  if (recs == 0) {
  }
  else {
    var selecRow = $('#UatpExceptionDetailsGrid').getGridParam('selrow');
    if (selecRow == null) {
        var rows = $("#UatpExceptionDetailsGrid").getDataIDs();
        $("#UatpExceptionDetailsGrid").jqGrid('setSelection', rows[0], true);
     // $("#UatpExceptionDetailsGrid").setSelection(rows[0], true);
    }
  else {
      $("#UatpExceptionDetailsGrid").jqGrid('setSelection', selecRow, true);
      //$("#UatpExceptionDetailsGrid").setSelection(selecRow, true);
    }
  }
}


function BatchUpdatebuttonclick() {
  if ($('#NewValue').val() == null || $('#NewValue').val() == "") {
      alert("new Value is required");
      return;
  }
  else {

    ss = $('#UatpExceptionSummary').getGridParam('selrow');
    var ExceptionSummaryId = jQuery("#UatpExceptionSummary").jqGrid('getCell', ss, 'Id');
    var oldvalue = $('#FieldValue').val();
    var exceptionCode = $('#UpdateExceptionCode').val();
    var fieldVal = $('#NewValue').val();
    var billingCategoryId = "1";

    $.ajax({
      type: "POST",
      url: ValidateErrorUrl,
      data: { exceptionCode: exceptionCode, NewValue: fieldVal, pkReferenceId: $('#PkReferenceId').val() },
      dataType: "json",
      success: function (result) {
        if (result == 1) {

          $.ajax({
            type: "POST",
            url: BatchUpdatedCountUrl,
            data: { summaryId: ExceptionSummaryId, oldvalue: oldvalue, exceptionCode: exceptionCode, billingCategoryId: billingCategoryId },
            dataType: "json",
            success: function (result) {
              if (result) {

                $('#BatchUpdatedFieldCount').val(result);
                $('#BatchUpdatePopup').dialog({ title: 'Batch Update Popup', height: 150, width: 200, modal: true, resizable: false });
              }
            }
          });

        }
        else if (result == 0) {
          alert("Invalid value entered. Please enter valid value");
        }
        else {
          alert("Unhandled error. Please contact SIS Ops");
        }
      }
    });



  }

}


function DisplayUpdateDetails(id) {


  s = $('#UatpExceptionDetailsGrid').getGridParam('selrow');

  var rowcells = jQuery("#UatpExceptionDetailsGrid").jqGrid('getCell', s, 'FieldName');

  ss = $('#UatpExceptionSummary').getGridParam('selrow');
  var sss = $('#UatpExceptionDetailsGrid').getGridParam('selrow');


  $('#FieldName').val(jQuery("#UatpExceptionDetailsGrid").jqGrid('getCell', s, 'FieldName'));
  $('#FieldValue').val(jQuery("#UatpExceptionDetailsGrid").jqGrid('getCell', s, 'FieldValue'));
  $('#UpdateFileName').val(jQuery("#UatpExceptionSummary").jqGrid('getCell', ss, 'FileName'));
  $('#ErrorDescription').val(jQuery("#UatpExceptionSummary").jqGrid('getCell', ss, 'ErrorDescription'));
  $('#UpdateExceptionCode').val(jQuery("#UatpExceptionSummary").jqGrid('getCell', ss, 'ExceptionCode'));
  $('#BatchUpdateAllowed').val(jQuery("#UatpExceptionSummary").jqGrid('getCell', ss, 'BatchUpdateAllowed'));
  $('#ExceptionSummaryId').val(jQuery("#UatpExceptionSummary").jqGrid('getCell', ss, 'Id'));
  $('#ExceptionDetailId').val(jQuery("#UatpExceptionDetailsGrid").jqGrid('getCell', sss, 'ExceptionDetailId'));
  $('#ErrorLevel').val(jQuery("#UatpExceptionDetailsGrid").jqGrid('getCell', sss, 'ErrorLevel'));
  // $('#NewValue').val(' ');

}

function closeBatchUpdate() {
  $('#BatchUpdatePopup').dialog('close');
}

var nonSamplingSearchResult;
function ShowSearchResult() {

    var billingMonthYearPeriod = $("#NonSamplingBillingYearMonth").val();

    if (billingMonthYearPeriod == "") {
        alert('Please select billing period');
        return false;
    }
    

  var billingMonthYearPeriodTokens = billingMonthYearPeriod.split('-');
  var sBillingPeriod = billingMonthYearPeriodTokens[2];
  var sBillingMonth = billingMonthYearPeriodTokens[1];
  var sBillingYear = billingMonthYearPeriodTokens[0];

  //var sBillingPeriod = $("#BillingPeriod").val();

  //var sBillingMonth = $("#BillingMonth").val();

  //var sBillingYear = $("#BillingYear").val();
  var sExceptionCodeId = $("#ExceptionCodeId").val();
  if ($("#ExceptionCode").val() == "")
    sExceptionCodeId = -1;

  var sBilledMemberId = $("#BilledMemberId").val();
  if ($("#BilledMember").val() == "")
    sBilledMemberId = -1;

  var sInvoiceNumber = $("#InvoiceNumber").val();
  var sFileName = $("#NSFileName").val();
  var sFileSubmissionDate = $("#FileSubmissionDate").val();
  var showSearchResultData = nonSamplingSearchResult;
//  $.ajax(
//            {
//              type: "POST",
//              url: showSearchResultData,
//              // data: { billingPeriod: sBillingPeriod, billingMonth: sBillingMonth },
//              data: { billingPeriod: sBillingPeriod, billingMonth: sBillingMonth, billingYear: sBillingYear, exceptionCode: sExceptionCodeId, billedMemberId: sBilledMemberId, invoiceNumber: sInvoiceNumber, fileSubmissionDate:sFileSubmissionDate, fileName: sFileName },
//              dataType: "json",
//              success: function (result) {
//                //$("#ISSearchResultListGrid").jqGrid('setGridParam', { url: url, page: 1 }).trigger("reloadGrid");
//                $("#UatpExceptionSummary").trigger("reloadGrid");
//                if (result == 1) {
//                  // alert(showSearchResultData);
//                }
//                else {
//                  //alert(result);
//                }
//              }
  //            });
  
  var url = showSearchResultData + "?" + $.param({ billingPeriod: sBillingPeriod, billingMonth: sBillingMonth, billingYear: sBillingYear, exceptionCode: sExceptionCodeId, billedMemberId: sBilledMemberId, invoiceNumber: sInvoiceNumber, fileSubmissionDate: sFileSubmissionDate, fileName: sFileName });
  $("#UatpExceptionSummary").jqGrid('setGridParam', { url: url, page: 1 }).trigger("reloadGrid");
  
}

//function selectFirstRowOnGridLoad() {
//  var rows = $("#UatpExceptionSummary").getDataIDs();
//  $("#UatpExceptionSummary").jqGrid('setSelection', rows[0], true);
//}

function updateCorrctLinkingbuttonclick() {
    
  if ($('#rmpopup').is(":visible") == true) {
    if ($('#YourInvoiceNo').val() == null || $('#YourInvoiceNo').val() == "") {
      alert("Rejected Invoice No field is mandatory");
      return;
    }

    else if ($('#YourInvoiceBillingDate').val() == null || $('#YourInvoiceBillingDate').val() == "") {
        alert("Your Invoice Billing Date field is mandatory");
      return;
    }
    else if ($('#YourInvoiceBillingDate').val().length != 8) {
        alert("Invalid Your Invoice Billing Date");
      return;
    }
    //    else if ($('FimBmCmNo').val() == null || ('FimBmCmNo').val() == "") {
    //      alert("FIM Number/Billing Memo/Credit Memo Number is mandatory");
    //    }

    //    else if ($('FimCouponNo').val() == null || ('FimCouponNo').val() == "") {
    //      alert("FIM Coupon Number is mandatory");
    //    }

  }
else if ($('#formdpopup').is(":visible") == true) {

if ($('#ProvisionalInvoiceNo').val() == null || $('#ProvisionalInvoiceNo').val() == "") {
          alert("Provisional Invoice No. is mandatory");
          return;

    }
  else if ($('#BatchSeqNo').val() == null || $('#BatchSeqNo').val() == "") {
      alert("Batch Number of Provisional Invoice is mandatory");
      return;
    }
  else if ($('#BatchRecordSeq').val() == null || $('#BatchRecordSeq').val() == "") {
      alert("Record Sequence within Batch of Provisional Invoice is mandatory");
      return;
    }
  }
  else {
    if ($('#CorrespondenceRefNo').val() == null || $('#CorrespondenceRefNo').val() == "") {
      alert("Please enter valid Correspondence Reference Number");
      return;
    }
    //    else if ($('#CorrespondenceRejInvoiceNo').val() == null || $('#CorrespondenceRejInvoiceNo').val() == "") {
    //      alert("Rejection Invoice Number field is mandatory");
    //      return;
    //    }
  }

  s = $('#UatpExceptionDetailsGrid').getGridParam('selrow');
  var summarygridSelectedrow = $('#UatpExceptionSummary').getGridParam('selrow');

  var billedMemberId = jQuery("#UatpExceptionSummary").jqGrid('getCell', summarygridSelectedrow, 'BilledMemberId');
  var billingMemberId = jQuery("#UatpExceptionSummary").jqGrid('getCell', summarygridSelectedrow, 'BillingMemberId');
  var detailid = jQuery("#UatpExceptionDetailsGrid").jqGrid('getCell', s, 'ExceptionDetailId');
  var invoiceId = jQuery("#UatpExceptionSummary").jqGrid('getCell', summarygridSelectedrow, 'InvoiceID');
  var errorLevel = jQuery("#UatpExceptionDetailsGrid").jqGrid('getCell', s, 'ErrorLevel');
  var yourBillingDate = $('#YourInvoiceBillingDate').val().toString();
  var yourbillingYear = 0; var yourbillingMonth = 0; var yourbillingPeriod = 0;

  if (yourBillingDate.length == 8) {
    yourbillingYear = yourBillingDate.substr(0, 4);
    yourbillingMonth = yourBillingDate.substr(4, 2);
    yourbillingPeriod = yourBillingDate.substr(6, 2);
  }

  var linkingDetail;

  if ($('#TranscationId').val() == 2) {
    linkingDetail = 1;
  }

  else if ($('#TranscationId').val() == 12) { //Write the logic for form D
    linkingDetail = 2;
  }

  else if ($('#TranscationId').val() == 9) {
    linkingDetail = 3;
    $('#YourInvoiceNo').val($('#CorrespondenceRejInvoiceNo').val());
  }
  else if ($('#TranscationId').val() == 8) {
    linkingDetail = 4;
  }
  
  var jsonObj = { "ReasonCode": $('#ReasonCode').val(),
    "YourInvoiceNo": $('#YourInvoiceNo').val(),
    "BillingMonth": $('#BillingMonth').val(),
    "BillingYear": $('#BillingYear').val(),
    "BillingPeriod": $('#BillingPeriod').val(),
    "YourBmCmNo": $('#YourBmCmNo').val(),
    "YourRejectionMemoNo": $('#YourRejectionMemoNo').val(),
    "BmCmIndicator": $('#BmCmIndicator').val(),
    "RejectionStage": $('#RejectionStage').val(),
    "BilledMemberId": billedMemberId,
    "BillingMemberId": billingMemberId,
    "InvoiceID": invoiceId,
    "Ignorevalidation": false,
    "PkReferenceId": $('#PkReferenceId').val(),
    "TranscationId": $('#TranscationId').val(),
    "ExceptionDetailId": detailid,
    "LinkingDetail": linkingDetail,
    "YourInvoiceMonth": yourbillingMonth,
    "YourInvoiceYear": yourbillingYear,
    "YourInvoicePeriod": yourbillingPeriod,
    "CorrespondenceRefNo": $('#CorrespondenceRefNo').val(),
    "BatchSeqNo": $('#BatchSeqNo').val(),
    "BatchRecordSeq": $('#BatchRecordSeq').val(),
    "FimBmCmNo": $('#FimBmCmNo').val(),
    "FimCouponNo": $('#FimCouponNo').val(),
    "InvoiceNo": $('#InvoiceNo').val(),
    "CouponNo": $('#CouponNo').val(),
    "FimBmCmIndicator": $('#FimBmCmIndicator').val(),
    "ProvisionalInvoiceNo": $('#ProvisionalInvoiceNo').val(),
    "ErrorLevel": errorLevel,
    "SourceCodeId": $('#SourceCodeId').val(),
    "LastUpdatedOn": $('#LastUpdatedOn').val()
};


$.ajax({
    type: "POST",
    url: UpdateCorrectLinkingErrorUrl,
    data: jsonObj,
    dataType: "json",
    success: function (result) {

        if (result == 1 || result == 3) {
            $('#CorrectLinkingErrorPopUp').dialog('close');
            $("#UatpExceptionSummary").trigger("reloadGrid");
        }

        else if (result == 2) {
            alert("Unable to perform update(s) as submission and/or late submission deadlines have passed");
            $('#CorrectLinkingErrorPopUp').dialog('close');
            $("#UatpExceptionSummary").trigger("reloadGrid");
        } //SCP252342 - SRM: ICH invoice in ready for billing status
        else if (result == -1) {
            alert("The transaction or invoice that you are attempting to correct has already been successfully corrected from another session. Please refresh the error correction screen to view and correct remaining errors, if any.");
            $('#CorrectLinkingErrorPopUp').dialog('close');
            $("#UatpExceptionSummary").trigger("reloadGrid");
        }
        else if (result == 9) {
            alert("Invalid Your Invoice Billing Date.");
        }
        else {
            //CMP#641: Time Limit Validation on Third Stage PAX Rejections
            if (result.indexOf("Time Limit") > -1) {
                alert(result.toString());
            }
            //CMP#674-Validation of Coupon and AWB Breakdowns in Rejections
            else if (result.indexOf("Mismatch in coupon") > -1) {
                alert(result.toString());
            }
            else {
                alert(result.toString() + "Linking is failed");
            }
        }
    }
});

}
