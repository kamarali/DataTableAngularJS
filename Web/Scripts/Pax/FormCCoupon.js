_amountDecimals = 2;
_percentDecimals = 3;
$(document).ready(function () {
  $('#TicketIssuingAirline').focus();
  $('#GrossAmountAlf').watermark('0.00');

  if (!$isOnView) {
    $("#formCCoupon").validate({
      rules: {
        ReasonCode: "required",
        SourceCodeId: "required",
        ProvisionalInvoiceNumber: "required",
        TicketIssuingAirline: "required",
        CouponNumber: "required",
        DocumentNumber: "required"
      },
      messages: {
        ReasonCode: "Rejection Reason Code Required",
        ProvisionalInvoiceNumber: { required: "Provisional Invoice No. Required" },
        BatchNumberOfProvisionalInvoice: { required: "Batch No. of Provisional Invoice Required" },
        RecordSeqNumberOfProvisionalInvoice: { required: "Record Seq. No. of Provisional Invoice Required" },
        SourceCodeId: "Source Code Required",
        TicketIssuingAirline: { required: "Ticket Issuing Airline Required" },
        CouponNumber: { required: "Coupon No. Required" },
        DocumentNumber: { required: "Ticket/Document No. Required" }
      },
      submitHandler: function (form) {
        $ElectronicTicketIndicator.removeAttr('disabled');
        $('#CouponNumber', '#content').removeAttr('disabled');
        onSubmitHandler(form);
      }
    });

    trackFormChanges('formCCoupon');

    //allow not more than 350 characters in Remarks section
    $("#Remarks").bind("keypress", function () { maxLength(this, 350) });
    $("#Remarks").bind("paste", function () { maxLengthPaste(this, 350) });
  }
})

var fromMember;
var provisionalBillingMember;
var provisionalBillingYear;
var provisionalBillingMonth;
var getCouponDetailsMethod;
var selectedCoupon = -1;
var linkedCoupons;

var $ProvisionalInvoiceNumber = $('#ProvisionalInvoiceNumber', '#content');
var $BatchNumberOfProvisionalInvoice = $('#BatchNumberOfProvisionalInvoice', '#content');
var $RecordSeqNumberOfProvisionalInvoice = $('#RecordSeqNumberOfProvisionalInvoice', '#content');
var $GrossAmountAlf = $('#GrossAmountAlf', '#content');
var $ElectronicTicketIndicator = $('#ElectronicTicketIndicator', '#content');

function InializeLinking(isLinkingSuccessful, fromMemberId, provBillingMemberId, provBillingYear, provBillingMonth, getCouponDetailsMethodName) {
  // Populate fields only when linking is successful.
  
  if (isLinkingSuccessful == "True") {
    fromMember = fromMemberId;
    provisionalBillingMember = provBillingMemberId;
    provisionalBillingYear = provBillingYear;
    provisionalBillingMonth = provBillingMonth;
    getCouponDetailsMethod = getCouponDetailsMethodName;
    disableOtherLinkingFields();
    $('#FetchButton').bind('click', function () {
      getCouponDetails();
    });

    $("#DocumentNumber").blur(function () {
      $('#SaveButton', '#content').attr('disabled', true);
    });

    $("#TicketIssuingAirline").blur(function () {
      $('#SaveButton', '#content').attr('disabled', true);
    });

    $("#CouponNumber").blur(function () {
      $('#SaveButton', '#content').attr('disabled', true);
    });

    $('#SaveButton', '#content').attr('disabled', true);
  }
  else {
    $('#FetchButton').hide();
  }
}

function getCouponDetails() {
  var ticketIssuingAirline = $('#TicketIssuingAirline', '#content').val();
  var documentNumber = $('#DocumentNumber', '#content').val();
  var couponNumber = $('#CouponNumber', '#content').val();
  var listingCurrency = $('#ListingCurrency', '#content').val();

  if ($.trim(ticketIssuingAirline) != '' && $.trim(documentNumber) != '' && couponNumber != '') {
    $.ajax({
      type: "POST",
      url: getCouponDetailsMethod + "?fromMemberId=" + fromMember + "&provisionalBillingMemberId=" + provisionalBillingMember +
      "&provisionalBillingMonth=" + provisionalBillingMonth + "&provisionalBillingYear=" + provisionalBillingYear +
      "&ticketIssuingAirline=" + ticketIssuingAirline + "&documentNumber=" + documentNumber + "&couponNumber=" + couponNumber +
      "&listingCurrency=" + listingCurrency,
      success: function (result) {
        linkedCoupons = result.LinkedCoupons;
        if (result.ErrorMessage) {
          showClientErrorMessage(result.ErrorMessage);
          setAjaxError();
          clearPopulatedFields();
        }
        else if (result.LinkedCoupons.length == 1) {
          // populate the fields
          clearMessageContainer();
          populateFields(result.LinkedCoupons[0]);
        }
        else {// display multiple occurrences of coupon
          clearMessageContainer();
          displayCoupons(result.LinkedCoupons);
        }
      }
    });
  }
  else {
    alert('Please enter ticket issuing airline, ticket/document number and coupon number.');
  }
}

function populateFields(coupon) {
  $ProvisionalInvoiceNumber.val(coupon.ProvisionalInvoiceNumber);
  $BatchNumberOfProvisionalInvoice.val(coupon.BatchNumberOfProvisionalInvoice);
  $RecordSeqNumberOfProvisionalInvoice.val(coupon.RecordSeqNumberOfProvisionalInvoice);
  $GrossAmountAlf.val(coupon.GrossAmountAlf.toFixed(3));
  $ElectronicTicketIndicator.prop('checked', coupon.ElectronicTicketIndicator);
  $('#AgreementIndicatorSupplied').val(coupon.AgreementIndicatorSupplied);
  $('#AgreementIndicatorValidated').val($.trim(coupon.AgreementIndicatorValidated));
  $('#OriginalPmi').val(coupon.OriginalPmi);
  $('#ValidatedPmi').val(coupon.ValidatedPmi);
}

function disableLinkingFields() {
  $('#TicketIssuingAirline', '#content').attr('readOnly', true);
  $('#DocumentNumber', '#content').attr('readOnly', true);
  $('#CouponNumber', '#content').attr('disabled', true);
}

function InitializeLinkingFieldsInEditMode(isLinkingSuccessful) {
  $('#FetchButton').hide();
  
  if (isLinkingSuccessful == "True") {
    disableLinkingFields();
    disableOtherLinkingFields();
  }
  
}

function disableOtherLinkingFields() {
  $ProvisionalInvoiceNumber.attr('readOnly', true);
  $BatchNumberOfProvisionalInvoice.attr('readOnly', true);
  $RecordSeqNumberOfProvisionalInvoice.attr('readOnly', true);
  $GrossAmountAlf.attr('readOnly', true);
  $ElectronicTicketIndicator.attr('disabled', true);
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
  populateFields(linkedCoupons[selectedCoupon]);
  closeDialog('#linkedCoupons');
}

function clearPopulatedFields() {
  $ProvisionalInvoiceNumber.val('');
  $BatchNumberOfProvisionalInvoice.val('');
  $RecordSeqNumberOfProvisionalInvoice.val('');
  $GrossAmountAlf.val('');
  $ElectronicTicketIndicator.prop('checked', false);
  $('#AgreementIndicatorSupplied').val('');
  $('#AgreementIndicatorValidated').val('');
  $('#OriginalPmi').val('');
  $('#ValidatedPmi').val('');
}
