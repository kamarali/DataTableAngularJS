function redirectToReport(formId, category) {

    var fdate;
    var fbmonth;
    var tbmonth;
    var billedOrganisation;
    var invoiceNo;

    
    fbmonth = $('#BillingMonth').val();
    tbmonth = $('#BillingYear').val();
    fdate = $('#BillingPeriod').val();
    billedOrganisation = $('#billedOrganisation').val();
    invoiceNo = $('#InvoiceNo').val();


    var rootpath = location.href.substring(0, location.href.indexOf("/Reports"));
    window.open(rootpath + "/ValidationDetailReport.aspx?fbmonth=" + fbmonth + "&tbmonth=" + tbmonth + "&fdate=" + fdate + "&fname=" + fname + "&fSubmissionDate=" + fSubmissionDate + "&Category=" + category + "&BilledOrg=" + billedOrganisation + "&InvoiceNo=" + invoiceNo + "", null, "scrollbars=yes,menubar=no,toolbar=no,location=no,status=yes,resizable=yes");

}




function validateExceptionSummary(formId, category) {
    $("#IsValidationDetail").validate({
        rules: {

            BillingMonth: {
                required: true
            },

            BillingYear: {
                required: true
            }


        },
        messages: {
            BillingMonth: "Billing Month is required.",
            BillingYear: "Billing Year is Required."
        },
        submitHandler: function (form) {
            $('#errorContainer').hide();

            redirectToReport(formId, category);
        },
        invalidHandler: function () {
            $('#errorContainer').show();
            $('#clientErrorMessageContainer').hide();
            $('#clientSuccessMessageContainer').hide();
            //  $.watermark.showAll();
        }
    });





}



