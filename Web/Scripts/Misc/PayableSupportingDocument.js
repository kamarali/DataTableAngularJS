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

});

var attachmentUploadMethod; 

var fileExtTypes = ''; var fileIps = 1; var currentIPFileE, attachmentDownload, uploadImageUrl;

function InitializeAttachmentGrid(fileExtns, downloadUrl) {
  uploadImageUrl = _loadingGif;
  fileExtTypes = fileExtns;
  attachmentDownload = downloadUrl;
}


function loadAttachment(invId) {
  
  $("#invoiceId", "form#ajaxUploadForm").val(invId);
  
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
  //TFS#9988 - Mozilla:V46: Pagination get overlapped on Add/Remove Attachments Popup for MISC Receivables Supporting Document Screen.  
  $("#AttachmentGrid_pager_center").width(297);
  $("#AttachmentGrid").jqGrid("clearGridData", true);
  $("#AttachmentGrid").setGridParam({ postData: { invoiceId: invId } }).trigger("reloadGrid");
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
  var invoiceId = $("#invoiceId", "form#ajaxUploadForm").val();
  $("#ajaxUploadForm").resetForm();
  $("#invoiceId", "form#ajaxUploadForm").val(invoiceId);
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
      data: {attachmentId: value },
      success: function (result) {
        $('#errorContainer').hide();
        if (result == true) {
                  

          // Toggle message containers.          
          showClientSuccessMessage('Record deleted.');
        }
        else {
          showClientErrorMessage('Record not deleted.');
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
  var invoiceId = $("#invoiceId", "form#ajaxUploadForm").val();
  return '<a style="cursor:hand;" target=_parent href="' + attachmentDownload + '?invoiceId=' + invoiceId + '&lineItemId=' + attachId + '" ><span>' + cellValue + '</span></a>';
}

//Close attachment details modal dialogue
function closeAttachmentDetail() {
  $("#invoiceId", "form#ajaxUploadForm").val('');
  $('#divAttachment').dialog('close');
}