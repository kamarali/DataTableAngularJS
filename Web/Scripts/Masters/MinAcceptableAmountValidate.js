$(document).ready(function () {
  ValidateFromToPeriod();
    $("#MinAcceptableAmountMaster").validate({
        rules: {
            EffectiveFromPeriod: {
                required: true,
                maxlength: 11,
                ValidFromToPeriod: true
            },
            EffectiveToPeriod: {
                required: true,
                maxlength: 11,
                ValidFromToPeriod: true
            },
            TransactionTypeId: {
                required: true
            },
            Amount: {
                required: true,
                maxlength: 11
            },
            ApplicableMinimumFieldId: {
                required: true
            },
            RejectionReasonCodeId: {
                required: true
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
            }
        },
        messages: {
          EffectiveFromPeriod: "Effective From Period Required And Should Be Valid",
          EffectiveToPeriod: "Effective To Period Required And Should Be Valid",
            TransactionTypeId: "Transaction Type Required.",
            RejectionReasonCodeId: "Rejection Reason Code Required.",
            ApplicableMinimumFieldId: "Applicable Minimum Field Required.",
            Amount: "Min amount Required and should be of maximum 10 digit number.",
            ClearingHouse: " Clearing House Required and should be either 'I' or 'A'"
        },
        invalidHandler: function () {
            $('#errorContainer').show();
            $('#clientErrorMessageContainer').hide();
            $('#clientSuccessMessageContainer').hide();
        }
    });

      });

      // Following method which validates whether Effective From and Effective To periods are in proper format
      function ValidateFromToPeriod() {
        jQuery.validator.addMethod('ValidFromToPeriod', function (value, element) {
          if (value != "") {
            var tokenArray = value.split("-");
            if (tokenArray.length != 3) {
              return false;
            }
            else {
//              var period = tokenArray[2];
//              var month = tokenArray[1];
//              var year = tokenArray[0];

              var period = tokenArray[0];
              var month = tokenArray[1];
              var year = tokenArray[2];

              var periodArray = new Array("01", "02", "03", "04");
              var monthArray = new Array("JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC");
              var isValidYear = /^[0-9]+$/.test(year);

              if ($.inArray(period, periodArray) <= -1 || $.inArray(month.toUpperCase(), monthArray) <= -1 || !isValidYear || year.length != 4) {
                return false;
              }
              return true;
            }
          }
          else {
            return true;
          }

        }, "Invalid Period.");
      }