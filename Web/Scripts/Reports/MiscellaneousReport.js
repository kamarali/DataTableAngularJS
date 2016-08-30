function redirectToMiscChargeSummary(formid) {
    var rootpath = location.href.substring(0, location.href.indexOf("/Reports"));
    //Changes to display search criteria on report
    //var SearchCriteria = 'From Billing Year:' + $('#FromYear').val() + ',From Billing Month:' + $('#FromMonth :selected').text() + ',From Period :' + $('#FromPeriod').val() + ',Settlement Method Indicator:' + $('#SettlementMethodStatusId :selected').text() + ',To Billing Year:' + $('#ToYear').val() + ',To Billing Month:' + $('#ToMonth :selected').text() + ',To Period:' + $('#ToPeriod').val() + ',Billing Member Code:' + ($('#BilledEntityCodeId').val() == '' ? 'All' : $('#BilledEntityCode').val()) + ',Charge Category:' + $('#ChargeCategory :selected').text();
    //var SearchCriteria = 'From Billing Year:' + $('#FromYear').val() + ',From Billing Month:' + $('#FromMonth :selected').text() + ',From Period :' + $('#FromPeriod').val() + ',Submission Method:' + $('#SubmissionMethod :selected').text() + ',Settlement Method Indicator:' + $('#SettlementMethodStatusId :selected').text() + ',To Billing Year:' + $('#ToYear').val() + ',To Billing Month:' + $('#ToMonth :selected').text() + ',To Period:' + $('#ToPeriod').val() + ',Billing Member Code:' + ($('#BilledEntityCodeId').val() == '' ? 'All' : $('#BilledEntityCode').val()) + ',Clearance Currency:' + $('#CurrencyCode :selected').text() + ',Charge Category:' + $('#ChargeCategory :selected').text();
    var BillingTypess = $('#BillingType').val();
   
//CMP521 : Clearance Amount Info in Payables Miscellaneous Invoice Summary Report
//    if (BillingTypess == "1") {
//        var SearchCriteria = 'From Billing Year:' + $('#FromYear').val() + ', From Billing Month:' + $('#FromMonth :selected').text() + ', From Period :' + $('#FromPeriod').val() + ', Settlement Method Indicator:' + $('#SettlementMethodStatusId :selected').text() + ', To Billing Year:' + $('#ToYear').val() + ', To Billing Month:' + $('#ToMonth :selected').text() + ', To Period:' + $('#ToPeriod').val() + ', Billing Member Code:' + ($('#BilledEntityCodeId').val() == '' ? 'All' : $('#BilledEntityCode').val()) + ', Charge Category:' + $('#ChargeCategory :selected').text();
//    }
//    else if (BillingTypess == "2") {
    var selectedCurrency = $('#CurrencyCode :selected').text();
    if (selectedCurrency == 'Please Select')
        selectedCurrency = '';

    //CMP#663 - MISC Invoice Summary Reports - Add 'Transaction Type'
    var SearchCriteria = 'From Billing Year:' + $('#FromYear').val() + ', From Billing Month:' + $('#FromMonth :selected').text() + ', From Period :' + $('#FromPeriod').val() + ', Submission Method:' + $('#SubmissionMethod :selected').text() + ', Settlement Method Indicator:' + $('#SettlementMethodStatusId :selected').text() + ', To Billing Year:' + $('#ToYear').val() + ', To Billing Month:' + $('#ToMonth :selected').text() + ', To Period:' + $('#ToPeriod').val() + ', Billed Member Code:' + ($('#BilledEntityCodeId').val() == '' ? 'All' : $('#BilledEntityCode').val()) + ', Clearance Currency:' + selectedCurrency + ', Charge Category:' + $('#ChargeCategory :selected').text() + ', Transaction Type:' + $('#InvoiceType :selected').text();  
    //}
    var regAnd = RegExp("\\&", "g");
    //& Sign causing value to be truncating on getting querystring, so replaced & with 'and'
    SearchCriteria = SearchCriteria.replace(regAnd, "and");
    var InvoiceType = $('#InvoiceType').val();
    //getDateTimeForReports() function is defined in site.jss
    //CMP#663 - MISC Invoice Summary Reports - Add 'Transaction Type'
    window.open(rootpath + "/MiscInvoiceSummary.aspx?fYear=" + $('#FromYear').val() + "&fMonth=" + $('#FromMonth').val() + "&fPeriod=" + $('#FromPeriod').val() + "&tYear=" + $('#ToYear').val() + "&tMonth=" + $('#ToMonth').val() + "&tPeriod=" + $('#ToPeriod').val() + "&SettlementId=" + $('#SettlementMethodStatusId').val() + "&DataSource=" + $('#SubmissionMethod').val() + "&BilledMemberCode=" + $('#BilledEntityCodeId').val() + "&ChargeCategory=" + $('#ChargeCategory').val() + "&CurrencyCode=" + $('#CurrencyCode').val() + "&InvoiceType=" + $('#InvoiceType').val() + "&BillingType=" + $('#BillingType').val() + "&SearchCriteria=" + SearchCriteria + "&BrowserDateTime=" + getDateTimeForReports(), null, "scrollbars=yes,menubar=no,toolbar=no,location=no,status=yes,resizable=yes");

}

