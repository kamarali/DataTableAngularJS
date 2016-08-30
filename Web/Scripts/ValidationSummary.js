

function redirectToReport(formId, category) {
  var isDate;
  var fdate;
  var tdate;
  var fbmonth;
  var tbmonth;
  var fname;
  var fSubmissionDate;
  var tSubmissionDate;

  fSubmissionDate = $('#fromSubmissionDate').val();
  fname = $('#filename').val();
  fbmonth = $('#BillingMonth').val();
  tbmonth = $('#BillingYear').val();
  fdate = $('#BillingPeriod').val();

    var rootpath = location.href.substring(0, location.href.indexOf("/Reports"));
    window.open(rootpath + "/PassengerValidationSummaryReport.aspx?fbmonth=" + fbmonth + "&tbmonth=" + tbmonth + "&fdate=" + fdate +  "&fname=" + fname + "&fSubmissionDate=" + fSubmissionDate + "&Category=" + category + "", null, "scrollbars=yes,menubar=no,toolbar=no,location=no,status=yes,resizable=yes");

}




function validateExceptionSummary(formId, category) {
   
    $("#IsValidationSummary").validate({
        rules: {

            BillingMonth: {
                required: true
            },

            BillingYear: {
                required: true
            }

           
        },
        messages: {
            BillingMonth: "From Billing Month is required.",
            BillingYear: "To Billing Month is Required."
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

