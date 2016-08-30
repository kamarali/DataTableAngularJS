<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.Pax.RMCoupon>" %>

<div class="buttonContainer">
  <input type="hidden" id="AttachmentId" name="AttachmentId" />
  <form id="ajaxUploadForm" action="<%: Url.Action("RMCouponFormFXFAttachmentUpload", "FormF", new { invoiceId = Model.RejectionMemoRecord.InvoiceId, transactionId = Model.Id })%>"
  method="post" enctype="multipart/form-data">
  <%: Html.AntiForgeryToken() %>
  <h2>
    Add Attachment(s)</h2>
  <label id="lblMultiFileUpload" class="labelAttachment">
    File to Upload: <span>
      <input id="file_element" type="file" name="file" size="35" /></span></label><br />
  <div>
    <input id="ajaxUploadButton" type="submit" value="Submit" class="secondaryButton ignoredirty" />
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
<div class ="buttonContainer">
    <input class="secondaryButton" type="button" value="Close" onclick="closeAttachmentDetail();" />
</div>