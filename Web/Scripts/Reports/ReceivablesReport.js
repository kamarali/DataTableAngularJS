function ValidateReceivalbesReport(formId, redirectUrl, validateBillYearMonthPeriodUrl) {

    $("#ReceivalbesReport").validate({
        rules: {

            FromBillingYear: {
                required: true
            },

            FromBillingMonth: {
                required: true
            }
        },
        messages: {
            FromBillingYear: "Billing year is required.",
            FromBillingMonth: "Billing Month is required.",
            FromPeriod: "Period is required."
        },
        submitHandler: function (form) {
            $('#errorContainer').hide();
            $('#successMessageContainer').hide();
            var SearchCriteria = 'Search Criteria : ' + 'Billing Year:' + $('#FromBillingYear').val() + '; Billing Month:' + $('#FromBillingMonth :selected').text() + '; Billing Period:' + $('#FromPeriod :selected').text() + '; Settlement Method:' + $('#SettlementMethod :selected').text() + '; Memo Type:' + $('#MemoType :selected').text() + '; Submission:' + $('#SubmissionMethodId :selected').text() + '; Member Code:' +
                                            ($('#BilledEntityCodeId').val() == '' ? 'All' : $('#BilledEntityCode').val().toString()) + '; Invoice No:' + $('#InvoiceNo').val() + '; RM/BM/CM Number:' + $('#RMBMCMNo').val() + '; Source Code:' + ($('#SourceCode').val() == null ? ' ' : $('#SourceCode').val());
            var regAnd = RegExp("\\&", "g");
            //& Sign causing value to be truncating on getting querystring, so replaced & with 'and'
            SearchCriteria = SearchCriteria.replace(regAnd, "and");

            var BrowserDateTime = getDateTimeForReports(); // function is defined in site.jss

            // set redirection URL with query string data (Search Criteria and Browser Date Time)
            $("#ReceivalbesReport").attr("action", redirectUrl + "?searchCriteria=" + SearchCriteria + "&broweserDateTime=" + BrowserDateTime);

            $.ajax({
        url: validateBillYearMonthPeriodUrl,
        data: { month: $('#FromBillingMonth :selected').val(), year: $('#FromBillingYear').val(), period: $('#FromPeriod').val() },
        success: function (errMsg) { 
          if(errMsg != "Valid Parameters"){
            alert(errMsg);
          }
          else{
            form.submit();            
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

function redirectToOwSamplingReport(formid) {
    var rootpath = location.href.substring(0, location.href.indexOf("/Reports"));
    //Changes to display search criteria on report
    //SCP345703: LX to LA nov-14 rejections
    var billingBilledLabel = $('#BillingType').val() == "2" ? ", Billed Member Code:" : ", Billing Member Code:"; //1=payable report, 2=receivable report
    var SearchCriteria = 'Provisional Billing Year:' + $('#Year').val() + ', Provisional Billing Month:' + $('#Month :selected').text() + billingBilledLabel + ($('#BilledEntityCodeId').val() == '' ? 'All' : $('#BilledEntityCode').val()) + ', Currency Code:' + $('#CurrencyId :selected').text();
    var regAnd = RegExp("\\&", "g");
    //& Sign causing value to be truncating on getting querystring, so replaced & with 'and'
    SearchCriteria = SearchCriteria.replace(regAnd, "and");

    //getDateTimeForReports() function is defined in site.jss
    window.open(rootpath + "/OwSampling.aspx?qsYear=" + $('#Year').val() + "&qsMonth=" + $('#Month').val() + "&qsBilledEntityCode=" + $('#BilledEntityCodeId').val() + "&qsCurrencyId=" + $('#CurrencyId').val() + "&qsBillingType=" + $('#BillingType').val() + "&SearchCriteria=" + SearchCriteria + "&BrowserDateTime=" + getDateTimeForReports(), null, "scrollbars=yes,menubar=no,toolbar=no,location=no,status=yes,resizable=yes");
}

function ValidateOwSamplingReport(formId) {

    $("#OwSamplingReportId").validate({
        rules: {

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
            Year: "Billing Year is required.",
            Month: "Billing Month is required.",
            CurrencyId: "Currency Code is required."
        },

        submitHandler: function (form) {
            $('#errorContainer').hide();

            redirectToOwSamplingReport(form.id);
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

function redirectToPaxInterlineBillingSummaryReport(formid) {

    var rootpath = location.href.substring(0, location.href.indexOf("/Reports"));
    //Changes to display search criteria on report
    var billingBilledLabel = $('#BillingType').val() == "2" ? ", Billed Member Code:" : ", Billing Member Code:"; //1=payable report, 2=receivable report
    var SearchCriteria = 'Billing Year:' + $('#Year').val() + ', Billing Month:' + $("#Month :selected").text() + ', Billing Period:' + $("#PeriodNo :selected").text() + billingBilledLabel + ($('#BilledEntityCodeId').val() == '' ? 'All' : $('#BilledEntityCode').val()) + ', Settlement Method Indicator:' + $("#SettlementMethodStatusId :selected").text() + ', Currency Code:' + $("#CurrencyId :selected").text();
    var regAnd = RegExp("\\&", "g");
    //& Sign causing value to be truncating on getting querystring, so replaced & with 'and'
    SearchCriteria = SearchCriteria.replace(regAnd, "and");

    //getDateTimeForReports() function is defined in site.jss
    window.open(rootpath + "/PaxInterlineBillingSummaryReport.aspx?qsBillingType=" + $('#BillingType').val() + "&qsYear=" + $('#Year').val() + "&qsMonth=" + $('#Month').val() + "&qsPeriod=" + $('#PeriodNo').val() + "&qsBilledEntityCode=" + $('#BilledEntityCodeId').val() + "&qsSettlementMethodId=" + $('#SettlementMethodStatusId').val() + "&qsCurrencyId=" + $('#CurrencyId').val() + "&SearchCriteria=" + SearchCriteria + "&BrowserDateTime=" + getDateTimeForReports(), null, "scrollbars=yes,menubar=no,toolbar=no,location=no,status=yes,resizable=yes");
}

function ValidatePaxInterlineBillingSummaryReport(formId) {

    $("#PaxInterlineBillingSummaryReportId").validate({
        rules: {

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
            Year: "Billing year is required.",
            Month: "Billing Month is required.",
            CurrencyId: "Currency Type is required."
        },

        submitHandler: function (form) {
            $('#errorContainer').hide();

            redirectToPaxInterlineBillingSummaryReport(form.id);
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

function redirectToPaxRejectionAnalysisNonSamplingReport(formid) {
    var rootpath = location.href.substring(0, location.href.indexOf("/Reports"));

    var IncludeFIM;
    if ($("#IncludeFIMData").prop('checked') == true) {
      IncludeFIM = 1;
    }
    else {
      IncludeFIM = 0;
    }

    //CMP #691: PAX Non-Sampling and CGO -Modifications to Rejection Analysis Reports
    //input parameter updated (From Year Month and To Year Month)

    //Changes to display search criteria on report
      var SearchCriteria = 'From Billing Year:' + $('#FromYear').val() + ', From Billing Month:' + $('#FromMonth :selected').text() + ', To Billing Year:' + $('#ToYear').val() + ', To Billing Month:' + $('#ToMonth :selected').text() + ', Billed Member Code:' + ($('#BilledEntityCodeId').val() == '' ? 'All' : $('#BilledEntityCode').val()) + ', Currency Code:' + $('#CurrencyId :selected').text() + ', Include FIM Data:' + (IncludeFIM == 1 ? 'Yes' : 'No');
    var regAnd = RegExp("\\&", "g");
    //& Sign causing value to be truncating on getting querystring, so replaced & with 'and'
    SearchCriteria = SearchCriteria.replace(regAnd, "and");

    //getDateTimeForReports() function is defined in site.jss
    window.open(rootpath + "/PaxRejectionAnalysisNonSamplingReport.aspx?qsBillingType=" + $('#BillingType').val() + "&qsFromYear=" + $('#FromYear').val() + "&qsFromMonth=" + $('#FromMonth').val() + "&qsToYear=" + $('#ToYear').val() + "&qsToMonth=" + $('#ToMonth').val() + "&qsBilledEntityCode=" + $('#BilledEntityCodeId').val() + "&qsIncludeFIMData=" + IncludeFIM + "&qsCurrencyId=" + $('#CurrencyId').val() + "&SearchCriteria=" + SearchCriteria + "&BrowserDateTime=" + getDateTimeForReports(), null, "scrollbars=yes,menubar=no,toolbar=no,location=no,status=yes,resizable=yes");
}

function ValidatePaxRejectionAnalysisNonSamplingReport(formId, validateBillYearMonthUrl) {
  //CMP #691: PAX Non-Sampling and CGO -Modifications to Rejection Analysis Reports
  //Validation logic updated
  $("#PaxRejectionAnalysisNonSamplingReportId").validate({
    rules: {
      FromYear: {
        required: true
      },
      FromMonth: {
        required: true
      },
      ToYear: {
        required: true
      },
      ToMonth: {
        required: true
      },
      CurrencyId: {
        required: true
      }
    },
    messages: {
      FromYear: "From Billing Year is required.",
      FromMonth: "From Billing Month is required.",
      ToYear: "To Billing Year is required.",
      ToMonth: "To Billing Month is required.",
      CurrencyId: "Currency Type is required."
    },
    submitHandler: function (form) {
      $('#errorContainer').hide();

      $.ajax({
        url: validateBillYearMonthUrl,
        //CMP #691: PAX Non-Sampling and CGO -Modifications to Rejection Analysis Reports
        //Validation logic updated
        data: { fromMonth: $('#FromMonth :selected').val(), fromYear: $('#FromYear').val(), toMonth: $('#ToMonth :selected').val(), toYear: $('#ToYear').val() },
        success: function (errMsg) {
          if (errMsg != "Valid Parameters") {
            alert(errMsg);
          }
          else {
            redirectToPaxRejectionAnalysisNonSamplingReport(form.id);
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