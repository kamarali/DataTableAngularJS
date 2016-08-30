<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.MiscUatp.MiscUatpAttachment>" %>
<%-- 
 CMP #665-User Related Enhancements-FRS-v1.2 [Sec 2.9: IS-WEB MISC Payables Invoice Search Screen] 
--%>
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