$(document).ready(function () {
  $("#ShowMismatchedTransactionFromBRDButton").click(function () {

    // get values for call server site method to fill mismatch grid 
    var billedYearMonth = $('#SupportingDocumentBillingYearMonthDetailView').val();
    var billedMonth = billedYearMonth.split("-");
    // to do
    var billedMemberId = $('#SupportingDocumentBilledMemeberIdDetailView').val();
    var billingMemberId = _billingMemberId;
    var clearanceMonth = billedMonth[1];
    var clearancePeriod = $('#SupportingDocumentBillingPeriodDetailView').val();
    var billingCategory = _billingCategoryId;
    var invoiceNumber = $('#SupportingDocumentInvoiceNoDetailView').val();

    // set the mismatch search criteria
    $('#MismatchTransactionBillingYearMonth').val(billedYearMonth);
    $('#MismatchTransactionBillingPeriod').val(clearancePeriod);
    $('#MismatchTransactionInvoiceNo').val(invoiceNumber);
    $('#MismatchTransactionBilledMember').val($('#SupportingDocumentBilledMemberDetailView').val());
    $('#HiddenBilledMemberId').val($('#SupportingDocumentBilledMemeberIdDetailView').val());
    if (_billingCategoryId == 3 || _billingCategoryId == 4) {  // For Misc billing category
      // call the function for Ajax call
      GetListOfMismatchTransactionDocument("N", billedMemberId, billingMemberId, clearanceMonth, clearancePeriod, billingCategory, invoiceNumber, "0", "0", "0", "0", "0");
    }
    else if(_billingCategoryId == 1) {

      var formC = $('#SupportingDocumentFormCDetailView').val();
      var batchNumber = $('#SupportingDocumentBatchNumberDetailView').val();
      var sequenceNumber = $('#SupportingDocumentSequenceNumberDetailView').val();
      var breakdownSerialNumber = $('#SupportingDocumentCouponBreakdownSerialNumberDetailView').val();

      if (formC == 'Y' && !clearancePeriod) {
        clearancePeriod = 0;
      }

      $('#MismatchTransactionFormC').val(formC);
      $('#MismatchTransactionBatchNumber').val(batchNumber);
      $('#MismatchTransactionSequenceNumber').val(sequenceNumber);
      $('#MismatchTransactionCouponBreakdownSerialNumber').val(breakdownSerialNumber);

      // call the function for Ajax call
      GetListOfMismatchTransactionDocument(formC, billedMemberId, billingMemberId, clearanceMonth, clearancePeriod, billingCategory, invoiceNumber, batchNumber, sequenceNumber, breakdownSerialNumber, "0", "0");
    }

     else if(_billingCategoryId == 2) {

      //var formC = $('#SupportingDocumentFormCDetailView').val();
      var batchNumber = $('#SupportingDocumentBatchNumberDetailView').val();
      var sequenceNumber = $('#SupportingDocumentSequenceNumberDetailView').val();
      var breakdownSerialNumber = $('#SupportingDocumentCouponBreakdownSerialNumberDetailView').val();

//      if (formC == 'Y' && !clearancePeriod) {
//        clearancePeriod = 0;
//      }

     // $('#MismatchTransactionFormC').val(formC);
      $('#MismatchTransactionBatchNumber').val(batchNumber);
      $('#MismatchTransactionSequenceNumber').val(sequenceNumber);
      $('#MismatchTransactionCouponBreakdownSerialNumber').val(breakdownSerialNumber);

      // call the function for Ajax call
      GetListOfMismatchTransactionDocument("N", billedMemberId, billingMemberId, clearanceMonth, clearancePeriod, billingCategory, invoiceNumber, batchNumber, sequenceNumber, breakdownSerialNumber, "0", "0");
    }

    // display dialog
    $('#divMismatchTransaction').dialog({ closeOnEscape: false, title: 'Mismatch Transaction From BRD', height: 515, width: 975, modal: true, resizable: false });
  });
});

// update button click event which update the details of unlinked document
$("#UpdateButton").click(function () {
  $('#SupportingDocumentBillingYearMonthDetailView').val($('#MismatchTransactionBillingYearMonth').val());
  $('#SupportingDocumentFormCDetailView').val($('#MismatchTransactionFormC').val());
  $('#SupportingDocumentBillingPeriodDetailView').val($('#MismatchTransactionBillingPeriod').val());
  $('#SupportingDocumentBilledMemberDetailView').val($('#MismatchTransactionBilledMember').val());
  $('#SupportingDocumentBilledMemeberIdDetailView').val($('#HiddenBilledMemberId').val());
  $('#SupportingDocumentInvoiceNoDetailView').val($('#MismatchTransactionInvoiceNo').val());
  $('#SupportingDocumentBatchNumberDetailView').val($('#MismatchTransactionBatchNumber').val());
  $('#SupportingDocumentSequenceNumberDetailView').val($('#MismatchTransactionSequenceNumber').val());
  $('#SupportingDocumentCouponBreakdownSerialNumberDetailView').val($('#MismatchTransactionCouponBreakdownSerialNumber').val());
  closeDialog('#divMismatchTransaction');
});

