
var muCorrespondenceTransactionType = 37;

var ExceptionSummaryGridDatasUrl, ExceptionDetailsGridDataUrl, UpdateCorrectLinkingErrorUrl, IsDisplayLinkingButtonUrl, UpdateUrl, ValidateErrorUrl, BatchUpdatedCountUrl;

function SamplingUpdatePopUpClick() {
    $('#SamplingUpdatePopUp').dialog({ title: 'Update Validation Error', height: 350, width: 1000, modal: true, resizable: false });

    s = $('#SamplingExceptionDetailsGrid').getGridParam('selrow');

    var rowcells = jQuery("#SamplingExceptionDetailsGrid").jqGrid('getCell', s, 'FieldName');

    ss = $('#SamplingExceptionSummaryGrid').getGridParam('selrow');
    var sss = $('#SamplingExceptionDetailsGrid').getGridParam('selrow');


    $('#SamplingFieldName').val(jQuery("#SamplingExceptionDetailsGrid").jqGrid('getCell', s, 'FieldName'));
    $('#SamplingFieldValue').val(jQuery("#SamplingExceptionDetailsGrid").jqGrid('getCell', s, 'FieldValue'));
    $('#SamplingUpdateFileName').val(jQuery("#SamplingExceptionSummaryGrid").jqGrid('getCell', ss, 'FileName'));
    $('#SamplingErrorDescription').val(jQuery("#SamplingExceptionSummaryGrid").jqGrid('getCell', ss, 'ErrorDescription'));
    $('#SamplingUpdateExceptionCode').val(jQuery("#SamplingExceptionSummaryGrid").jqGrid('getCell', ss, 'ExceptionCode'));
    $('#BatchUpdateAllowed').val(jQuery("#SamplingExceptionSummaryGrid").jqGrid('getCell', ss, 'BatchUpdateAllowed'));
    $('#ExceptionSummaryId').val(jQuery("#SamplingExceptionSummaryGrid").jqGrid('getCell', ss, 'Id'));
    $('#ExceptionDetailId').val(jQuery("#SamplingExceptionDetailsGrid").jqGrid('getCell', sss, 'ExceptionDetailId'));
    $('#ErrorLevel').val(jQuery("#SamplingExceptionDetailsGrid").jqGrid('getCell', sss, 'ErrorLevel'));
    $('#PkReferenceId').val(jQuery("#SamplingExceptionDetailsGrid").jqGrid('getCell', sss, 'PkReferenceId'));
    //SCP252342 - SRM: ICH invoice in ready for billing status
    $('#LastUpdatedOn').val(jQuery("#SamplingExceptionDetailsGrid").jqGrid('getCell', sss, 'LastUpdatedOn'));
   
    if ($('#BatchUpdateAllowed').val() == "No") {
        
        $('#BatchUpdateButtonAllowed').attr('disabled', 'disabled');

    }
    else {
        $('#BatchUpdateButtonAllowed').removeAttr('disabled');
    }
}



