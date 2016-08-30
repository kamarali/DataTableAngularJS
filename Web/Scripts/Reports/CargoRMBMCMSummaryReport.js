function ValidateCargoRMBMCMSummaryReport(formId, redirectUrl, validateBillYearMonthPeriodUrl) {
  $("#CargoRMBMCMSummaryReport").validate({
    rules: {

      Month: {
        required: true
      },

      Year: {
        required: true
      }
    },
    messages: {
      Month: "Billing Month is required.",
      Year: "Billing Year is required."
    },
    submitHandler: function (form) {
      $('#errorContainer').hide();
      $('#successMessageContainer').hide();
      var billingOrBilledLabel = ($('#BillingType').val() == '1' ? ', Billing Member Code:' : ', Billed Member Code:'); //1=payable, 2=receivable
      var SearchCriteria = 'Search Criteria : ' + 'Billing Year:' + $('#Year').val() + ', Billing Month:' + $('#Month :selected').text() + ', Billing Period:' + $('#Period :selected').text() + ', Settlement Method:' + $('#SettlementMethodIndicatorId :selected').text() + ', Memo Type:' + $('#MemoType :selected').text() + ', Submission Method:' + $('#SubmissionMethodId :selected').text() + billingOrBilledLabel + ($('#AirlineId').val() == '' ? 'All' : $('#AirlineCode').val()) + ', Invoice Number:' + $('#InvoiceNo').val() + ' , RM/BM/CM Number:' + $('#RMBMCMNo').val();
      var regAnd = RegExp("\\&", "g");
      //& Sign causing value to be truncating on getting querystring, so replaced & with 'and'
      SearchCriteria = SearchCriteria.replace(regAnd, "and");
      var BrowserDateTime = getDateTimeForReports();

      // set redirection URL with query string data (Search Criteria and Browser Date Time)
      $("#CargoRMBMCMSummaryReport").attr("action", redirectUrl + "?searchCriteria=" + SearchCriteria + "&broweserDateTime=" + BrowserDateTime);
      //SCP237392: Report - Receivable - Non Sample Rejection Analysis
      $.ajax({
        url: validateBillYearMonthPeriodUrl,
        data: { month: $('#Month :selected').val(), year: $('#Year').val(), period: $('#Period').val() },
        success: function (errMsg) {
          if (errMsg != "Valid Parameters") {
            alert(errMsg);
          }
          else {
            form.submit();
            $.watermark.showAll();
          }
        },
        dataType: "text"
      });
    },
    invalidHandler: function () {
      $('#errorContainer').show();
      $('#clientErrorMessageContainer').hide();
      $('#clientSuccessMessageContainer').hide();
      $.watermark.showAll();
    }
  });
}