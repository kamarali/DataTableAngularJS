<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Iata.IS.Model.Common.Country>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
     SIS :: Master Maintenance :: Area Related :: ISO Country and DS Setup
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        ISO Country and DS Setup</h1>
    <% using (Html.BeginForm("Index", "Country", FormMethod.Post))
       {%>
       <%: Html.AntiForgeryToken() %>
    <div>
        <% Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchCountry.ascx"); %>
    </div>
    <div class="buttonContainer">
    <%
           if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Masters.Masters.CountryEditOrDelete))
           {%>
        <input type="button" class="primaryButton" value="Add" id="btnAdd" name="Add" onclick="javascript:location.href = '<%:Url.Action("Create", "Country")%>'" />
        <%
           }%>
        <input type="submit" class="primaryButton" value="Search" id="btnSearch" name="Search" />
    </div>
    <%} %>
    <h2>
        Search Results</h2>
    <%Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchCountryGrid.ascx", ViewData["CountryGrid"]); %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
</asp:Content>
