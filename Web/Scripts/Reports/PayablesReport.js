function ValidatePayablesReport(formId, redirectUrl, validateBillYearMonthPeriodUrl) {
    $("#PayablesReport").validate({
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
            var SearchCriteria = 'Search Criteria : ' + 'Billing Year:' + $('#FromBillingYear').val() + '; Billing Month:' + $('#FromBillingMonth :selected').text() + '; Billing Period:' + $('#FromPeriod :selected').text() +
                                 '; Settlement Method:' + $('#SettlementMethod :selected').text() + '; Memo Type:' + $('#MemoType :selected').text() + '; Submission:' + $('#SubmissionMethodId :selected').text() + '; Member Code:' + ($('#BillingEntityCodeId').val() == '' ? 'All' : $('#BillingEntityCode').val()) + '; Invoice Number: ' + $('#InvoiceNo').val() + '; RM/BM/CM Number:' + $('#RMBMCMNo').val() + '; Source Code:' +  ($('#SourceCode').val() == null ? ' ' : $('#SourceCode').val());
            var regAnd = RegExp("\\&", "g");
            //& Sign causing value to be truncating on getting querystring, so replaced & with 'and'
            SearchCriteria = SearchCriteria.replace(regAnd, "and");

            var BrowserDateTime = getDateTimeForReports(); // function is defined in site.jss

            // set redirection URL with query string data (Search Criteria and Browser Date Time)
            $("#PayablesReport").attr("action", redirectUrl + "?searchCriteria=" + SearchCriteria + "&broweserDateTime=" + BrowserDateTime);

      $.ajax({
        url: validateBillYearMonthPeriodUrl,
        data: { month: $('#FromBillingMonth :selected').val(), year: $('#FromBillingYear').val(), period: $('#FromPeriod').val() },
        success: function (errMsg) {
          if (errMsg != "Valid Parameters") {
            alert(errMsg);
          }
          else {
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

function redirectToCargoPayableReport(formid) {
    var rootpath = location.href.substring(0, location.href.indexOf("/Reports"));
    window.open(rootpath + "/RMBMCMSummaryPaybleReport.aspx?BillingMonth=" + $('#Month').val() + "&BillingYear=" + $('#Year').val() + "&PeriodNo=" + $('#Period').val() + "&SettlementMethod=" + $('#SettlementMethodStatusId').val() + "&MemoType=" + $('#MemoType').val() + "&DataSource=" + $('#SubmissionMethodId').val() + "&AirlineCode=" + $('#BilledEntityCode').val() + "&InvoiceNumber=" + $('#InvoiceNo').val() + "&RMBMCMNumber=" + $('#RmbmcmNumber').val() + "&=BillingType" + $('#BillingType').val(), null, "scrollbars=yes,menubar=no,toolbar=no,location=no,status=yes,resizable=yes");

}

function ValidateCargoPayblesReport(formId) {

    $("#CargoPayablesReport").validate({
        rules: {

            Month: {
                required: true
            },

            Year: {
                required: true
            },

            SettlementMethodStatusId: {
                required: true
            },
            SubmissionMethodId: {
                required: true
            }
        },
        messages: {
            Month: "Billing Month is required.",
            SettlementMethodStatusId: "Settlement Method is required.",
            SubmissionMethodId: "Data Source is required. ",
            Year: "Billing Year is required."
        },
        //        up to here 
        submitHandler: function (form) {
            $('#errorContainer').hide();

            redirectToCargoPayableReport(form.id);
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