function SamplingCorrectLinkingErrorClick() {

    $('#SamplingCorrectLinkingErrorPopUp').dialog({ title: 'Correct Linking Error', height: 350, width: 1000, modal: true, resizable: false });

    s = $('#SamplingExceptionDetailsGrid').getGridParam('selrow');

    var rowcells = jQuery("#SamplingExceptionDetailsGrid").jqGrid('getCell', s, 'FieldName');

    ss = $('#SamplingExceptionSummaryGrid').getGridParam('selrow');
    var sss = $('#SamplingExceptionDetailsGrid').getGridParam('selrow');


    $('#SamplingYourInvoiceNo').val(jQuery("#SamplingExceptionDetailsGrid").jqGrid('getCell', s, 'YourInvoiceNo'));
   
    $('#CorrespondenceRejInvoiceNo').val(jQuery("#SamplingExceptionDetailsGrid").jqGrid('getCell', s, 'YourInvoiceNo'));
    $('#YourInvoiceBillingDate').val(jQuery("#SamplingExceptionDetailsGrid").jqGrid('getCell', s, 'YourInvoiceBillingDate'));
    $('#samplingFileName').val(jQuery("#SamplingExceptionSummaryGrid").jqGrid('getCell', ss, 'FileName'));
       
    $('#SamplingLinkErrorDescription').val(jQuery("#SamplingExceptionSummaryGrid").jqGrid('getCell', ss, 'ErrorDescription'));
    $('#SamplingExceptionCode').val(jQuery("#SamplingExceptionSummaryGrid").jqGrid('getCell', ss, 'ExceptionCode'));
    $('#YourRejectionMemoNo').val(jQuery("#SamplingExceptionDetailsGrid").jqGrid('getCell', s, 'YourRejectionMemoNo'));

    $('#BmCmIndicator').val(jQuery("#SamplingExceptionDetailsGrid").jqGrid('getCell', sss, 'BmCmIndicator'));
    if ($('#BmCmIndicator').val() != 1 && $('#BmCmIndicator').val() != "") {
        $('#YourBmCmNo').val(jQuery("#SamplingExceptionDetailsGrid").jqGrid('getCell', s, 'YourBmCmNo'));
    }
    $('#TranscationId').val(jQuery("#SamplingExceptionDetailsGrid").jqGrid('getCell', s, 'TranscationId'));
    $('#CorrespondenceRefNo').val(jQuery("#SamplingExceptionDetailsGrid").jqGrid('getCell', s, 'CorrespondenceRefNo'));
    $('#ReasonCode').val(jQuery("#SamplingExceptionDetailsGrid").jqGrid('getCell', s, 'ReasonCode'));
    $('#RejectionStage').val(jQuery("#SamplingExceptionDetailsGrid").jqGrid('getCell', s, 'RejectionStage'));
    $('#InvoiceID').val(jQuery("#SamplingExceptionSummaryGrid").jqGrid('getCell', ss, 'InvoiceID'));
    $('#PkReferenceId').val(jQuery("#SamplingExceptionDetailsGrid").jqGrid('getCell', s, 'PkReferenceId'));
    //SCP252342 - SRM: ICH invoice in ready for billing status
    $('#LastUpdatedOn').val(jQuery("#SamplingExceptionDetailsGrid").jqGrid('getCell', s, 'LastUpdatedOn'));
    $('#SamplingBatchSeqNo').val(jQuery("#SamplingExceptionDetailsGrid").jqGrid('getCell', s, 'LineItemOrBatchNo'));

    $('#SamplingBatchRecordSeq').val(jQuery("#SamplingExceptionDetailsGrid").jqGrid('getCell', s, 'LineItemDetailOrSequenceNo'));
    $('#FimBmCmNo').val(jQuery("#SamplingExceptionDetailsGrid").jqGrid('getCell', s, 'FimBmCmNo'));
    $('#FimCouponNo').val(jQuery("#SamplingExceptionDetailsGrid").jqGrid('getCell', s, 'FimCouponNo'));
    $('#InvoiceNo').val(jQuery("#SamplingExceptionSummaryGrid").jqGrid('getCell', ss, 'InvoiceNo'));
    
    if ($('#TranscationId').val() == 8) {

        $('#rmpopupFormc').show();
        $('#bmpopupFormc').hide();
    }
    //    else if ($('#TranscationId').val() == 12) {
    //        $('#rmpopup').hide();
    //        $('#bmpopup').show();
    //    }
}


function SamplingCloseCorrectLinkingErrorClick() {
    $('#SamplingCorrectLinkingErrorPopUp').dialog('close');
}

function SamplingcloseUpdate() {
    //$('#SamplingUpdatePopUp').Close();
    /* SCP#422361 - KAL:ISSUE WITH FORM C ERROR CORRECTION.
    Desc: Added exception handlng to prevent code from crashing across browsers.*/
    try {
            updateform.resetForm();
        }catch(e) { }
    $('#SamplingUpdatePopUp').dialog('close');

}



function SamplingValidateUpdatePopup() {
    updateform = $("#SamplingUpdateValidationForm").validate({
        rules: {
            SamplingNewValue: "required"
        },
        messages: {
            SamplingNewValue: "Invalid value entered. Please enter valid value"
        }
    });

    $('#SamplingUpdatePopUp').bind('dialogclose', function (event) {
        /* SCP#422361 - KAL:ISSUE WITH FORM C ERROR CORRECTION.
        Desc: Added exception handlng to prevent code from crashing across browsers.*/
        try {
            updateform.resetForm();
        }catch(e) { }

    });
}




