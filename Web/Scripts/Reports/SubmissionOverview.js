

function ValidateOverviewDocument(formId) {
    $("#CargoSubmissionOverview").validate({
        rules: {
            BillingMonthFrom: "required",
            BillingMonthTo: "required",
            BillingYearFrom: "required",
            BillingYearTo: "required",
            PeriodNoFrom: "required",
            PeriodNoTo: "required",
            Output: "required"
        },
        messages: {
            BillingMonthFrom: "Billing Month From Required",
            BillingMonthTo: "Billing Month To Required",
            BillingYearFrom: "Billing Year From Required",
            BillingYearTo: "Billing Year To Required",
            PeriodNoFrom: "Billing Period From Required",
            PeriodNoTo: "Billing Period To Required",
            Output: "Output Required"
        },

        submitHandler: function (form) {
            var FromDate = new Date($('#BillingYearFrom').val() + '/' + $('#BillingMonthFrom').val() + '/' + $('#PeriodNoFrom').val());
            var ToDate = new Date($('#BillingYearTo').val() + '/' + $('#BillingMonthTo').val() + '/' + $('#PeriodNoTo').val());
            if (FromDate > ToDate)
                alert("From Billing Year/Month/Period : " + $('#BillingYearFrom').val() + " / " + $('#BillingMonthFrom').val() + " / " + $('#PeriodNoFrom').val() + "\n     To Billing Year/Month/Period : " + $('#BillingYearTo').val() + " / " + $('#BillingMonthTo').val() + " / " + $('#PeriodNoTo').val() + "\nThe Combination of 'To' Billing Year/Month/Period Should Not Be Earlier Than The Combination of 'From' Billing Year/Month/Period.");
            else {
                $('#errorContainer').hide();
                redirectToSubmissionOverviewReport(formId);
            }
        },
        invalidHandler: function () {
            $('#errorContainer').show();
            $('#clientErrorMessageContainer').hide();
            $('#clientSuccessMessageContainer').hide();
            $.watermark.showAll();
        }

    })
    
}

function redirectToSubmissionOverviewReport(formId) {
    var rootpath = location.href.substring(0, location.href.indexOf("/Reports"));
    //Changes to display search criteria on report
    if(formId == 'CargoSubmissionOverviewReceivables')
        var BilledOrBilling =  ', Billed Member Code:' + ($('#EntityId').val() == '' ? 'All' : $('#BilledEntity').val());
    else //CargoSubmissionOverviewPayables
        var BilledOrBilling =  ', Billing Member Code:'+ ($('#BillingEntity').val() == '' ? 'All' : $('#BillingEntity').val());
    //To identify if form is called for payable or receivable
    var SearchCriteria = 'Billing Year From:' + $('#BillingYearFrom').val() + ', Billing Month From:' + $('#BillingMonthFrom :selected').text() + ', Billing Period From:' + $('#PeriodNoFrom').val() + ', Billing Year To:' + $('#BillingYearTo').val() + ', Billing Month To:' + $('#BillingMonthTo :selected').text() + ', Billing Period To:' + $('#PeriodNoTo').val() + BilledOrBilling + ', Settlement Method:' + $('#SettlementMethodStatusId :selected').text() + ', Output:' + $('#Output :selected').text();
    var regAnd = RegExp("\\&", "g");
    //& Sign causing value to be truncating on getting querystring, so replaced & with 'and'
    SearchCriteria = SearchCriteria.replace(regAnd, "and");

    //getDateTimeForReports() function is defined in site.jss
    window.open(rootpath + "/CargoSubmissionOverview.aspx?&bMonthFrom=" + $('#BillingMonthFrom').val() + "&bMonthTo=" + $('#BillingMonthTo').val() + "&bYearFrom=" + $('#BillingYearFrom').val() + "&bYearTo=" + $('#BillingYearTo').val() + "&PeriodFrom=" + $('#PeriodNoFrom').val() + "&PeriodTo=" + $('#PeriodNoTo').val() + "&SetelmentMethodId=" + $('#SettlementMethodStatusId').val() + "&BilledEntity=" + $('#EntityId').val() + "&BillingEntity=" + $('#EntityId').val() + "&Output=" + $('#Output').val() + "&BillingType=" + $('#BillingType').val() + "" + "&SearchCriteria=" + SearchCriteria + "&BrowserDateTime=" + getDateTimeForReports(), null, "scrollbars=yes,menubar=no,toolbar=no,location=no,status=yes,resizable=yes");        
}



    