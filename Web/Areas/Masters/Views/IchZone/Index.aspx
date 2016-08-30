<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.IchZone>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Ich Zone
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Manage Ich Zone
    </h2>
    <% using (Html.BeginForm("Index", "IchZone", FormMethod.Post))
       {%>
    <div>
        <% Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchIchZone.ascx"); %>
    </div>
    <div class="buttonContainer">
        <input type="button" class="primaryButton" value="Add" id="btnAdd" name="Add" onclick="javascript:location.href = '<%:Url.Action("Create","IchZone") %>'" />
        <input type="submit" class="primaryButton" value="Search" id="btnSearch" name="Search" />
    </div>
    <%} %>
     <h2>Search Results</h2>
    <%Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchIchZoneGrid.ascx", ViewData["IchZoneGrid"]); %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    
</asp:Content>