// click event of close button
$("#CloseButton").click(function () {
  closeDialog('#divMismatchTransaction');
});

// click event of search button on mismatch transaction page
$("#MismatchSearchButton").click(function () {

  // get values for call server site method to fill mismatch grid 
  var billedYearMonth = $('#MismatchTransactionBillingYearMonth').val();
  var billedMonth = billedYearMonth.split("-");
  // TODO
  var billedMemberId = $('#HiddenBilledMemberId').val();
  var billingMemberId = _billingMemberId;
  var clearanceMonth = billedMonth[1];
  var clearancePeriod = $('#MismatchTransactionBillingPeriod').val();
  var billingCategory = _billingCategoryId;
  var invoiceNumber = $('#MismatchTransactionInvoiceNo').val();
  var mismatchCheckbox = $('#MismatchTransactionCases').is(':checked'); // $('#MismatchTransactionCases').val();

  if (_billingCategoryId == 3 || _billingCategoryId == 4) {  // For Misc billing category
    var chargeCategory = $('#MismatchChargeCategory').val();
    GetListOfMismatchTransactionDocument("N", billedMemberId, billingMemberId, clearanceMonth, clearancePeriod, billingCategory, invoiceNumber, "0", "0", "0", chargeCategory,mismatchCheckbox);
  }
  else {
    var formC = $('#MismatchTransactionFormC').val();
    var batchNumber = $('#MismatchTransactionBatchNumber').val();
    var sequenceNumber = $('#MismatchTransactionSequenceNumber').val();
    var breakdownSerialNumber = $('#MismatchTransactionCouponBreakdownSerialNumber').val();

    if (formC == 'Y' && !clearancePeriod) {
      clearancePeriod = 0;
    }

    GetListOfMismatchTransactionDocument(formC, billedMemberId, billingMemberId, clearanceMonth, clearancePeriod, billingCategory, invoiceNumber, batchNumber, sequenceNumber, breakdownSerialNumber,"0",mismatchCheckbox);
  }
});

// function is used for ajax call and get the mismatch records from the database
function GetListOfMismatchTransactionDocument(formC, billedMemberId, billingMemberId, clearanceMonth, clearancePeriod, billingCategory, invoiceNumber, batchNumber, sequenceNumber, breakdownSerialNumber, chargeCategoryId,mismatchCheckbox) {

    if (mismatchCheckbox == false) {
        mismatchCheckbox = 0;
    }
    else {
        mismatchCheckbox = 1;
    }

     if (billingCategory == 2 || billingCategory == 1) {
        if (batchNumber == "") {
            alert("Batch Number field is mandatory");
            return 0;  
        }
        if (sequenceNumber == "") {
            alert("Sequence Number field is mandatory");
            return 0;  
        }
        if (breakdownSerialNumber == "") {
            alert("Break down serial number field is mandatory");
            return 0;  
        }
          
    }

    if (billedMemberId == "") {
        alert("Billed member is mandatory field");
    }

    else if (clearanceMonth == undefined) {
        alert("Clearence month is mandatory field");
    }
      //SCP162502: Form C - AC OAR Jul P3 failure - No alert received
      else if (clearancePeriod == 0 && formC != 'Y') {
        alert("Period is mandatory field");
    }

    else if (invoiceNumber == null || invoiceNumber == '') {
        alert("Invoice Number is mandatory field");
    }

    

    else {
        $.ajax({
            type: 'POST',
            url: _searchResultMismatchGridData,
            data: { formC: formC, billedMemberId: billedMemberId, billingMemberId: billingMemberId, clearanceMonth: clearanceMonth, clearancePeriod: clearancePeriod, billingCategory: billingCategory, invoiceNumber: invoiceNumber, batchNumber: batchNumber, sequenceNumber: sequenceNumber, breakdownSerialNumber: breakdownSerialNumber, chargeCategoryId: chargeCategoryId, Mismatch: mismatchCheckbox },
            dataType: "json",
            error: function (obj) {
                var i = obj.toString();
                //alert(i);
            },
            success: function (result) {
                PopulateMismatchTransactionDocument(result);
            }
        });
    }
}

