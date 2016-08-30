<%@ Page Title="Error Occurred" Language="C#" MasterPageFile="~/Views/Shared/Anonymous.Master" Inherits="System.Web.Mvc.ViewPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <h1>
    <% if (HttpContext.Current.Request.QueryString["title"] == "Session Expired") {%>	
        <%: HttpContext.Current.Request.QueryString["title"] %>
    <% }
       else if (HttpContext.Current.Request.QueryString["title"] == "Wrong Input")
       { %>
       <%: HttpContext.Current.Request.QueryString["title"] %>
    <% } else if (HttpContext.Current.Request.QueryString["title"] == "Invalid Member")
       { %>
       <%: HttpContext.Current.Request.QueryString["title"] %>
    <% }
       else if (HttpContext.Current.Request.QueryString["title"] == "anti-forgery")
       { %>
     Access Denied.
    <% } else { %>
    An Unexpected error Occurred
    <% } %>
</h1>
  <h2>
    <% if (HttpContext.Current.Request.QueryString["title"] == "Session Expired") {%>	
    <%--CMP685: hyperlink removed here--%>
    <%--The user session has expired. Please go to the <a href="<%: Url.Action("LogOn", "Account", new { area = "" }) %>">Login</a> page to login again.--%>
    The user session has expired. Please go to the Login page to login again.
    <% }
       else if (HttpContext.Current.Request.QueryString["title"] == "Wrong Input")
       { %>
       Invalid characters were entered. Operation did not complete. <a href = "javascript:history.back()">Go Back.</a>
    <% } else if (HttpContext.Current.Request.QueryString["title"] == "Invalid Member")
       { %>
       Unauthorized access. Not allowed to view other member profile. <a href = "javascript:history.back()">Go Back.</a>
    <% }
       else if (HttpContext.Current.Request.QueryString["title"] == "anti-forgery")
       { %>
       A required anti-forgery token was not supplied or was invalid.
    <% } else { %>
    Sorry, an error occurred while processing your request.
    <% } %>
  </h2>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
</asp:Content>
