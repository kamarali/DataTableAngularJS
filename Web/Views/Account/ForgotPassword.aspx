<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Anonymous.Master"
  Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="forgotPasswordTitle" ContentPlaceHolderID="TitleContent" runat="server">
  Forgot Password
</asp:Content>
<asp:Content ID="forgotPasswordContent" ContentPlaceHolderID="MainContent" runat="server">
  <img alt="Simplified Interline Settlement" src="<%: Url.Content("~/Content/Images/SIS_login_banner_new.png") %>"
    id="banner" />
  <% using (Html.BeginForm("ForgotPassword", "Account", FormMethod.Post, new { id = "frmForgotPassword" }))
     { %>
  <div style="color: Red">
    <%: Html.ValidationSummary("") %>
  </div>
  <%: Html.AntiForgeryToken() %>
  <h1 style="padding-top: 10px;">
    Forgot Password</h1>
  <div>
    <div id="divuserID">
      <table class="basictable" width="200px">
        <tr>
          <td align="right">
            Username:
          </td>
          <td>
            <%: Html.TextBox("userID", ViewData["UserID"], new { maxlength = 200,autocomplete = "off" })%>
          </td>
        </tr>
        <tr>
          <td colspan="2" align="right">
            <img style="height: 25px;" id="imgCaptcha" src='<%:Url.Action("ShowCaptchaImage", "Account") %>'
              alt="" />
            <img alt="Refresh" style="cursor: pointer; margin-bottom: 0px; margin-right: 5px;
              height: 15px;" src="<%: Url.Content("~/Content/Images/refresh.png") %>" onclick="RefreshCaptcha();" />
          </td>
        </tr>
        <tr>
          <td colspan="2">
            Please enter the text as shown above:
          </td>
        </tr>
        <tr>
          <td>
            Captcha:
          </td>
          <td>
            <%: Html.TextBox("captchaText", ViewData["CaptchaText"], new { maxlength = 5, autocomplete = "off" })%>
          </td>
        </tr>
      </table>
    </div>
    <div>
      <input class="primaryButton" type="submit" id="btnNext" name="btnNext" value="Next"
        style="vertical-align: top;" />
    </div>
  </div>
  <% } %>
  <br />
  <%:Html.ActionLink ("Back to Login Page","LogOn","Account")%>
</asp:Content>
<asp:Content ID="script" ContentPlaceHolderID="Script" runat="server">
  <script src="<%:Url.Content("~/Scripts/jquery.validate.min.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/jquery.dirtyforms.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/validator.js")%>" type="text/javascript"></script>
  <script src="<%:Url.Content("~/Scripts/jquery.form.js")%>" type="text/javascript"></script>
  <script type="text/javascript">   

  $(document).ready(function () {
      $("#userID").watermark("Username").focus();
      $("#captchaText").watermark("Captcha");
      $('#captchaText').val('');    
            
    });
    

    function RefreshCaptcha() {
        var captcha = "<%:Url.Action("ShowCaptchaImage", "Account" )%>";            
            $("#imgCaptcha").attr("src", captcha + "?t=" + new Date().getTime());
        }  

  </script>
</asp:Content>
