﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Cargo.CargoCorrespondence>" %>

<div class="buttonContainer">
    <input type="hidden" id="AttachmentId" name="AttachmentId" />   
    <form id="ajaxUploadForm" action="<%: Url.Action("CorrespondenceAttachmentUpload", "Correspondence", new {invoiceId = ViewData[ViewDataConstants.InvoiceId], transactionId = Model.Id })%>"
    method="post" enctype="multipart/form-data">
    <%: Html.AntiForgeryToken() %>
    <h2>
        Add Attachment(s)</h2>
    <label id="lblMultiFileUpload" class="labelAttachment">
        File to Upload: <span>
            <input id="file_element" type="file" name="file" size="35" /></span></label><br />
    <div class="inputAttachment" id="divIpAttachment">
        <input id="ajaxUploadButton" type="submit" value="Submit" class="secondaryButton ignoredirty" />
        <input class="secondaryButton" type="button" value="Close" onclick="closeAttachmentDetail();" />
    </div>   
    </form>
</div>
<h2>
    Attached Files List</h2>
<div>
    <table id="attachmentGrid">
    </table>
</div>