<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Iata.IS.Model.Pax.Common.SampleDigit>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    SIS :: Master Maintenance :: Passenger Related :: Sample Digit Setup
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
       Sample Digit Setup
    </h1>
    <% using (Html.BeginForm("Index", "SampleDigit", FormMethod.Post))
       {%>
       <%: Html.AntiForgeryToken() %>
    <div>
        <% Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchSampleDigit.ascx"); %>
    </div>
    <div class="buttonContainer">
    <%if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Masters.Masters.SampleDigitEditOrDelete))
{%>
        <input type="button" class="primaryButton" value="Add" id="btnAdd" name="Add" onclick="javascript:location.href = '<%:Url.Action("Create", "SampleDigit")%>'" />
        <%
}%>
        <input type="submit" class="primaryButton" value="Search" id="btnSearch" name="Search" />
    </div>
    <%} %>
     <h2>Search Results</h2>
    <%Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchSampleDigitGrid.ascx", ViewData["SampleDigitGrid"]); %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
    
</asp:Content>