function Samplingupdatebuttonclick(isbatchupdated) {

    
    if ($('#SamplingNewValue').val() == null || $('#SamplingNewValue').val() == "") {
        alert("new Value is required");
        return;
    }
    else {


        var billingCategory = "1";
        
        $.ajax({
            type: "POST",
            url: UpdateUrl,
            data: { filename: $('#SamplingUpdateFileName').val(), ExceptionCode: $('#SamplingUpdateExceptionCode').val(), ErrorDescription: $('#SamplingErrorDescription').val(), FieldName: $('#SamplingFieldName').val(), FieldValue: $('#SamplingFieldValue').val().toString(), NewValue: $('#SamplingNewValue').val(), exceptionSummaryId: $('#ExceptionSummaryId').val(), exeptionDetailsId: $('#ExceptionDetailId').val(), isBatchUpdate: isbatchupdated, billingCat: billingCategory, errorLevel: $('#ErrorLevel').val(), pkReferenceId: $('#PkReferenceId').val(), lastUpdatedOn: $('#LastUpdatedOn').val() },
            dataType: "json",
            success: function (result) {
                if (result) {
                 
                    if (result == "Error") {
                        alert("Invalid value entered. Please enter valid value");
                    }
                    else if (result == 2) {
                        alert("Unable to perform update(s) as submission and/or late submission deadlines have passed");
                        if (isbatchupdated == 1) {
                        $('#BatchUpdatePopup').dialog('close');
                        }   
                        $('#SamplingUpdatePopUp').dialog('close');
                        $("#SamplingExceptionSummaryGrid").trigger("reloadGrid");
                    } //SCP252342 - SRM: ICH invoice in ready for billing status
                     else if (result == -1) {
                        alert("The transaction or invoice that you are attempting to correct has already been successfully corrected from another session. Please refresh the error correction screen to view and correct remaining errors, if any.");
                         if (isbatchupdated == 1) {
                        $('#BatchUpdatePopup').dialog('close');
                        }   
                        $('#SamplingUpdatePopUp').dialog('close');
                        $("#SamplingExceptionSummaryGrid").trigger("reloadGrid");
                    }
                     else  if (result == "InvoiceDeleted") {
                    alert("This invoice has been deleted.");
                    }
                    else {
                      //  $('#frmValidationError').reload();
                         if (isbatchupdated == 1) {
                        $('#BatchUpdatePopup').dialog('close');
                        }   
                        $('#SamplingUpdatePopUp').dialog('close');
                        $("#SamplingExceptionSummaryGrid").trigger("reloadGrid");
                        $("#SamplingExceptionDetailsGrid").trigger("reloadGrid");
                    }
                }
                else {
                    alert("Unhandled error. Please contact SIS Ops");
                }
            }
        });
    }
}

function SamplingDisplayDetails(id) {

    s = $('#SamplingExceptionSummaryGrid').getGridParam('selrow');

    var rowcells = jQuery("#SamplingExceptionSummaryGrid").jqGrid('getCell', s, 'Id');

    var exceptionCode = jQuery("#SamplingExceptionSummaryGrid").jqGrid('getCell', s, 'ExceptionCode');

    var billingCategory = "1";

    $.ajax({
        type: "POST",
        url: ExceptionDetailsGridDataUrl,
        data: { rowcells: rowcells, billingCategoryType: billingCategory, exceptionCode: exceptionCode },
        dataType: "json",
        success: function (result) {
            if (result) {

                // Create URL to call "GetInvoiceAndFileGridData" action passing it filter criteria
                var url = ExceptionDetailsGridDataUrl + "?" + $.param({ rowcells: rowcells, exceptionCode: exceptionCode });
                $("#SamplingExceptionDetailsGrid").setGridParam({ url: url }).trigger("reloadGrid");
            }
        }
    });

    $.ajax({
      type: "POST",
      url: IsDisplayLinkingButtonUrl,
      data: { exceptionCode: exceptionCode },
      dataType: "json",
      success: function (result) {
        if (result) {
          if (result == 1) {
            $('#SamplingCorrectLinkingErrorButton1').removeAttr('disabled');
            $('#SamplingUpdateButton1').attr('disabled', 'disabled');
            // Buttons which have disabled attribute are not grayed out in Firefox, so if browser is Firefox remove "primaryButton" class and add "disabledButtonClassForMozilla" class 
            if ($.browser.mozilla) {
              $("#SamplingCorrectLinkingErrorButton1").removeClass('disabledButtonClassForMozilla');
              $("#SamplingCorrectLinkingErrorButton1").addClass('primaryButton');
              $("#SamplingUpdateButton1").removeClass('primaryButton');
              $("#SamplingUpdateButton1").addClass('disabledButtonClassForMozilla');
            }
          }
          else {
            $('#SamplingUpdateButton1').removeAttr('disabled');
            $('#SamplingCorrectLinkingErrorButton1').attr('disabled', 'disabled');
            // Buttons which have disabled attribute are not grayed out in Firefox, so if browser is Firefox remove "primaryButton" class and add "disabledButtonClassForMozilla" class 
            if ($.browser.mozilla) {
              $("#SamplingUpdateButton1").removeClass('disabledButtonClassForMozilla');
              $("#SamplingUpdateButton1").addClass('primaryButton');
              $("#SamplingCorrectLinkingErrorButton1").removeClass('primaryButton');
              $("#SamplingCorrectLinkingErrorButton1").addClass('disabledButtonClassForMozilla');
            }
          }
        }
      }
    });

}