function PopulateMismatchTransactionDocument(selectedRecord) {
  // get the mismatch records and create the grid
  displayRecords(selectedRecord);
}
var unlinkedDocId;
// call fucntion for get selected record and display details of unlinked document
function EditUnlinkedSupportingDocument(par1, id, par2) {
  unlinkedDocId = id;
  // Call server method through Ajax 
  $.ajax({
    type: 'POST',
    url: _getSelectedUnlinkedSupportingDocumentDetails,
    data: { unlinkedDocumentId: id },
    dataType: "json",
    error: function (obj) {
    },
    success: function (result) {
      PopulateLinkingDetails(result);
    }
  });
}
// initials method for ajax method and billingmemberId
function InitialiseGetSelectedUnlinkedSupportingDocumentDetailsMethod(getSelectedUnlinkedSupportingDocumentDetails, searchResultMismatchGridData, LinkDocuments, billingMemberId, billingCategoryId) {
  _getSelectedUnlinkedSupportingDocumentDetails = getSelectedUnlinkedSupportingDocumentDetails;
  _searchResultMismatchGridData = searchResultMismatchGridData;
  _LinkDocuments = LinkDocuments;
  _billingMemberId = billingMemberId;
  _billingCategoryId = billingCategoryId;
  if ($("#SubmissionDate").val() == '01-Jan-01')
    $("#SubmissionDate").val("");
}
// populate the unlinked document details
function PopulateLinkingDetails(selectedRecord) {
  if (selectedRecord != null) {

    $('#divUnlinkedSupportingDocumentDetails').removeClass('hidden');
    $('#divUnlinkedSupportingDocumentDetails').addClass('show');
    $('#SupportingDocumentBillingYearMonthDetailView').val(selectedRecord.BillingYear + '-' + selectedRecord.BillingMonth);
    $('#SupportingDocumentFormCDetailView').val(selectedRecord.IsFormC);
    $('#SupportingDocumentBillingPeriodDetailView').attr('disabled', 'disabled');

    //SCP162502: Form C - AC OAR Jul P3 failure - No alert received
    if (selectedRecord.IsFormC != 'Y') {
      $('#SupportingDocumentBillingPeriodDetailView').removeAttr('disabled');
    }

    $('#SupportingDocumentBillingPeriodDetailView').val(selectedRecord.PeriodNumber);
    $('#SupportingDocumentBilledMemberDetailView').val(selectedRecord.BilledMemberName);
    $('#SupportingDocumentBilledMemeberIdDetailView').val(selectedRecord.BilledMemberId);
    $('#SupportingDocumentInvoiceNoDetailView').val(selectedRecord.InvoiceNumber);
    $('#SupportingDocumentBatchNumberDetailView').val(selectedRecord.BatchNumber);
    $('#SupportingDocumentSequenceNumberDetailView').val(selectedRecord.SequenceNumber);
    $('#SupportingDocumentCouponBreakdownSerialNumberDetailView').val(selectedRecord.CouponBreakdownSerialNumber);
    $('#SupportingDocumentFileNameDetailView').val(selectedRecord.OriginalFileName);
    orignalFilePath = selectedRecord.FilePath;
    // TODO: set value of ChargeCategory for MISC.
  }
}

var orignalFilePath;
var selectedRecord = -1;
var linkedRecords;
var linkedRecordsGrid;
// data fields
var IsFormCYNDF = 'IsFormC';
var BillingYearMonthDF = 'BillingYearMonth';
var BillingPeriodDF = 'BillingPeriod';
var BilledmemberCommercialNameDF = 'BilledmemberCommercialName';
var InvoiceNumberDF = 'InvoiceNumber';
var BatchNumberDF = 'BatchNumber';
var SequenceNumberDF = 'SequenceNumber';
var BreakdownSerialNumberDF = 'BreakdownSerialNumber';
var RdbColumn = 'RdbColumn';
var ChargeCategoryDF = 'ChargeCategory';

// display names
var IsFormCYNDN = 'Form C (Y/N)';
var BillingYearMonthDN = 'Billing Year/Month';
var BillingPeriodDN = 'Billing Period';
var BilledmemberCommercialNameDN = 'Billed Member';
var InvoiceNumberDN = 'Invoice Number';
var BatchNumberDN = 'Batch Number';
var SequenceNumberDN = 'Sequence Number';
var BreakdownSerialNumberDN = 'Breakdown Serial No.';
var ChargeCategoryDN = 'Charge Category';

