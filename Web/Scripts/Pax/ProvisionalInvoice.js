/// <reference path="site.js" />
var BillingCurrencyId;
var ProvisionalBillingMonth;
var ActionMethodForExchangeRate;
// Added for Linking purpose
var SMI;
var BilateralSMIList;

_amountDecimals = 2;
_percentDecimals = 3;

$(document).ready(function () {

  clearDefaultZeroValue();
  $("#InvoiceDate").datepicker('option', 'dateFormat', 'dd-M-y');
  $("#InvoiceDate").watermark("DD-MM-YY");

  if (!$isOnView) {
    $("#ProvisionalInvoiceForm").validate({
      rules: {
        InvoiceNumber: "required",
        InvoiceDate: "required",
        BillingPeriodNo: "required",
        InvoiceListingCurrencyId: "required",
        InvoiceListingAmount: "required",
        ListingToBillingRate: "required",
        InvoiceBillingAmount: "required"
      },
      messages: {
        InvoiceNumber: "Provisional Invoice Number Required",
        InvoiceDate: "Provisional Invoice Date Required",
        BillingPeriodNo: "Provisional Billing Period No. Required",
        InvoiceListingCurrencyId: "Provisional Invoice Listing Currency Required",
        InvoiceListingAmount: {
          required: "Provisional Invoice Listing Amount Required",
          min: "Value should be between -9999999999999.9 and 9999999999999.9",
          max: "Value should be between -9999999999999.9 and 9999999999999.9"
        },
        ListingToBillingRate: {
          required: "Provisional Listing To Billing Rate Required",
          min: "Value should be between -99999999999.99999 and 99999999999.99999",
          max: "Value should be between -99999999999.99999 and 99999999999.99999"
        },
        InvoiceBillingAmount: " Prov. Invoice Amount in Billing Currency Required"
      },
      submitHandler: function () {
        addProvisionalInvoice();
      }, highlight: function () { $("#errorContainer").show(); $(".serverErrorMessage").hide(); $(".serverSuccessMessage").hide(); },
      onkeyup: false
    });

    trackFormChanges('ProvisionalInvoiceForm');

    $("#InvoiceListingAmount").blur(function () {
      //trim the whitespace
      var listingAmount = $('#InvoiceListingAmount').val().replace(/^\s\s*/, '').replace(/\s\s*$/, '');
      setBillingAmount(listingAmount);
    }); //end blur

    $("#ListingToBillingRate").blur(function () {
      //trim the whitespace
      var listingAmount = $('#InvoiceListingAmount').val().replace(/^\s\s*/, '').replace(/\s\s*$/, '');
      setBillingAmount(listingAmount);
    }); //end blur

  }
});

function setBillingAmount(listingAmount) {
  //trim white space
  var exchangeRate = $("#ListingToBillingRate").val().replace(/^\s\s*/, '').replace(/\s\s*$/, '');
  if (exchangeRate != 0) {
    var billingAmount = listingAmount / exchangeRate;
    if (!isNaN(billingAmount))
      $("#InvoiceBillingAmount").val(billingAmount.toFixed(_amountDecimals));
  }
  else {
    var zeroValue = Number('0');
    $("#InvoiceBillingAmount").val(zeroValue.toFixed(_amountDecimals));
  }
}


function SetAutoPopulatedExchangeRateDetails(billingCurrency, provBillingYear, provBillingMonth, actionUrl) {
  ActionMethodForExchangeRate = actionUrl;
  BillingCurrencyId = billingCurrency;
  ProvisionalBillingMonth = provBillingYear + "-" + provBillingMonth;

  $("#InvoiceListingCurrencyId").bind('change', function () {
    GetExchangeRate($("#InvoiceListingCurrencyId").val());
  });

  $("#ListingToBillingRate").bind('onchange', function () {
    //trim the whitespace
    var listingAmount = $('#InvoiceListingAmount').val().replace(/^\s\s*/, '').replace(/\s\s*$/, '');
    setBillingAmount(listingAmount);
  });

  $("#ListingToBillingRate").attr("readonly", "readonly");
}

