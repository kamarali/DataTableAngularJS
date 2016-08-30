$(document).ready(function () {

  $("#SupportingDocSearchForm").validate({
    rules: {
      BillingYearMonth: "required",
      BillingMemberText: "required"
    },
    messages: {
      BillingYearMonth: "Billing Year / Month required",
      BillingMemberText: "Billing Member required"
    }
  });

  $("#SupportingDocumentTypeId").bind("change", OnSupportingDocumentTypeChange);

  var selectedId = $("#SupportingDocumentTypeId").val();
  if (selectedId == 2) {
    $('#BillingPeriod').attr('disabled', true);
  }
  else {
    $('#BillingPeriod').attr('disabled', false);
  }
});

function OnSupportingDocumentTypeChange() {
  var selectedId = $("#SupportingDocumentTypeId").val();
  if (selectedId == 0) {
    return;
  }

  $.ajax({
    type: "Post",
    url: _getSuppDocTypeUrl,
    data: { supportingDocumentTypeId: selectedId },
    dataType: "json",
    success: PopulateBillingYearMonth,
    failure: function (response) {
      $("#ChargeCodeTypeId").val("");
      $("#chargeCodeTypeDiv").hide();
    }
  });
  if (selectedId == 2) {
    $('#BillingPeriod').attr('disabled', true);
  }
  else {
    $('#BillingPeriod').attr('disabled', false);
  }
}

function PopulateBillingYearMonth(response) {
  
  if (response.length > 0) {

    $("#BillingYearMonth").empty();

    for (i = 0; i < response.length; i++) {
      $("#BillingYearMonth").append($("<option></option>").val(response[i].Value).html(response[i].Text));
    };

  }
}

var attachmentUploadMethod;var _getSuppDocTypeUrl;

var fileExtTypes = ''; var fileIps = 1; var currentIPFileE, attachmentDownload, uploadImageUrl;

function InitializeAttachmentGrid(fileExtns, downloadUrl, getSuppDocTypeUrl) {
  uploadImageUrl = _loadingGif;
  fileExtTypes = fileExtns;
  attachmentDownload = downloadUrl;
  _getSuppDocTypeUrl = getSuppDocTypeUrl;
}


function loadAttachment(url, transacId, invId, transacType) {
  attachmentUploadMethod = url;
  $("#invoiceId", "form#ajaxUploadForm").val(invId);
  $("#transactionId", "form#ajaxUploadForm").val(transacId);
  $("#transactionType", "form#ajaxUploadForm").val(transacType);
  
  $('#divAttachment').dialog({ closeOnEscape: false, title: 'View Attachments', height: 450, width: 850, modal: true, resizable: false });
  fileIps = 1;
  multi_selector = new MultiSelector(document.getElementById('files_list'), 1);
  multi_selector.addElement(document.getElementById('file_element'));

  $("#ajaxUploadForm").ajaxForm({
    iframe: true,
    dataType: "json",
    type: "POST",
    beforeSubmit: function () {
      if ($("#file_element")[0].value.length == 0 || !$isToUpload) return false;
      $("#ajaxUploadForm").block({ message: '<h1><img src="' + uploadImageUrl + '" /> Uploading file...</h1>' });
    },
    success: function (result) {
      OnUploadSuccess(result);
    },
    error: function (xhr, textStatus, errorThrown) {
      $("#ajaxUploadForm").unblock(); $("#ajaxUploadForm").resetForm(); //$.growlUI(null, 'Error uploading file');
      if (errorThrown.description && errorThrown.description.length > 1 && errorThrown.description.indexOf("Access is denied") != -1)
        alert("Error in uploading file. Please make sure that file size is less than 5MB.");
      else alert("Error in uploading file.");
    }
  });
  
  $("#AttachmentGrid_pager_center").width(297);
  $("#AttachmentGrid").jqGrid("clearGridData", true);
  $("#AttachmentGrid").setGridParam({ postData: { invoiceId: invId, transactionId: transacId, transactionType: transacType} }).trigger("reloadGrid");
}

function OnUploadSuccess(result) {
  if (result != null && !result.IsFailed && result.Attachment != null) {
    $("#AttachmentGrid").trigger("reloadGrid");
    $("#SupportingDocSearchResultGrid").trigger("reloadGrid");
    // Toggle message containers.          
    showClientSuccessMessage(result.Message);
  }
  if (result.IsFailed) {
    showClientErrorMessage(result.Message);
  }
  $("#ajaxUploadForm").unblock();

  var invId = $("#invoiceId", "form#ajaxUploadForm").val();
  var transacId = $("#transactionId", "form#ajaxUploadForm").val();
  var transacType = $("#transactionType", "form#ajaxUploadForm").val();

  $("#ajaxUploadForm").resetForm();

  $("#invoiceId", "form#ajaxUploadForm").val(invId);
  $("#transactionId", "form#ajaxUploadForm").val(transacId);
  $("#transactionType", "form#ajaxUploadForm").val(transacType);

  if (result != null && result.Message) $.growlUI(null, result.Message); //alert(result.Message);  //
  fileIps = 1;
}

function deleteAttachment(methodName, value) {
  $('#successMessageContainer').hide();

  if (confirm("Are you sure you want to delete this record?")) {
  var recordType = $("#transactionType", "form#ajaxUploadForm").val();
    $.ajax({
      type: "POST",
      url: methodName,
      data: {attachmentId: value, transactionType: recordType},
      success: function (result) {
        $('#errorContainer').hide();
        if (result.IsFailed == false) {
                  

          // Toggle message containers.          
          showClientSuccessMessage(result.Message);
        }
        else {
          showClientErrorMessage(result.Message);
        }
        $("#AttachmentGrid").trigger("reloadGrid");
        $("#SupportingDocSearchResultGrid").trigger("reloadGrid");
      }
    });
  }
}

//Formatter function for file name link, to download file
function GetLinkForSupportingDocFileName(cellValue, options, cellObject) {
  var attachId = cellObject.Id;
  var recordType = $("#transactionType", "form#ajaxUploadForm").val();
  return '<a style="cursor:hand;" target=_parent href="' + attachmentDownload + '?attachmentId=' + attachId + '&transactionType=' + recordType + '" ><span>' + cellValue + '</span></a>';
}

//Close attachment details modal dialogue
function closeAttachmentDetail() {
  $("#invoiceId", "form#ajaxUploadForm").val('');
  $("#transactionId", "form#ajaxUploadForm").val('');
  $("#transactionType", "form#ajaxUploadForm").val('');
  $('#divAttachment').dialog('close');
}

//Formatter to diplay empty string when 0 is selected
function DisplayNullableIntegerFormatter(cellValue, options, cellObject) {
  if (cellValue == '0')
    return '';
  else
    return cellValue;
}