<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.Common.Rfisc>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Master Maintenance :: Passenger Related :: EMD RFISC Setup
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        EMD RFISC Setup</h1>
    <% using (Html.BeginForm("Index", "Rfisc", FormMethod.Post))
       {%>
       <%: Html.AntiForgeryToken() %>
    <div>
        <% Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchRfisc.ascx"); %>
    </div>
    <div class="buttonContainer">
    <%if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Masters.Masters.RFISCEditOrDelete))
{%>
        <input type="button" class="primaryButton" value="Add" id="btnAdd" name="Add" onclick="javascript:location.href = '<%:Url.Action("Create", "Rfisc")%>'" />
        <%
}%>
        <input type="submit" class="primaryButton" value="Search" id="btnSearch" name="Search" />
    </div>
    <%} %>
    <h2>
        Search Results</h2>
    <%Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchRfiscGrid.ascx", ViewData["RfiscGrid"]); %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
</asp:Content>
