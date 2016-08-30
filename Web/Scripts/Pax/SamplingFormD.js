_amountDecimals = 2;
_percentDecimals = 3;
$(document).ready(function () {

  if (!$isOnView) {
    $("#formDCoupon").validate({
      rules: {

        SourcecodeId: "required",
        TicketIssuingAirline: "required",
        TicketDocNumber: "required",
        CouponNumber: "required",
        ProvisionalInvoiceNumber: "required",
        BatchNumberOfProvisionalInvoice: "required",
        RecordSeqNumberOfProvisionalInvoice: "required",
        ProvisionalGrossAlfAmount: "required",
        EvaluatedNetAmount: "required"
      },
      messages: {

        SourcecodeId: "Source Code Required",
        TicketIssuingAirline: "Ticket Issuing Airline Required",
        TicketDocNumber: "Ticket Document No. Required",
        CouponNumber: "Coupon No. Required",
        ProvisionalInvoiceNumber: "Provisional Invoice No. Required",
        BatchNumberOfProvisionalInvoice: "Batch No. of Provisional Invoice Required",
        RecordSeqNumberOfProvisionalInvoice: "Record Seq. No. of Provisional Invoice Required",
        ProvisionalGrossAlfAmount: {
          required: "Provisional Gross/ALF Amount Required",
          min: "Value should be between -999999999.99 and 999999999.99",
          max: "Value should be between -999999999.99 and 999999999.99"
        },
        EvaluatedNetAmount: {
          required: "Evaluated Net Amount required",
          min: "Value should be between 0 and 999999999.99",
          max: "Value should be between 0 and 999999999.99"
        }
      },
      submitHandler: function (form) {
        $('#OriginalPmi', '#content').attr('disabled', false);
        $('#ValidatedPmi', '#content').attr('disabled', false);
        $('#CouponNumber', '#content').attr('disabled', false);

        // Re-calculate amounts on click of Save.
        calculateAmounts();

        onSubmitHandler(form);
      }

    });

    trackFormChanges('formDCoupon');

    $("#TicketDocNumber").blur(function () {
      $('#SaveButton', '#content').attr('disabled', isFormAB == 'True');
    });

    $("#TicketIssuingAirline").blur(function () {
      $('#SaveButton', '#content').attr('disabled', isFormAB == 'True');
    });

    $("#CouponNumber").blur(function () {
      $('#SaveButton', '#content').attr('disabled', isFormAB == 'True');
    });

    $("#EvaluatedGrossAmount").blur(function () {
      //trim the whitespace
      var couponGrossValue = $('#EvaluatedGrossAmount').val().replace(/^\s\s*/, '').replace(/\s\s*$/, '');

      calculateISCAmount();
      calculateOtherCommissionAmount();
      calculateUATPAmount();

      function calculateISCAmount() {
        setISCAmount(couponGrossValue);
      }

      function calculateOtherCommissionAmount() {
        setOtherCommAmount(couponGrossValue);
      }

      function calculateUATPAmount() {
        setUATPAmount(couponGrossValue);
      }
    });

    $("#EvaluatedGrossAmount").blur(function () {
      calculateNetAmount("#EvaluatedGrossAmount", "#IscAmount", "#OtherCommissionAmount", "#UatpAmount", "#HandlingFeeAmount", "#TaxAmount", "#VatAmount");
    });

    $("#HandlingFeeAmount").blur(function () {
      calculateNetAmount("#EvaluatedGrossAmount", "#IscAmount", "#OtherCommissionAmount", "#UatpAmount", "#HandlingFeeAmount", "#TaxAmount", "#VatAmount");
    });

    $("#IscPercent").blur(function () {
      var couponGrossValue = $('#EvaluatedGrossAmount').val().replace(/^\s\s*/, '').replace(/\s\s*$/, '');
      setISCAmount(couponGrossValue);
      calculateNetAmount("#EvaluatedGrossAmount", "#IscAmount", "#OtherCommissionAmount", "#UatpAmount", "#HandlingFeeAmount", "#TaxAmount", "#VatAmount");
    });

    $("#OtherCommissionPercent").blur(function () {
      var couponGrossValue = $('#EvaluatedGrossAmount').val().replace(/^\s\s*/, '').replace(/\s\s*$/, '');
      setOtherCommAmount(couponGrossValue);
      calculateNetAmount("#EvaluatedGrossAmount", "#IscAmount", "#OtherCommissionAmount", "#UatpAmount", "#HandlingFeeAmount", "#TaxAmount", "#VatAmount");
    });

    $("#UatpPercent").blur(function () {
      var couponGrossValue = $('#EvaluatedGrossAmount').val().replace(/^\s\s*/, '').replace(/\s\s*$/, '');
      setUATPAmount(couponGrossValue);
      calculateNetAmount("#EvaluatedGrossAmount", "#IscAmount", "#OtherCommissionAmount", "#UatpAmount", "#HandlingFeeAmount", "#TaxAmount", "#VatAmount");
    });

    $("#OtherCommissionAmount").blur(function () {
      calculateNetAmount("#EvaluatedGrossAmount", "#IscAmount", "#OtherCommissionAmount", "#UatpAmount", "#HandlingFeeAmount", "#TaxAmount", "#VatAmount");
    });

    $("#VatAmount").blur(function () {
      calculateNetAmount("#EvaluatedGrossAmount", "#IscAmount", "#OtherCommissionAmount", "#UatpAmount", "#HandlingFeeAmount", "#TaxAmount", "#VatAmount");
    });

    $("#TaxAmount").blur(function () {
      calculateNetAmount("#EvaluatedGrossAmount", "#IscAmount", "#OtherCommissionAmount", "#UatpAmount", "#HandlingFeeAmount", "#TaxAmount", "#VatAmount");
    });

  }
});

