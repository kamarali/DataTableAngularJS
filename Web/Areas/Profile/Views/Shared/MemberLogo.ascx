<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<dynamic>" %>
<form id="imageUploadForm" action="<%: Url.Action("MemberLogoUpload", "Member", new { area = "Profile"} )%>"
method="post" enctype="multipart/form-data">
<%: Html.AntiForgeryToken() %>
<div id="divLogoUpload" class="content">
  <label for="LogoImage">
    Logo:</label>
  <div id="containerMemberLogo">
    <input type="file" id="memberLogoUpload" size="23" /></div>
  <div class="buttonContainer">
    <div id="files_list">
    </div>
  </div>
  <input id="btnMemeberLogoUploadOK" class="primaryButton" type="button" value="OK"
    onclick="OnOKMemeberLogoUpload()" />
  <input id="submitImage" class="hidden" type="submit" />
  <div id="IsEmailToSend" class="hidden">
    <input id="hiddenSendMail" name="hiddenSendMail" type="hidden" /></div>
  <input id="btnMemeberLogoUploadCancel" class="secondaryButton" type="button" value="Cancel"
    onclick="OnCancelMemeberLogoUpload()" />
</div>
</form>
