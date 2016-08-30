/*Used if attachment has to be added/deleted to/from db from within the popup.(Used in Misc/Uatp)*/
var $attachmentCurrent = 1;
var $attachmentList; var $isToUpload = true;
var attachmentFields = InitializeAttachmentFields();
var fileExtTypes; var fileIps = 1; var currentIPFileE, attachmentDownload, attachmentDelete, uploadImageUrl;
var $attachmentCount = 0;

function InitializeAttachmentGrid(attachmentData, fileExtns, deleteCall, downloadCall, imageUrl) {
    var columnNames = '';
    var columnModel = '';

    // If BillingType is 'Payables' only display Serial No, FileName and FileSize columns on Attachment grid, else for 'Receivables' display all columns
    if (billingType == 'Payables') {
        columnNames = [attachmentFields.AttachmentId.DisplayName, attachmentFields.AttachmentSrNo.DisplayName, attachmentFields.AttachmentFileName.DisplayName, attachmentFields.AttachmentFileSize.DisplayName];
    }
    else {
        columnNames = [attachmentFields.AttachmentId.DisplayName, attachmentFields.AttachmentSrNo.DisplayName, attachmentFields.AttachmentFileName.DisplayName, attachmentFields.AttachmentFileSize.DisplayName, attachmentFields.AttachmentTimeStamp.DisplayName, attachmentFields.AttachmentUploadedBy.DisplayName];
    }

    // If BillingType is 'Payables' column model should contain Serial No, FileName and FileSize values on Attachment grid, else for 'Receivables' display all values
    if (billingType == 'Payables') {
        columnModel = [{ name: 'Id', index: 'Id', sorttype: 'int', sortable: false, formatter: buttonFormatter, width: 40, hidden: $isOnView }, { name: 'AttachmentSrNo', index: 'AttachmentSrNo', sortable: false, width: 40 }, { name: 'FileName', index: 'FileName', sortable: false, formatter: GetLinkForFileName }, { name: 'FileSizeInKb', index: 'FileSizeInKb', sortable: false, width: 50}];
    }
    else {
        columnModel = [{ name: 'Id', index: 'Id', sorttype: 'int', sortable: false, formatter: buttonFormatter, width: 40, hidden: $isOnView }, { name: 'AttachmentSrNo', index: 'AttachmentSrNo', sortable: false, width: 40 }, { name: 'FileName', index: 'FileName', sortable: false, formatter: GetLinkForFileName }, { name: 'FileSizeInKb', index: 'FileSizeInKb', sortable: false, width: 50 }, { name: 'LastUpdatedOn', index: 'LastUpdatedOn', sortable: false, DataFormatString: _gridColumnDateFormat }, { name: 'LastUpdatedBy', index: 'LastUpdatedBy', sortable: false}];
    }

    //Initialize attachment grid
    attachmentDownload = downloadCall;
    attachmentDelete = deleteCall;
    uploadImageUrl = imageUrl;
    fileExtTypes = fileExtns;
    $.Attachment = {};
    $.Attachment.FileNames = [];
    $attachmentList = $('#attachmentGrid');
    $attachmentList.jqGrid({
        datatype: 'local',
        width: 775,
        height: 280,
        colNames: columnNames,
        colModel: columnModel
    });

    attachmentData = eval(attachmentData);
    // Populate data in attachment grid with existing attachment records
    if (attachmentData != null) {
        for ($attachmentCurrent; $attachmentCurrent < attachmentData.length + 1; $attachmentCurrent++) {
            // Convert FileSize from Bytes to KB and round it to 2 decimal places and display it on AttachmentGrid
            attachmentData[$attachmentCurrent - 1]["FileSizeInKb"] = (attachmentData[$attachmentCurrent - 1]["FileSize"] / 1024).toFixed(2);
            row = { Id: attachmentData[$attachmentCurrent - 1]["Id"], AttachmentSrNo: $attachmentCurrent, FileName: attachmentData[$attachmentCurrent - 1].OriginalFileName, FileSizeInKb: attachmentData[$attachmentCurrent - 1]["FileSizeInKb"], LastUpdatedOn: attachmentData[$attachmentCurrent - 1]["LastUpdatedOnInString"], LastUpdatedBy: attachmentData[$attachmentCurrent - 1]["UserName"] };
            $attachmentList.addRowData(row["Id"], row);
            $.Attachment.FileNames.push(CreateAttachmentField(row["Id"], attachmentData[$attachmentCurrent - 1].OriginalFileName));
            ++$attachmentCount;
        }
    }
}

//Create object for AttachmentField constant
function CreateAttachmentField(id, displayName) {
    obj = [];
    obj.Id = id;
    obj.DisplayName = displayName;
    return obj;
}

//Initializes attachmentField constant which contains values for Controls and display name for grid title
function InitializeAttachmentFields() {
    var fields = new Array();
    fields.AttachmentId = CreateAttachmentField('AttachmentId', 'Action');
    fields.AttachmentSrNo = CreateAttachmentField('AttachmentSrNo', 'Sr. No.');
    fields.AttachmentFileName = CreateAttachmentField('AttachmentFileName', 'File Name');
    fields.AttachmentFileSize = CreateAttachmentField('AttachmentFileSize', 'File Size (KB)');
    fields.AttachmentTimeStamp = CreateAttachmentField('AttachmentTimeStamp', 'Upload/Link Timestamp');
    fields.AttachmentUploadedBy = CreateAttachmentField('AttachmentUploadedBy', 'Uploaded By');
    return fields;
}