function SamplingSelectFirstRow() {

    //If last contact deleted successfully then clear the form elements showing information of last record.
    var recs = $("#SamplingExceptionSummaryGrid").getGridParam("records");
    if (recs == 0) {
        $("#SamplingExceptionDetailsGrid").trigger("reloadGrid");
    }
    //clear_form_elements(".tempEdit");
    //alert('Remove This');
    else {
        var selecRow = $('#SamplingExceptionSummaryGrid').getGridParam('selrow');
        if (selecRow == null) {
            var rows = $("#SamplingExceptionSummaryGrid").getDataIDs();
            // $("#SamplingExceptionSummaryGrid").setSelection(rows[0], true);
            $("#SamplingExceptionSummaryGrid").jqGrid('setSelection', rows[0], true);
        }
        else {
            // $("#SamplingExceptionSummaryGrid").setSelection(selecRow, true);
            $("#SamplingExceptionSummaryGrid").jqGrid('setSelection', selecRow, true);
        }
    }
}

function SamplingSelectFirstDetailRow() {

    //If last contact deleted successfully then clear the form elements showing information of last record.
    var recs = $("#SamplingExceptionDetailsGrid").getGridParam("records");
    if (recs == 0) {
    }
    else {
        var selecRow = $('#SamplingExceptionDetailsGrid').getGridParam('selrow');
        if (selecRow == null) {
            var rows = $("#SamplingExceptionDetailsGrid").getDataIDs();
            //$("#SamplingExceptionDetailsGrid").setSelection(rows[0], true);
            $("#SamplingExceptionDetailsGrid").jqGrid('setSelection', rows[0], true);
        }
        else {
            // $("#SamplingExceptionDetailsGrid").setSelection(selecRow, true);
            $("#SamplingExceptionDetailsGrid").jqGrid('setSelection', selecRow, true);
        }
    }
}


