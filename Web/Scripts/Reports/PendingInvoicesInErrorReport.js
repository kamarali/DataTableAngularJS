function redirectToPendingInvoicesInErrorReport(formid) {
    var IsTotalRequired;
    if ($("#IsTotalsRequired").prop('checked') == true) {
        IsTotalRequired = 1;
    }
    else {
        IsTotalRequired = 0;
    }

    //Changes to display search criteria on report
    var currencyName;
    var billingMemberName;
    var settlementMethod;
    var SearchCriteria;

    //start--Changes to display search criteria on report
    SearchCriteria = 'Billing Year:' + $('#Year').val() + ', Billing Month:' + $('#Month :selected').text() + ', Billing Period:' + $('#Period').val() +
        ', Billing Category:' + ($('#BillingCategory').val() == -1 ? 'All' : $('#BillingCategory :selected').text().toString()) +
        ', Settlement Method:' + ($('#SettlementMethodStatusId').val() == -1? 'All' : $('#SettlementMethodStatusId :selected').text().toString()) +
        ', Error Type:' + ($('#ErrorTypeId').val() == -1 ? 'All' : $('#ErrorTypeId :selected').text().toString()) +
        ', Totals Required:' + (IsTotalRequired == 1 ? 'Yes' : 'No');
    var regAnd = RegExp("\\&", "g");
    //& Sign causing value to be truncating on getting querystring, so replaced & with 'and'
    SearchCriteria = SearchCriteria.replace(regAnd, "and");
    var rootpath = location.href.substring(0, location.href.indexOf("/Reports"));
    //getDateTimeForReports() function is defined in site.jss
    window.open(rootpath + "/CrystalReports/PendingInvoicesInErrorReport.aspx?month=" + $('#Month').val() + "&year=" + $('#Year').val() + "&periodNo=" + $('#Period').val() + "&billingCategory=" + $('#BillingCategory').val() + "&settlementMethod=" + $('#SettlementMethodStatusId').val() + "&errorType=" + $('#ErrorTypeId').val() + "&isTotalRequired=" + IsTotalRequired + "&SearchCriteria=" + SearchCriteria + "&BrowserDateTime=" + getDateTimeForReports(), null, "scrollbars=yes,menubar=no,toolbar=no,location=no,status=yes,resizable=yes");    
    }

function ValidateReport(formId, validateBillYearMonthPeriodUrl) {

    $("#PendingInvoicesInErrorReport").validate({
        rules: {

            Year: {
                required: true
            },

            Month: {
                required: true
            },
            Period: {
                required: true
            },
            BillingCategory: {
                required: true
            },

            SettlementMethodStatusId: {
                required: true
            }
        },
        messages: {
            Year: "Billing Year is required.",
            Month: "Billing Month is required.",
            Period: "Billing Period is required.",
            BillingCategory: "Billing Category is required.", 
            SettlementMethodStatusId: "Settlement Method is required."
        },

        submitHandler: function (form) {
            $('#errorContainer').hide();

      $.ajax({
        url: validateBillYearMonthPeriodUrl,
        data: { month: $('#Month :selected').val(), year: $('#Year').val(), period: $('#Period').val() },
        success: function (errMsg) { 
          if(errMsg != "Valid Parameters"){
            alert(errMsg);
          }
          else{
            redirectToPendingInvoicesInErrorReport(form.id);
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