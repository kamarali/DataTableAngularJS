<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.FileFormat>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
     SIS :: Master Maintenance :: General :: File Format Setup
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        File Format Setup
    </h1>
    <% using (Html.BeginForm("Index", "FileFormat", FormMethod.Post))
       {%>
       <%: Html.AntiForgeryToken() %>
    <div>
        <% Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchFileFormat.ascx"); %>
    </div>
    <div class="buttonContainer">
    <%
           if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Masters.Masters.FileFormatEditOrDelete))
           {%>
        <input type="button" class="primaryButton" value="Add" id="btnAdd" name="Add" onclick="javascript:location.href = '<%:Url.Action("Create", "FileFormat")%>'" />
        <%
           }%>
        <input type="submit" class="primaryButton" value="Search" id="btnSearch" name="Search" />
    </div>
    <%} %>
     <h2>Search Results</h2>
    <%Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchFileFormatGrid.ascx", ViewData["FileFormatGrid"]); %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    
</asp:Content>
