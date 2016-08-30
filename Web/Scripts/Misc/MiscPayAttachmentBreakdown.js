/* 
* CMP #665-User Related Enhancements-FRS-v1.2 [Sec 2.9: IS-WEB MISC Payables Invoice Search Screen] 
* This Js file is used to load attachment grid data. 
*/
var attachmentDownload;

//Initialize Attachment Grid
function InitializeAttachmentGrid(downloadUrl) {
       attachmentDownload = downloadUrl;
   }

   function loadAttachment(invId) {
       $('#divAttachment').dialog({ closeOnEscape: false, title: 'View Attachments', height: 300, width: 510, modal: true, resizable: false });

       $("#AttachmentGrid_pager_center").width(297);
       $("#AttachmentGrid").jqGrid("clearGridData", true);
       $("#AttachmentGrid").setGridParam({ postData: { invoiceId: invId} }).trigger("reloadGrid");
   }

   //Formatter function for file name link, to download file
   function GetLinkForMiscPayableFileName(cellValue, options, cellObject) {
      //TFS#9937 : IE:Version 11: "Unexpected Error" occured in Payable for Miscellaneous.
       var attachId = cellObject.Id;
       return '<a style="cursor:hand;" target=_parent href="' + attachmentDownload + '/' + attachId + '" ><span>' + cellValue + '</span></a>';
   }

   //Close attachment details modal dialogue
   function closeAttachmentDetail() {
       $('#divAttachment').dialog('close');
   }