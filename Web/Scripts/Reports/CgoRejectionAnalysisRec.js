


function redirectToReport(formId) {

  //CMP #691: PAX Non-Sampling and CGO -Modifications to Rejection Analysis Reports
  //input parameter updated (From Year Month and To Year Month)

    var fromBillingMonth;
    var fromBillingYear;
    var toBillingMonth;
    var toBillingYear;
    var loginIntityID;
    var againstEntityId;
    var currencyCode;
    var IsPayableReport;    

    fromBillingMonth = $('#FromMonth').val();
    fromBillingYear = $('#FromYear').val();
    toBillingMonth = $('#ToMonth').val();
    toBillingYear = $('#ToYear').val();

    againstEntityId = $('#EntityCodeId').val();
    currencyCode = $('#CurrencyId').val();
    IsPayableReport = $('#IsPayableReport').val();

    var rootpath = location.href.substring(0, location.href.indexOf("/Reports"));

    //start--Changes to display search criteria on report
    var BilledOrBillingMemberLabel = (IsPayableReport==true ? ', Billing Member Code:' : ', Billed Member Code:');
    var SearchCriteria = 'From Billing Year:' + fromBillingYear + ', From Billing Month:' + $('#FromMonth :selected').text() + ', To Billing Year:' + toBillingYear + ', To Billing Month:' + $('#ToMonth :selected').text() + BilledOrBillingMemberLabel + ($('#EntityCodeId').val() == '' ? 'All' : $('#EntityCode').val()) + ', Currency:' + $('#CurrencyId :selected').text();
    var regAnd = RegExp("\\&", "g");
    //& Sign causing value to be truncating on getting querystring, so replaced & with 'and'
    SearchCriteria = SearchCriteria.replace(regAnd, "and");

    //getDateTimeForReports() function is defined in site.jss
    window.open(rootpath + "/CgoRejectionAnalysisRecReport.aspx?bFromMonth=" + fromBillingMonth + "&bFromYear=" + fromBillingYear + "&bToMonth=" + toBillingMonth + "&bToYear=" + toBillingYear
    + "&againstEntityId=" + againstEntityId + "&currencyCode=" + currencyCode + "&isPayableReport=" + IsPayableReport + "&SearchCriteria=" + SearchCriteria + "&BrowserDateTime=" + getDateTimeForReports());

}

function RejectionAnalysisReport(validateBillYearMonthUrl) {

  Cgo_RejectionAnalysisRecValidate(validateBillYearMonthUrl);
}

function Cgo_RejectionAnalysisRecValidate(validateBillYearMonthUrl) {

  //CMP #691: PAX Non-Sampling and CGO -Modifications to Rejection Analysis Reports
  //Validation logic updated
  $("#CgoRejectionAnalysisRec").validate({
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
      CurrencyId: "Currency code required"
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
            redirectToReport(form.id);
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

