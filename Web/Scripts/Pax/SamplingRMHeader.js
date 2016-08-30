_amountDecimals = 2;

function InitializeRMEditHeader(isSubmittedStatus, isTransactionExists) {
   
  if (isSubmittedStatus == 'True') {
    //disable all fields    
    $('input[type=text]').attr('disabled', 'disabled');
    $('select').attr('disabled', 'disabled');
    $("#BilledMemberText").autocomplete({ disabled: true });
    $("#SaveHeader").hide();
    SetPageToViewMode(isSubmittedStatus);

    var divActionOnInvoice = $('#divTransactions');
    $('input[type=submit]', divActionOnInvoice).show();
    $('input[type=submit]', divActionOnInvoice).attr('disabled', false);
  }
  //user should not be able to modify certain fields in header, if transaction exists for that invoice.
  else {
    validationSamplingInvoiceNumber(); 
    validateHeader(isSubmittedStatus, isTransactionExists);
    if (isTransactionExists == 'True') {
      disableLinkingFields();
    }
  }
}

function disableLinkingFields() {
  $('#FormDEProvisionalBillingMonth').attr('disabled', 'disabled');
  $('#SamplingConstant').attr('disabled', 'disabled');

  $('#BilledMemberText').attr('disabled', 'disabled');
  $("#BilledMemberText").autocomplete({ disabled: true });

  $('#SettlementMethodId').attr('disabled', 'disabled');
  $('#ListingCurrencyId').attr('disabled', 'disabled');
  $('#BillingCurrencyId').attr('disabled', 'disabled');
  $("#ListingToBillingRate").attr("readonly", true);
}

function InitializeCreateRMHeader(pageModeCreate) {
  validationSamplingInvoiceNumber();
  validateHeader(false, false);

  if (pageModeCreate && $('#SamplingConstant').val() == 0)
    $('#SamplingConstant').removeAttr('value');
}

function validateHeader(isSubmittedStatus, isTransactionExists) {

  $("#SamplingRMForm").validate({
    rules: {
      InvoiceNumber: { required: true, ValidSamplingInvoiceNumber: true },
      FormDEProvisionalBillingMonth: "required",
      BilledMemberText: "required",
      SettlementMethodId: "required",
      InvoiceDate: "required",
      BillingYearMonthPeriod: "required",
      BilledMemberText:
      {
        required: true,
        checkMemberEquality: true
      },
      SamplingConstant: "required",
      ListingCurrencyId: "required",
      BillingCurrencyId: "required",
      ListingToBillingRate: {min:0.00001}
    },
    messages: {
      InvoiceNumber: "Invoice Number required and should be valid",
      FormDEProvisionalBillingMonth: "Provisional Billing Month Required",
      BilledMemberText: "Billed Member Required",
      SettlementMethodId: "Settlement Method Required",
      InvoiceDate: "Invoice Date Required",
      BillingYearMonthPeriod: "Billing Year/Month/Period Required",
      SamplingConstant: { required: "Sampling Constant Required" },
      ListingCurrencyId: "Currency of Listing/Evaluation Required",
      BillingCurrencyId: "Currency of Billing Required",
      ListingToBillingRate: "Exchange Rate cannot be zero or negative"
    },
    invalidHandler: function (form, validator) {
      if (isTransactionExists == 'True' || isSubmittedStatus == 'True') {
        disableLinkingFields();
      }
    },
    submitHandler: function (form) {
      $('#FormDEProvisionalBillingMonth').removeAttr('disabled');
      $('#SamplingConstant').removeAttr('disabled');
      $('#BilledMemberText').removeAttr('disabled');
      $('#SettlementMethodId').removeAttr('disabled');
      $('#ListingCurrencyId').removeAttr('disabled');
      $('#BillingCurrencyId').removeAttr('disabled');

      // Call onSubmitHandler() function which will disable Submit buttons and will submit the form
      onSubmitHandler(form);
    }
  });

  trackFormChanges('SamplingRMForm');

}


function validationSamplingInvoiceNumber() {
  jQuery.validator.addMethod('ValidSamplingInvoiceNumber',
      function (value, element) {

        var regEx = '^[a-zA-Z0-9]+$'; // without underscore        
        var re = new RegExp(regEx);
        if (!element.value.match(re)) {          
          return false;
        }
        else { return true;
        }
      },
    "Invoice Number invalid.");
}

var getSamplingConstantMethodName;
var $billedMember = $('#BilledMemberId', '#content');
var $provBillingMonth = $('#FormDEProvisionalBillingMonth', '#content');
var billMemID;
var billedMemberText;
var provBillingMonth;
function InitializeLinking(getSamplingConstantMethod, billingMember) {
  billedMemberText = $billedMember.val();
  provBillingMonth = $provBillingMonth.val();
  billMemID = billingMember;
  getSamplingConstantMethodName = getSamplingConstantMethod;
  $('#BilledMemberText', '#content').bind('change', function () { getSamplingConstant(); });
  $provBillingMonth.bind('change', function () {
    getSamplingConstant(); 
  });
}


function getSamplingConstant() {
// new values
  var billedMember = $billedMember.val();
  var provisionalBillingMonth = $provBillingMonth.val();
  if(billedMember!='' && billedMember!= 0 && provisionalBillingMonth != ''){
    if (billedMember != billedMemberText || provisionalBillingMonth != provBillingMonth)
      $.ajax({
        type: "POST",
        url: getSamplingConstantMethodName,
        data: { billingMemId: billMemID, provisionalBillingMonthYear: provisionalBillingMonth, billedMemberId: billedMember },
        dataType: "json",
        success: function (result) {
          var hasError = false;
          if (result != null) {
            if (result.ErrorMessage != '' && result.ErrorMessage != null) {
              if (result.ErrorMessage.indexOf("Exception") != -1) {
                hasError = true;
              }
            }
          }
          
          if (hasError == true) {
            showClientErrorMessage(result.ErrorMessage);
            $provBillingMonth.val('');
          }
          else {
            if (result.ErrorMessage && result.ErrorMessage != '') {
              showClientSuccessMessage(result.ErrorMessage);
            }
            if (result.IsFormDataFound == true) {
              $('#IsFormDEViaIS').val('true');
              $('#IsFormFViaIS').val('true');
              var $samplingConstant = $('#SamplingConstant', '#content');
              if ($samplingConstant.val() != '') {
                alert('Sampling constant will be overwritten by sampling constant of linked Form D/E.');
              }

              $samplingConstant.val(result.SamplingConstant);
            }
            else {
              $('#IsFormDEViaIS').val('false');
              $('#IsFormFViaIS').val('false');
            }
          }
        }
      });
    }

    billedMemberText = $billedMember.val();
    provBillingMonth = $provBillingMonth.val();
  }
