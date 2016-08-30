function redirectToInterlineBillingSummaryReport(formid) {
    var rootpath = location.href.substring(0, location.href.indexOf("/Reports"));
    //start--Changes to display search criteria on report  
    var billingBilledLabel = $('#BillingType').val() == "2" ? ", Billed Member Code:" : ", Billing Member Code:"; //1=payable report, 2=receivable report
    SearchCriteria = 'Billing Year From:' + $('#FromYear').val() + ', Billing Month From:' + $('#FromMonth :selected').text() + ', Billing Period From:' + $('#FromPeriod').val() +
        ', Billing Year To:' + $('#ToYear').val() + ', Billing Month To:' + $('#ToMonth :selected').text() + ', Billing Period To:' + $('#ToPeriod').val() +
        billingBilledLabel + ($('#AirlineId').val() == '' ? 'All' : $('#AirlineCode').val()) + ', Settlement Method:' + $('#SettlementMethodStatusId :selected').text() + ', Currency Code:' + $('#CurrencyId :selected').text();
    var regAnd = RegExp("\\&", "g");
    //& Sign causing value to be truncating on getting querystring, so replaced & with 'and'
    SearchCriteria = SearchCriteria.replace(regAnd, "and");

    //getDateTimeForReports() function is defined in site.jss
    window.open(rootpath + "/CargoInterlineBillingSummaryReport.aspx?fMonth=" + $('#FromMonth').val() + "&tMonth=" + $('#ToMonth').val() + "&fYear=" + $('#FromYear').val() + "&tYear=" + $('#ToYear').val() + "&fPeriodNo=" + $('#FromPeriod').val() + "&tPeriodNo=" + $('#ToPeriod').val() + "&settlementMethod=" + $('#SettlementMethodStatusId').val() + "&airlineCode=" + $('#AirlineId').val() + "&currencyId=" + $('#CurrencyId').val() + "&bType=" + $('#BillingType').val() + "&SearchCriteria=" + SearchCriteria + "&BrowserDateTime=" + getDateTimeForReports(), null, "scrollbars=yes,menubar=no,toolbar=no,location=no,status=yes,resizable=yes");

}

function ValidateReport(formId) {

    $("#CargoIntelineBillingSummaryReport").validate({
        rules: {

            FromYear: {
                required: true
            },

            ToYear: {
                required: true
            },

            FromMonth: {
                required: true
            },
            ToMonth: {
                required: true
            },
            FromPeriod: {
                required: true
            },
            ToPeriod: {
                required: true
            },
			CurrencyId:{
                required: true
            }
        },
        messages: {
            FromYear: "From Billing Year is required.",
            ToYear: "To Billing Year is required.",
            FromMonth: "From Billing Month is required.",
            ToMonth: "To Billing Month is required. ",
            FromPeriod: "From Billing Period is required.",
            ToPeriod: "To Billing Period is required. ",
            CurrencyId: "Currency Code is required."
        },
        
        submitHandler: function (form) {
            var FromDate = new Date($('#FromYear').val() + '/' + $('#FromMonth').val() + '/' + $('#FromPeriod').val());
            var ToDate = new Date($('#ToYear').val() + '/' + $('#ToMonth').val() + '/' + $('#ToPeriod').val());
            if (FromDate > ToDate)
                alert("From Billing Year/Month/Period : " + $('#FromYear').val() + " / " + $('#FromMonth').val() + " / " + $('#FromPeriod').val() + "\n     To Billing Year/Month/Period : " + $('#ToYear').val() + " / " + $('#ToMonth').val() + " / " + $('#ToPeriod').val() + "\nThe Combination of 'To' Billing Year/Month/Period Should Not Be Earlier Than The Combination of 'From' Billing Year/Month/Period.");
            else {
                $('#errorContainer').hide();
                redirectToInterlineBillingSummaryReport(form.id);
                $.watermark.showAll();
            }
        },
        invalidHandler: function () {
            $('#errorContainer').show();
            $('#clientErrorMessageContainer').hide();
            $('#clientSuccessMessageContainer').hide();
            $.watermark.showAll();
        }
    });


}