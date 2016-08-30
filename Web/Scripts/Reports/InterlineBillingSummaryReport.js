function redirectToInterlineBillingSummaryReport(formid) {
    var rootpath = location.href.substring(0, location.href.indexOf("/Reports"));
    var IsTotalRequired;
    //Changes to display search criteria on report
    var currencyName;
    var billingMemberName;
    var settlementMethod;
    var SearchCriteria;
    var browserTime;

    if ($("#IsTotalsRequired").prop('checked') == true) {
        IsTotalRequired = 1;
    }
    else {
        IsTotalRequired = 0;
    }

    //start--Changes to display search criteria on report
    currencyName = $('#CurrencyId :selected').text().toString();
    billingMemberName = ($('#BilledEntityCode').val() == '') ? 'All' : $('#BilledEntityCode').val().toString(); //''= Billing Member Code=Blank
    settlementMethod = $('#SettlementMethodStatusId :selected').text().toString();

    SearchCriteria = 'Billing Year:' + $('#Year').val() + ', Billing Month:' + $('#Month :selected').text() + ', Billing Period:' + $('#PeriodNo').val() +
        ', Settlement Method:' + settlementMethod + ', Currency Code:' + currencyName + ', Member Code:' + billingMemberName + ', Totals Required:' + (IsTotalRequired == 1 ? 'Yes' : 'No');
    var regAnd = RegExp("\\&", "g");
    //& Sign causing value to be truncating on getting querystring, so replaced & with 'and'
    SearchCriteria = SearchCriteria.replace(regAnd, "and");
    
    //getDateTimeForReports() function is defined in site.jss
    window.open(rootpath + "/InterlineBillingSummaryReport.aspx?&qsYear=" + $('#Year').val() + "&qsMonth=" + $('#Month').val() + "&qsPeriodNo=" + $('#PeriodNo').val() + "&qsSettlementMethodStatusId=" + $('#SettlementMethodStatusId').val() + "&qsCurrencyId=" + $('#CurrencyId').val() + "&qsBilledEntityCode=" + $('#BilledEntityCodeId').val() + "&qsIsTotalsRequired=" + IsTotalRequired + "&SearchCriteria=" + SearchCriteria + "&BrowserDateTime=" + getDateTimeForReports(), null, "scrollbars=yes,menubar=no,toolbar=no,location=no,status=yes,resizable=yes");
}

function ValidateInterlineBillingSummaryReport(formId) {

    $("#InterlineBillingSummaryReportId").validate({
        rules: {

            Year: {
                required: true
            },

            Month: {
                required: true
            },

            CurrencyId: {
                required: true
            },
            PeriodNo: {
                required: true
            }
        },
        messages: {
            Year: "Billing year is required.",
            Month: "Billing Month is required.",
            CurrencyId: "Currency Code is required.",
            PeriodNo: "Billing Period is required."
        },

        submitHandler: function (form) {
            $('#errorContainer').hide();

            redirectToInterlineBillingSummaryReport(form.id);
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