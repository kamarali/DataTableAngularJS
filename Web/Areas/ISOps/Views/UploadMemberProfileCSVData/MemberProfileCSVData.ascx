<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Iata.IS.Model.UploadMemberProfileCSVData.MemberProfileCSVData>" %>
<div>
   <form id="ajaxUploadForm" action="<%= Url.Action("AjaxUpload", "UploadMemberProfileCSVData")%>" method="post" enctype="multipart/form-data">
    <%: Html.AntiForgeryToken() %>
    <%if (ViewData.ContainsKey("DownloadFileError")) %>
<%{ %>
<font color="red">
  <%= Html.Encode(ViewData["DownloadFileError"])%></font>
<%} %>
  <div class="searchCriteria">
    <div class="solidBox">
      <div class="fieldContainer horizontalFlow" style="height: 80px;">
        <div class="buttonContainer">
          <h2>
            All files must be in compressed format with a .zip extension.</h2>
          <br />
          <div style="float: left; width: 580px;">

          <b>File Name:</b>
            

          <input type="file" name="file" id="uploadedFile" />

            <input class="primaryButton" id="ajaxUploadButton" type="submit"  value="Submit" />

            
          </div>
        </div>
      </div>
    </div>
  </div>
  </form>
   
</div>
<br />
<div>

  <h2>List of files submitted till date</h2>

  <%Html.RenderPartial("MemberUploadCSVGrid", ViewData["MemberUploadGridData"]); %>
  
</div>