// Create and Fill the grid
function displayRecords(records) {

    if (_billingCategoryId == 1) {
        var colNames = [IsFormCYNDN, BillingYearMonthDN, BillingPeriodDN, BilledmemberCommercialNameDN, InvoiceNumberDN, BatchNumberDN, SequenceNumberDN, BreakdownSerialNumberDN];
        var colModel = [
                { name: IsFormCYNDF, index: IsFormCYNDF, sortable: false },
                { name: BillingYearMonthDF, index: BillingYearMonthDF, sortable: false },
                { name: BillingPeriodDF, index: BillingPeriodDF, sortable: false },
                { name: BilledmemberCommercialNameDF, index: BilledmemberCommercialNameDF, sortable: false },
                { name: InvoiceNumberDF, index: InvoiceNumberDF, sortable: false },
                { name: BatchNumberDF, index: BatchNumberDF, sortable: false },
                { name: SequenceNumberDF, index: SequenceNumberDF, sortable: false },
                { name: BreakdownSerialNumberDF, index: BreakdownSerialNumberDF, sortable: false }
              ];
    }
    else if (_billingCategoryId == 2) {

        var colNames = [BillingYearMonthDN, BillingPeriodDN, BilledmemberCommercialNameDN, InvoiceNumberDN, BatchNumberDN, SequenceNumberDN, BreakdownSerialNumberDN];
        var colModel = [
                { name: BillingYearMonthDF, index: BillingYearMonthDF, sortable: false },
                { name: BillingPeriodDF, index: BillingPeriodDF, sortable: false },
                { name: BilledmemberCommercialNameDF, index: BilledmemberCommercialNameDF, sortable: false },
                { name: InvoiceNumberDF, index: InvoiceNumberDF, sortable: false },
                { name: BatchNumberDF, index: BatchNumberDF, sortable: false },
                { name: SequenceNumberDF, index: SequenceNumberDF, sortable: false },
                { name: BreakdownSerialNumberDF, index: BreakdownSerialNumberDF, sortable: false }
              ];
    }

  else if (_billingCategoryId == 3 || _billingCategoryId == 4) {  // For Misc billing category
    colNames = [BillingYearMonthDN, BillingPeriodDN, BilledmemberCommercialNameDN, InvoiceNumberDN, ChargeCategoryDN];
    colModel = [{ name: BillingYearMonthDF, index: BillingYearMonthDF, sortable: false }, { name: BillingPeriodDF, index: BillingPeriodDF, sortable: false },
                { name: BilledmemberCommercialNameDF, index: BilledmemberCommercialNameDF, sortable: false }, { name: InvoiceNumberDF, index: InvoiceNumberDF, sortable: false },
                { name: ChargeCategoryDF, index: ChargeCategoryDF, sortable: false}];
  }

  linkedRecordsGrid = $('#MismatchTransactionGrid');
  linkedRecordsGrid.jqGrid({
    autoencode: true,
    datatype: 'local',
    width: 800,
    height: 250,
    colNames: colNames,
    colModel: colModel
  });

  // get IDs of all the rows of jqGrid
  var rowIds = linkedRecordsGrid.jqGrid('getDataIDs');

  // iterate through the rows and delete each of them
  for (var i = 0, len = rowIds.length; i < len; i++) {
    var currRow = rowIds[i];
    linkedRecordsGrid.jqGrid('delRowData', currRow);
  }
  selectedRecord = -1;

  if (records != null) {
    records = eval(records);
    linkedRecords = records;
    recordCurrent = 1;
    // TODO

    if (_billingCategoryId == 3 || _billingCategoryId == 4) {  // For Misc billing category
        row = { BillingYearMonth: records.BillingYearMonth, BillingPeriod: records.BillingPeriod, BilledmemberCommercialName: records.BilledMemberCommercialName, InvoiceNumber: records.InvoiceNumber, ChargeCategory: records.ChargeCategory };
    }
    else if (_billingCategoryId == 1) {
        row = { IsFormC: records.IsFormC, BillingYearMonth: records.BillingYearMonth, BillingPeriod: records.BillingPeriod, BilledmemberCommercialName: records.BilledMemberCommercialName, InvoiceNumber: records.InvoiceNumber, BatchNumber: records.BatchNumber, SequenceNumber: records.SequenceNumber, BreakdownSerialNumber: records.BreakdownSerialNo, ChargeCategory: records.ChargeCategory };
    }
    else if (_billingCategoryId == 2) {
        row = { BillingYearMonth: records.BillingYearMonth, BillingPeriod: records.BillingPeriod, BilledmemberCommercialName: records.BilledMemberCommercialName, InvoiceNumber: records.InvoiceNumber, BatchNumber: records.BatchNumber, SequenceNumber: records.SequenceNumber, BreakdownSerialNumber: records.BreakdownSerialNo, ChargeCategory: records.ChargeCategory };
    }
    linkedRecordsGrid.jqGrid('addRowData', recordCurrent, row);
    
    //            }
  }
}

