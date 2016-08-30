$(document).ready(function () {
  if (!$isOnView) {
    $("#samplingformC").validate({
      rules: {
        ListingCurrencyId: { required: function (element) {
          if ($($NilFormCIndicator).val() == 'Y') {
            return false;
          }
          else {
            return true;
          }
        } 
        },
        ProvisionalBillingMemberText: "required"
      },
      messages: {
        ListingCurrencyId: "Listing Currency Required",
        ProvisionalBillingMemberText: "Provisional Billing Member Required"
      }
    });

    trackFormChanges('samplingformC');

    $('#ProvisionalBillingMonthYear').change(function () {
      GetFormABListingCurrency();
    })

    $($NilFormCIndicator).change(function () {
      if (this.value == 'Y') {
        $('#ListingCurrencyDiv').hide();
      }
      else if (this.value == 'N') {
        $('#ListingCurrencyDiv').show();
      }
    })
  }

  if ($($NilFormCIndicator).val() == 'Y')
    $('#ListingCurrencyDiv').hide();
});

var $NilFormCIndicator = $('#NilFormCIndicator');
function InitializeFormC(getFormABListingCurrencyMethod, fromMemberId) {
  _getFormABListingCurrencyMethod = getFormABListingCurrencyMethod;
  _fromMemberId = fromMemberId;
}

var _getFormABListingCurrencyMethod;
var _fromMemberId;
function GetFormABListingCurrency() {
  
  var provBillingMember = $('#ProvisionalBillingMemberId').val();
  
  // For Nil Form C, listing currency is not asked for.
  if(provBillingMember == '' || provBillingMember == 0 || $NilFormCIndicator.val() == 'Y')
  {
    return;
  }

  var monthYearTokens = $('#ProvisionalBillingMonthYear').val().split('-');
  var month = monthYearTokens[1];
  var year = monthYearTokens[0];

  $.ajax({
    type: "POST",
    url: _getFormABListingCurrencyMethod,
    data: { provisionalBillingMemberId: provBillingMember, fromMemberId: _fromMemberId, provisionalBillingMonth: month, provisionalBillingYear: year },
    success: function (result) {
      // set the listing currency.
      $('#ListingCurrencyId').val(result);
    }
  });
}