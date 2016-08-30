<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.Sampling.SamplingFormDRecord>" %>
<h2>
  Attached Files List</h2>
<div class="buttonContainer">
  <input type="hidden" id="AttachmentId" name="AttachmentId"/>
  <form id="ajaxUploadForm" action="<%: Url.Action("FormDAttachmentUpload", "FormDE", new { invoiceId = Model.Invoice.Id, transactionId = Model.Id })%>" method="post" enctype="multipart/form-data">
  <%: Html.AntiForgeryToken() %>
  <div>
    <h2> Add Attachment(s)</h2>
    <label id="lblMultiFileUpload">
      <span>*</span> File to Upload:
      <span><input id="file_element" type="file" name="file" size="40"/></span></label>
      <input id="ajaxUploadButton" type="submit" value="Submit" class="secondaryButton ignoredirty" />
    <div id="files_list">
    </div>
  </div>
  </form>
</div>

<div>
<table id="attachmentGrid"></table>
</div>
<div class ="buttonContainer">
<input class="secondaryButton" type="button" value="Close" onclick="closeAttachmentDetail();" />
</div>