function calculateAmounts() {
  calculateNetAmount("#EvaluatedGrossAmount", "#IscAmount", "#OtherCommissionAmount", "#UatpAmount", "#HandlingFeeAmount", "#TaxAmount", "#VatAmount");
}

//function setISCAmount(couponGrossValue) {
 // trim white space
 // var iscValue = $("#IscPercent").val().replace(/^\s\s*/, '').replace(/\s\s*$/, '');
 // var iscPercentage = iscValue / 100 * couponGrossValue;
 // if (!isNaN(iscPercentage)) {
 //   $("#IscAmount").val(iscPercentage.toFixed(_amountDecimals));
 // }
//}

//SCP:53676 - Error - Incorrect ISC Amount for BM with Reason Code 8E 
//Function is use to calculate percentage for ISC amount
function setISCAmount(couponGrossValue) {
    if (couponGrossValue != '') {
  var iscValue = $("#IscPercent").val().replace(/^\s\s*/, '').replace(/\s\s*$/, '');

  var percent = (couponGrossValue * iscValue) / 100;
 
     percent = parseFloat(roundNumber(percent, _amountDecimals));
        if (!isNaN(percent))
            $("#IscAmount").val(percent);
    }
    else {
        $("#IscAmount").val(0.00);
    }

}

function setUATPAmount(couponGrossValue) {
  var UATPValue = $("#UatpPercent").val().replace(/^\s\s*/, '').replace(/\s\s*$/, '');
  var UATPPercentage = UATPValue / 100 * couponGrossValue;
  if (!isNaN(UATPPercentage)) {
    $("#UatpAmount").val(UATPPercentage.toFixed(_amountDecimals));
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


function setOtherCommAmount(couponGrossValue) {
 // var otherCommValue = $("#OtherCommissionPercent").val().replace(/^\s\s*/, '').replace(/\s\s*$/, '');
  //var otherCommPercentage = otherCommValue / 100 * couponGrossValue;
 // if (!isNaN(otherCommPercentage)) {
 //   $("#OtherCommissionAmount").val(otherCommPercentage.toFixed(_amountDecimals));
//  }
    
  if (couponGrossValue != '') {
  var otherCommValue = $("#OtherCommissionPercent").val().replace(/^\s\s*/, '').replace(/\s\s*$/, '');

  var percent = (couponGrossValue * otherCommValue) / 100;
 
     percent = parseFloat(roundNumber(percent, _amountDecimals));
        if (!isNaN(percent))
            $("#OtherCommissionAmount").val(percent);
    }
    else {
        $("#OtherCommissionAmount").val(0.00);
    }
}
function setUATPAmount(couponGrossValue) {
  var UATPValue = $("#UatpPercent").val().replace(/^\s\s*/, '').replace(/\s\s*$/, '');
  var UATPPercentage = UATPValue / 100 * couponGrossValue;
  if (!isNaN(UATPPercentage)) {
    $("#UatpAmount").val(UATPPercentage.toFixed(_amountDecimals));
  }
}


function calculateNetAmount(sourceControl1, sourceControl2, sourceControl3, sourceControl4, sourceControl5, sourceControl6, sourceControl7) {
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

  var netAmount = grossAmount + iscAmount + ocAmount + uatpAmount + hfAmount + taxAmount + vatAmount;
  if (!isNaN(netAmount))
    $("#EvaluatedNetAmount").val(netAmount.toFixed(_amountDecimals));
}


//Clear default 0 value in integer fields
function clearAmountDefaultZeroValue() {
  $('.numeric').removeAttr('value');
  $('.percentageTextfield').removeAttr('value');
  $('.amountTextfield').removeAttr('value');
  $('#EvaluatedGrossAmount').removeAttr('value');
  $('#HandlingFeeAmount').removeAttr('value');
  $('#ProvisionalGrossAlfAmount').removeAttr('value');
  $('#EvaluatedNetAmount').removeAttr('value');
  $('#VatBaseAmount').val('');
  $('#VatPercentage').val('');
}

var getLinkedCouponDetailsMethod;
var invoiceGuid;
var isFormAB = 'False';

function InitializeLinking(isFormABViaIS, invoiceId, getLinkedCouponDetailsMethodName) {
  getLinkedCouponDetailsMethod = getLinkedCouponDetailsMethodName;
  isFormAB = isFormABViaIS;
  invoiceGuid = invoiceId;

  if (isFormABViaIS == 'True') {
    $('#FetchButton', '#content').bind('click', function () { GetLinkedCouponDetails(); });
    InitializeLinkingControls();
  }
  else
    $('#FetchButton', '#content').hide();
}

function GetLinkedCouponDetails() {

  var ticketIssuingAirline = $('#TicketIssuingAirline', '#content').val();
  var documentNumber = $('#TicketDocNumber', '#content').val();
  var couponNumber = $('#CouponNumber', '#content').val();
  if ($.trim(ticketIssuingAirline) != '' && $.trim(documentNumber) != '' && couponNumber != '') {
    $.ajax({
      type: "POST",
      url: getLinkedCouponDetailsMethod + "?invoiceId=" + invoiceGuid +
      "&issuingAirline=" + ticketIssuingAirline + "&ticketDocNumber=" + documentNumber + "&ticketCouponNumber=" + couponNumber,

      success: function (result) {
        linkedCoupons = result.LinkedCoupons;
        if (result.ErrorMessage) {
          showClientErrorMessage(result.ErrorMessage);
          //to disable the Submit type buttons.
          setAjaxError();
          clearPopulatedFields();
        }
        else if (result.LinkedCoupons.length == 1) {
          // populate the fields
          populateFields(result.LinkedCoupons[0]);
        }
        else {// display multiple occurrences of coupon
          displayCoupons(result.LinkedCoupons);
        }
      },
      failure: function () { alert('An error occurred while fetching the details.'); }
    });
  } else {
    alert('Please enter ticket issuing airline, ticket/document number and coupon number.');
  }
}

//set all the controls as per the linking
function InitializeLinkingControls() {
  $('#ProvisionalInvoiceNumber', '#content').attr('readOnly', true);
  $('#BatchNumberOfProvisionalInvoice', '#content').attr('readOnly', true);
  $('#RecordSeqNumberOfProvisionalInvoice', '#content').attr('readOnly', true);
  $('#ProvisionalGrossAlfAmount', '#content').attr('readOnly', true);
  $('#AgreementIndicatorSupplied', '#content').attr('readOnly', true);
  $('#AgreementIndicatorValidated', '#content').attr('readOnly', true);
  $('#OriginalPmi', '#content').attr('disabled', true);
  $('#ValidatedPmi', '#content').attr('disabled', true);
  $('#ProrateMethodology', '#content').attr('readOnly', true);
  $('#SaveButton', '#content').attr('disabled', true);
}

function InitializeFormDControls(isFormABViaIS, isViewMode) {
  if (isViewMode)
    setViewModeControl();
  else {
    if (isFormABViaIS == 'True') {
      InitializeLinkingControls();
      disableLinkingFields();
    }
  }
  $('#FetchButton', '#content').hide();
}

// data fields
var InvoiceNumberDF = 'InvoiceNumber';
var BatchNumberDF = 'BatchNumber';
var RecordSequenceNumberDF = 'RecordSequenceNumber';
var RdbColumn = 'RdbColumn';
var SerialNoDF = 'SerialNo';

// display names
var InvoiceNumberDN = 'Invoice Number';
var BatchNumberDN = 'Batch Number';
var RecordSequenceNumberDN = 'Record Sequence Number';
var SerialNoDN = 'Sr. No.';
var linkedCoupons;

function displayCoupons(coupons) {
  $linkedCouponsGrid = $('#linkedCouponsGrid');
  $linkedCouponsGrid.jqGrid({
    autoencode: true,
    datatype: 'local',
    width: 475,
    height: 250,
    colNames: ['', SerialNoDN, InvoiceNumberDN, BatchNumberDN, RecordSequenceNumberDN],
    colModel: [
                { name: RdbColumn, index: RdbColumn, sortable: false, width: 30, formatter: rdbFormatter }, // for radio button
                {name: SerialNoDF, index: SerialNoDF, sortable: false, width: 30, hidden: $isOnView },
                { name: InvoiceNumberDF, index: InvoiceNumberDF, sortable: false, hidden: $isOnView },
                { name: BatchNumberDF, index: BatchNumberDF, sortable: false },
                { name: RecordSequenceNumberDF, index: RecordSequenceNumberDF, sortable: false }
              ]
  });

  $('#linkedCoupons').dialog({ closeOnEscape: false, title: '', height: 400, width: 500, modal: true, resizable: false });
  // get IDs of all the rows of jqGrid
  var rowIds = $linkedCouponsGrid.jqGrid('getDataIDs');

  // iterate through the rows and delete each of them
  for (var i = 0, len = rowIds.length; i < len; i++) {
    var currRow = rowIds[i];
    $linkedCouponsGrid.jqGrid('delRowData', currRow);
  }
  selectedCoupon = -1;
  // Populate data in tax grid with existing tax records
  if (coupons != null) {
    coupons = eval(coupons);
    $couponCurrent = 1;
    for ($couponCurrent; $couponCurrent < coupons.length + 1; $couponCurrent++) {
      row = { RdbColumn: $couponCurrent - 1, SerialNo: $couponCurrent, InvoiceNumber: coupons[$couponCurrent - 1]["ProvisionalInvoiceNumber"], BatchNumber: coupons[$couponCurrent - 1]["BatchNumberOfProvisionalInvoice"], RecordSequenceNumber: coupons[$couponCurrent - 1]["RecordSeqNumberOfProvisionalInvoice"] };
      $linkedCouponsGrid.jqGrid('addRowData', $couponCurrent, row);
    }
  }
}

function rdbFormatter(cellValue, options, cellObject) {
  return '<input type="radio" name="rdbCoupon" value=cellValue onclick="setSelectedCoupon(' + cellValue + ');" />';
}

function setSelectedCoupon(selectedIndex) {
  selectedCoupon = selectedIndex;
}

function onLinkingDialogClose() {
  //populate details of selected index
  if (selectedCoupon == -1) {
    alert('Please select at least one coupon.');
    return;
  }

  // populate the details of the selected coupon.
  populateFields(linkedCoupons[selectedCoupon]);
  closeDialog('#linkedCoupons');
}

function disableLinkingFields() {
  $('#TicketIssuingAirline', '#content').attr('readOnly', true);
  $('#TicketIssuingAirline', '#content').autocomplete('disabled', true);
  $('#TicketDocNumber', '#content').attr('readOnly', true);
  $('#CouponNumber', '#content').attr('disabled', true);
}

var $ProvisionalInvoiceNumber = $('#ProvisionalInvoiceNumber', '#content');
var $BatchNumberOfProvisionalInvoice = $('#BatchNumberOfProvisionalInvoice', '#content');
var $RecordSeqNumberOfProvisionalInvoice = $('#RecordSeqNumberOfProvisionalInvoice', '#content');
var $GrossAmountAlf = $('#ProvisionalGrossAlfAmount', '#content');

function populateFields(coupon) {
  $ProvisionalInvoiceNumber.val(coupon.ProvisionalInvoiceNumber);
  $BatchNumberOfProvisionalInvoice.val(coupon.BatchNumberOfProvisionalInvoice);
  $RecordSeqNumberOfProvisionalInvoice.val(coupon.RecordSeqNumberOfProvisionalInvoice);
  $GrossAmountAlf.val(coupon.GrossAmountAlf.toFixed(_amountDecimals));
  $('#AgreementIndicatorSupplied').val(coupon.AgreementIndicatorSupplied);
  $('#AgreementIndicatorValidated').val(coupon.AgreementIndicatorValidated);
  $('#OriginalPmi').val(coupon.OriginalPmi);
  $('#ValidatedPmi').val(coupon.ValidatedPmi);
  $('#ProrateMethodology').val(coupon.ValidatedPmi);
}

function clearPopulatedFields() {
  $ProvisionalInvoiceNumber.val('');
  $BatchNumberOfProvisionalInvoice.val('');
  $RecordSeqNumberOfProvisionalInvoice.val('');
  $GrossAmountAlf.val('');

  $('#AgreementIndicatorSupplied').val('');
  $('#AgreementIndicatorValidated').val('');
  $('#OriginalPmi').val('');
  $('#ValidatedPmi').val('');
  $('#ProrateMethodology').val('');
}

function setViewModeControl() {
  $('input[type=text]').attr('disabled', 'disabled');
  $('select').attr('disabled', 'disabled');
  $("#SaveFormD").hide();
  $('textarea').attr('disabled', 'disabled');
  $('input[type=checkbox]').attr('disabled', 'disabled');
}