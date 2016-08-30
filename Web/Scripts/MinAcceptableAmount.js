$(document).ready(function () {
  formatDateControlsForToleranceAndMinMaxMaster();
});
var GetRejectionReasonForTransactionType = '/Data/GetRejectionReasonForTransactionType';

function BindEventForDate() {
    $("#EffectiveFromPeriod").bind("change", ValidateDate);
    $("#EffectiveToPeriod").bind("change", ValidateDate);

}
function BindEventOnCreateMinAccepatableAmount() {
     $("#TransactionTypeId").bind("change", PopulateRejectionReasonCodeValues);
     $("#MaxAcceptableAmountMaster").bind("onload", PopulateRejectionReasonCodeValues);
     $("#TransactionTypeId").bind("load", PopulateRejectionReasonCodeValues);
     
}

function InitialiseInvoiceHeader(rejectionReasonCodeForTransaction) {
    GetRejectionReasonForTransactionType = rejectionReasonCodeForTransaction;
  }

//Display Rejection reason code
  function PopulateRejectionReasonCodeValues() {
    var transactionTypeId = $('#TransactionTypeId').val();
    if ($.trim(transactionTypeId) != '') {
        $.ajax({
            type: "Post",
            url: GetRejectionReasonForTransactionType,
            data: { transactionTypeId: transactionTypeId },
            dataType: "json",
            success: function (response) {
                if (response) {
                    PopulateRejectionReasonCodeData(response);
                }
            }
        });
    }
}

var rejectionReasonCode;
//Populate rejection reason code
function PopulateRejectionReasonCodeData(response) {
    $("#RejectionReasonCode").empty();
    $("#RejectionReasonCode").append($("<option></option>").val('').html('Please Select'));
    if (response.length > 0) {

        //Add option label for dropdown
         for (i = 0; i < response.length; i++) {
            if (response[i].Value == rejectionReasonCode) {
                $("#RejectionReasonCode").append($("<option selected='selected' title='"  + response[i].Text + "'></option>").val(response[i].Value).html(response[i].Value + "-" + response[i].Text));
            }
            else {
                $("#RejectionReasonCode").append($("<option title='"  + response[i].Text + "'></option>").val(response[i].Value).html(response[i].Value + "-" + response[i].Text));
            }
        };
        
    }
}
function validateDateRange(startDateId, endDateId) {
    var result = true;
    var startDateVal = $('#' + startDateId).datepicker("getDate");
    var endDateVal = $('#' + endDateId).datepicker("getDate");
    if ((startDateVal != null) && (endDateVal != null))
        result = endDateVal >= startDateVal ? true : false;
        
    return result;
}

function ValidateDate() {
    $("#EffectiveFromPeriod").change(function () {
        var dateComparisonResult = validateDateRange('EffectiveFromPeriod', 'EffectiveToPeriod');
        if (!dateComparisonResult) {
            alert("From date must be lesser than to date");
            $("#EffectiveFromPeriod").val('');
        }
    });
    $("#EffectiveToPeriod").change(function () {
        var dateComparisonResult = validateDateRange('EffectiveFromPeriod', 'EffectiveToPeriod');
        if (!dateComparisonResult) {
            alert("To date must be greater than from date");
            $("#EffectiveToPeriod").val('');
        }
    });

}