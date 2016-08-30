

//Region Pax Cgo MscTop Ten Partner Report
function PCMTopTenPartnerReport(formId) {
    var billingMonth;
    var billingYear;
    var billingCategory;
    var currencyCode;
    var IsPayableReport;
    //Changes to display search criteria on report
    var currencyName;
    var billingMemberName;
    var settlementMethod;
    var SearchCriteria;

    billingMonth = $('#Month').val();
    billingYear = $('#Year').val();
    billingCategory = $('#BillingCategory').val();
    currencyCode = $('#CurrencyId').val();
    IsPayableReport = $('#IsPayableReport').val();

    //start--Changes to display search criteria on report
    SearchCriteria = 'Billing Year:' + billingYear + ', Billing Month:' + $('#Month :selected').text() + ', Billing Category:' + $('#BillingCategory :selected').text() +
        ', Currency:' + $('#CurrencyId :selected').text();
    var rootpath = location.href.substring(0, location.href.indexOf("/Reports"));
    var regAnd = RegExp("\\&", "g");
    //& Sign causing value to be truncating on getting querystring, so replaced & with 'and'
    SearchCriteria = SearchCriteria.replace(regAnd, "and");

    //getDateTimeForReports() function is defined in site.jss
    window.open(rootpath + "/PaxCgoMscTopTenPartnerReport.aspx?bMonth=" + billingMonth + "&bYear=" + billingYear
    + "&billingCategory=" + billingCategory + "&currencyCode=" + currencyCode + "&isPayableReport=" + IsPayableReport + "&SearchCriteria=" + SearchCriteria + "&BrowserDateTime=" + getDateTimeForReports());
}

function PaxCgoMscTopTenPartnerReport() {

    $("#PaxCgoMscTopTenPartner").validate({
        rules: {

            BillingCategory: {
                required: true
            },

            Year: {
                required: true
            },

            Month: {
                required: true
            },

            CurrencyId: {
                required: true
            }
        },
        messages: {
            BillingCategory: "Billing Category required.",
            Month: " Billing month required.",
            Year: " Billing Year required.",
            CurrencyId: "Currency code required"
        },
        submitHandler: function (form) {
                   
                $('#errorContainer').hide();
                PCMTopTenPartnerReport(form.id);
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
//End Region Pax Cgo Msc Top Ten Partner Report


//Region Interline Payables Analysis Report
function InterlinePayablesAnalysis(formId) {
    var billingMonth;
    var billingYear;
    var periodNo;
    var currencyCode;
    var billingEntityId;
    var settlementMethod;
    var comparisonPeriod;
    var isTotalsRequired;


    //Changes to display search criteria on report
    var currencyName;
    var billingMemberName;
    var settlementMethod;
    var SearchCriteria;

    billingMonth = $('#Month').val();
    billingYear = $('#Year').val();
    periodNo = $('#PeriodNo').val();
    currencyCode = $('#CurrencyId').val();
    billingEntityId = $('#EntityCodeId').val();
    settlementMethodId = $('#SettlementMethodStatusId').val();
    comparisonPeriod = $('#ComparisonPeriod').val();
    if ($("#IsTotalsRequired").prop('checked') == true) {
        isTotalsRequired = 1;
    }
    else {
        isTotalsRequired = 0;
    }

    //start--Changes to display search criteria on report
    currencyName = $('#CurrencyId :selected').text().toString();
    billingMemberName = (billingEntityId == '') ? 'All' : $('#EntityCode').val().toString(); //''= Billing Member Code=Blank
    settlementMethod = $('#SettlementMethodStatusId :selected').text().toString();

    SearchCriteria = 'Billing Year:' + billingYear.toString() + ', Billing Month:' + $('#Month :selected').text().toString() + ', Billing Period:' + periodNo.toString() +
        ', Currency Code:' + currencyName + ', Billing Member Code:' + billingMemberName + ', Settlement Method:' + settlementMethod + ', Comparison Period:' + $('#ComparisonPeriod :selected').text() +
        ', Totals Required:' + (isTotalsRequired == 1 ? 'Yes' : 'No');
    var regAnd = RegExp("\\&", "g");
    //& Sign causing value to be truncating on getting querystring, so replaced & with 'and'
    SearchCriteria = SearchCriteria.replace(regAnd, "and");
           
    var rootpath = location.href.substring(0, location.href.indexOf("/Reports"));
    //getDateTimeForReports() function is defined in site.jss
    window.open(rootpath + "/InterlinePayablesAnalysis.aspx?bMonth=" + billingMonth + "&bYear=" + billingYear
    + "&periodNo=" + periodNo + "&currencyCode=" + currencyCode + "&billingEntityId=" + billingEntityId
    + "&settlementMethodId=" + settlementMethodId + "&comparisonPeriod=" + comparisonPeriod
    + "&isTotalsRequired=" + isTotalsRequired + "&SearchCriteria=" + SearchCriteria + "&BrowserDateTime=" + getDateTimeForReports());
    //end--Changes to display search criteria on report
}

function InterlinePayablesAnalysisReport() {

    $("#InterlinePayablesAnalysis").validate({
        rules: {

            PeriodNo: {
                required: true
            },

            Year: {
                required: true
            },

            Month: {
                required: true
            },

            CurrencyId: {
                required: true
            },
            ComparisonPeriod: {
                required: true
            }
        },
        messages: {
            PeriodNo: "Period No required.",
            Month: " Billing month required.",
            Year: " Billing Year required.",
            CurrencyId: "Currency code required",
            ComparisonPeriod: "Comparison Period required"
        },
        submitHandler: function (form) {

            $('#errorContainer').hide();
            InterlinePayablesAnalysis(form.id);
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

//End Region Interline Payables Analysis Report