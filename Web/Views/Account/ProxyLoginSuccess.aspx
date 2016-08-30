<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="changePasswordTitle" ContentPlaceHolderID="TitleContent" runat="server">
  <%=Iata.IS.Web.AppSettings.ProxyLoginSuccessText%>
</asp:Content>

<asp:Content ID="changePasswordSuccessContent" ContentPlaceHolderID="MainContent" runat="server">
  <h2><%=Iata.IS.Web.AppSettings.ProxyLoginSuccessText%></h2>
   
</asp:Content>

