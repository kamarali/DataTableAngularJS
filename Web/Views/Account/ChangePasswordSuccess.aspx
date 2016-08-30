<%@Language="C#" MasterPageFile="~/Views/Shared/Anonymous.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="changePasswordTitle" ContentPlaceHolderID="TitleContent" runat="server">
  Change Password
</asp:Content>

<asp:Content ID="changePasswordSuccessContent" ContentPlaceHolderID="MainContent" runat="server">
  <h2>Change Password</h2>
  <p>
   Your password has been changed successfully.
   Please go to the <a href="<%: Url.Action("LogOn", "Account", new { area = "" }) %>">Login</a> page to login again.
  </p>
</asp:Content>