<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Anonymous.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
  ChangePasswordFailed
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h2>
    ChangePasswordFailed</h2>
  <p>
    <%:Html.Encode(ViewData["FailedStatusMessage"]) %>
  </p>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
</asp:Content>
