<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Anonymous.Master"
  Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="ForgotPasswordSuccessTitle" ContentPlaceHolderID="TitleContent"
  runat="server">
  Forgot Password
</asp:Content>
<asp:Content ID="ForgotPasswordSuccessContent" ContentPlaceHolderID="MainContent"
  runat="server">
  <h2>
    Forgot Password</h2>
  <p>
     If the email exist in our database, you will receive an email shortly. 
  </p>
  <p>
    To go back to the log on screen
    <% =Html.ActionLink("Click Here", "LogOn") %></p>
</asp:Content>
<asp:Content ID="Script" ContentPlaceHolderID="Script" runat="server">
</asp:Content>
