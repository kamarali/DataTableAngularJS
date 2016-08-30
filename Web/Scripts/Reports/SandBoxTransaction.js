
function redirectToReport(formId) {
    var fDate;
    var tDate;
    var memberId;
    var fileFormatId;
    var billingCategoryId;
    var requestTypeId;
    var transactionGroupId;

    fDate = $('#FileSubmittedFromdate').val();
    tDate = $('#FileSubmittedToDate').val();
    memberId = $('#MemberId').val();
    fileFormatId = $('#FileFormatId').val();
    billingCategoryId = $('#BillingCategoryId').val();
    requestType = $('#RequestType').val();
    transactionGroupId = $('#TransactionGroupId').val();

    var rootpath = location.href.substring(0, location.href.indexOf("/Reports"));
    window.open(rootpath + "/SandBoxTransactionReport.aspx?fDate=" + fDate + "&tDate=" + tDate + "&memberId=" + memberId + "&fileFormatId=" + fileFormatId + "&billingCategoryId=" + billingCategoryId + "&requestType=" + requestType + "&transactionGroupId=" + transactionGroupId + "");
}

function SandBoxTransactionValidation(formId) {

    $("#SandBoxTransaction").validate({
        rules: {

            FileSubmittedFromdate: "required",
            FileSubmittedToDate: "required"
        },

        messages: {
            FileSubmittedFromdate: "From Date Required.",
            FileSubmittedToDate: "To Date Required."
        },

        submitHandler: function (form) {
            $('#errorContainer').hide();


            var dateComparisonResult = validateDateRange('FileSubmittedFromdate', 'FileSubmittedToDate');
            if (dateComparisonResult) {
                redirectToReport(form.id);
            }
            else {
                alert("From Date must be less than or equal to the To Date");
            }
        },
        invalidHandler: function () {
            $('#errorContainer').show();
            $('#clientErrorMessageContainer').hide();
            $('#clientSuccessMessageContainer').hide();
        }
    });
}

function validateDateRange(startDateId, endDateId) {
    var startDateVal = $('#' + startDateId).datepicker("getDate");
    var endDateVal = $('#' + endDateId).datepicker("getDate");
    return endDateVal >= startDateVal;
}