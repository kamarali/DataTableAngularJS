<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.TimeLimit>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	 SIS :: Master Maintenance :: General :: Time Limit Setup
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<h1>Time Limit Setup</h1>
<% using (Html.BeginForm("Index", "TimeLimit", FormMethod.Post, new { id = "SearchTimeLimit" }))
       {%>
       <%: Html.AntiForgeryToken() %>
    <div>
        <% Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchTimeLimit.ascx"); %>
    </div>
    <div class="buttonContainer">
    <%
           if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Masters.Masters.TimeLimitEditOrDelete))
           {%>
        <input type="button" class="primaryButton" value="Add" id="btnAdd" name="Add"  onclick="javascript:location.href = '<%:Url.Action("Create", "TimeLimit")%>'" />
        <%
           }%>
        <input type="submit" class="primaryButton" value="Search" id="btnSearch" name="Search" />
    </div>
    <%} %>
     <h2>Search Results</h2>
    <%Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchTimeLimitGrid.ascx", ViewData["TimeLimitGrid"]); %>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
<script src="<%:Url.Content("~/Scripts/Masters/TimeLimitValidate.js")%>" type="text/javascript"></script>
</asp:Content>
