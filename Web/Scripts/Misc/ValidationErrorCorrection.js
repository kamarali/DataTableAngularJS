
var muCorrespondenceTransactionType = 37;
var UpdateValidationErrorUrl, ExceptionDetailsGridDataUrl, IsDisplayLinkingButtonUrl, ValidateErrorUrl, BatchUpdatedCountUrl, UpdateCorrectLinkingErrorUrl;

function UpdatePopUpClick() {
  $('#UpdatePopUp').dialog({ title: 'Update Validation Error', height: 350, width: 1000, modal: true, resizable: false });

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
  //SCP252342 - SRM: ICH invoice in ready for billing status
  $('#LastUpdatedOn').val(jQuery("#UatpExceptionDetailsGrid").jqGrid('getCell', sss, 'LastUpdatedOn'));

  if ($('#BatchUpdateAllowed').val() == "No") {
    $('#BatchUpdateButtonAllowed').attr('disabled', 'disabled');

  }
  else {
    $('#BatchUpdateButtonAllowed').removeAttr('disabled');
  }
}



function CorrectLinkingErrorClick() {

    $('#CorrectLinkingErrorPopUp').dialog({ title: 'Correct Linking Error', height: 350, width: 1000, modal: true, resizable: false });

    s = $('#UatpExceptionDetailsGrid').getGridParam('selrow');

    var rowcells = jQuery("#UatpExceptionDetailsGrid").jqGrid('getCell', s, 'FieldName');

    ss = $('#UatpExceptionSummary').getGridParam('selrow');
    var sss = $('#UatpExceptionDetailsGrid').getGridParam('selrow');


    $('#YourInvoiceNo').val(jQuery("#UatpExceptionDetailsGrid").jqGrid('getCell', s, 'YourInvoiceNo'));
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
    //SCP252342 - SRM: ICH invoice in ready for billing status
    $('#LastUpdatedOn').val(jQuery("#UatpExceptionDetailsGrid").jqGrid('getCell', sss, 'LastUpdatedOn'));

    if ($('#TranscationId').val() == muCorrespondenceTransactionType) {


        //$('#YourInvoiceNo').attr('name', 'RejInvoiceNo'); // dummy name
        //$('#CorrespondenceRejInvoiceNo').attr('name', 'YourInvoiceNo');

        $('#bmpopup').show();
        $('#rmpopup').hide();
    }
    else {

        $('#rmpopup').show();
        $('#bmpopup').hide();
    }
}


function CloseCorrectLinkingErrorClick() {
    $('#CorrectLinkingErrorPopUp').dialog('close');
}

function closeUpdate() {
    //$('#UpdatePopUp').Close();
    updateform.resetForm();
    $('#UpdatePopUp').dialog('close');

}



function ValidateUpdatePopup() {
  updateform = $("#UpdateValidationForm").validate({ });

  $('#UpdatePopUp').bind('dialogclose', function (event) {
      updateform.resetForm();
  });
}