//Save provisional invoice
function addProvisionalInvoice() {
  $("#errorContainer").hide();
  var invoiceNumber = $('#InvoiceNumber').val();
  var billingPeriodNo = $('#BillingPeriodNo' + " option:selected").text();
  var invoiceDate = $('#InvoiceDate').datepicker("getDate");
  var invoiceListingCurrencyId = $('#InvoiceListingCurrencyId' + " option:selected").val();
  var invoiceListingAmount = $('#InvoiceListingAmount').val();
  var listingToBillingRate = $('#ListingToBillingRate').val();
  var invoiceBillingAmount = $('#InvoiceBillingAmount').val();
  var invoiceId = $('#InvoiceId').val();

  var provInvoiceObject = {
    InvoiceNumber: invoiceNumber,
    BillingPeriodNo: billingPeriodNo,
    InvoiceDate: new Date(Date.UTC(invoiceDate.getFullYear(), invoiceDate.getMonth(), invoiceDate.getDate(), 0, 0, 0, 0)),
    InvoiceListingCurrencyId: invoiceListingCurrencyId,
    InvoiceListingAmount: invoiceListingAmount,
    ListingToBillingRate: listingToBillingRate,
    InvoiceBillingAmount: invoiceBillingAmount,
    InvoiceId: invoiceId
  };

  saveProvisionalInvoiceinDB(provInvoiceObject);

 
}

//Save provisional invoice in DB
function saveProvisionalInvoiceinDB(datarow) {

  var jsonData = JSON.stringify(datarow);
  var invId = $("InvoiceId").val();
  $.ajax({
      url: 'ProvisionalInvoiceCreate',
      type: 'POST',
      data: { form: jsonData, invoiceId: invId },
      dataType: 'json',
      error: function () {
          showClientErrorMessage("Error while saving record.");
      },
      success: function (result) {
          if (result.IsFailed == false) {
              showClientSuccessMessage(result.Message);
              $("#ProvisionalInvoiceGrid").trigger("reloadGrid");
              //Clear values of textboxes in capture div
              $('#InvoiceNumber').val('');
              $('#BillingPeriodNo').val('');
              $('#InvoiceDate').val('');
              $("#InvoiceDate").blur();
              $('#InvoiceListingCurrencyId').val('');
              $('#InvoiceListingAmount').val('');
              $('#ListingToBillingRate').val('');
              $('#InvoiceBillingAmount').val('');
              $parentForm.resetDirty();
          }
          else {
              showClientErrorMessage(result.Message);
          }
      }
  });
}

//Get exchange rate for given billing currency, listing currency and provisional billing month
function GetExchangeRate(listingCurrency) {

  if (listingCurrency != "" && BillingCurrencyId != "" && ProvisionalBillingMonth != "") {
    $.ajax({
      type: "POST",
      url: ActionMethodForExchangeRate,
      data: { listingCurrencyId: listingCurrency, billingCurrencyId: BillingCurrencyId, billingPeriod: ProvisionalBillingMonth },
      dataType: "json",
      success: function (response) {
        if (response.Message) {
          showClientErrorMessage(response.Message);
          $("#ListingToBillingRate").val("0.00000");
        }
        else {
          clearMessageContainer();
          $("#ListingToBillingRate").val(response);
        }
        $("#ListingToBillingRate").blur();
      },
      failure: function (response) {
        $("#ListingtoBillingRate").val("0.00000");
      }
    });
  }
  else {
    $("#ListingtoBillingRate").val("0.00000");
  }
}

//Clear default 0 value in integer fields
function clearDefaultZeroValue() {
  $('#InvoiceDate').removeAttr('value');
  $('#InvoiceListingAmount').removeAttr('value');
  $('#ListingToBillingRate').removeAttr('value');
  $('#InvoiceBillingAmount').removeAttr('value');
}

//Set the global variable values
function setGlobalVariables(SMIValue, SMIBValues) {
  SMI = SMIValue;
  BilateralSMIList = SMIBValues;
}

//Changing  the control visibility 
function InializeLinking() {
  if(jQuery.inArray(SMI, BilateralSMIList) > -1){
    $('#ListingToBillingRate', '#content').attr('readOnly', false);
  }
}