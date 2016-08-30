﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.Common.SupportingDocumentAttachment>" %>

<%: ScriptHelper.GenerateSupportingDocDeleteScript(Url, ControlIdConstants.AttachmentGrid,
                Url.Action("AttachmentDelete", "SupportingDoc", new { area = "Pax" }))%>
<div style=" visibility:hidden">
  <form id="ajaxUploadForm" action="<%: Url.Action("UploadAttachment", "SupportingDoc")%>"
  method="post" enctype="multipart/form-data">
<%: Html.AntiForgeryToken() %>
  <h2>
    Add Attachment(s)</h2>
      <input type="hidden" id="invoiceId" name="invoiceId" />
      <input type="hidden" id="transactionId" name="transactionId" />
      <input type="hidden" id="transactionType" name="transactionType" />
  <label id="lblMultiFileUpload" class="labelAttachment">
    File to Upload: <span>
      <input id="file_element" type="file" name="file" size="35" /></span></label><br />
  <div>
    <input id="ajaxUploadButton" type="submit" value="Submit" class="secondaryButton" />
  </div>
  <div id="files_list">
  </div>
  </form>
</div>
<h2>
  Attached Files List</h2>
<div>
  <table id="attachmentGrid">
  </table>
</div>
<div>
    <% Html.RenderPartial("GridControl", ViewData[ViewDataConstants.AttachmentResultGrid]); %>
  </div>
<div class ="buttonContainer">
    <input class="secondaryButton" type="button" value="Close" onclick="closeAttachmentDetail();" />
</div>