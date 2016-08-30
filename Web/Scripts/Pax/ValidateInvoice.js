function validateInvoice(value, actionUrl) {
  $.ajax({
    type: "POST",
    url: actionUrl,
    data: "invoiceId=" + value,
    success: function (result) {
      if (result.IsFailed == false) {
        showClientSuccessMessage(result.Message);
      }
      else {
        showClientErrorMessage(result.Message);
      }
    }
  });
}

