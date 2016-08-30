function redirectToRMBMCMDetailsReport(formid) {
    var rootpath = location.href.substring(0, location.href.indexOf("/Reports"));
    //Changes to display search criteria on report
    var SearchCriteria = 'Billing Year:' + $('#billingYear').val() + ', Billing Month:' + $('#ClearanceMonth :selected').text() + ', Period No:' + $('#PeriodNo :selected').text() + ', Billing Type:' + $('#BillingType :selected').text() + ', Settlement Method:' + $('#SettlementMethodStatusId :selected').text() + ', Memo Type:' + $('#MemoType :selected').text() + ', Submission Method:' + $('#DataSource :selected').text() + ', Member Code:' + ($('#AirlineId').val() == '' ? 'All' : $('#AirlineCode').val()) + ', Invoice Number:' + $('#InvoiceNumber').val() + ' , Output:' + $('#Output :selected').text() + ', RM/BM/CM Number:' + $('#RMBMCMNumber').val();
    //you will get date time like Thu Apr 12 16:06:18 UTC 0530 2012, but as per Robin you have to create like Thu Apr 12 2012 16:06:18, so following  procedure
    var regAnd = RegExp("\\&", "g");
    //& Sign causing value to be truncating on getting querystring, so replaced & with 'and'
    SearchCriteria = SearchCriteria.replace(regAnd, "and");
    //getDateTimeForReports() function is defined in site.jss
    window.open(rootpath + "/RMBMCMDetailsReport.aspx?ClearanceMonth=" + $('#ClearanceMonth').val() + "&billingYear=" + $('#billingYear').val() + "&PeriodNo=" + $('#PeriodNo').val() + "&BillingType=" + $('#BillingType').val() + "&SettlementMethod=" + $('#SettlementMethodStatusId').val() + "&MemoType=" + $('#MemoType').val() + "&DataSource=" + $('#DataSource').val() + "&AirlineCode=" + $('#AirlineId').val() + "&InvoiceNumber=" + $('#InvoiceNumber').val() + "&Output=" + $('#Output').val() + "&RMBMCMNumber=" + $('#RMBMCMNumber').val() + "&SearchCriteria=" + SearchCriteria + "&BrowserDateTime=" + getDateTimeForReports(), null, "scrollbars=yes,menubar=no,toolbar=no,location=no,status=yes,resizable=yes");

}

function ValidateReceivalbesReport(formId, validateBillYearMonthPeriodUrl) {

    $("#RMBMCMDetails").validate({
        rules: {

            ClearanceMonth: {
                required: true
            },

            billingYear: {
                required: true
            },

            BillingType: {
                required: true
            },

          //SettelmentMethod: {
          //      required: false
          //  },
          //  DataSource: {
          //      required: flase
          //  },
            Output: {
                required: true
            }
        },
        messages: {
            ClearanceMonth: "Billing Month is required.",
            BillingType: "Billing Type is required.",
            //SettelmentMethod: "Settlement Method is required.",
            //DataSource: "Data Source is required. ",
            Output: "Output is required. ",
            billingYear: "Billing Year is required."
        },
//        up to here 
        submitHandler: function (form) {
            $('#errorContainer').hide();

            $.ajax({
              url: validateBillYearMonthPeriodUrl,
              data: { month: $('#ClearanceMonth :selected').val(), year: $('#billingYear').val(), period: $('#PeriodNo').val() },
              success: function (errMsg) {
                if (errMsg != "Valid Parameters") {
                  alert(errMsg);
                }
                else {
            redirectToRMBMCMDetailsReport(form.id);
                }
              },
              dataType: "text"
            });

            
            $.watermark.showAll();
        },
        invalidHandler: function () {
            $('#errorContainer').show();
            $('#clientErrorMessageContainer').hide();
            $('#clientSuccessMessageContainer').hide();
            $.watermark.showAll();
        }
    });


}