// key event : click event of LinkButton
$("#LinkButton").click(function () {

    var billedYearMonthVal = $('#SupportingDocumentBillingYearMonthDetailView').val();
    var billedYearMonth = billedYearMonthVal.split("-");
    var invoiceNumber = $('#SupportingDocumentInvoiceNoDetailView').val();
    var billingMemberId = _billingMemberId;

    var billedMemberId = $('#SupportingDocumentBilledMemeberIdDetailView').val();
    // TODO
    var billingYear = billedYearMonth[0];
    var billingMonth = billedYearMonth[1];
    var periodNumber = $('#SupportingDocumentBillingPeriodDetailView').val();
    var filePath = orignalFilePath;
    var originalFileName = $('#SupportingDocumentFileNameDetailView').val();

    if (billedMemberId == "") {
        alert("Billed member is required field");
    }
    else if (billingYear == "0") {
    alert("Billing Year  is required field");
}
    else if (billingMonth == undefined) {
    alert("Billing Month is required field");
    }
    else if (periodNumber == "") {
    alert("Period No is required field");
    }
    else if (invoiceNumber == "") {
    alert("Invoice Number is required field");
    }
    else {

        if (_billingCategoryId == 3 || _billingCategoryId == 4) {  // For Misc billing category
            AjaxCallForLinkDocument(invoiceNumber, billingMemberId, billedMemberId, billingYear, billingMonth, periodNumber, "0", "0", "0", filePath, originalFileName, unlinkedDocId);
        }
        else if(_billingCategoryId == 1) {
            if ($("#SupportingDocumentFormCDetailView").val() == 'Y' && !periodNumber) {
                periodNumber = 0;
            }
            var batchNumber = $('#SupportingDocumentBatchNumberDetailView').val();
            var sequenceNumber = $('#SupportingDocumentSequenceNumberDetailView').val();
            var breakdownSerialNumber = $('#SupportingDocumentCouponBreakdownSerialNumberDetailView').val();
            AjaxCallForLinkDocument(invoiceNumber, billingMemberId, billedMemberId, billingYear, billingMonth, periodNumber, batchNumber, sequenceNumber, breakdownSerialNumber, filePath, originalFileName, unlinkedDocId);
            }
            else if(_billingCategoryId == 2) {
            var batchNumber = $('#SupportingDocumentBatchNumberDetailView').val();
            var sequenceNumber = $('#SupportingDocumentSequenceNumberDetailView').val();
            var breakdownSerialNumber = $('#SupportingDocumentCouponBreakdownSerialNumberDetailView').val();
            AjaxCallForLinkDocument(invoiceNumber, billingMemberId, billedMemberId, billingYear, billingMonth, periodNumber, batchNumber, sequenceNumber, breakdownSerialNumber, filePath, originalFileName, unlinkedDocId);
            }
        }
   // }

});

function AjaxCallForLinkDocument(invoiceNumber, billingMemberId, billedMemberId, billingYear, billingMonth, periodNumber, batchNumber, sequenceNumber, breakdownSerialNumber, filePath, originalFileName, id) {
  // Call server method through Ajax
  $.ajax({
    type: 'POST',
    url: _LinkDocuments,
    data: { invoiceNumber: invoiceNumber, billingMemberId: billingMemberId, billedMemberId: billedMemberId, billingYear: billingYear, billingMonth: billingMonth, periodNumber: periodNumber, batchNumber: batchNumber, sequenceNumber: sequenceNumber, breakdownSerialNumber: breakdownSerialNumber, filePath: filePath, originalFileName: originalFileName, id: id },
    dataType: "json",
    error: function (obj) {
      showClientErrorMessage("Unlinked Supporting Document Linking Error.");
    },
    success: function (result) {
      if (result == "") {
        $("#SupportingDocumentSearchGrid").trigger("reloadGrid");
        orignalFilePath = '';
        // Hide description Div
        $('#divUnlinkedSupportingDocumentDetails').removeClass('show');
        $('#divUnlinkedSupportingDocumentDetails').addClass('hidden');
        showClientSuccessMessage("Unlinked Supporting Document Linking is Successful.");
      }
      else {
        showClientErrorMessage(result);
      }
    }
  });
  var grid = jQuery("#SupportingDocumentSearchGrid");
  grid.trigger("reloadGrid");
}   
