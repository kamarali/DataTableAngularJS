<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Calendar.ISCalendar>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	IS Calendar
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>
        Manage IS Calendar</h2>
    <% using (Html.BeginForm("Index", "ISCalendar", FormMethod.Post))
       {%>
    <div>
        <% Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchISCalendar.ascx"); %>
    </div>
    <div class="buttonContainer">
        <input type="button" class="primaryButton" value="Add" id="btnAdd" name="Add" onclick="javascript:location.href = '<%:Url.Action("Create","ISCalendar") %>'" />
        <input type="submit" class="primaryButton" value="Search" id="btnSearch" name="Search" />
    </div>
    <%} %>
    <%Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchISCalendarGrid.ascx", ViewData["ISCalendarGrid"]); %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
</asp:Content>