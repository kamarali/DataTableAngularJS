<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.ReasonCode>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	 SIS :: Master Maintenance :: General :: Reason Code
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Reason Code Setup</h1>
    <% using (Html.BeginForm("Index", "ReasonCode", FormMethod.Post))
       {%>
       <%: Html.AntiForgeryToken() %>
    <div>
        <% Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchReasonCode.ascx"); %>
    </div>
    <div class="buttonContainer">
    <%if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Masters.Masters.ReasonCodeEditOrDelete))
{%>
        <input type="button" class="primaryButton" value="Add" id="btnAdd" name="Add" onclick="javascript:location.href = '<%:Url.Action("Create", "ReasonCode")%>'" />
        <%
}%>
        <input type="submit" class="primaryButton" value="Search" id="btnSearch" name="Search" />
    </div>
    <%} %>
     <h2>Search Results</h2>
    <%Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchReasonCodeGrid.ascx", ViewData["ReasonCodeGrid"]); %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    
</asp:Content>
