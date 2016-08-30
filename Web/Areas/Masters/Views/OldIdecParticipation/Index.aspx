<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.OldIdecParticipation>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Old Idec Participation
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Manage Old Idec Participation
    </h2>
    <% using (Html.BeginForm("Index", "OldIdecParticipation", FormMethod.Post))
       {%>
    <div>
        <% Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchOldIdecParticipation.ascx"); %>
    </div>
    <div class="buttonContainer">
        <input type="button" class="primaryButton" value="Add" id="btnAdd" name="Add" onclick="javascript:location.href = '<%:Url.Action("Create","OldIdecParticipation") %>'" />
        <input type="submit" class="primaryButton" value="Search" id="btnSearch" name="Search" />
    </div>
    <%} %>
     <h2>Search Results</h2>
    <%Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchOldIdecParticipationGrid.ascx", ViewData["OldIdecParticipationGrid"]); %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    
</asp:Content>