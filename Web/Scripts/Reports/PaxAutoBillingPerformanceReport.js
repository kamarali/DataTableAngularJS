function redirectToAutoBillingPerforamanceReport(formid) {
    var rootpath = location.href.substring(0, location.href.indexOf("/Reports"));
    //changes to display search criteria on report
    var SearchCriteria = 'Clearance Year : ' + $('#Year').val() + ', Clearance Month : ' + $('#Month :selected').text() + ', Billed Member Code : ' + ($('#BilledEntityId').val() == '' ? 'All' : $('#BilledEntityCode').val().toString()) + ', Currency Code : ' + $('#CurrencyId :selected').text();
    var regAnd = RegExp("\\&", "g");
    //& Sign causing value to be truncating on getting querystring, so replaced & with 'and'
    SearchCriteria = SearchCriteria.replace(regAnd, "and");

    //getDateTimeForReports() function is defined in site.jss
    window.open(rootpath + "/AutoBillingPerformanceReport.aspx?clearanceYear=" + $('#Year').val() + "&clearanceMonth=" + $('#Month').val() + "&entityId=" + $('#BilledEntityId').val() + "&currencyCode=" + $('#CurrencyId').val() + "&SearchCriteria=" + SearchCriteria + "&BrowserDateTime=" + getDateTimeForReports(), null, "scrollbars=yes,menubar=no,toolbar=no,location=no,status=yes,resizable=yes");
}
      


function ValidateReport(formId) {
  $("#AutoBillingPerformanceReport").validate({
    rules: {
      Year: "required",
      Month: "required",
      CurrencyId: "required"
    },
    messages: {
      Year: "Year Required",
      Month: "Month Required",
      CurrencyId: "Currency Required"
    },

    submitHandler: function (form) {

      $('#errorContainer').hide();
      redirectToAutoBillingPerforamanceReport(formId);
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