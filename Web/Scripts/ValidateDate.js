function BindEventForDate() {
    $("#FromDate").bind("change", ValidateDateCorrespondence);
    $("#ToDate").bind("change", ValidateDateCorrespondence);

}

function validateDateRange(startDateId, endDateId) {
    var result = true;
    var startDateVal = $('#' + startDateId).datepicker("getDate");
    var endDateVal = $('#' + endDateId).datepicker("getDate");
    if ((startDateVal != null) && (endDateVal != null))
        result = endDateVal >= startDateVal ? true : false;

    return result;
}

// Minacceptableamount
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

// Billing History Correspondence 
function ValidateDateCorrespondence() {
    $("#FromDate").change(function () {
        var dateComparisonResult = validateDateRange('FromDate', 'ToDate');
        if (!dateComparisonResult) {
            alert("From date must be lesser than to date");
            $("#FromDate").val('');
        }
    });
    $("#ToDate").change(function () {
        var dateComparisonResult = validateDateRange('FromDate', 'ToDate');
        if (!dateComparisonResult) {
            alert("To date must be greater than from date");
            $("#ToDate").val('');
        }
    });
}