//Custom formatter to display delete button in grid
function buttonFormatter(cellValue, options, cellObject) {
    return '<a style=cursor:hand target=_parent title="Delete" href=javascript:deleteAttachmentRecord("' + cellValue + '");><div class="deleteIcon ignoredirty"></div></a>';
}

function GetLinkForFileName(cellValue, options, cellObject) {
    return '<a style="cursor:hand;" target=_parent href="' + attachmentDownload + '/' + options.rowId + '" ><span class="ignoredirty">' + cellValue + '</span></a>';
}

// Method to delete tax record
function deleteAttachmentRecord(id) {
    // Delete record from grid
    if (confirm("Are you sure you want to delete this record?")) {
        $.ajax({
            type: "POST",
            url: attachmentDelete,
            data: "id=" + id,
            success: function (result) {
                if (result == true) {
                    $attachmentList.delRowData(id);
                    --$attachmentCount; $attachmentCurrent = 1;
                    var i = 0; var rowData, colData;
                    rowIds = $attachmentList.getDataIDs();
                    for (i = 0; i < rowIds.length; i++) {
                        try {
                            $attachmentList.setCell(rowIds[i], 'AttachmentSrNo', $attachmentCurrent, {}, {});
                        }
                        catch (e) { }
                        ++$attachmentCurrent;
                    }

                    if ($attachmentCount == 0)
                        $("#AttachmentIndicatorOriginal").val(false);
                    var i = 0;
                    for (i = 0; i < $.Attachment.FileNames.length; i++) {
                        if ($.Attachment.FileNames[i].Id == id) {
                            $.Attachment.FileNames.splice(i, 1);
                            break;
                        }
                    }

                    // Set the parent form dirty.
                    // Commenting the setDirty code as attachments are added/deleted without requiring to click on 'Save'.
                    //$parentForm.setDirty();
                }
            }
        });
    }
}

function addAttachment(row) {
    // Convert FileSize from Bytes to KB and round it to 2 decimal places and display it on AttachmentGrid
    row["FileSizeInKb"] = (row["FileSize"] / 1024).toFixed(2);
    // Add record in grid
    var rows = { Id: row["Id"], AttachmentSrNo: $attachmentCurrent, FileName: row.OriginalFileName, FileSizeInKb: row["FileSizeInKb"], LastUpdatedOn: row["LastUpdatedOnInString"], LastUpdatedBy: row["UserName"] };
    $attachmentList.addRowData(rows["Id"], rows);
    $.Attachment.FileNames.push(CreateAttachmentField(row.Id, row.OriginalFileName));
    $attachmentCurrent++; ++$attachmentCount;
    $("#AttachmentIndicatorOriginal").val(true);

    // Set the parent form dirty.
    // Commenting the setDirty code as attachments are added/deleted without requiring to click on 'Save'.
    //$parentForm.setDirty();
}

// Close attachment dialog
function closeAttachmentDetail() {
    $('#divAttachment').dialog('close');
}

function OnUploadSuccess(result) {
    if (result != null && !result.IsFailed && result.Attachment != null) {
            if (result.Length <= 0) {
            result.Message = 'Error in uploading file. Please make sure that file size should be greater than 0 byte.';
            alert('Error in uploading file. Please make sure that file size should be greater than 0 byte.');
        }
        
        var i = 0;
        for (i = 0; i < result.Length; i++) {
            addAttachment(result.Attachment[i]);
        }
    }
    $("#ajaxUploadForm").unblock();
    $("#ajaxUploadForm").resetForm(); $.growlUI(null, result.Message);
    fileIps = 1;
}

function openAttachment() {

    //SCP305855 - UAT: Session expired when rasing a RM- Spira Case 9766
    checkUserSessionsForAjaxRequest();

    var height = 520; if ($isOnView == true) { $("#ajaxUploadForm").hide(); height = 450; }
    $('#divAttachment').dialog({ closeOnEscape: false, title: 'Add/Remove Attachments', height: height, width: 800, modal: true, resizable: false });

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
            $("#ajaxUploadForm").unblock(); $("#ajaxUploadForm").resetForm();
            if (errorThrown.description.length > 1 && errorThrown.description.indexOf("Access is denied") != -1)
                alert("Error in uploading file. Please make sure that file size is less than 5MB.");
            else alert("Error in uploading file.");
        }
    });

    return false;
}

function bindAttachmentDialogCloseEvent() {
    $("#divAttachment").bind("dialogclose", function (event, ui) {
        var lineItemCount = jQuery("#LineItemGrid").getGridParam("records");
        if (lineItemCount > 0 || $attachmentCount > 0) {
            $('#BilledMemberText').attr('readonly', 'readonly');
        }
        else {
            $('#BilledMemberText').removeAttr('readonly');
        }
    });
}
