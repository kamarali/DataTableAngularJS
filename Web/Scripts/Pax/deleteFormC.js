/// <reference path="site.js" />
function deleteRecord(methodName, billingYear, billingMonth, billingMemberId, fromMemberId, listingCurrencyId, invoiceStatusId, gridId) {
  
  if (confirm("Are you sure you want to delete this record?")) {
    var url;
    if (listingCurrencyId == 'NO_VALUE')
      url = methodName + "/" + billingYear + "/" + billingMonth + "/" + billingMemberId + "/" + fromMemberId + "/" + invoiceStatusId;
    else
      url = methodName + "/" + billingYear + "/" + billingMonth + "/" + billingMemberId + "/" + fromMemberId + "/" + invoiceStatusId + "/" + listingCurrencyId;

    $.ajax({
      type: "POST",
      url: url,
      success: function (result) {
        if (result.IsFailed == false) {
          showClientSuccessMessage(result.Message);
          $(gridId).trigger("reloadGrid");
        }
        else {
          showClientErrorMessage(result.Message);
        }
      }
    });    
  }
}