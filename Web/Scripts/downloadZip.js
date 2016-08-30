/// <reference path="site.js" />
var _id, _methodName;
function downloadZip(methodName, value, gridId) {
  _id = value;
  _methodName = methodName
  $("#InvoiceDownloadOptions").dialog({ title: 'Choose invoice download options', closeOnEscape: 'true' });
}

function closeInvoiceDownloadOptions() {
    $('input[id=1]').prop("checked", "");
    $('input[id=2]').prop("checked", "");
    $('input[id=4]').prop("checked", "");
    $('input[id=8]').prop("checked", "");
  $("#selectAll").attr('value', 'Select All');
  $("#InvoiceDownloadOptions").dialog('close');
}

function selectAllCheckBox() {
  var button = $("#selectAll");
  if (button.val() == "Select All") {
      $('input[name=downloadOptions]').prop('checked', true);
    button.attr('value', 'Unselect All');
  }
  else {
      $('input[name=downloadOptions]').prop('checked', false);
    button.attr('value', 'Select All');
  }
}

function downloadInvoice(methodName, value, gridId) {

  var selectedOptions;
  var arr = new Array();
  if ($('input[id=1]').prop('checked')) {
    arr.push("1");
  }
  if ($('input[id=2]').prop('checked')) {
    arr.push("2");
  }
  if ($('input[id=4]').prop('checked')) {
    arr.push("3");
  }
  if ($('input[id=8]').prop('checked')) {
    arr.push("4");
  }

  for (var i = 0; i < arr.length; i++) {
    if (i == 0)
      selectedOptions = arr[i];
    else
      selectedOptions = selectedOptions + "," + arr[i];
  }

  // Check if user has chosen at least one option
  if (arr.length <= 0) {
    alert('Please choose at least one option to download the invoice!');
    return false;
  }

  closeInvoiceDownloadOptions();

  //SCP334940: SRM Exception occurred in Iata.IS.Service.Iata.IS.Service.OfflineCollectionDownloadService. - SIS Production
  checkUserSessionsForAjaxRequest();

  $.ajax({
    type: "POST",
    url: _methodName,
    data: { id: _id, options: selectedOptions },
    success: function (result) {
      if (result.IsFailed == false) {
        showClientSuccessMessage(result.Message);
      }
      else {
        if (result.RedirectUrl != null) {
          location.href = result.RedirectUrl;
        }
        else {
          showClientErrorMessage(result.Message);
        }
      }
      $(gridId).trigger("reloadGrid");
    }
  });
}