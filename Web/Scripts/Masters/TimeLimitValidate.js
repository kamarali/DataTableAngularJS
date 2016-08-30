$(document).ready(function () {
  validationEffectiveFromToPeriod();
  $("#TimeLimitMaster").validate({
    rules: {
      EffectiveFromPeriod: { required: true, compareEffectiveFromToPeriods: true },
      EffectiveToPeriod: { required: true },
      SettlementMethodId: { required: true },
      Limit: {
        required: true,
        maxlength: 3
      },
      ClearingHouse: {
        required: function (element) {
          var clearingHouse = $("#ClearingHouse").val();
          if (clearingHouse != "") {
            if (clearingHouse == "I" || clearingHouse == "i" || clearingHouse == "A" || clearingHouse == "a") {
              return false;
            }
            else {
              $("#ClearingHouse").val('');
              return true;
            }
          } else {
            return true;
          }
        },
        maxlength: 1
      },
      TransactionTypeId: {
        required: true
      },
      CalculationMethod: {
        required: true,
        maxlength: 255
      }
    },
    messages: {
      Limit: { required: "Time limit Required", maxlength: "Time limit should be of maximum 3 digits." },
      ClearingHouse: "Clearing House Required and should be either 'I' or 'A'",
      TransactionTypeId: { required: "Transaction Type Required" },
      CalculationMethod: "Calculation Method Required and should be of maximum 2 characters",
      SettlementMethodId: { required: "Settlement Method Required" },
      EffectiveFromPeriod: { required: "Effective From Period Required" },
      EffectiveToPeriod: { required: "Effective To Period Required" },
      CalculationMethod: { required: "Calculation Method Required" }
    },
    invalidHandler: function () {
      $('#errorContainer').show();
      $('#clientErrorMessageContainer').hide();
      $('#clientSuccessMessageContainer').hide();
    }
  });
  
  $("#SearchTimeLimit").validate({
    rules: {
      EffectiveFromPeriod: { compareEffectiveFromToPeriods: true }
    }
  });
  
});

function validationEffectiveFromToPeriod() { 
  jQuery.validator.addMethod('compareEffectiveFromToPeriods',
    function (value, element) {   
      if ($("#EffectiveToPeriod").val() != '') {
        var effectiveFromPeriod = $("#EffectiveFromPeriod").datepicker("getDate");
        var effectiveToPeriod = $("#EffectiveToPeriod").datepicker("getDate");
        if (effectiveFromPeriod != null && effectiveToPeriod != null && effectiveFromPeriod.getTime() > effectiveToPeriod.getTime()) {
          return false;
        }
        else return true;
      }

      return true;
    },
    "Effective From Period should not be greater than or equal to Effective To Period.");
}