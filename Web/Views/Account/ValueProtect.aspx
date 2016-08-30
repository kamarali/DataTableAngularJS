<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Anonymous.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  ValueProtect
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<form action="<%:Url.Action("UploadSystemParam", "Account") %>" method="post" enctype="multipart/form-data">
 <%: Html.AntiForgeryToken() %>
  <h2>
    Value Protect</h2>
  <fieldset class="solidBox dataEntry">
    
        <legend style="color: #0075BD; font: bold 10pt Arial,Helvetica,sans-serif; margin: 0;
          padding-top: 5px;">Web.Config Connection String</legend>
        <input class="primaryButton" type="button" id="btnProtect" name="btnProtect" value="Protect Connection String"
         style="vertical-align: top;" />
        <input class="primaryButton" type="button" id="btnUnProtect" name="btnUnProtect"
          value="UnProtect Connection String" style="vertical-align: top;" />
      
    
  </fieldset>

  <fieldset class="solidBox dataEntry">
      <legend style="color: #0075BD; font: bold 10pt Arial,Helvetica,sans-serif; margin: 0;
        padding-top: 5px;">Remove Connection string From Cache</legend>
           <div style="float: left; width: 580px;">
          <input class="primaryButton" type="button" id="btnconnection" name="btnconnection"
          value="Flush Connection String" style="vertical-align: top;" /><br /><br />
            Note : Please check out (TFS) Web.config file
          </div>
  </fieldset>

  <fieldset class="solidBox dataEntry">
      <legend style="color: #0075BD; font: bold 10pt Arial,Helvetica,sans-serif; margin: 0;
        padding-top: 5px;">System Parameter</legend>
           <div style="float: left; width: 580px;">
            <b>File Name:</b>
            <input type="file" size="50px" name="uploadedFile" id="uploadedFile"  />
            <input class="primaryButton" type="submit" name="submit" value="Submit" />
          </div>
  </fieldset>
  </form>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
  <script type="text/javascript">
    $(document).ready(function (event) {

      $('#btnProtect').click(function (event) {

        $.ajax({
          url: '<%:Url.Action("ProtectString", "Account", new { area = ""}) %>',
          type: "POST"

        });

      });

      $('#btnUnProtect').click(function (event) {
        $.ajax({
          url: '<%:Url.Action("UnProtectString", "Account", new { area = ""}) %>',
          type: "POST"
        });

      });



      $('#btnconnection').click(function (event) {
        $.ajax({
          url: '<%:Url.Action("RemoveConnectionCache", "Account", new { area = ""}) %>',
          type: "POST"
        });

      });

    });

  </script>
</asp:Content>