function updatebuttonclick(isbatchupdated) {

    if ($('#NewValue').val() == null || $('#NewValue').val() == "") {
      alert("New Value is required");
    }
    else {


        var billingCategory = "3";

        $.ajax({
            type: "POST",
            url: UpdateValidationErrorUrl,
            data: { filename: $('#UpdateFileName').val(), ExceptionCode: $('#UpdateExceptionCode').val(), ErrorDescription: $('#ErrorDescription').val(), FieldName: $('#FieldName').val(), FieldValue: $('#FieldValue').val().toString(), NewValue: $('#NewValue').val(), exceptionSummaryId: $('#ExceptionSummaryId').val(), exeptionDetailsId: $('#ExceptionDetailId').val(), isBatchUpdate: isbatchupdated, billingCat: billingCategory, errorLevel: $('#ErrorLevel').val(), pkReferenceId: $('#PkReferenceId').val(), lastUpdatedOn: $('#LastUpdatedOn').val() },
            dataType: "json",
            success: function (result) {
                if (result) {
                  
                    if (result == "Error") {
                        alert("Invalid value entered. Please enter valid value");
                    }
                    else  if (result == "InvoiceDeleted") {
                        alert("This invoice has been deleted.");
                    }
                    else if (result == 2) {
                        alert("Billing period and Late submission window of the Invoice is closed, Invoice is marked as Error Non correctable");
                        //TFS#9906 :IE:Version 11 - Update Validation Error screen is not performing any action.
                        if (isbatchupdated == 1) {
                            $('#BatchUpdatePopup').dialog('close');
                        }   
                        $('#UpdatePopUp').dialog('close');
                        $("#UatpExceptionSummary").trigger("reloadGrid");
                    } // SCP252342 - SRM: ICH invoice in ready for billing status
                    else if (result == -1) {
                        alert("The transaction or invoice that you are attempting to correct has already been successfully corrected from another session. Please refresh the error correction screen to view and correct remaining errors, if any.");
                        //TFS#9906 :IE:Version 11 - Update Validation Error screen is not performing any action.
                        if (isbatchupdated == 1) {
                            $('#BatchUpdatePopup').dialog('close');
                        }   
                        $('#UpdatePopUp').dialog('close');
                        $("#UatpExceptionSummary").trigger("reloadGrid");
                    }
                    else {
                        // updateform.resetForm();
                        //TFS#9906 :IE:Version 11 - Update Validation Error screen is not performing any action.
                        if (isbatchupdated == 1) {
                            $('#BatchUpdatePopup').dialog('close');
                        }  
                        $('#UpdatePopUp').dialog('close');
                        $("#UatpExceptionSummary").trigger("reloadGrid");
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

    var billingCategory = "3";

    $.ajax({
        type: "POST",
        url: ExceptionDetailsGridDataUrl,
        data: { rowcells: rowcells, billingCategoryType: billingCategory, exceptionCode: exceptionCode },
        dataType: "json",
        success: function (result) {
            if (result) {

                // Create URL to call "GetInvoiceAndFileGridData" action passing it filter criteria
                var url = ExceptionDetailsGridDataUrl + "?" + $.param({ rowcells: rowcells, exceptionCode: exceptionCode });
                $("#UatpExceptionDetailsGrid").setGridParam({ url: url }).trigger("reloadGrid");
            }
        }
    });

    $.ajax({
      type: "POST",
      url: IsDisplayLinkingButtonUrl,
      data: { exceptionCode: exceptionCode, billingCategoryId: "3" },
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
    // debugger;
    $(':input', '#frmValidationError')
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
           // $("#UatpExceptionSummary").setSelection(rows[0], true);
            $("#UatpExceptionSummary").jqGrid('setSelection', rows[0], true);
        }
        else {
            //$("#UatpExceptionSummary").setSelection(selecRow, true);
            $("#UatpExceptionSummary").jqGrid('setSelection', selecRow, true);
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
           // $("#UatpExceptionDetailsGrid").setSelection(rows[0], true);
            $("#UatpExceptionDetailsGrid").jqGrid('setSelection', rows[0], true);
        }
        else {
           // $("#UatpExceptionDetailsGrid").setSelection(selecRow, true);
            $("#UatpExceptionDetailsGrid").jqGrid('setSelection', selecRow, true);
        }
    }
}


function BatchUpdatebuttonclick() {
    if ($('#NewValue').val() == null || $('#NewValue').val() == "") {
        //alert("new Value is required");
    }
    else {

        ss = $('#UatpExceptionSummary').getGridParam('selrow');
        var ExceptionSummaryId = jQuery("#UatpExceptionSummary").jqGrid('getCell', ss, 'Id');
        var oldvalue = $('#FieldValue').val();
        var exceptionCode = $('#UpdateExceptionCode').val();
        var fieldVal = $('#NewValue').val();
        var billingCategoryId ="3"

        $.ajax({
            type: "POST",
            url: ValidateErrorUrl,
            data: { exceptionCode: exceptionCode, NewValue: fieldVal, pkReferenceId: $('#PkReferenceId').val(), billingCategoryId: billingCategoryId },
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

function updateCorrctLinkingbuttonclick() {
    if ($('#rmpopup').is(":visible") == true) {
        if ($('#YourInvoiceNo').val() == null || $('#YourInvoiceNo').val() == "") {
            alert("Rejected Invoice No field is mandatery");
            return;
        }

        else if ($('#YourInvoiceBillingDate').val() == null || $('#YourInvoiceBillingDate').val() == "") {
            alert("Settlement Period field is mandatery");
            return;
        }
        else if ($('#YourInvoiceBillingDate').val().length != 8) {
            alert("Settlement Period is invalid");
            return;
        }
    }
    else {
        if ($('#CorrespondenceRefNo').val() == null || $('#CorrespondenceRefNo').val() == "") {
            alert("Correspondence Ref Number field is mandatery");
            return;
        }
        else if ($('#CorrespondenceRejInvoiceNo').val() == null || $('#CorrespondenceRejInvoiceNo').val() == "") {
            alert("Rejection Invoice Number field is mandatery");
            return;
        }
    }

    s = $('#UatpExceptionDetailsGrid').getGridParam('selrow');
    var summarygridSelectedrow = $('#UatpExceptionSummary').getGridParam('selrow');

    var billedMemberId = jQuery("#UatpExceptionSummary").jqGrid('getCell', summarygridSelectedrow, 'BilledMemberId');
    var billingMemberId = jQuery("#UatpExceptionSummary").jqGrid('getCell', summarygridSelectedrow, 'BillingMemberId');
    var detailid = jQuery("#UatpExceptionDetailsGrid").jqGrid('getCell', s, 'ExceptionDetailId');
    var invoiceId = jQuery("#UatpExceptionSummary").jqGrid('getCell', summarygridSelectedrow, 'InvoiceID');
    var yourBillingDate = $('#YourInvoiceBillingDate').val().toString();
    var yourbillingYear = 0; var yourbillingMonth = 0; var yourbillingPeriod = 0;

    if (yourBillingDate.length == 8) {
        yourbillingYear = yourBillingDate.substr(0, 4);
        yourbillingMonth = yourBillingDate.substr(4, 2);
        yourbillingPeriod = yourBillingDate.substr(6, 2);
    }

    var linkingDetail;

    if ($('#TranscationId').val() != muCorrespondenceTransactionType) {
        linkingDetail = 1;
    }

    else {
        linkingDetail = 2;
        $('#YourInvoiceNo').val($('#CorrespondenceRejInvoiceNo').val());
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
        "LastUpdatedOn": $('#LastUpdatedOn').val()
    };

    //var json = $.toJSON(jsonObj);


    $.ajax({
        type: "POST",
        url: UpdateCorrectLinkingErrorUrl,
        data: jsonObj,
        dataType: "json",
        success: function (result) {
            if (result == 1) {

                $('#CorrectLinkingErrorPopUp').dialog('close');
                $("#UatpExceptionSummary").trigger("reloadGrid");
            }
            else if (result == 2) {
                alert("Billing period and Late submission window of the Invoice is closed, Invoice is marked as Error Non correctable");
                $('#CorrectLinkingErrorPopUp').dialog('close');
                $("#UatpExceptionSummary").trigger("reloadGrid");
            } // SCP252342 - SRM: ICH invoice in ready for billing status
            else if (result == -1) {
                alert("The transaction or invoice that you are attempting to correct has already been successfully corrected from another session. Please refresh the error correction screen to view and correct remaining errors, if any.");
                $('#CorrectLinkingErrorPopUp').dialog('close');
                $("#UatpExceptionSummary").trigger("reloadGrid");
            } 
            else if (result == 9) {
                alert("Invalid Your Invoice Billing Date.");
            }
            /* SCP280744: MISC UATP Exchange Rate population/validation during error correction. */
            else if (result == 10) {
                alert("Linking failed because an Exchange Rate was already provided for this Invoice and it does not match with the Applicable Exchange Rate based on updated linking information provided.");
            }
            else if (result == 11) {
                alert("Linking failed because an Amount in Currency of Clearance was already provided for this Invoice and it does not match with the Applicable Amount in Currency of Clearance using the Applicable Exchange Rate based on updated linking information provided.");
            }
            else if (result == 12) {
                alert("Linking failed.");
            }
            else if (result.indexOf('Applicable Time Limit') >= 0) {
                alert(result.toString());
            }
            else {
                alert(result.toString() + "Linking is failed");
            }
        }
    });

}
