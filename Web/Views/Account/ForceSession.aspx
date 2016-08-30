<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Anonymous.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  SIS :: Force SIS Session
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <img alt="Simplified Interline Settlement" src="<%: Url.Content("~/Content/Images/SIS_login_banner_new.png") %>"
    id="banner" />
  <table>
    <tr>
      <td>
        <div style="width: 100%; float: none;">
          <table class="basictable" width="600px" style="font-size: 10pt; border: 1px solid #000;
            height: 70px;">
            <tr>
              <td colspan="2" style="font-size: 10pt; text-align: left; padding-left: 20px;">
                You are already logged in to the system from other active web browser session(s).
                <br />
                <br />
                Click “OK” to continue logging in with this session; and to keep other session(s)
                active.
                <br />
                Click “CANCEL” to log in with this session; and deactivate/logoff other session(s).
              </td>
            </tr>
            <tr>
              <td style="text-align: left; width:15%; padding-left: 20px;">
                <% using (Html.BeginForm("Index", "Home", FormMethod.Get, new { id = "frmforceSession" }))
                   { %>
                <input class="primaryButton" style="width: 60px" type="submit" value="OK" />
                <%} %>
              </td>
              <td style="text-align: left;width:85%;">
                <% using (Html.BeginForm("DeleteUserOtherActiveSessions", "Account", FormMethod.Post, new { id = "frmforceSessionCancel" }))
                   { %>
                <input class="primaryButton" style="width: 60px" type="submit" value="CANCEL" />
                <%} %>
              </td>
            </tr>
          </table>
        </div>
      </td>
    </tr>
  </table>
  <br />
</asp:Content>
