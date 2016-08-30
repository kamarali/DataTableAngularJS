<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.SandBox.CertificationParameterMaster>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Master Maintenance :: SandBox Setup
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        Certification Parameters</h1>
    <% using (Html.BeginForm("Index", "SandBox", FormMethod.Post))
       {%>
       <%: Html.AntiForgeryToken() %>
    <div>
        <% Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchSandbox.ascx"); %>
    </div>
    <div class="buttonContainer">
        <input type="submit" class="primaryButton" value="Search" id="btnSearch" name="Search" />
    </div>
    <%} %>
    <h2>
        Search Results</h2>
    <%Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchSandboxGrid.ascx", ViewData["SandBoxGrid"]); %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
</asp:Content>
