<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.MiscUatp.Common.AchCurrencySetUp>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
   SIS :: ACH Ops ::  ACH Currencies of Clearance
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        Allowed ACH Currencies of Clearance Setup
    </h1>
    <% using (Html.BeginForm("Index", "AchCurrencySetUp", FormMethod.Post))
       {%>
    <div>
        <% Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchAchCurrencySetUp.ascx"); %>
    </div>
    <div class="buttonContainer">
        <%if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Masters.Masters.AchCurrencySetUpEditOrDelete))
          {%>
        <input type="button" class="primaryButton" value="Add" id="btnAdd" name="Add" onclick="javascript:location.href = '<%:Url.Action("Create", "AchCurrencySetUp")%>'" />
        <%
}%>
        <input type="submit" class="primaryButton" value="Search" id="btnSearch" name="Search" />
    </div>
    <%} %>
    <h2>
        Search Results</h2>
    <%Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchAchCurrencySetUpGrid.ascx", ViewData["AchCurrencySetUpGrid"]); %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
</asp:Content>