function ValidateMiscChargeSummary(formId) {

    $("#ChargeSummaryReportId").validate({
        rules: {

            FromYear: {
                required: true
            },

            FromMonth: {
                required: true
            },

            FromPeriod: {
                required: true
            },
            ToYear: {
                required: true
            },

            ToMonth: {
                required: true
            },

            ToPeriod: {
                required: true
            }


        },
        messages: {
            FromYear: "From Billing Year is required.",
            FromMonth: "From Billing Month is required.",
            FromPeriod: "From Period is required.",
            ToYear: "To Billing Year is required.",
            ToMonth: "To Billing Month is required.",
            ToPeriod: "To Period is required."

        },

        submitHandler: function (form) {
            var FromDate = new Date($('#FromYear').val() + '/' + $('#FromMonth').val() + '/' + $('#FromPeriod').val());
            var ToDate = new Date($('#ToYear').val() + '/' + $('#ToMonth').val() + '/' + $('#ToPeriod').val());
            if (FromDate > ToDate)
                alert("From Billing Year/Month/Period : " + $('#FromYear').val() + " / " + $('#FromMonth').val() + " / " + $('#FromPeriod').val() + "\n     To Billing Year/Month/Period : " + $('#ToYear').val() + " / " + $('#ToMonth').val() + " / " + $('#ToPeriod').val() + "\nThe Combination of 'To' Billing Year/Month/Period Should Not Be Earlier Than The Combination of 'From' Billing Year/Month/Period.");
            else {
                $('#errorContainer').hide();

                redirectToMiscChargeSummary(form.id);
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

function redirectToMiscSubstitutionValuesReport(formid) {
    var rootpath = location.href.substring(0, location.href.indexOf("/Reports"));
    //Changes to display search criteria on report
    var SearchCriteria = 'Billing Year From:' + $('#FromYear').val() + ', Billing Month From:' + $('#FromMonth :selected').text() + ', Billing Period From:' + $('#FromPeriod').val() + ', Billing Year To:' + $('#ToYear').val() + ', Billing Month To:' + $('#ToMonth :selected').text() + ', Billing Period To:' + $('#ToPeriod').val() + ', Billing Member:' + ($('#BillingEntityCodeId').val() == '' ? 'All' : $('#BillingEntityCode').val()) + ', Billed Member:' + ($('#BilledEntityCodeId').val() == '' ? 'All' : $('#BilledEntityCode').val()) + ', Invoice Number:' + $('#InvoiceNumber').val() + ' , Charge Category:' + $('#ChargeCategory :selected').text() + ', Charge Code:' + $('#ChargeCode :selected').text() + ', Transaction Type:' + $('#InvoiceType :selected').text();
    var regAnd = RegExp("\\&", "g");
    //& Sign causing value to be truncating on getting querystring, so replaced & with 'and'
    SearchCriteria = SearchCriteria.replace(regAnd, "and");

    //getDateTimeForReports() function is defined in site.jss
    window.open(rootpath + "/MiscSubstitutionValuesReport.aspx?fYear=" + $('#FromYear').val() + "&fMonth=" + $('#FromMonth').val() + "&fPeriod=" + $('#FromPeriod').val() + "&tYear=" + $('#ToYear').val() + "&tMonth=" + $('#ToMonth').val() + "&tPeriod=" + $('#ToPeriod').val() + "&BillingMemberCode=" + $('#BillingEntityCodeId').val() +  "&BilledMemberCode=" + $('#BilledEntityCodeId').val() + "&ChargeCategory=" + $('#ChargeCategory').val() + "&ChargeCode=" + $('#ChargeCode').val() + "&InvoiceNumber=" + $('#InvoiceNumber').val() + "&SearchCriteria=" + SearchCriteria + "&BrowserDateTime=" + getDateTimeForReports(), null, "scrollbars=yes,menubar=no,toolbar=no,location=no,status=yes,resizable=yes");
}
function ValidateMiscSubstitutionValuesReport(formId, validateBillYearMonthPeriodUrl) {

    $("#MiscSubstitutionValuesReportId").validate({
        rules: {

            FromYear: {
                required: true
            },

            FromMonth: {
                required: true
            },

            FromPeriod: {
                required: true
            },
            ToYear: {
                required: true
            },

            ToMonth: {
                required: true
            },

            ToPeriod: {
                required: true
            },

            BillingEntityCode: {
                required: true
            }
        },
        messages: {
            FromYear: "From Billing Year is required.",
            FromMonth: "From Billing Month is required.",
            FromPeriod: "From Period is required.",
            ToYear: "To Billing Year is required.",
            ToMonth: "To Billing Month is required.",
            ToPeriod: "To Period is required.",
            BillingEntityCode: "Billing Member is required."
        },
        submitHandler: function (form) {
            var FromDate = new Date($('#FromYear').val() + '/' + $('#FromMonth').val() + '/' + $('#FromPeriod').val());
            var ToDate = new Date($('#ToYear').val() + '/' + $('#ToMonth').val() + '/' + $('#ToPeriod').val());
            if (FromDate > ToDate)
                alert("From Billing Year/Month/Period : " + $('#FromYear').val() + " / " + $('#FromMonth').val() + " / " + $('#FromPeriod').val() + "\n     To Billing Year/Month/Period : " + $('#ToYear').val() + " / " + $('#ToMonth').val() + " / " + $('#ToPeriod').val() + "\nThe Combination of 'To' Billing Year/Month/Period Should Not Be Earlier Than The Combination of 'From' Billing Year/Month/Period.");
            else {
                $('#errorContainer').hide();

                $.ajax({
                  url: validateBillYearMonthPeriodUrl,
                  data: { month: $('#FromMonth :selected').val(), year: $('#FromYear').val(), period: $('#FromPeriod').val() },
                  success: function (errMsg) {
                    if (errMsg != "Valid Parameters") {
                      alert(errMsg);
                    }
                    else {
                redirectToMiscSubstitutionValuesReport(form.id);
                    }
                  },
                  dataType: "text"
                });

                
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
