<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Common.MinMaxAcceptableAmount>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
     SIS :: Master Maintenance :: General :: Minimum / Maximum Value Setup
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
       Minimum / Maximum Value Setup
    </h1>
    <% using (Html.BeginForm("Index", "MinMaxAcceptableAmount", FormMethod.Post))
       {%>
    <div>
        <% Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchMinMaxAcceptableAmount.ascx"); %>
    </div>
    <div class="buttonContainer">
      <%if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Masters.Masters.MinMaxAcceptableAmtEditOrDelete))
{%>
        <input type="button" class="primaryButton" value="Add" id="btnAdd" name="Add" onclick="javascript:location.href = '<%:Url.Action("Create", "MinMaxAcceptableAmount")%>'" />
        <%
}%>
        <input type="submit" class="primaryButton" value="Search" id="btnSearch" name="Search" />
    </div>
    <%} %>
     <h2>Search Results</h2>
    <%Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchMinMaxAcceptableAmountGrid.ascx", ViewData["MinMaxAcceptableAmountGrid"]); %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    
</asp:Content>