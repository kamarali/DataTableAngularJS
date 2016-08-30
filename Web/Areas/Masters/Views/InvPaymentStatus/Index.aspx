<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Iata.IS.Model.Common.InvPaymentStatus>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
   SIS :: Master Maintenance :: MISC Payment Status Related :: MISC Payment Status Setup
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        MISC Payment Status Setup
    </h1>
    <% using (Html.BeginForm("Index", "InvPaymentStatus", FormMethod.Post))
       {%>
       <%: Html.AntiForgeryToken() %>
    <div>
        <% Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchInvPaymentStatus.ascx"); %>
    </div>
   <div class="buttonContainer">
    <%if (Html.IsAuthorized(Iata.IS.Business.Security.Permissions.Masters.Masters.MISCPaymentStatusEditOrDelete))
{%>
        <input type="button" class="primaryButton" value="Add" id="btnAdd" name="Add" onclick="javascript:location.href = '<%:Url.Action("Create", "InvPaymentStatus")%>'" />
        <%
}%>
        <input type="submit" class="primaryButton" value="Search" id="btnSearch" name="Search" />
    </div>
    <%} %>
    <h2>
        Search Results</h2>
    <%Html.RenderPartial("~/Areas/Masters/Views/Shared/SearchInvPaymentStatusGrid.ascx", ViewData["InvPaymentStatusGrid"]); %>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Script" runat="server">
</asp:Content>
