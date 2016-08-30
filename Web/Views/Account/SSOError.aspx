<%@ Page Title="Error Occurred" Language="C#" MasterPageFile="~/Views/Shared/Anonymous.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%--CMP #665: User Related Enhancements-FRS-v1.2.doc--%>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1><%: HttpContext.Current.Request.QueryString["title"] ?? "An Unexpected error Occurred" %></h1>
  <h2>   
    Sorry, an error occurred while processing your request.
  </h2>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
</asp:Content>