function SamplingBatchUpdatebuttonclick() {
    if ($('#SamplingNewValue').val() == null || $('#SamplingNewValue').val() == "") {
        //alert("new Value is required");
    }
    else {

        ss = $('#SamplingExceptionSummaryGrid').getGridParam('selrow');
        var ExceptionSummaryId = jQuery("#SamplingExceptionSummaryGrid").jqGrid('getCell', ss, 'Id');
        var oldvalue = $('#SamplingFieldValue').val();
        var exceptionCode = $('#SamplingUpdateExceptionCode').val();
        var fieldVal = $('#SamplingNewValue').val();
        var billingCategoryId = "1"

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


function SamplingDisplayUpdateDetails(id) {


    s = $('#SamplingExceptionDetailsGrid').getGridParam('selrow');

    var rowcells = jQuery("#SamplingExceptionDetailsGrid").jqGrid('getCell', s, 'FieldName');

    ss = $('#SamplingExceptionSummaryGrid').getGridParam('selrow');
    var sss = $('#SamplingExceptionDetailsGrid').getGridParam('selrow');


    $('#SamplingFieldName').val(jQuery("#SamplingExceptionDetailsGrid").jqGrid('getCell', s, 'FieldName'));
    $('#SamplingFieldValue').val(jQuery("#SamplingExceptionDetailsGrid").jqGrid('getCell', s, 'FieldValue'));
    $('#SamplingUpdateFileName').val(jQuery("#SamplingExceptionSummaryGrid").jqGrid('getCell', ss, 'FileName'));
    $('#SamplingErrorDescription').val(jQuery("#SamplingExceptionSummaryGrid").jqGrid('getCell', ss, 'ErrorDescription'));
    $('#SamplingUpdateExceptionCode').val(jQuery("#SamplingExceptionSummaryGrid").jqGrid('getCell', ss, 'ExceptionCode'));
    $('#BatchUpdateAllowed').val(jQuery("#SamplingExceptionSummaryGrid").jqGrid('getCell', ss, 'BatchUpdateAllowed'));
    $('#ExceptionSummaryId').val(jQuery("#SamplingExceptionSummaryGrid").jqGrid('getCell', ss, 'Id'));
    $('#ExceptionDetailId').val(jQuery("#SamplingExceptionDetailsGrid").jqGrid('getCell', sss, 'ExceptionDetailId'));
    $('#ErrorLevel').val(jQuery("#SamplingExceptionDetailsGrid").jqGrid('getCell', sss, 'ErrorLevel'));   
    /* SCP#422361 - KAL:ISSUE WITH FORM C ERROR CORRECTION.
    Desc: Blank out new field value by default */
    $('#SamplingNewValue').val('');
}

function SamplingcloseBatchUpdate() {
    $('#BatchUpdatePopup').dialog('close');
}
var samplingSearchResult;
function ShowSearchResultPax() {

    var billingMonthYearPeriod = $("#BillingYearMonth").val();

    var billingMonthYearPeriodTokens = billingMonthYearPeriod.split('-');

    var sBillingMonth = billingMonthYearPeriodTokens[1];
    var sBillingYear = billingMonthYearPeriodTokens[0];
    //var sBillingPeriod = $("#BillingPeriod").val();

    //var sBillingMonth = $("#BillingMonth").val();

    //var sBillingYear = $("#BillingYear").val();


    var sExceptionCodeId = $("#ExceptionCodeId").val();
    if ($("#paxExceptionCode").val() == "")
        sExceptionCodeId = -1;

    var sBilledMemberId = $("#BilledMemberId").val();
    if ($("#provBillingMember").val() == "")
        sBilledMemberId = -1;

    var sFileSubmissionDate = $("#paxFileSubmissionDate").val();
    var sFileName = $("#FileName").val();

    var showSearchResultData = samplingSearchResult;
    //    $.ajax(
    //            {
    //                type: "POST",
    //                url: showSearchResultData,
    //                // data: { billingPeriod: sBillingPeriod, billingMonth: sBillingMonth },
    //                data: { billingMonth: sBillingMonth, billingYear: sBillingYear, exceptionCode: sExceptionCodeId, billedMemberId: sBilledMemberId, fileName: sFileName, fileSubmissionDate: sFileSubmissionDate },
    //                dataType: "json",
    //                success: function (result) {
    //                    if (result == 1) {
    //                        // alert(showSearchResultData);
    //                    }
    //                    else {
    //                        //alert(result);
    //                    }
    //                }
    //});

    var url = showSearchResultData + "?" + $.param({ billingMonth: sBillingMonth, billingYear: sBillingYear, exceptionCode: sExceptionCodeId, billedMemberId: sBilledMemberId, fileName: sFileName, fileSubmissionDate: sFileSubmissionDate });
    $("#SamplingExceptionSummaryGrid").jqGrid('setGridParam', { url: url, page: 1, loadComplete: function () { selectFirstSamplingRowOnGridLoad(); } }).trigger("reloadGrid");
}

function selectFirstSamplingRowOnGridLoad() {
    var rows = $("#SamplingExceptionSummaryGrid").getDataIDs();
    $("#SamplingExceptionSummaryGrid").jqGrid('setSelection', rows[0], true);
}

function SamplingupdateCorrctLinkingbuttonclick() {
    if ($('#rmpopupFormc').is(":visible") == true) {
       
        if ($('#SamplingYourInvoiceNo').val() == null || $('#SamplingYourInvoiceNo').val() == "") {
            alert("Provisional Invoice No. is mandatory");
            return;

        }
        else if ($('#SamplingBatchSeqNo').val() == null || $('#SamplingBatchSeqNo').val() == "") {
            alert("Batch Number of Provisional Invoice is mandatory");
            return;
        }
        else if ($('#SamplingBatchRecordSeq').val() == null || $('#SamplingBatchRecordSeq').val() == "") {
            alert("Record Sequence within Batch of Provisional Invoice is mandatory");
            return;
        }
    }


    s = $('#SamplingExceptionDetailsGrid').getGridParam('selrow');
    var summarygridSelectedrow = $('#SamplingExceptionSummaryGrid').getGridParam('selrow');

    var billedMemberId = jQuery("#SamplingExceptionSummaryGrid").jqGrid('getCell', summarygridSelectedrow, 'BilledMemberId');
    var billingMemberId = jQuery("#SamplingExceptionSummaryGrid").jqGrid('getCell', summarygridSelectedrow, 'BillingMemberId');
    var detailid = jQuery("#SamplingExceptionDetailsGrid").jqGrid('getCell', s, 'ExceptionDetailId');
    var invoiceId = jQuery("#SamplingExceptionSummaryGrid").jqGrid('getCell', summarygridSelectedrow, 'InvoiceID');
    var yourBillingDate = $('#YourInvoiceBillingDate').val().toString();
    var yourbillingYear = 0; var yourbillingMonth = 0; var yourbillingPeriod = 0;

    if (yourBillingDate.length == 8) {
        yourbillingYear = yourBillingDate.substr(0, 4);
        yourbillingMonth = yourBillingDate.substr(4, 2);
        yourbillingPeriod = yourBillingDate.substr(6, 2);
    }

    var linkingDetail;

    if ($('#TranscationId').val() == 8) {
        linkingDetail = 4;
    }

    var jsonObj = { "ReasonCode": $('#ReasonCode').val(),
        "YourInvoiceNo": $('#SamplingYourInvoiceNo').val(),
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
        "BatchSeqNo": $('#SamplingBatchSeqNo').val(),
        "BatchRecordSeq": $('#SamplingBatchRecordSeq').val(),
        "FimBmCmNo": $('#FimBmCmNo').val(),
        "FimCouponNo": $('#FimCouponNo').val(),
        "InvoiceNo": $('#InvoiceNo').val(),
        "ProvisionalInvoiceNo": $('#SamplingYourInvoiceNo').val(),
        "LastUpdatedOn": $('#LastUpdatedOn').val(),
     };


    $.ajax({
        type: "POST",
        url: UpdateCorrectLinkingErrorUrl,
        data: jsonObj,
        dataType: "json",
        success: function (result) {
       
            if (result == 1 || result == 3) {
                $('#SamplingCorrectLinkingErrorPopUp').dialog('close');
                $("#SamplingExceptionSummaryGrid").trigger("reloadGrid");
            }

            else if (result == 2) {
                alert("Cannot update invoice status as billing period and Late submission window is closed");
                $('#SamplingCorrectLinkingErrorPopUp').dialog('close');
                $("#SamplingExceptionSummaryGrid").trigger("reloadGrid");
            } //SCP252342 - SRM: ICH invoice in ready for billing status
            else if (result == -1) {
                alert("The transaction or invoice that you are attempting to correct has already been successfully corrected from another session. Please refresh the error correction screen to view and correct remaining errors, if any.");
                $('#SamplingCorrectLinkingErrorPopUp').dialog('close');
                $("#SamplingExceptionSummaryGrid").trigger("reloadGrid");
            }
            else {
                alert(result.toString() + "Linking failed. Please enter correct linking information ");
            }
        }
    });

}

//Reset function
function resetForm() {
    
    $(':input', '#SamplingValidationErrorCorrection')
        .not(':button, :submit, :reset, :hidden')
        .val('')
        .removeAttr('selected');
    // $("#ChargeCategoryId").val("-1");
    ResetSearch();
}

function ResetSearch() {
   
    $.ajax({
        type: "POST",
        url: clearSearchUrl,
        dataType: "json",
        success: function (response) {
            if (response) {
                alert(response);
                $('#BillingYearMonth', '#content').val(response.Year + '-' + response.Month + '-' + response.Period);
            }
        }
    });
}
