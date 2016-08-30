<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Anonymous.Master"
  Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="ForgotPasswordFailedTitle" ContentPlaceHolderID="TitleContent" runat="server">
  Forgot Password
</asp:Content>
<asp:Content ID="ForgotPasswordFailedContent" ContentPlaceHolderID="MainContent"
  runat="server">
  <h2>
    Forgot Password</h2>
  <p>
    Forgot password failed to reset your password.
    <br />    
  </p>
  <p>
    To go back to the log on screen
    <% =Html.ActionLink("Click Here",  "LogOn") %>.</p>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
</asp:Content>
